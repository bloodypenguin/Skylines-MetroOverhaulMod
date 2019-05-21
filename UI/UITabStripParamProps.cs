using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class UITabstripParamProps : UIParamPropsBase
    {
        public int StartSelectedIndex;
        public UITabstripParamProps()
        {
            Margins = new Vector4(0, 0);
        }
    }
}
