using LNF.Cache;
using LNF.Data;
using LNF.Email;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LNF.Ordering
{
    public static class EmailUtility
    {
        public static void SendApprovalEmail(PurchaseOrder po)
        {
            Account acct = po.GetAccount();

            if (acct == null)
                throw new Exception("The IOF must have an account before sending for approval.");

            SendMessageArgs args = new SendMessageArgs();
            args.ClientID = CacheManager.Current.ClientID;
            args.From = Providers.Context.Current.GetAppSetting("SystemEmail");
            args.DisplayName = "LNF Ordering";
            args.To = new[] { po.Approver.PrimaryEmail() };
            args.Subject = string.Format("IOF# {0}: Request By {1}", po.GetDisplayPOID(), po.Client.DisplayName);
            args.IsHtml = true;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div style=\"font-family: arial;\">");
            sb.AppendLine(string.Format("<h2>Approver: {0}</h2><hr>", po.Approver.DisplayName));

            int cellPadding = 6;

            sb.AppendLine("<h4>IOF Info</h4>");
            sb.AppendLine("<div style=\"margin-bottom: 10px; padding: 5px; border: solid 1px #aaaaaa;\">");
            sb.AppendLine("<table style=\"font-family: arial; border-collapse: collapse;\">");
            sb.AppendLine(string.Format("<tr><td style=\"text-align: right; padding: {0}px;\"><b>IOF #</b></td><td style=\"padding: {0}px;\">{1}</td></tr>", cellPadding, po.GetDisplayPOID()));
            sb.AppendLine(string.Format("<tr><td style=\"text-align: right; padding: {0}px;\"><b>Vendor</b></td><td style=\"padding: {0}px;\">{1}</td></tr>", cellPadding, po.Vendor.VendorName));
            sb.AppendLine(string.Format("<tr><td style=\"text-align: right; padding: {0}px;\"><b>Account</b></td><td style=\"padding: {0}px;\">{1}</td></tr>", cellPadding, acct.GetNameWithShortCode()));
            sb.AppendLine(string.Format("<tr><td style=\"text-align: right; padding: {0}px;\"><b>Date Needed</b></td><td style=\"padding: {0}px;\">{1:M/d/yyyy}</td></tr>", cellPadding, po.NeededDate));
            sb.AppendLine(string.Format("<tr><td style=\"text-align: right; padding: {0}px;\"><b>Notes</b></td><td style=\"padding: {0}px;\">{1}</td></tr>", cellPadding, po.Notes));
            sb.AppendLine("</table></div>");

            sb.AppendLine("<h4>IOF Items</h4>");
            sb.AppendLine("<div style=\"margin-bottom: 10px; padding: 5px; border: solid 1px #aaaaaa;\">");
            sb.AppendLine("<table style=\"font-family: arial; border-collapse: collapse;\">");
            sb.AppendLine("<tr>");
            sb.AppendLine(string.Format("<th style=\"border-bottom: solid 2px #dddddd; padding: {0}px; text-align: left;\">Part #</th>", cellPadding));
            sb.AppendLine(string.Format("<th style=\"border-bottom: solid 2px #dddddd; padding: {0}px; text-align: left;\">Description</th>", cellPadding));
            sb.AppendLine(string.Format("<th style=\"border-bottom: solid 2px #dddddd; padding: {0}px; text-align: left;\">Category</th>", cellPadding));
            sb.AppendLine(string.Format("<th style=\"border-bottom: solid 2px #dddddd; padding: {0}px; text-align: right;\">Qty</th>", cellPadding));
            sb.AppendLine(string.Format("<th style=\"border-bottom: solid 2px #dddddd; padding: {0}px; text-align: right;\">Unit Price</th>", cellPadding));
            sb.AppendLine("</tr>");
            foreach (var pod in po.GetDetails().ToList())
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=\"border-bottom: solid 1px #dddddd; padding: {0}px;\">{1}</td>", cellPadding, pod.Item.PartNum));
                sb.AppendLine(string.Format("<td style=\"border-bottom: solid 1px #dddddd; padding: {0}px;\">{1}</td>", cellPadding, pod.Item.Description));
                sb.AppendLine(string.Format("<td style=\"border-bottom: solid 1px #dddddd; padding: {0}px;\">{1} - {2}</td>", cellPadding, pod.Category.CatNo, pod.Category.CatName));
                sb.AppendLine(string.Format("<td style=\"border-bottom: solid 1px #dddddd; padding: {0}px; text-align: right;\">{1}</td>", cellPadding, string.Format("{0} {1}", pod.Quantity, pod.Unit).Trim()));
                sb.AppendLine(string.Format("<td style=\"border-bottom: solid 1px #dddddd; padding: {0}px; text-align: right;\">{1:C}</td>", cellPadding, pod.UnitPrice));
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine(string.Format("<h4>IOF Total: {0:C}</h4>", po.GetTotalPrice()));
            sb.AppendLine("</div>");

            sb.AppendLine("<h4>Attachments</h4>");
            sb.AppendLine("<div style=\"margin-bottom: 10px; padding: 5px; border: solid 1px #aaaaaa;\">");
            IEnumerable<Attachment> attachments = Attachment.GetAttachments(po.POID);
            if (attachments.Count() > 0)
            {
                sb.AppendLine("<ol>");
                foreach (var a in attachments)
                    sb.AppendLine(string.Format("<li><a href=\"{0}\">{1}</a></li>", a.Url, a.FileName));
                sb.AppendLine("</ol>");
            }
            else
            {
                sb.AppendLine("<i style=\"color: #808080;\">No attachments found.</i>");
            }
            sb.AppendLine("</div>");

            sb.AppendLine("<hr>");
            sb.AppendLine(string.Format("<div style=\"margin-top: 20px;\"><b><a href=\"{0}\">Approve this IOF</a></b></div>", GetApproveUrl(po)));
            sb.AppendLine(string.Format("<div style=\"margin-top: 20px;\"><b><a href=\"{0}\">Reject this IOF</a></b></div>", GetRejectUrl(po)));

            sb.AppendLine("</div>");

            args.Body = sb.ToString();

            Providers.Email.SendMessage(args);
        }

        private static string GetApproveUrl(PurchaseOrder po)
        {
            string hash = GetHash(string.Format("{0}:{1}:{2}", po.POID, "approve", "go blue wolverines"));
            var result = Providers.Context.Current.GetRequestUrl().GetLeftPart(UriPartial.Authority);
            result += Providers.Context.Current.GetAbsolutePath(string.Format("~/approver/approve/{0}?code={1}", po.POID, hash));
            return result;
        }

        private static string GetRejectUrl(PurchaseOrder po)
        {
            string hash = GetHash(string.Format("{0}:{1}:{2}", po.POID, "reject", "go blue wolverines"));
            var result = Providers.Context.Current.GetRequestUrl().GetLeftPart(UriPartial.Authority);
            result += Providers.Context.Current.GetAbsolutePath(string.Format("~/approver/reject/{0}?code={1}", po.POID, hash));
            return result;
        }

        private static string GetHash(string input)
        {
            var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
