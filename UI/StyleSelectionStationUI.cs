using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.UI
{
    class StyleSelectionStationUI : StyleSelectionUI
    {
        public StyleSelectionStationUI()
        {
            this.window = new Rect((float)(Screen.width - 400), (float)(Screen.height - 300), 300f, 220f);
            this.fence = false;
        }
        protected override void OnGUI()
        {
            if (!this.showWindow)
            {
                return;
            }
            this.window = GUI.Window(29292929, this.window, Window, "Metro Track Style");
            if (binfo != null)
            {
                this.window.height = 65;
            }
            else
            {
                this.window.height = 220;
            }
        }
    }
}
