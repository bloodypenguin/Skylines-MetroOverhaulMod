using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using MetroOverhaul.OptionsFramework;
using UnityEngine;

namespace MetroOverhaul.UI {
    public class NoPrefabHookUI : MetroBaseUI
    {
        private bool m_IsActivated = false;
        public override void Update()
        {
            if (!Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
            {
                isVisible = true;
            }
        }

        protected override void CreateUI()
        {
            backgroundSprite = "GenericPanel";
            color = new Color32(68, 84, 68, 170);
            width = 200;
            height = 300;
            opacity = 60;
            position = Vector3.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Prefab Hook Required");
            UILabel mainLabel = AddUIComponent<UILabel>();
            mainLabel.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
            mainLabel.atlas = atlas;
            mainLabel.bringTooltipToFront = true;
            mainLabel.cachedName = "Message";
            mainLabel.isInteractive = true;
            mainLabel.text = "The Mod PrefabHook is Required for this option\n<a href=\"http://www.cnn.com/\">here</a>";
            mainLabel.wordWrap = true;

            UIButton goToModPage = CreateButton("Cancel", 1, (c, u) =>
                {
                    OptionsWrapper<Options>.Options.ingameTrainMetroConverter = false;
                    isVisible = false;
                });
        }
    }
}
