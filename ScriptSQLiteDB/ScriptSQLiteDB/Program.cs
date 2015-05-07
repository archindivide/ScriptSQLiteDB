using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace ScriptSQLiteDB
{
    class Program
    {
        public static string settingsLocation = "Settings.xml";
        public static Settings settings;
        public static string[] sqliteSurroundInQuotesTypenames = {
                                                         "CHARACTER",
                                                         "VARCHAR",
                                                         "VARYING CHARACTER",
                                                         "NCHAR",
                                                         "NATIVE CHARACTER",
                                                         "NVARCHAR",
                                                         "TEXT",
                                                         "CLOB",
                                                         "DATETIME",
                                                         "DATE"
                                                     };

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
            try
            {
                using (SQLiteConnection sqlconn = new SQLiteConnection("Data Source=" + settings.DatabaseLocation + settings.DatabaseName))
                {
                    List<string> tableNames = new List<string>();
                    sqlconn.Open();
                    SQLiteCommand sqlcmd = new SQLiteCommand(sqlconn);
                    sqlcmd.CommandText = "SELECT tbl_name FROM sqlite_master WHERE type = 'table'";
                    SQLiteDataReader reader = sqlcmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tableNames.Add(reader[0].ToString());
                    }
                    reader.Close();

                    foreach (string table in tableNames)
                    {
                        StringBuilder sqlScript = new StringBuilder();
                        //Dictionary containing string for the column name and bool for if the column is a text column
                        Dictionary<string, bool> colNames = new Dictionary<string, bool>();
                        sqlcmd.CommandText = "PRAGMA table_info(" + table + ")";
                        reader = sqlcmd.ExecuteReader();
                        while (reader.Read())
                        {
                            colNames.Add(reader["name"].ToString(),
                                //Test against list of known quoted types in sqlite, matches things like NVARCHAR(50) using starts with
                                sqliteSurroundInQuotesTypenames.Any(s => reader["type"].ToString().ToUpper().StartsWith(s)));
                        }
                        reader.Close();

                        sqlcmd.CommandText = "SELECT * FROM " + table;
                        reader = sqlcmd.ExecuteReader();
                        while (reader.Read())
                        {
                            sqlScript.Append("INSERT INTO " + table + " VALUES (");
                            foreach (var col in colNames)
                            {
                                if (col.Value)
                                    //replace ' in text fields with &#39; (html encoding for ')
                                    sqlScript.Append("'" + reader[col.Key].ToString().Replace("'", "&#39;") + "',");
                                else
                                    sqlScript.Append(reader[col.Key] + ",");
                            }
                            sqlScript.Remove(sqlScript.Length - 1, 1);
                            sqlScript.Append(")\n");
                        }
                        reader.Close();

                        using (StreamWriter output = new StreamWriter(settings.DatabaseLocation + table + ".sql", false))
                        {
                            output.Write(sqlScript.ToString());
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
                    Console.WriteLine("Error reading the database file");
                }
            }
        }

        private static void ReadSettings()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                StreamReader reader = new StreamReader(settingsLocation);
                settings = (Settings)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                }
                else
                {
                    Console.WriteLine("Error deserializing the settings file");
                }
            }
        }

        internal static void SetSettingsLocation(Option arg)
        {
            settingsLocation = arg.Parameter;
        }
    }
}
