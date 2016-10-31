using ColossalFramework.UI;
using ICities;
using System;
using MetroOverhaul;
using UnityEngine;
using ColossalFramework.Threading;
using System.Reflection;

namespace UIMod
{
    public class MetroUILoader : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            UIComponent uic = UIView.GetAView().AddUIComponent(typeof(MetroStationCustomizer));
        }
    }
    public class MetroStationCustomizer : UIPanel
    {
        private float m_setDepth = 4;
        private float m_setLength = 88;
        private bool m_valueChanged = false;
        private UITextField m_lengthTextbox = new UITextField();
        private UITextField m_depthTextbox = new UITextField();
        private BulldozeTool m_bulldozeTool;
        private BuildingTool m_buildingTool;
        private NetTool m_netTool;
        private UIButton m_upgradeButtonTemplate;
        private BuildingInfo m_currentBuilding;
        private bool m_activated;
        public static MetroStationCustomizer instance;
        public override void Update()
        {
            if (m_buildingTool == null)
                return;
            try
            {
                BuildingInfo bInfo = m_buildingTool.enabled ? m_buildingTool.m_prefab : null;
                if (bInfo == m_currentBuilding)
                {
                    return;
                }
                else if (bInfo != null && bInfo.IsUndergroundMetroStation())
                {
                    Activate(bInfo);
                }
                else
                {
                    Deactivate();
                }
            }
            catch (Exception ex)
            {
                Next.Debug.Log("Update Failed");
                Next.Debug.Error(ex);
                Deactivate();
            }

        }
        public override void Start()
        {
            m_buildingTool = FindObjectOfType<BuildingTool>();
            if (m_buildingTool == null)
            {
                Next.Debug.Log("BuildingTool Not Found");
                enabled = false;
                return;
            }

            m_bulldozeTool = FindObjectOfType<BulldozeTool>();
            if (m_bulldozeTool == null)
            {
                Next.Debug.Log("BulldozeTool Not Found");
                enabled = false;
                return;
            }
            m_netTool = FindObjectOfType<NetTool>();
            if (m_netTool == null)
            {
                Next.Debug.Log("NetTool Not Found");
                enabled = false;
                return;
            }
            //m_buildErrors = m_netTool.GetType().GetField("m_buildErrors", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_elevationField = m_netTool.GetType().GetField("m_elevation", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_elevationUpField = m_netTool.GetType().GetField("m_buildElevationUp", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_elevationDownField = m_netTool.GetType().GetField("m_buildElevationDown", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_buildingElevationField = m_buildingTool.GetType().GetField("m_elevation", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_controlPointCountField = m_netTool.GetType().GetField("m_controlPointCount", BindingFlags.NonPublic | BindingFlags.Instance);
            //m_upgrading = m_netTool.GetType().GetField("m_upgrading", BindingFlags.NonPublic | BindingFlags.Instance);

            try
            {
                m_upgradeButtonTemplate = GameObject.Find("RoadsSmallPanel").GetComponent<GeneratedScrollPanel>().m_OptionsBar.Find<UIButton>("Upgrade");
            }
            catch
            {
                Next.Debug.Log("Upgrade button template not found");
            }

            CreateUI();
        }

        private void CreateUI()
        {
            Next.Debug.Log("MOM GUI Created");
            Action stationMechanicsTask = DoStationMechanics;
            Task t = Task.Create(stationMechanicsTask);

            backgroundSprite = "GenericPanel";
            color = new Color32(68, 84, 68, 170);
            width = 200;
            height = 100;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.width = width;
            dragHandle.height = 20;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            UILabel titleLabel = AddUIComponent<UILabel>();
            titleLabel.relativePosition = new Vector3() { x = 5, y = 0, z = 0 };
            titleLabel.text = "Station Attributes";
            titleLabel.isInteractive = false;

            UILabel lengthTitleLabel = AddUIComponent<UILabel>();
            lengthTitleLabel.relativePosition = new Vector3() { x = 8, y = 20, z = 0 };
            lengthTitleLabel.text = "Station Length";
            lengthTitleLabel.isInteractive = false;

            UILabel depthTitleLabel = AddUIComponent<UILabel>();
            depthTitleLabel.relativePosition = new Vector3() { x = 8, y = 60, z = 0 };
            depthTitleLabel.text = "Station Depth";
            depthTitleLabel.height = 10;
            depthTitleLabel.isInteractive = false;

            UIPanel lengthSliderPanel = AddUIComponent<UIPanel>();
            lengthSliderPanel.atlas = atlas;
            lengthSliderPanel.backgroundSprite = "GenericPanel";
            lengthSliderPanel.color = new Color32(206, 206, 206, 255);
            lengthSliderPanel.size = new Vector2(width - 16, 16);
            lengthSliderPanel.relativePosition = new Vector2(8, 40);

            UIPanel lengthSliderLeftPanel = lengthSliderPanel.AddUIComponent<UIPanel>();
            lengthSliderLeftPanel.name = "length panel left";
            lengthSliderLeftPanel.height = lengthSliderPanel.height;
            lengthSliderLeftPanel.width = (0.7f * lengthSliderPanel.width) - 5;
            lengthSliderLeftPanel.relativePosition = new Vector2(0, 0);

            UISlider lengthSlider = lengthSliderLeftPanel.AddUIComponent<UISlider>();
            lengthSlider.name = "Length Slider";
            lengthSlider.maxValue = 144;
            lengthSlider.minValue = 88;
            lengthSlider.stepSize = 8;
            lengthSlider.relativePosition = new Vector2(0, 0);
            lengthSlider.size = lengthSliderLeftPanel.size;
            lengthSlider.eventValueChanged += (c, v) =>
            {
                if (m_lengthTextbox.text != v.ToString())
                {
                    m_valueChanged = true;
                    if (v > lengthSlider.minValue)
                    {
                        m_lengthTextbox.text = v.ToString();
                        m_setLength = v;
                    }
                    else
                    {
                        m_lengthTextbox.text = "Default";
                        m_setLength = 88;
                    }
                }
            };
            lengthSlider.eventMouseUp += (c, e) =>
            {
                if (m_valueChanged)
                {
                    m_valueChanged = false;
                    t.Run();
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
            m_lengthTextbox.text = "Default";
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
            };

            UIPanel depthSliderPanel = AddUIComponent<UIPanel>();
            depthSliderPanel.atlas = atlas;
            depthSliderPanel.backgroundSprite = "GenericPanel";
            depthSliderPanel.color = new Color32(206, 206, 206, 255);
            depthSliderPanel.size = new Vector2(width - 16, 16);
            depthSliderPanel.relativePosition = new Vector2(8, 80);

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
                    m_valueChanged = true;
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

            depthSlider.eventMouseUp += (c, e) =>
            {
                if (m_valueChanged)
                {
                    m_valueChanged = false;
                    t.Run();
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
            m_depthTextbox.text = "Default";
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
            };
        }

        private void Activate(BuildingInfo bInfo)
        {
            if (bInfo != null && bInfo.IsUndergroundMetroStation())
            {
                m_activated = true;
                m_currentBuilding = bInfo;
                isVisible = true;
                DoStationMechanics();
            }
        }
        private void Deactivate()
        {
            if (!m_activated)
            {
                return;
            }
            m_currentBuilding = null;
            isVisible = false;
            m_activated = false;
        }

        private void DoStationMechanics()
        {
            m_currentBuilding.SetStation(m_setDepth, m_setLength);
        }
    }
}
