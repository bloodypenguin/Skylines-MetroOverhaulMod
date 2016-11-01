using System.Xml.Serialization;

namespace MetroOverhaul.OptionsFramework
{
    public interface IModOptions
    {
        [XmlIgnore]
        string FileName
        {
            get;
        }
    }
}