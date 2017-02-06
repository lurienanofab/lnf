using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Reflection;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Data;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using LNF;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.CommonTools;

namespace LNF.Scripting
{
    public class Engine
    {
        delegate void EchoDelegate(string text);
        delegate IEnumerable QueryDelegate(string query);
        delegate IEnumerable SqlQueryDelegate(string query);
        delegate IEnumerable SetDataDelegate(object data = null, string key = "default");
        delegate void SetHeaderDelegate(string name, string text = null, string type = null, string key = "default");
        delegate void SetHtmlDelegate(string text);
        delegate string CreateTagDelegate(string tagName, IDictionary<object, object> attributes = null, string innerHtml = null);
        delegate object GetParameterValueDelegate(string key, object defval = null);
        delegate PythonDictionary SendMailDelegate(string from, string to, string subject, string body, bool isHtml = true);
        delegate void ExecuteFeedDelegate(string alias, IDictionary<object, object> parameters = null);

        private ScriptEngine engine;
        private ScriptScope scope;
        private Result _Result;
        private Parameters _Parameters;
        private Exception _LastException;

        public IList<Include> Includes { get; set; }
        public Parameters Parameters { get { return _Parameters; } }
        public Exception LastException { get { return _LastException; } }

        public Result Result { get { return _Result; } }

        public Engine()
        {
            Inititialize(new List<Include>());
        }

        public Engine(IList<Include> includes)
        {
            Inititialize(includes);
        }

        public void Inititialize(IList<Include> includes, StringBuilder initScript = null)
        {
            Includes = includes;
            _Parameters = Parameters.Create();

            _Result = new Result();
            engine = Python.CreateEngine();
            scope = engine.Runtime.CreateScope();

            StringBuilder sb = DefaultInitScript();
            if (initScript != null) sb = sb.Append(initScript.ToString());

            try
            {
                engine.CreateScriptSourceFromString(sb.ToString(), SourceCodeKind.Statements).Execute(scope);
            }
            catch (Exception ex)
            {
                _LastException = ex;
                return;
            }

            if (Includes != null)
            {
                ICollection<string> paths = engine.GetSearchPaths();
                foreach (Include inc in Includes)
                {
                    string dir = Path.GetDirectoryName(inc.File.FullName);
                    if (!paths.Contains(dir)) paths.Add(dir);
                }

                engine.SetSearchPaths(paths);

                foreach (Include inc in Includes)
                {
                    if (!string.IsNullOrEmpty(inc.ClassName))
                        engine.CreateScriptSourceFromString(string.Format("import {0}", inc.ClassName), SourceCodeKind.SingleStatement).Execute(scope);
                }
            }

            try
            {
                SetVariable("query", new QueryDelegate(this.Query));
                SetVariable("sqlquery", new SqlQueryDelegate(this.SqlQuery));
                SetVariable("tools", new Tools());
                SetVariable("echo", new EchoDelegate(this.Echo));
                SetVariable("html", new SetHtmlDelegate(this.SetHtml));
                SetVariable("tag", new CreateTagDelegate(this.CreateTag));
                SetVariable("data", new SetDataDelegate(this.SetData));
                SetVariable("feed", new ExecuteFeedDelegate(this.ExecuteFeed));
                SetVariable("header", new SetHeaderDelegate(this.SetHeader));
                SetVariable("mail", new SendMailDelegate(this.SendMail));
                SetVariable("title", new EchoDelegate(this.SetTitle));
                SetVariable("err", new EchoDelegate(this.SetError));
                SetVariable("param", new GetParameterValueDelegate(this.GetParameterValue));
                SetVariable("nl", Environment.NewLine);
            }
            catch (Exception ex)
            {
                _LastException = ex;
                return;
            }
        }

        public object GetParameterValue(string key, object defval = null)
        {
            if (Parameters == null)
                return defval;

            object result = Parameters[key];

            if (result == null) return defval;
            else return result;
        }

