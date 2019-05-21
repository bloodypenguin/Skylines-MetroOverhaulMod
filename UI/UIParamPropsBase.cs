using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class UIParamPropsBase
    {
        public string Text;
        public int ColumnCount;
        public int ColShare;
        public int ColOffset;
        public MouseEventHandler EventClick;
        public bool SameLine;
        public bool ForceRowEnd;
        public float Width;
        public float Height;
        public string NormalFgSprite;
        public string DisabledFgSprite;
        public string FocusedFgSprite;
        public string HoveredFgSprite;
        public string PressedFgSprite;
        public string NormalBgSprite;
        public string DisabledBgSprite;
        public string FocusedBgSprite;
        public string HoveredBgSprite;
        public string PressedBgSprite;
        public bool HasFgBgSprites;
        public bool StackWidths;
        public UIComponent Component;
        public UIComponent ParentComponent;
        public Vector4 Margins;
        public UITextureAtlas Atlas;
        public string Name;
        public UIParamPropsBase()
        {
            SameLine = false;
            ForceRowEnd = false;
            HasFgBgSprites = false;
            StackWidths = false;
            Margins = new Vector4(8, 8, 0, 0);
            Width = 0;
            Height = 0;
            ColShare = -1;
        }
        public Vector3 GetRelativePositionByStackedWidths(float stackedWidths)
        {
            return new Vector3(Margins.x + stackedWidths, ParentComponent.height + Margins.w);
        }
        public Vector3 GetRelativePositionByColumnCount(int colIndex)
        {
            var retval = new Vector3(Margins.x + (ParentComponent.width * ((float)(colIndex + ColOffset) / ColumnCount)), ParentComponent.height + Margins.w);
            Debug.Log("the set relpos was " + retval.ToString());
            return retval;
        }
        public Vector3 GetRelativePositionByColumnShare(int totalColShare)
        {
            return new Vector3(Margins.x + (ParentComponent.width * ((float)(totalColShare + ColOffset) / 12)), ParentComponent.height + Margins.w);
        }
        public float GetWidth()
        {
            if (ColShare > -1)
            {
                return (float)Math.Floor(ParentComponent.width * (((float)ColShare - ColOffset) / 12) - Margins.x - Margins.z);
            }
            else if (Width == 0)
            {
                return (float)Math.Floor(ParentComponent.width / ColumnCount) - Margins.x - Margins.z;
            }
            return Width;
        }
    }
}
