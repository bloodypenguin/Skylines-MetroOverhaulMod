using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul.UI {
    class NoPrefabHookHelper {
        public static void ChangeState(bool state)
        {
            if (state && !Util.IsHooked())
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("PrefabHook Missing!", "This feature requires the Prefab Hook mod.  Please subscribe/enable it in order to use this feature.", false);
            }
        }
    }
}
