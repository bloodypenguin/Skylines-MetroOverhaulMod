using System.Xml.Serialization;
using MetroOverhaul.Detours;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul
{
    public class Options : IModOptions
    {
        private const string WIP = "WIP features";
        private const string EXPERIMENT = "Experimental features";

        public Options()
        {
#if DEBUG
            steelTracks = false;
            steelTracksNoBar = false;
            concreteTracksNoBar = false;
#endif
            improvedPassengerTrainAi = true;
            improvedMetroTrainAi = true;
            metroUi = true;
        }
        [Checkbox("Metro track customization UI (requires reloading from main menu)", null, null, EXPERIMENT)]
        public bool metroUi { set; get; }
        [Checkbox("Improved PassengerTrainAI (Allows trains to return to depots)", nameof(PassengerTrainAIDetour), nameof(PassengerTrainAIDetour.ChangeDeployState), EXPERIMENT)]
        public bool improvedPassengerTrainAi { set; get; }
        [Checkbox("Improved MetroTrainAI (Allows trains to properly spawn at surface)", nameof(MetroTrainAIDetour), nameof(MetroTrainAIDetour.ChangeDeployState), EXPERIMENT)]
        public bool improvedMetroTrainAi { set; get; }
#if DEBUG
        [Checkbox("Steel tracks", null, null, WIP)]
        public bool steelTracks { set; get; }
        [Checkbox("Steel tracks (no bar)", null, null, WIP)]
        public bool steelTracksNoBar { set; get; }
        [Checkbox("Concrete tracks (no bar)", null, null, WIP)]
        public bool concreteTracksNoBar { set; get; }
#endif
        [XmlIgnore]
        public string FileName => "CSL-MetroOverhaul.xml";
    }
}