using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Dev.Editor.Configuration
{
    public abstract class BaseTypedValue<T> : BaseValue, INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private T value;
        private readonly T defaultValue;
        private readonly XmlStoragePlace xmlStoragePlace;

        // Private methods ----------------------------------------------------

        private void SaveToAttribute(XmlNode rootNode)
        {
            if (Value != null)
            {
                XmlAttribute attr = rootNode.OwnerDocument.CreateAttribute(XmlName);
                attr.InnerText = SerializeValue(Value);
                rootNode.Attributes.Append(attr);
            }
        }

        private void LoadFromAttribute(XmlNode rootNode)
        {
            var attr = rootNode.Attributes[XmlName];
            InternalLoadFromNode(attr);
        }

        private void SaveToSubnode(XmlNode rootNode)
        {
            if (Value != null)
            {
                XmlElement element = rootNode.OwnerDocument.CreateElement(XmlName);
                element.InnerText = Value.ToString();
                rootNode.AppendChild(element);
            }
        }

        private void LoadFromSubnode(XmlNode rootNode)
        {
            var element = rootNode[XmlName];
            InternalLoadFromNode(element);
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected abstract string SerializeValue(T value);

        protected abstract T DeserializeValue(string text);

        protected void InternalLoadFromNode(XmlNode node)
        {
            if (node != null)
            {
                try {
                    Value = DeserializeValue(node.InnerText);
                }
                catch
                {
                    Value = defaultValue;
                }
            }
            else
            {
                Value = defaultValue;
            }
        }

        protected override void InternalLoad(XmlNode rootNode)
        {
            switch (xmlStoragePlace)
            {
                case XmlStoragePlace.Attribute:
                    {
                        LoadFromAttribute(rootNode);
                        break;
                    }
                case XmlStoragePlace.Subnode:
                    {
                        LoadFromSubnode(rootNode);
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported XML storage place!");
            }
        }

        protected override void InternalSave(XmlNode rootNode)
        {
            switch (xmlStoragePlace)
            {
                case XmlStoragePlace.Attribute:
                    {
                        SaveToAttribute(rootNode);
                        break;
                    }
                case XmlStoragePlace.Subnode:
                    {
                        SaveToSubnode(rootNode);
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported XML storage place!");
            }
        }

        // Public methods -----------------------------------------------------

        public BaseTypedValue(string xmlName, BaseItemContainer owner, T defaultValue = default(T), XmlStoragePlace xmlStoragePlace = XmlStoragePlace.Subnode)
            : base(xmlName, owner)
        {
            this.xmlStoragePlace = xmlStoragePlace;
            this.defaultValue = defaultValue;
            this.value = defaultValue;
        }

        public override sealed void SetDefaults()
        {
            Value = defaultValue;
        }

        // Public properties --------------------------------------------------

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!object.Equals(this.value, value))
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
