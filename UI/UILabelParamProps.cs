using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class UILabelParamProps : UIParamPropsBase
    {
        public Color32 TextColor; 
        public UIHorizontalAlignment TextAlignment;
        public float TextScale; 
        public UILabelParamProps()
        {
            Margins = new Vector2(8, 4);
            TextColor = new Color32(186, 186, 186, 255);
            TextAlignment = UIHorizontalAlignment.Center;
            TextScale = 0.85f;
        }
    }
}
