using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using UnityEngine;

namespace MetroOverhaul.UI {
    public class MetroBaseUI : UIPanel{
        protected virtual void CreateUI() { }
        protected virtual void SubStart() { }
        public override void Start()
        {
            CreateUI();
            SubStart();
        }
        protected void CreateDragHandle(string title)
        {
            UIPanel dragHandlePanel = AddUIComponent<UIPanel>();
            dragHandlePanel.atlas = atlas;
            dragHandlePanel.backgroundSprite = "GenericPanel";
            dragHandlePanel.width = width;
            dragHandlePanel.height = 20;
            dragHandlePanel.opacity = 100;
            dragHandlePanel.color = new Color32(21, 140, 34, 255);
            dragHandlePanel.relativePosition = Vector3.zero;

            UIDragHandle dragHandle = dragHandlePanel.AddUIComponent<UIDragHandle>();
            dragHandle.width = width;
            dragHandle.height = dragHandle.parent.height;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            UILabel titleLabel = dragHandlePanel.AddUIComponent<UILabel>();
            titleLabel.relativePosition = new Vector3() { x = 5, y = 5, z = 0 };
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.text = title;
            titleLabel.isInteractive = false;
        }

        protected int m_rowIndex = 1;
        private int m_colIndex = 0;
        protected UIButton CreateButton(string text, int columnCount, MouseEventHandler eventClick, bool sameLine = false)
        {
            var button = this.AddUIComponent<UIButton>();
            button.relativePosition = new Vector3(8 + (((float)m_colIndex / columnCount) * width), m_rowIndex * 50);
            button.width = (width / columnCount) - 16;
            button.height = 30;
            button.normalBgSprite = "ButtonMenu";
            button.color = new Color32(150, 150, 150, 255);
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.disabledColor = new Color32(204, 204, 204, 255);
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.hoveredColor = new Color32(163, 255, 16, 255);
            button.focusedBgSprite = "ButtonMenu";
            button.focusedColor = new Color32(163, 255, 16, 255);
            button.pressedBgSprite = "ButtonMenuPressed";
            button.pressedColor = new Color32(163, 255, 16, 255);
            button.textColor = new Color32(255, 255, 255, 255);
            button.normalBgSprite = "ButtonMenu";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.playAudioEvents = true;
            button.opacity = 95;
            button.dropShadowColor = new Color32(0, 0, 0, 255);
            button.dropShadowOffset = new Vector2(-1, -1);
            button.text = text;
            button.eventClick += eventClick;
            m_colIndex++;
            if (m_colIndex == columnCount)
            {
                m_colIndex = 0;
                if (sameLine == false)
                    m_rowIndex++;
            }
            return button;
        }
    }
}
