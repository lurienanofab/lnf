using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace LNF.Helpdesk
{
    public enum TicketPriorty
    {
        GeneralQuestion = 1,
        ProcessIssue = 2,
        HardwareIssue = 3
    }

    public class Service
    {
        private string _ApiUrl;
        private string _ApiKey;

        public Service(string apiUrl, string apiKey)
        {
            _ApiUrl = apiUrl;
            _ApiKey = apiKey;
        }

        public static string GetApiKey()
        {
            Dictionary<string, string> keys = new Dictionary<string, string>(){
		        {"141.213.7.23",    "53475715FC332BA1E5320CB0DCB45089"},
                {"168.61.39.172",   "FF051DC3C5699012965578752FA34E85"},
                {"141.213.8.37",    "BFCCB07172D97BB934253D0709FEC278"},
                {"137.117.73.153",  "3A34BDBAFBC2BB5EE72316841068A214"}
            };
            string ip = ServiceProvider.Current.Context.ServerVariables["LOCAL_ADDR"];
            if (keys.ContainsKey(ip))
                return keys[ip];
            else
                return string.Empty;
        }

        public static string GetUrlBase()
        {
            return ConfigurationManager.AppSettings["HelpdeskApiUrl"];
        }

        public DataTable SelectTickets(bool test = false)
        {
            return SelectTickets(0, test);
        }

        public DataTable SelectTickets(int resourceId, bool test = false)
        {
            return GetData(_ApiUrl, new { action = "select-tickets", resourceId = resourceId, test = test });
        }

        public DataTable SelectTicketsByEmail(string email, bool test = false)
        {
            return GetData(_ApiUrl, new { action = "select-tickets-by-email", email = email, test = test });
        }

        public TicketDetailResponse SelectTicketDetail(int ticketId)
        {
            string raw = SendPostRequest(_ApiUrl, new { Command = "select-ticket-detail", TicketID = ticketId, f = "json" });
            TicketDetailResponse result = ServiceProvider.Current.Serialization.Json.Deserialize<TicketDetailResponse>(raw);
            return result;
        }

        public CreateTicketResult CreateTicket(int resourceId, string name, string email, string queue, string subject, string message, TicketPriorty priority, bool test = false)
        {
            try
            {
                return new CreateTicketResult(
                    ResponseToTable(
                        SendPostRequest(_ApiUrl, new
                        {
                            action = "add-ticket",
                            resourceId = resourceId,
                            name = ServiceProvider.Current.Context.UrlEncode(name),
                            email = ServiceProvider.Current.Context.UrlEncode(email),
                            queue = ServiceProvider.Current.Context.UrlEncode(queue),
                            subject = ServiceProvider.Current.Context.UrlEncode(subject),
                            message = ServiceProvider.Current.Context.UrlEncode(message),
                            priority = (int)priority,
                            test = test
                        })
                    )
                );
            }
            catch (Exception ex)
            {
                return new CreateTicketResult(ex);
            }
        }

        public TicketDetailResponse PostMessage(int ticketId, string message)
        {
            string raw = SendPostRequest(_ApiUrl, new { Command = "post-message", TicketID = ticketId, Message = message, f = "json" });
            TicketDetailResponse result = ServiceProvider.Current.Serialization.Json.Deserialize<TicketDetailResponse>(raw);
            return result;
        }

        private DataTable GetData(string url, object postData)
        {
            return ResponseToTable(SendPostRequest(url, postData));
        }

        private DataTable ResponseToTable(string resp)
        {
            if (string.IsNullOrEmpty(resp))
                return null;
            DataSet ds = new DataSet();
            TextReader reader = new StringReader(resp);
            try
            {
                ds.ReadXml(reader, XmlReadMode.Auto);
            }
            catch (Exception ex)
            {
                throw new Exception(resp, ex);
            }
            reader.Close();
            return (ds.Tables.Contains("row")) ? ds.Tables["row"] : null;
        }

        private byte[] ConvertPostDataToByteArray(object postData)
        {
            return Encoding.UTF8.GetBytes(
                string.Join("&", postData.GetType().GetProperties().Select(p => string.Format("{0}={1}", p.Name, p.GetValue(postData, null))))
            );
        }

        private string SendPostRequest(string url, object postData)
        {
            byte[] byteArray = ConvertPostDataToByteArray(postData);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.UserAgent = _ApiKey;
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = byteArray.Length;
            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            dataStream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(dataStream);
            string result = sr.ReadToEnd();
            sr.Close();
            dataStream.Close();
            resp.Close();
            return result;
        }
    }
}
