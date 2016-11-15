
using MetroOverhaul.Detours;
using MetroOverhaul.OptionsFramework.Attibutes;

namespace MetroOverhaul
{
    [Options("MetroOverhaul")]
    public class Options
    {
        private const string WIP = "WIP features";
        private const string STYLES = "Additional styles";
        public Options()
        {
#if DEBUG
            steelTracks = false;
            steelTracksNoBar = false;
#endif
            concreteTracksNoBar = true;
            improvedPassengerTrainAi = true;
            improvedMetroTrainAi = true;
            metroUi = true;
            replaceExistingNetworks = false;
        }
        [Checkbox("Concrete tracks (no fence)", STYLES)]
        public bool concreteTracksNoBar { set; get; }

        [Checkbox("Metro track customization UI (requires reloading from main menu)")]
        public bool metroUi { set; get; }
        [Checkbox("Improved PassengerTrainAI (Allows trains to return to depots)", null, nameof(PassengerTrainAIDetour), nameof(PassengerTrainAIDetour.ChangeDeployState))]
        public bool improvedPassengerTrainAi { set; get; }
        [Checkbox("Improved MetroTrainAI (Allows trains to properly spawn at surface)", null, nameof(MetroTrainAIDetour), nameof(MetroTrainAIDetour.ChangeDeployState))]
        public bool improvedMetroTrainAi { set; get; }
        [Checkbox("Replace vanilla metro tracks with MOM tracks", WIP)]
        public bool replaceExistingNetworks { set; get; }

#if DEBUG
        [Checkbox("Steel tracks", STYLES)]
        public bool steelTracks { set; get; }
        [Checkbox("Steel tracks (no fence)", STYLES)]
        public bool steelTracksNoBar { set; get; }
#endif
    }
}