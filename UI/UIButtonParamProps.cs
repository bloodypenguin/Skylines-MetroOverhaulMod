using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class UIButtonParamProps : UIParamPropsBase
    {
        public bool AddUIComponent;
        public UIButtonParamProps()
        {
            Margins = new Vector4(8, 20);
            AddUIComponent = true;
            Height = 30;
        }
    }
}
