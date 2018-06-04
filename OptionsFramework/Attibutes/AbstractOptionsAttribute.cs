using System;
using System.Reflection;

namespace MetroOverhaul.OptionsFramework.Attibutes
{
    public abstract class AbstractOptionsAttribute : Attribute
    {
        protected AbstractOptionsAttribute(string description, string group, Type actionClass, string actionMethod)
        {
            Description = description;
            Group = group;
            ActionClass = actionClass;
            ActionMethod = actionMethod;
        }

        public string Description { get; }
        public string Group { get; }

        public Action<T> Action<T>()
        {
            if (ActionClass == null || ActionMethod == null)
            {
                return s => { };
            }
            var method = ActionClass.GetMethod(ActionMethod, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return s => { };
            }
            return s =>
            {
                method.Invoke(null, new object[] { s });
            };
        }

        private Type ActionClass { get; }

        private string ActionMethod { get; }


    }
}