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
        public bool DisableState;
        public bool SelectOnCreate;
        public UIButtonParamProps()
        {
            DisableState = false;
            Margins = new Vector4(8, 8);
            AddUIComponent = true;
            SelectOnCreate = false;
            Height = 30;
        }
    }
}
