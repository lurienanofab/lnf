using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using LNF.Repository;

namespace LNF.Scripting
{
    public class ScriptHost
    {
        public class Command
        {
            private string _Name;
            private string _Syntax;
            private string[] _Arguments;
            private string _Example;
            private string _HelpSummary;
            private string _HelpDetail;
            private Delegate _Method;

            public string Name { get { return _Name; } }
            public string Syntax { get { return _Syntax; } }
            public string[] Arguments { get { return _Arguments; } }
            public string Example { get { return _Example; } }
            public string HelpSummary { get { return _HelpSummary; } }
            public string HelpDetail { get { return _HelpDetail; } }
            public Delegate Method { get { return _Method; } }

            private Command() { }

            public static Command Create(string name, string syntax, string[] args, string example, string helpSummary, string helpDetail, Delegate method)
            {
                Command result = new Command();
                result._Name = name;
                result._Syntax = syntax;
                result._Arguments = args;
                result._Example = example;
                result._HelpSummary = helpSummary;
                result._HelpDetail = helpDetail;
                result._Method = method;
                return result;
            }
        }

        public class HostState
        {
            public bool Exit { get; set; }

            public HostState()
            {
                Exit = false;
            }
        }

        public class Result : ModelBase
        {
            private IDictionary<string, object> items { get; set; }

            public bool Success { get; set; }
            public string Message { get; set; }
            public IEnumerable Data { get; set; }

            public Result Set(string key, object value)
            {
                if (items == null)
                    items = new Dictionary<string, object>();

                if (items.ContainsKey(key))
                    items[key] = value;
                else
                    items.Add(key, value);

                return this;
            }

            public object Get(string key)
            {
                if (items == null)
                    return null;

                if (items.ContainsKey(key))
                    return items[key];

                return null;
            }

            public string[] Keys
            {
                get
                {
                    if (items == null)
                        return new string[] { };

                    return items.Keys.ToArray();
                }
            }

            public Result()
            {
                items = null;

                Success = false;
                Message = null;
                Data = null;
            }

            public override string ToString()
            {
                return Message;
            }
        }

        private ScriptEngine engine;
        private ScriptScope scope;
        private HostState _State;

        public HostState State { get { return _State; } }

        public ScriptHost()
        {
            engine = Python.CreateEngine();
            scope = engine.Runtime.CreateScope();
            _State = new HostState();
        }

        public void Register(string name, object value)
        {
            scope.SetVariable(name, value);
        }

        public Result Run(string script)
        {
            if (DA.Current == null)
                throw new Exception("DA.Current IS NULL!!!!");

            ScriptSource source = engine.CreateScriptSourceFromString(script, SourceCodeKind.AutoDetect);
            return ExecuteSource(source);
        }

        public void Run(FileInfo file)
        {
            ScriptSource source = engine.CreateScriptSourceFromFile(file.FullName, Encoding.Default, SourceCodeKind.AutoDetect);
            ExecuteSource(source);
        }

        private Result ExecuteSource(ScriptSource source)
        {
            dynamic executeResult = source.Execute(scope);
            if (executeResult == null)
                return null;
            else if (executeResult is Result)
                return executeResult as Result;
            else
            {
                return new Result()
                {
                    Success = true,
                    Message = executeResult.ToString(),
                    Data = null,
                };
            }
        }
    }
}
