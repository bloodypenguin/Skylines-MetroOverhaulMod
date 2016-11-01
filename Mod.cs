using ICities;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul
{
    public class Mod : IUserMod
    {
        public string Name => "Metro Overhaul";
        public string Description => "Metro Overhaul";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
        }
    }
}
