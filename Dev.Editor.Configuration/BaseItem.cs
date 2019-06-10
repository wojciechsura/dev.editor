using System;

namespace Dev.Editor.Configuration
{
    public class BaseItem : BaseItemContainer
    {
        // Private fields -----------------------------------------------------

        private readonly BaseItemContainer parent;

        // Public methods -----------------------------------------------------

        public BaseItem(string xmlName, BaseItemContainer parent) 
            : base(xmlName)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            parent.RegisterSubItem(this);
        }

        // Public properties --------------------------------------------------

        public BaseItemContainer Parent
        {
            get
            {
                return parent;
            }
        }
    }
}