using ColossalFramework.UI;
using ICities;
using System;
using MetroOverhaul;
using UnityEngine;
using ColossalFramework.Threading;
using System.Reflection;
using MetroOverhaul.Extensions;

namespace UIMod
{
    public class MetroStationCustomizer : UIPanel
    {
        private const int MAX_DEPTH = 36;
        private const int MIN_DEPTH = 12;
        private const int INT_DEPTH = 3;
        private const int MAX_LENGTH = 144;
        private const int MIN_LENGTH = 88;
        private const int INT_LENGTH = 8;
        private float m_setDepth;
        private float m_setLength;
        private bool m_valueChanged = false;
        private UITextField m_lengthTextbox = new UITextField();
        private UITextField m_depthTextbox = new UITextField();
        private BulldozeTool m_bulldozeTool;
        private BuildingTool m_buildingTool;
        private NetTool m_netTool;
        private UIButton m_upgradeButtonTemplate;
        private BuildingInfo m_currentBuilding;
        private bool m_activated = false;
        public static MetroStationCustomizer instance;
        public override void Update()
        {
            if (m_buildingTool == null)
            {
                return;
            }
            try
            {
                var toolInfo = m_buildingTool.enabled ? m_buildingTool.m_prefab : null;
                if (toolInfo == m_currentBuilding)
                {
                    return;
                }
                BuildingInfo finalInfo = null;
                if (toolInfo != null)
                {
                    if (toolInfo.IsUndergroundMetroStation())
                    {
                        finalInfo = toolInfo;
                    }
                    else if (toolInfo.m_subBuildings != null)
                    {
                        foreach (var subInfo in toolInfo.m_subBuildings)
                        {
                            if (subInfo.m_buildingInfo == null || !subInfo.m_buildingInfo.IsUndergroundMetroStation())
                            {
                                continue;
                            }
                            finalInfo = subInfo.m_buildingInfo;
                            break;
                        }
                    }
                }
                if (finalInfo == m_currentBuilding)
                {
                    return;
                }
                if (finalInfo != null)
                {
                    m_setDepth = MIN_DEPTH;
                    m_setLength = MIN_LENGTH;
                    Activate(finalInfo);
                }
                else
                {
                    Deactivate();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                Deactivate();
            }

        }
        public override void Start()
        {
            m_buildingTool = FindObjectOfType<BuildingTool>();
            if (m_buildingTool == null)
            {
#if DEBUG
                Next.Debug.Log("BuildingTool Not Found");
#endif
                enabled = false;
                return;
            }

            m_bulldozeTool = FindObjectOfType<BulldozeTool>();
            if (m_bulldozeTool == null)
            {
#if DEBUG
                Next.Debug.Log("BulldozeTool Not Found");
#endif
                enabled = false;
                return;
            }
            m_netTool = FindObjectOfType<NetTool>();
            if (m_netTool == null)
            {
#if DEBUG
                Next.Debug.Log("NetTool Not Found");
#endif
                enabled = false;
                return;
            }

            try
            {
                m_upgradeButtonTemplate = GameObject.Find("RoadsSmallPanel").GetComponent<GeneratedScrollPanel>().m_OptionsBar.Find<UIButton>("Upgrade");
            }
            catch
            {
#if DEBUG
                Next.Debug.Log("Upgrade button template not found");
#endif
            }

            CreateUI();
        }

        private void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM GUI Created");
#endif
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
            lengthSlider.maxValue = MAX_LENGTH;
            lengthSlider.minValue = MIN_LENGTH;
            lengthSlider.stepSize = INT_LENGTH;
            lengthSlider.relativePosition = new Vector2(0, 0);
            lengthSlider.size = lengthSliderLeftPanel.size;
            lengthSlider.eventValueChanged += (c, v) =>
            {
                if (m_lengthTextbox.text != v.ToString())
                {
                    m_valueChanged = true;
                    if (v > MIN_LENGTH)
                    {
                        m_lengthTextbox.text = v.ToString();
                        m_setLength = v;
                    }
                    else
                    {
                        m_lengthTextbox.text = "Default";
                        m_setLength = MIN_LENGTH;
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
                    m_setLength = MIN_LENGTH;
                    if (lengthSlider.value != MIN_LENGTH)
                        lengthSlider.value = MIN_LENGTH;
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
            depthSlider.maxValue = MAX_DEPTH;
            depthSlider.minValue = MIN_DEPTH;
            depthSlider.stepSize = INT_DEPTH;
            depthSlider.relativePosition = new Vector2(0, 0);
            depthSlider.size = depthSliderLeftPanel.size;
            depthSlider.eventValueChanged += (c, v) =>
            {

                if (m_depthTextbox.text != v.ToString())
                {
                    m_valueChanged = true;
                    if (v > MIN_DEPTH)
                    {
                        m_depthTextbox.text = v.ToString();
                        m_setDepth = v;
                    }
                    else
                    {
                        m_depthTextbox.text = "Default";
                        m_setDepth = MIN_DEPTH;
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
                    m_setDepth = MIN_DEPTH;
                    if (depthSlider.value != MIN_DEPTH)
                        depthSlider.value = MIN_DEPTH;
                }
            };
        }

        private void Activate(BuildingInfo bInfo)
        {
            m_activated = true;
            m_currentBuilding = bInfo;
            isVisible = true;
            DoStationMechanics();
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
            SetStationDepthLength.ModifyStation(m_currentBuilding, m_setDepth, m_setLength);
        }
    }
}
