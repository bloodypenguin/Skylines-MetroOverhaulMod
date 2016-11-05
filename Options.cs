using System.Xml.Serialization;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul
{
    public class Options : IModOptions
    {
        private const string WIP = "WIP features";
        private const string EXPERIMENT = "Experimental features";

        public Options()
        {
            steelTracks = false;
            steelTracksNoBar = false;
            concreteTracksNoBar = false;
            improvedPassengerTrainAi = false;
            improvedMetroTrainAi = false;
        }

        [Checkbox("Improved PassengerTrainAI (Should allow trains returning to depots)", null, null, EXPERIMENT)]
        public bool improvedPassengerTrainAi { set; get; }
        [Checkbox("Improved MetroTrainAI (So far causes citizens not boarding trains)", null, null, EXPERIMENT)]
        public bool improvedMetroTrainAi { set; get; }
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