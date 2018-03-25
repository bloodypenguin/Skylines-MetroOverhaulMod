using ICities;
using MetroOverhaul.OptionsFramework.Extensions;

namespace MetroOverhaul
{
    public class Mod : IUserMod
    {
        public string Name => "Metro Overhaul";
        public string Description => "Brings metro depots, ground and elevated metro tracks";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
        }
    }
}
