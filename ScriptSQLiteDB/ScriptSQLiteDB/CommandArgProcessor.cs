using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptSQLiteDB
{
    public class CommandArgProcessor
    {
        private List<Option> _cmdArgs;

        public List<Option> CmdArgs
        {
            get
            {
                if (_cmdArgs == null)
                    _cmdArgs = new List<Option>();
                return _cmdArgs;
            }
            set
            {
                _cmdArgs = value;
            }
        }

        /// <summary>
        /// Parses through command line arguements, creates a command argument object for each
        /// option parameter pair.
        /// </summary>
        /// <param name="args"></param>
        internal void ParseArguments(string[] args)
        {
            bool createNewCmdArg = true;
            Option cmdArg = null;

            foreach (string arg in args)
            {
                string argLower = arg.ToLower();

                if (createNewCmdArg)
                    cmdArg = new Option();
                //option
                if (argLower.StartsWith("-"))
                {
                    cmdArg.Name = argLower.Replace("-", "");

                    if (Options.OptionList.Any(o => o.Name == cmdArg.Name && !o.HasParameter))
                    {
                        cmdArg.HasParameter = false;
                        CmdArgs.Add(cmdArg);
                        createNewCmdArg = true;
                    }
                    else
                    {
                        cmdArg.HasParameter = true;
                        createNewCmdArg = false;
                    }
                }
                //parameter
                else
                {
                    cmdArg.Parameter = argLower;
                    CmdArgs.Add(cmdArg);
                    createNewCmdArg = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0 - Changes applied successfully, continue with normal execution
        /// 1 - Showing help
        /// 2 - Error executing arguments
        /// </returns>
        internal ExecutingArgumentsResponse ExecuteArguments()
        {
            ExecutingArgumentsResponse response = ExecutingArgumentsResponse.NoError;

            //If command argument name is null/empty/whitespace or the command argument has a parameter 
            //and that parameter is null/empty/whitespace then return ErrorExecutingArguments
            if (CmdArgs.Any(cmd => string.IsNullOrWhiteSpace(cmd.Name) ||
                (string.IsNullOrWhiteSpace(cmd.Parameter) && cmd.HasParameter)))
                return ExecutingArgumentsResponse.ErrorExecutingArguments;

            if (CmdArgs.Any(cmd => cmd.Name == "help"))
                return ExecutingArgumentsResponse.ShowHelp;

            foreach (Option arg in CmdArgs)
            {
                switch(arg.Name)
                {
                    case "s":
                        Program.SetSettingsLocation(arg);
                        break;
                    case "d":
                        Program.SetDatabaseLocation(arg);
                        break;
                    default:
                        response = ExecutingArgumentsResponse.ErrorExecutingArguments;
                        break;
                }
            }
            return response;
        }
    }

    public enum ExecutingArgumentsResponse
    {
        NoError,
        ShowHelp,
        ErrorExecutingArguments
    }
}
