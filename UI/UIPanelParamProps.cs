using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class UIPanelParamProps : UIParamPropsBase
    {
        public bool AddUIComponent;
        public string BackgroundSprite;
        public Color32? Color = null;
        public UIPanelParamProps()
        {
            Margins = new Vector4(0, 0);
        }
    }
}
