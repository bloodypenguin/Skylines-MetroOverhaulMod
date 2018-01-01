
using MetroOverhaul.Detours;
using MetroOverhaul.OptionsFramework.Attibutes;

namespace MetroOverhaul
{
    [Options("MetroOverhaul")]
    public class Options
    {
        private const string WIP = "WIP features";
        private const string STYLES = "Additional styles";
        private const string GENERAL = "General settings";
        public Options()
        {
            steelTracks = true;
            improvedPassengerTrainAi = true;
            improvedMetroTrainAi = true;
            metroUi = true;
            replaceExistingNetworks = false;
        }
        [Checkbox("Metro track customization UI (requires reloading from main menu)", GENERAL)]
        public bool metroUi { set; get; }

        [Checkbox("Improved PassengerTrainAI (Allows trains to return to depots)", GENERAL, nameof(PassengerTrainAIDetour), nameof(PassengerTrainAIDetour.ChangeDeployState))]
        public bool improvedPassengerTrainAi { set; get; }

        [Checkbox("Improved MetroTrainAI (Allows trains to properly spawn at surface)", GENERAL, nameof(MetroTrainAIDetour), nameof(MetroTrainAIDetour.ChangeDeployState))]
        public bool improvedMetroTrainAi { set; get; }

        [Checkbox("Replace vanilla metro tracks with MOM tracks", WIP)]
        public bool replaceExistingNetworks { set; get; }

        [Checkbox("Steel tracks", STYLES)]
        public bool steelTracks { set; get; }
    }
}