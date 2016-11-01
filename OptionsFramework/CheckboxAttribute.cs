using System;
using System.Reflection;

namespace MetroOverhaul.OptionsFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckboxAttribute : AbstractOptionsAttribute
    {

        public CheckboxAttribute(string description, string actionClass, string actionMethod, string group = null) : base(description, group)
        {
            ActionClass = actionClass;
            ActionMethod = actionMethod;
        }

        public CheckboxAttribute(string description, string group = null) : base(description, group)
        {
            ActionClass = null;
            ActionMethod = null;
        }

        public Action<bool> Action
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
                return b =>
                {
                    method.Invoke(null, new object[] { b });
                };
            }
        }

        private string ActionClass { get; }

        private string ActionMethod { get; }
    }
}