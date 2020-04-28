using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LNF.Scripting
{
    public class ScriptHost
    {
        public class Command
        {
            public string Name { get; private set; }
            public string Syntax { get; private set; }
            public string[] Arguments { get; private set; }
            public string Example { get; private set; }
            public string HelpSummary { get; private set; }
            public string HelpDetail { get; private set; }
            public Delegate Method { get; private set; }

            private Command() { }

            public static Command Create(string name, string syntax, string[] args, string example, string helpSummary, string helpDetail, Delegate method)
            {
                Command result = new Command
                {
                    Name = name,
                    Syntax = syntax,
                    Arguments = args,
                    Example = example,
                    HelpSummary = helpSummary,
                    HelpDetail = helpDetail,
                    Method = method
                };
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
            protected IDictionary<string, object> Items { get; set; }

            public bool Success { get; set; }
            public string Message { get; set; }
            public IEnumerable Data { get; set; }

            public Result Set(string key, object value)
            {
                if (Items == null)
                    Items = new Dictionary<string, object>();

                if (Items.ContainsKey(key))
                    Items[key] = value;
                else
                    Items.Add(key, value);

                return this;
            }

            public object Get(string key)
            {
                if (Items == null)
                    return null;

                if (Items.ContainsKey(key))
                    return Items[key];

                return null;
            }

            public string[] Keys
            {
                get
                {
                    if (Items == null)
                        return new string[] { };

                    return Items.Keys.ToArray();
                }
            }

            public Result()
            {
                Items = null;

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
        private readonly HostState _State;

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
