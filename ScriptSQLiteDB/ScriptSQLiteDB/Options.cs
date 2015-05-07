using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptSQLiteDB
{
    public static class Options
    {
        private static List<Option> _options;

        public static List<Option> OptionList
        {
            get
            {
                if (_options == null)
                {
                    _options = new List<Option>();

                    _options.Add(new Option()
                    {
                        Name = "s",
                        Description = "Used to define the location of the settings file.",
                        Example = @"Ex. -s C:\Settings.xml",
                        HasParameter = true
                    });

                    _options.Add(new Option()
                    {
                        Name = "help",
                        Description = @"Displays this help section.",
                        HasParameter = false
                    });
                }
                return _options;
            }
            set
            {
                _options = value;
            }
        }
    }

    public class Option
    {
        public string Name { get; set; }
        public string Parameter { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
        public bool HasParameter { get; set; }
    }
}
