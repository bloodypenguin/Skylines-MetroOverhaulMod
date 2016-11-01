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
            steelTracksNoBar = false;
            concreteTracksNoBar = false;
        }

        [Checkbox("Steel tracks", null, null, WIP)]
        public bool steelTracks { set; get; }
        [Checkbox("Steel tracks (no bar)", null, null, WIP)]
        public bool steelTracksNoBar { set; get; }
        [Checkbox("Concrete tracks (no bar)", null, null, WIP)]
        public bool concreteTracksNoBar { set; get; }

        [XmlIgnore]
        public string FileName => "CSL-MetroOverhaul.xml";
    }
}