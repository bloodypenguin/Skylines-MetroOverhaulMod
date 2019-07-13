using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.UI
{
    class StationTypeSelectorTooltip : UIPanel
    {
        public override void Start()
        {
            backgroundSprite = "GenericPanel";
            name = "StationTypeSelectorTooltip";
            color = new Color32(73, 68, 84, 170);
            width = 200;
            height = 100;
            opacity = 100;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };
            var tf = AddUIComponent<UITextField>();
            tf.text = "THIS IS A TEST FOOL!";
        }
    }
}
