using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Xml;
using System.Diagnostics;

namespace ScriptSQLiteDB
{
    class Program
    {
        public static string SettingsLocation = "Settings.xml";
        public static string DatabaseLocation;

        static void Main(string[] args)
        {
            try
            {
                CommandArgProcessor cmdArgPrc = new CommandArgProcessor();
                ExecutingArgumentsResponse response = ExecutingArgumentsResponse.NoError;
                //If program arguments use them
                if (args.Length > 0)
                {
                    cmdArgPrc.ParseArguments(args);
                    response = cmdArgPrc.ExecuteArguments();
                }

                switch (response)
                {
                    case ExecutingArgumentsResponse.NoError:
                        Execute();
                        break;
                    case ExecutingArgumentsResponse.ErrorExecutingArguments:
                        Console.Write("Error processing arguments.\n");
                        ShowHelp();
                        break;
                    case ExecutingArgumentsResponse.ShowHelp:
                        ShowHelp();
                        break;
                    default:
                        throw new NotImplementedException();
                }

            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                }
                else
                    Console.WriteLine("Something went wrong, please attach a debugger or email me at a@archdev.org");
            }
        }

        private static void ShowHelp()
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.Append("*****************************************************************\n");
            helpMessage.Append("ScriptSQLiteDB\n\n");
            helpMessage.Append("Console application written in C# to go through\n");
            helpMessage.Append("the tables in a SQLite database and script the\n");
            helpMessage.Append("data in them into .sql files.\n\n");
            helpMessage.Append("Written by Matthew Jones (a@archdev.org)\n");
            helpMessage.Append("*****************************************************************\n\n");

            foreach (Option opt in Options.OptionList)
            {
                string formatParameters = opt.HasParameter ? " [parameter]\t" : "\t\t";
                helpMessage.Append("-" + opt.Name + formatParameters + opt.Description + "\n");
                helpMessage.Append("\t\t" + opt.Example + "\n\n");
            }
            Console.Write(helpMessage.ToString());
        }

        private static void Execute()
        {
            ReadSettings();
            ConvertDatabaseToSqlScripts();
        }

        private static void ConvertDatabaseToSqlScripts()
        {
            
        }

        private static void ReadSettings()
        {
            try
            {
                using (XmlTextReader xmlr = new XmlTextReader(SettingsLocation))
                {
                    while (xmlr.Read())
                    {
                        switch (xmlr.NodeType)
                        {
                            case XmlNodeType.Element:
                                break;
                            case XmlNodeType.Text:
                                break;
                            case XmlNodeType.EndElement:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                }
                else
                {
                    Console.WriteLine("Error reading the settings file");
                }
            }
        }

        internal static void SetSettingsLocation(Option arg)
        {
            SettingsLocation = arg.Parameter;
        }

        internal static void SetDatabaseLocation(Option arg)
        {
            DatabaseLocation = arg.Parameter;
        }
    }
}
