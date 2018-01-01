using System;

namespace MetroOverhaul.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SliderAttribute : AbstractOptionsAttribute
    {
        public SliderAttribute(string description, float min, float max, float step, string group = null, string actionClass = null, string actionMethod = null) : base(description, group, actionClass, actionMethod)
        {
            Min = min;
            Max = max;
            Step = step;
        }

        public float Min { get; private set;  }

        public float Max { get; private set; }

        public float Step { get; private set; }
    }
}
