using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ScriptSQLiteDB
{
    [Serializable()]
    [XmlRoot("Settings")]
    public class Settings
    {
        [XmlElement("DatabaseName")]
        public string DatabaseName { get; set; }

        [XmlElement("DatabaseLocation")]
        public string DatabaseLocation { get; set; }
    }
}
