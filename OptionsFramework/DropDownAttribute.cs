using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace MetroOverhaul.OptionsFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DropDownAttribute : AbstractOptionsAttribute
    {
        public DropDownAttribute(string description, string itemsClass, string actionClass, string actionMethod, string group = null) : base(description, group)
        {
            ActionClass = actionClass;
            ActionMethod = actionMethod;
            ItemsClass = itemsClass;
        }

        public DropDownAttribute(string description, string itemsClass, string group = null) : base(description, group)
        {
            ActionClass = null;
            ActionMethod = null;
            ItemsClass = itemsClass;
        }

        public IList<KeyValuePair<string, int>> Items
        {
            get
            {
                var type = Util.FindType(ItemsClass);
                var enumValues = Enum.GetValues(type);
                var items = new List<KeyValuePair<string, int>>();
                foreach (var enumValue in enumValues)
                {
                    var code = (int)enumValue;
                    var memInfo = type.GetMember(Enum.GetName(type, enumValue));
                    var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),
                        false);
                    var description = ((DescriptionAttribute)attributes[0]).Description;
                    items.Add(new KeyValuePair<string, int>(description, code));
                }
                return items;
            }
        }


        public Action<int> Action
        {
            get
            {
                if (ActionClass == null || ActionMethod == null)
                {
                    return null;
                }
                var method = Util.FindType(ActionClass).GetMethod(ActionMethod, BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                {
                    return null;
                }
                return i =>
                {
                    method.Invoke(null, new object[] { i });
                };
            }
        }

        private string ActionClass { get; }

        private string ActionMethod { get; }

        private string ItemsClass { get; }
    }
}