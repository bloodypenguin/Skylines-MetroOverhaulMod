using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MetroOverhaul.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DropDownAttribute : AbstractOptionsAttribute
    {
        public DropDownAttribute(string description, Type itemsClass, string group = null, Type actionClass = null, string actionMethod = null) : base(description, group, actionClass, actionMethod)
        {
            ItemsClass = itemsClass;
        }

        public IList<KeyValuePair<string, int>> Items
        {
            get
            {
                var enumValues = Enum.GetValues(ItemsClass);
                return (from object enumValue in enumValues
                        let code = (int)enumValue
                        let memInfo = ItemsClass.GetMember(Enum.GetName(ItemsClass, enumValue))
                        let attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                        let description = ((DescriptionAttribute)attributes[0]).Description
                        select new KeyValuePair<string, int>(description, code)).ToList();
            }
        }

        private Type ItemsClass { get; }
    }
}