        public void Run(string script, IDictionary<object, object> parameters, bool resetResult = true)
        {
            _LastException = null;

            if (resetResult)
                _Result = new Result();

            _Result.Exception = null;

            Parameters.Merge(parameters);

            _Result.Title = Parameters.Replace(_Result.Title);

            try
            {
                ScriptSource source = engine.CreateScriptSourceFromString(Parameters.Replace(script), SourceCodeKind.Statements);
                source.Execute(scope);
            }
            catch (Exception ex)
            {
                _Result.Exception = ex;
            }
        }

        public void SetError(string text)
        {
            _Result.Exception = new Exception(text);
        }

        public void SetTitle(string title)
        {
            _Result.Title = title;
        }

        public void SetVariable(string name, object action)
        {
            scope.SetVariable(name, action);
        }

        public void Echo(string text)
        {
            _Result.Buffer.AppendLine(text);
        }

        public IEnumerable SqlQuery(string query)
        {
            return Repository.SqlQuery(query, Parameters);
        }

        public IEnumerable Query(string query)
        {
            return Repository.Query(query, Parameters);
        }

        public void ExecuteFeed(string alias, IDictionary<object, object> parameters = null)
        {
            DataFeed feed = DA.Current.Query<DataFeed>().FirstOrDefault(x => x.FeedAlias == alias);
            if (feed != null)
                ScriptingContext.Engine.Run(feed.FeedQuery, parameters, false);
        }

        public IEnumerable SetData(object data = null, string key = "default")
        {
            if (data != null)
            {
                ResultUtility.SetData(_Result, data, key);
                IEnumerable list = data as IEnumerable;
                if (list == null) list = new object[] { data };
                return list;
            }
            else
            { 
                return ResultUtility.GetData(_Result, key);
            }
        }

        public void SetHeader(string name, string text = null, string type = null, string key = "default")
        {
            ResultUtility.SetHeader(_Result, name, text, type, key);
        }

        public void SetHtml(string text)
        {
            _Result.Html.AppendLine(text);
        }

        public string CreateTag(string tagName, IDictionary<object, object> attributes = null, string innerHtml = null)
        {
            string result = ResultUtility.CreateTag(_Result, tagName, attributes, innerHtml);
            return result;
        }

        public PythonDictionary SendMail(string from, string to, string subject, string body, bool isHtml = true)
        {
            PythonDictionary result = new PythonDictionary();

            try
            {
                SmtpClient client = new SmtpClient("127.0.0.1");
                MailMessage mm = new MailMessage(from, to, subject, body);
                mm.IsBodyHtml = isHtml;
                client.Send(mm);
                result.Add("success", true);
                result.Add("message", string.Empty);
            }
            catch (Exception ex)
            {
                result.Add("success", false);
                result.Add("message", ex.Message);
            }

            return result;
        }

        public StringBuilder DefaultInitScript()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("import clr");
            //sb.AppendLine("clr.AddReference('NHibernate')");
            sb.AppendLine("clr.AddReference('LNF')");
            //sb.AppendLine("import NHibernate");
            //sb.AppendLine("import NHibernate.Criterion");
            sb.AppendLine("import LNF.Repository");
            sb.AppendLine("import LNF.Repository.Data");
            //sb.AppendLine("import LNF.Repository.Scheduler");
            //sb.AppendLine("import LNF.Scripting.Entities");
            //sb.AppendLine("import LNF.Scripting.Entities.Scheduler");
            //sb.AppendLine("from NHibernate.Criterion import *");
            //sb.AppendLine("from NHibernate import *");
            //sb.AppendLine("from LNF.Repository import *");
            sb.AppendLine("from LNF.Repository.Data import *");
            //sb.AppendLine("from LNF.Repository.Scheduler import *");
            //sb.AppendLine("from LNF.Scripting.Entities import *");
            //sb.AppendLine("from LNF.Scripting.Entities.Scheduler import *");
            sb.AppendLine("def iif(condition, t, f=None):");
            sb.AppendLine("  if condition:");
            sb.AppendLine("    return t");
            sb.AppendLine("  return f");
            return sb;
        }
    }
}