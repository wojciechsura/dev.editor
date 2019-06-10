using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Configuration
{
    public abstract class BaseItemCollection : BaseItem
    {
        // Public methods -----------------------------------------------------

        public BaseItemCollection(string xmlName, BaseItemContainer parent)
            : base(xmlName, parent)
        {

        }
    }
}
