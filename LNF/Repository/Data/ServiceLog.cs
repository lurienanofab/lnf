using System;
using System.Xml.Linq;

namespace LNF.Repository.Data
{
    /// <summary>
    /// A log entry recorded by a service application
    /// </summary>
    public class ServiceLog : IDataItem
    {
        private XElement _data;

        /// <summary>
        /// The unique id of a ServiceLog
        /// </summary>
        public virtual int ServiceLogID { get; set; }

        /// <summary>
        /// The service that made this log entry
        /// </summary>
        public virtual string ServiceName { get; set; }

        /// <summary>
        /// The date and time this log entry was made
        /// </summary>
        public virtual DateTime LogDateTime { get; set; }

        /// <summary>
        /// The subject of this log entry
        /// </summary>
        public virtual string LogSubject { get; set; }

        /// <summary>
        /// The log level
        /// </summary>
        public virtual string LogLevel { get; set; }

        /// <summary>
        /// The log message
        /// </summary>
        public virtual string LogMessage { get; set; }

        /// <summary>
        /// A unique message id
        /// </summary>
        public virtual Guid? MessageID { get; set; }

        /// <summary>
        /// The XML data associated with the log entry
        /// </summary>
        public virtual string Data
        {
            get
            {
                return _data.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _data = XElement.Parse("<root/>");
                else
                    _data = XElement.Parse(value);
            }
        }

        /// <summary>
        /// Instantiate a new ServiceLog object
        /// </summary>
        public ServiceLog()
        {
            SetData(XElement.Parse("<root/>"));
        }

        /// <summary>
        /// Adds a node to Data
        /// </summary>
        /// <param name="message">The message to add - can use standard string format placeholder syntax for formatting for example {0}, {1:C} etc.</param>
        /// <param name="args">Replacement values used to format the message</param>
        public virtual void AppendData(string message, params object[] args)
        {
            XElement node = XElement.Parse("<data/>");
            node.Value = string.Format(message, args);
            _data.Add(node);
            Data = _data.ToString();
        }

        /// <summary>
        /// Returns Data as an XElement object
        /// </summary>
        /// <returns></returns>
        public virtual XElement GetData()
        {
            return _data;
        }

        /// <summary>
        /// Sets Data using an XElement object
        /// </summary>
        /// <param name="value">An XElement object</param>
        public virtual void SetData(XElement value)
        {
            _data = value;
            Data = _data.ToString();
        }
    }


}
