using System;
using System.Reflection;

namespace MetroOverhaul.OptionsFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TextFieldAttribute : AbstractOptionsAttribute
    {
        public TextFieldAttribute(string description, string actionClass, string actionMethod, string group = null) : base(description, group)
        {
            ActionClass = actionClass;
            ActionMethod = actionMethod;
        }

        public TextFieldAttribute(string description, string group = null) : base(description, group)
        {
            ActionClass = null;
            ActionMethod = null;
        }

        public Action<string> Action
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
                return s =>
                {
                    method.Invoke(null, new object[] {s});
                };
            }
        }

        private string ActionClass { get; }

        private string ActionMethod { get; }
    }
}