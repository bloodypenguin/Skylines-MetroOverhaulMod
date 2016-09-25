using ColossalFramework.UI;
using ICities;
using System;
using MetroOverhaul;
using UnityEngine;
using ColossalFramework.Threading;

namespace UIMod
{
    public class MetroUI : IUserMod
    {
        public string Description { get { return "MetroUI"; } }
        public string Name { get { return "MetroUI"; } }
    }

    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            UIView v = UIView.GetAView();
            UIComponent uic = v.AddUIComponent(typeof(MetroStationCustomizer));
        }
    }

    public class MetroStationCustomizer : UIPanel
    {
        private float m_setDepth = 12;
        private float m_setLength = 88;
        private UITextField m_lengthTextbox = new UITextField();
        private UITextField m_depthTextbox = new UITextField();
        public override void Start()
        {
            Action stationMechanicsTask = DoStationMechanics;
            Task t = Task.Create(stationMechanicsTask);
            backgroundSprite = "GenericPanel";
            color = new UnityEngine.Color32(255, 100, 94, 190);
            width = 200;
            height = 100;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.size = size;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;


            UILabel titleLabel = AddUIComponent<UILabel>();
            titleLabel.relativePosition = new UnityEngine.Vector3() { x = 0, y = 0, z = 0 };
            titleLabel.text = "Station Attributes";
            titleLabel.isInteractive = false;

            UIPanel lengthSliderPanel = AddUIComponent<UIPanel>();
            lengthSliderPanel.atlas = atlas;
            lengthSliderPanel.backgroundSprite = "GenericPanel";
            lengthSliderPanel.color = new Color32(206, 206, 206, 255);
            lengthSliderPanel.size = new Vector2(width - 16, 16);
            lengthSliderPanel.relativePosition = new Vector2(8, 28);

            UIPanel lengthSliderLeftPanel = lengthSliderPanel.AddUIComponent<UIPanel>();
            lengthSliderLeftPanel.name = "length panel left";
            lengthSliderLeftPanel.height = lengthSliderPanel.height;
            lengthSliderLeftPanel.width = (0.7f * lengthSliderPanel.width) - 5;
            lengthSliderLeftPanel.relativePosition = new Vector2(0, 0);

            UISlider lengthSlider = lengthSliderLeftPanel.AddUIComponent<UISlider>();
            lengthSlider.name = "Length Slider";
            lengthSlider.maxValue = 300;
            lengthSlider.minValue = 90;
            lengthSlider.stepSize = 5;
            lengthSlider.relativePosition = new Vector2(0, 0);
            lengthSlider.size = lengthSliderLeftPanel.size;
            lengthSlider.eventValueChanged += (c, v) =>
            {
                if (m_lengthTextbox.text != v.ToString())
                {
                    if (v > lengthSlider.minValue)
                    {
                        m_lengthTextbox.text = v.ToString();
                        m_setLength = v;
                    }
                    else
                    {
                        m_lengthTextbox.text = "default";
                        m_setLength = 88;
                    }
                }
            };

            UISlicedSprite lengthSliderBgSprite = lengthSliderLeftPanel.AddUIComponent<UISlicedSprite>();
            lengthSliderBgSprite.isInteractive = false;
            lengthSliderBgSprite.atlas = atlas;
            lengthSliderBgSprite.spriteName = "BudgetSlider";
            lengthSliderBgSprite.size = lengthSliderLeftPanel.size;
            lengthSliderBgSprite.relativePosition = new Vector2(0, 0);

            UISlicedSprite lengthSliderMkSprite = lengthSliderLeftPanel.AddUIComponent<UISlicedSprite>();
            lengthSliderMkSprite.atlas = atlas;
            lengthSliderMkSprite.spriteName = "SliderBudget";
            lengthSliderMkSprite.isInteractive = false;
            lengthSlider.thumbObject = lengthSliderMkSprite;

            m_lengthTextbox = lengthSliderPanel.AddUIComponent<UITextField>();
            m_lengthTextbox.height = lengthSliderPanel.height;
            m_lengthTextbox.width = lengthSliderPanel.size.x - lengthSliderLeftPanel.size.x;
            m_lengthTextbox.relativePosition = new Vector2(lengthSliderLeftPanel.width, 0);
            m_lengthTextbox.eventTextChanged += (c, v) =>
            {
                float val = 0;
                if (float.TryParse(v, out val))
                {
                    m_setLength = val;
                    if (lengthSlider.value != val)
                        lengthSlider.value = val;
                }
                else
                {
                    m_setLength = 88;
                    if (lengthSlider.value != lengthSlider.minValue)
                        lengthSlider.value = lengthSlider.minValue;
                }

                try
                {
                    t.Run();
                }
                catch (Exception ex)
                {
                    Next.Debug.Log(ex.Message);
                }
            };





            UIPanel depthSliderPanel = AddUIComponent<UIPanel>();
            depthSliderPanel.atlas = atlas;
            depthSliderPanel.backgroundSprite = "GenericPanel";
            depthSliderPanel.color = new Color32(206, 206, 206, 255);
            depthSliderPanel.size = new Vector2(width - 16, 16);
            depthSliderPanel.relativePosition = new Vector2(8, 56);

            UIPanel depthSliderLeftPanel = depthSliderPanel.AddUIComponent<UIPanel>();
            depthSliderLeftPanel.name = "depth panel left";
            depthSliderLeftPanel.height = depthSliderPanel.height;
            depthSliderLeftPanel.width = (0.7f * depthSliderPanel.width) - 5;
            depthSliderLeftPanel.relativePosition = new Vector2(0, 0);

            UISlider depthSlider = depthSliderLeftPanel.AddUIComponent<UISlider>();
            depthSlider.name = "depth Slider";
            depthSlider.maxValue = 36;
            depthSlider.minValue = 9;
            depthSlider.stepSize = 3;
            depthSlider.relativePosition = new Vector2(0, 0);
            depthSlider.size = depthSliderLeftPanel.size;
            depthSlider.eventValueChanged += (c, v) =>
            {

                if (m_depthTextbox.text != v.ToString())
                {
                    if (v > depthSlider.minValue)
                    {
                        m_depthTextbox.text = v.ToString();
                        m_setDepth = v;
                    }
                    else
                    {
                        m_depthTextbox.text = "Default";
                        m_setDepth = 4;
                    }
                }
            };

            UISlicedSprite depthSliderBgSprite = depthSliderLeftPanel.AddUIComponent<UISlicedSprite>();
            depthSliderBgSprite.isInteractive = false;
            depthSliderBgSprite.atlas = atlas;
            depthSliderBgSprite.spriteName = "BudgetSlider";
            depthSliderBgSprite.size = depthSliderLeftPanel.size;
            depthSliderBgSprite.relativePosition = new Vector2(0, 0);

            UISlicedSprite depthSliderMkSprite = depthSliderLeftPanel.AddUIComponent<UISlicedSprite>();
            depthSliderMkSprite.atlas = atlas;
            depthSliderMkSprite.spriteName = "SliderBudget";
            depthSliderMkSprite.isInteractive = false;
            depthSlider.thumbObject = depthSliderMkSprite;

            m_depthTextbox = depthSliderPanel.AddUIComponent<UITextField>();
            m_depthTextbox.height = depthSliderPanel.height;
            m_depthTextbox.width = depthSliderPanel.size.x - depthSliderLeftPanel.size.x;
            m_depthTextbox.relativePosition = new Vector2(depthSliderLeftPanel.width, 0);
            m_depthTextbox.eventTextChanged += (c, v) =>
            {
                float val = 0;
                if (float.TryParse(v, out val))
                {
                    m_setDepth = val;
                    if (depthSlider.value != val)
                        depthSlider.value = val;
                }
                else
                {
                    m_setDepth = 4;
                    if (depthSlider.value != depthSlider.minValue)
                        depthSlider.value = depthSlider.minValue;
                }
                
                try
                {
                    t.Run();
                }
                catch (Exception ex)
                {
                    Next.Debug.Log(ex.Message);
                }
            };
        }
        private void DoStationMechanics()
        {
            MetroStations.UpdateMetro(m_setDepth, m_setLength);
        }
    }
}
