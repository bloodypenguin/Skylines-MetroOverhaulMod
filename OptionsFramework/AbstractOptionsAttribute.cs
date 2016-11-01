using System;

namespace MetroOverhaul.OptionsFramework
{
    public abstract class AbstractOptionsAttribute : Attribute 
    {
        protected AbstractOptionsAttribute(string description, string group)
        {
            Description = description;
            Group = group;
        }

        public string Description { get; }
        public string Group { get; }
    }
}