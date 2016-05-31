using System.Configuration;

namespace DoddleReport.Configuration
{
    [ConfigurationCollection(typeof(StyleElement), CollectionType = ConfigurationElementCollectionType.BasicMap, AddItemName="style")]
    public class StyleElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new StyleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StyleElement)element).Name;
        }

        public StyleElement this[int index]
        {
            get
            {
                return (StyleElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new StyleElement this[string name]
        {
            get
            {
                return (StyleElement)BaseGet(name) ?? new StyleElement();
            }
        }

        public StyleElement this[ReportRowType rowType]
        {
            get
            {
                return this[rowType.ToString()];
            }
        }

    }
}