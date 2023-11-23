using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Assets
{
    public class ConfigurationReaderV2
    {
        public class Statement
        {
            private int commandIndex = 0;
            public string[] Commands;
            public Statement(string[] commands)
            {
                this.Commands = commands;
            }
            public string GetCurrentCommand()
            {
                if ((this.Commands.Length - 1) < this.commandIndex) return null;
                string res = this.Commands[commandIndex];
                commandIndex++;
                return res;
            }
        }
        public class ConfigReader
        {
            private object CurrentObject;
            /// <summary>
            /// Stores all local variables;
            /// </summary>
            private IDictionary<string, object> InternalValues;
            /// <summary>
            /// The path to the configuration file
            /// </summary>
            private readonly string ConfigPath;
            public ConfigReader(string path)
            {
                this.InternalValues = new Dictionary<string, object>();
                this.ConfigPath = path;
            }
            public object Read(object caller)
            {
                string[][] stringStatements;
                Statement[] statements;
                using (StreamReader r = new StreamReader(this.ConfigPath))
                {
                    stringStatements = r.ReadToEnd().Replace("\n", "").Replace("\r", "").Split(';').Where(e => !e.StartsWith('#')).Select(e => e.Split(' ')).ToArray();
                    //stringStatements = (from s1 in (from s in r.ReadToEnd().Replace("\n", "").Replace("\r", "").Split(';') where !s.StartsWith("#") select s) select s1.Split(' ')).ToArray();
                };

                statements = (from strStatement in stringStatements select new Statement(strStatement)).ToArray();
                foreach (Statement statement in statements)
                {
                    switch (statement.GetCurrentCommand())
                    {
                        case "obj":
                            Command_Obj(statement, out string varName, out object v);
                            this.InternalValues[varName] = v;
                            this.CurrentObject = v;
                            break;
                        case "prop":
                            string propName = statement.GetCurrentCommand();
                            string methodOrValue = statement.GetCurrentCommand();

                            // If () exists, this is a method call so we must pass it
                            if(methodOrValue.Contains("()"))
                            {
                                // Strip away () so we get the method name
                                methodOrValue = methodOrValue.Replace("()", "");

                                // Obtain the method
                                MethodInfo met = this.CurrentObject.GetType().GetMethod(methodOrValue);

                                List<object> parameters = new List<object>();
                                while(statement.GetCurrentCommand() == "obj") 
                                {
                                    Command_Obj(statement, out string ignored, out object val); 
                                    parameters.Add(val); 
                                }
                                met.Invoke(this.CurrentObject, parameters.ToArray());
                            }
                            break;
                    }
                }

                return null;
            }

            private bool Parse_Parameters(Statement statement, out object[] parameters)
            {
                List<object> 
            }

            private bool Command_Obj(Statement statement, out string varName, out object value)
            {
                object v;
                varName = statement.GetCurrentCommand();
                string typeName = statement.GetCurrentCommand();

                if (typeName is null)
                {
                    if (!this.InternalValues.TryGetValue(varName, out v))
                    {
                        value = null; 
                        return false;
                    }
                }
                else
                {
                    Type t = Type.GetType(typeName);
                    v = Activator.CreateInstance(t);
                }

                value = v;

                return true;
            }
        }
    }
}
