using System.Xml.Serialization;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul
{
    public class Options : IModOptions
    {
        private const string WIP = "WIP features";

        public Options()
        {
            steelTracks = false;
            steelTracksNoBars = false;
            concreteTracksNoBars = false;
        }

        [Checkbox("Steel tracks", null, null, WIP)]
        public bool steelTracks { set; get; }
        [Checkbox("Steel tracks (no bars)", null, null, WIP)]
        public bool steelTracksNoBars { set; get; }
        [Checkbox("Concrete tracks (no bars)", null, null, WIP)]
        public bool concreteTracksNoBars { set; get; }

        [XmlIgnore]
        public string FileName => "CSL-MetroOverhaul.xml";
    }
}