using ICities;
using MetroOverhaul.OptionsFramework.Extensions;

namespace MetroOverhaul
{
    public class Mod : IUserMod
    {
#if IS_PATCH
        public const bool isPatch = true;
#else
        public const bool isPatch = false;
#endif

        public string Name => "Metro Overhaul" + (isPatch ? " [Patched]" : "");
        public string Description => "Brings metro depots, ground and elevated metro tracks";

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Options>();
        }
    }
}
