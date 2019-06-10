using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Configuration
{
    public class BaseObject
    {
        private readonly string xmlName;

        public BaseObject(string xmlName)
        {
            this.xmlName = xmlName;
        }

        public string XmlName
        {
            get
            {
                return xmlName;
            }
        }
    }
}
