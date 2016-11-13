using System;

namespace MetroOverhaul.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class OptionsAttribute : Attribute
    {
        public OptionsAttribute(string fileName, string legacyFileName = "")
        {
            FileName = fileName;
            LegacyFileName = legacyFileName;
        }

        //file name in local app data
        public string FileName { get; }

        //file name in Cities: Skylines folder
        public string LegacyFileName { get; }
    }
}