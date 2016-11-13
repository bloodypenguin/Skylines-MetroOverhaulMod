using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MetroOverhaul.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DropDownAttribute : AbstractOptionsAttribute
    {
        public DropDownAttribute(string description, string itemsClass, string group = null, string actionClass = null, string actionMethod = null) : base(description, group, actionClass, actionMethod)
        {
            ItemsClass = itemsClass;
        }

        public IList<KeyValuePair<string, int>> Items
        {
            get
            {
                var type = Util.FindType(ItemsClass);
                var enumValues = Enum.GetValues(type);
                return (from object enumValue in enumValues
                        let code = (int)enumValue
                        let memInfo = type.GetMember(Enum.GetName(type, enumValue))
                        let attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                        let description = ((DescriptionAttribute)attributes[0]).Description
                        select new KeyValuePair<string, int>(description, code)).ToList();
            }
        }

        private string ItemsClass { get; }
    }
}