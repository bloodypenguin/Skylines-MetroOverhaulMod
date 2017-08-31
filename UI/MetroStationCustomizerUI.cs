using System;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class MetroStationCustomizerUI : UIPanel
    {
        private const int DEPTH_STEP = 3;
        private const int LENGTH_STEP = 8;
        private const int ANGLE_STEP = 15;
        private float m_setDepth;
        private float m_setLength;
        private float m_setAngle;
        private float m_oldAngle;
        private bool m_valueChanged = false;
        private UITextField m_lengthTextbox = new UITextField();
        private UITextField m_depthTextbox = new UITextField();
        private UITextField m_angleTextbox = new UITextField();
        private BulldozeTool m_bulldozeTool;
        private BuildingTool m_buildingTool;
        private NetTool m_netTool;
        private UIButton m_upgradeButtonTemplate;
        private BuildingInfo m_currentBuilding;
        private bool m_activated = false;
        private TrackType m_TrackType = TrackType.SidePlatform;
        private TrackType m_PrevTrackType = TrackType.SidePlatform;
        public static MetroStationCustomizerUI instance;
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
                    if (toolInfo.HasUndergroundMetroStationTracks())
                    {
                        finalInfo = toolInfo;
                    }
                    else if (toolInfo.m_subBuildings != null)
                    {
                        foreach (var subInfo in toolInfo.m_subBuildings)
                        {
                            if (subInfo.m_buildingInfo == null || !subInfo.m_buildingInfo.HasUndergroundMetroStationTracks())
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
            m_setDepth = SetStationCustomizations.MIN_DEPTH;
            m_setLength = SetStationCustomizations.MIN_LENGTH;
            m_setAngle = 0;
            m_oldAngle = 0;
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
            height = 230;
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

            UILabel angleTitleLabel = AddUIComponent<UILabel>();
            angleTitleLabel.relativePosition = new Vector3() { x = 8, y = 100, z = 0 };
            angleTitleLabel.text = "Station Angle";
            angleTitleLabel.height = 10;
            angleTitleLabel.isInteractive = false;

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
            lengthSlider.maxValue = SetStationCustomizations.MAX_LENGTH;
            lengthSlider.minValue = SetStationCustomizations.MIN_LENGTH;
            lengthSlider.stepSize = LENGTH_STEP;
            lengthSlider.relativePosition = new Vector2(0, 0);
            lengthSlider.size = lengthSliderLeftPanel.size;
            lengthSlider.eventValueChanged += (c, v) =>
            {
                if (m_lengthTextbox.text != v.ToString())
                {
                    m_valueChanged = true;
                    if (v > SetStationCustomizations.MIN_LENGTH)
                    {
                        m_lengthTextbox.text = v.ToString();
                        m_setLength = v;
                    }
                    else
                    {
                        m_lengthTextbox.text = "Default";
                        m_setLength = SetStationCustomizations.MIN_LENGTH;
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
                    m_setLength = SetStationCustomizations.MIN_LENGTH;
                    if (lengthSlider.value != SetStationCustomizations.MIN_LENGTH)
                        lengthSlider.value = SetStationCustomizations.MIN_LENGTH;
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
            depthSlider.name = "Depth Slider";
            depthSlider.maxValue = SetStationCustomizations.MAX_DEPTH;
            depthSlider.minValue = SetStationCustomizations.MIN_DEPTH;
            depthSlider.stepSize = DEPTH_STEP;
            depthSlider.relativePosition = new Vector2(0, 0);
            depthSlider.size = depthSliderLeftPanel.size;
            depthSlider.eventValueChanged += (c, v) =>
            {

                if (m_depthTextbox.text != v.ToString())
                {
                    m_valueChanged = true;
                    if (v > SetStationCustomizations.MIN_DEPTH)
                    {
                        m_depthTextbox.text = v.ToString();
                        m_setDepth = v;
                    }
                    else
                    {
                        m_depthTextbox.text = "Default";
                        m_setDepth = SetStationCustomizations.MIN_DEPTH;
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
                    m_setDepth = SetStationCustomizations.MIN_DEPTH;
                    if (depthSlider.value != SetStationCustomizations.MIN_DEPTH)
                        depthSlider.value = SetStationCustomizations.MIN_DEPTH;
                }
            };

            UIPanel angleSliderPanel = AddUIComponent<UIPanel>();
            angleSliderPanel.atlas = atlas;
            angleSliderPanel.backgroundSprite = "GenericPanel";
            angleSliderPanel.color = new Color32(206, 206, 206, 255);
            angleSliderPanel.size = new Vector2(width - 16, 16);
            angleSliderPanel.relativePosition = new Vector2(8, 120);

            UIPanel angleSliderLeftPanel = angleSliderPanel.AddUIComponent<UIPanel>();
            angleSliderLeftPanel.name = "Angle panel left";
            angleSliderLeftPanel.height = angleSliderPanel.height;
            angleSliderLeftPanel.width = (0.7f * angleSliderPanel.width) - 5;
            angleSliderLeftPanel.relativePosition = new Vector2(0, 0);

            UISlider angleSlider = angleSliderLeftPanel.AddUIComponent<UISlider>();
            angleSlider.name = "Angle Slider";
            angleSlider.maxValue = SetStationCustomizations.MAX_ANGLE;
            angleSlider.minValue = SetStationCustomizations.MIN_ANGLE;
            angleSlider.stepSize = ANGLE_STEP;
            angleSlider.relativePosition = new Vector2(0, 0);
            angleSlider.size = angleSliderLeftPanel.size;
            angleSlider.eventValueChanged += (c, v) =>
            {

                if (m_angleTextbox.text != v.ToString())
                {
                    m_valueChanged = true;
                    if (v <= SetStationCustomizations.MIN_ANGLE)
                    {
                        m_angleTextbox.text = "Default";
                        m_setAngle = SetStationCustomizations.MIN_ANGLE;
                    }
                    else
                    {
                        m_angleTextbox.text = v.ToString();
                        m_setAngle = v;
                    }
                }
            };

            angleSlider.eventMouseUp += (c, e) =>
            {
                if (m_valueChanged)
                {
                    m_valueChanged = false;
                    t.Run();
                }

            };

            UISlicedSprite angleSliderBgSprite = angleSliderLeftPanel.AddUIComponent<UISlicedSprite>();
            angleSliderBgSprite.isInteractive = false;
            angleSliderBgSprite.atlas = atlas;
            angleSliderBgSprite.spriteName = "BudgetSlider";
            angleSliderBgSprite.size = angleSliderLeftPanel.size;
            angleSliderBgSprite.relativePosition = new Vector2(0, 0);

            UISlicedSprite angleSliderMkSprite = angleSliderLeftPanel.AddUIComponent<UISlicedSprite>();
            angleSliderMkSprite.atlas = atlas;
            angleSliderMkSprite.spriteName = "SliderBudget";
            angleSliderMkSprite.isInteractive = false;
            angleSlider.thumbObject = angleSliderMkSprite;

            m_angleTextbox = angleSliderPanel.AddUIComponent<UITextField>();
            m_angleTextbox.text = "Default";
            m_angleTextbox.height = angleSliderPanel.height;
            m_angleTextbox.width = angleSliderPanel.size.x - angleSliderLeftPanel.size.x;
            m_angleTextbox.relativePosition = new Vector2(angleSliderLeftPanel.width, 0);
            m_angleTextbox.eventTextChanged += (c, v) =>
            {
                float val = 0;
                if (float.TryParse(v, out val))
                {
                    m_setAngle = val;
                    if (angleSlider.value != val)
                        angleSlider.value = val;
                }
                else
                {
                    m_setAngle = SetStationCustomizations.MIN_ANGLE;
                    if (angleSlider.value != SetStationCustomizations.MIN_ANGLE)
                        angleSlider.value = SetStationCustomizations.MIN_ANGLE;
                }
            };
            UICheckBox useIslandPlatformCheckBox = AddUIComponent<UICheckBox>();
            UICheckBox UseSidePlatformCheckBox = AddUIComponent<UICheckBox>();
            UICheckBox UseSingleTrackCheckBox = AddUIComponent<UICheckBox>();

            UseSidePlatformCheckBox.text = "Side Platform";
            UseSidePlatformCheckBox.size = new Vector2(width - 16, 16);
            UseSidePlatformCheckBox.relativePosition = new Vector2(8, 160);
            UseSidePlatformCheckBox.isInteractive = true;
            UseSidePlatformCheckBox.eventCheckChanged += (c, v) =>
            {
                if (UseSidePlatformCheckBox.isChecked)
                {
                    m_PrevTrackType = m_TrackType;
                    m_TrackType = TrackType.SidePlatform;
                    useIslandPlatformCheckBox.isChecked = false;
                    UseSingleTrackCheckBox.isChecked = false;
                    TunnelStationTrackToggleStyles(m_currentBuilding);
                    t.Run();
                }
                else
                {
                    if (useIslandPlatformCheckBox.isChecked == false && UseSingleTrackCheckBox.isChecked == false)
                    {
                        UseSidePlatformCheckBox.isChecked = true;
                    }
                }
            };

            m_UseSidePlatformCheckBoxClicker = UseSidePlatformCheckBox.AddUIComponent<UISprite>();
            m_UseSidePlatformCheckBoxClicker.atlas = atlas;
            m_UseSidePlatformCheckBoxClicker.spriteName = "check-unchecked";
            m_UseSidePlatformCheckBoxClicker.relativePosition = new Vector2(0, 0);
            m_UseSidePlatformCheckBoxClicker.size = new Vector2(16, 16);
            m_UseSidePlatformCheckBoxClicker.isInteractive = true;

            UILabel UseSidePlatformLabel = UseSidePlatformCheckBox.AddUIComponent<UILabel>();
            UseSidePlatformLabel.relativePosition = new Vector2(20, 0);
            UseSidePlatformLabel.text = "Side Platform";
            UseSidePlatformLabel.height = 16;
            UseSidePlatformLabel.isInteractive = true;


            useIslandPlatformCheckBox.text = "Island Platform";
            useIslandPlatformCheckBox.size = new Vector2(width - 16, 16);
            useIslandPlatformCheckBox.relativePosition = new Vector2(8, 180);
            useIslandPlatformCheckBox.isInteractive = true;
            useIslandPlatformCheckBox.eventCheckChanged += (c, v) =>
             {
                 if (useIslandPlatformCheckBox.isChecked)
                 {
                     m_PrevTrackType = m_TrackType;
                     m_TrackType = TrackType.IslandPlatform;
                     UseSingleTrackCheckBox.isChecked = false;
                     UseSidePlatformCheckBox.isChecked = false;
                     TunnelStationTrackToggleStyles(m_currentBuilding);
                     t.Run();
                 }
                 else
                 {
                     if (UseSidePlatformCheckBox.isChecked == false && UseSingleTrackCheckBox.isChecked == false)
                     {
                         useIslandPlatformCheckBox.isChecked = true;
                     }
                 }
             };

            m_UseIslandPlatformCheckBoxClicker = useIslandPlatformCheckBox.AddUIComponent<UISprite>();
            m_UseIslandPlatformCheckBoxClicker.atlas = atlas;
            m_UseIslandPlatformCheckBoxClicker.spriteName = "check-unchecked";
            m_UseIslandPlatformCheckBoxClicker.relativePosition = new Vector2(0, 0);
            m_UseIslandPlatformCheckBoxClicker.size = new Vector2(16, 16);
            m_UseIslandPlatformCheckBoxClicker.isInteractive = true;

            UILabel useIslandPlatformLabel = useIslandPlatformCheckBox.AddUIComponent<UILabel>();
            useIslandPlatformLabel.relativePosition = new Vector2(20, 0);
            useIslandPlatformLabel.text = "Island Platform";
            useIslandPlatformLabel.height = 16;
            useIslandPlatformLabel.isInteractive = true;

            UseSingleTrackCheckBox.text = "Single Track";
            UseSingleTrackCheckBox.size = new Vector2(width - 16, 16);
            UseSingleTrackCheckBox.relativePosition = new Vector2(8, 200);
            UseSingleTrackCheckBox.isInteractive = true;
            UseSingleTrackCheckBox.eventCheckChanged += (c, v) =>
            {
                if (UseSingleTrackCheckBox.isChecked)
                {
                    m_PrevTrackType = m_TrackType;
                    m_TrackType = TrackType.SingleTrack;
                    useIslandPlatformCheckBox.isChecked = false;
                    UseSidePlatformCheckBox.isChecked = false;
                    TunnelStationTrackToggleStyles(m_currentBuilding);
                    t.Run();
                }
                else
                {
                    if (UseSidePlatformCheckBox.isChecked == false && useIslandPlatformCheckBox.isChecked == false)
                    {
                        UseSingleTrackCheckBox.isChecked = true;
                    }
                }
            };

            m_UseSingleTrackCheckBoxClicker = UseSingleTrackCheckBox.AddUIComponent<UISprite>();
            m_UseSingleTrackCheckBoxClicker.atlas = atlas;
            m_UseSingleTrackCheckBoxClicker.spriteName = "check-unchecked";
            m_UseSingleTrackCheckBoxClicker.relativePosition = new Vector2(0, 0);
            m_UseSingleTrackCheckBoxClicker.size = new Vector2(16, 16);
            m_UseSingleTrackCheckBoxClicker.isInteractive = true;

            UILabel UseSingleTrackLabel = UseSingleTrackCheckBox.AddUIComponent<UILabel>();
            UseSingleTrackLabel.relativePosition = new Vector2(20, 0);
            UseSingleTrackLabel.text = "Single Track";
            UseSingleTrackLabel.height = 16;
            UseSingleTrackLabel.isInteractive = true;
        }
        private UISprite m_UseIslandPlatformCheckBoxClicker = null;
        private UISprite m_UseSingleTrackCheckBoxClicker = null;
        private UISprite m_UseSidePlatformCheckBoxClicker = null;

        private void TunnelStationTrackToggleStyles(BuildingInfo info)
        {
            if (info?.m_paths == null)
            {
                return;
            }
            for (var i = 0; i < info.m_paths.Length;i++)
            {
                var path = info.m_paths[i];
                if (path?.m_netInfo?.name == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    continue;
                }
                switch (m_TrackType)
                {
                    case TrackType.SidePlatform:
                        {
                            if (m_UseSidePlatformCheckBoxClicker.spriteName == "check-unchecked")
                            {
                                m_UseSidePlatformCheckBoxClicker.spriteName = "check-checked";
                                m_UseIslandPlatformCheckBoxClicker.spriteName = "check-unchecked";
                                m_UseSingleTrackCheckBoxClicker.spriteName = "check-unchecked";
                            }

                            if (m_PrevTrackType != TrackType.SidePlatform)
                            {
                                path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
                            }
                        }
                        break;
                    case TrackType.IslandPlatform:
                        {
                            if (m_UseIslandPlatformCheckBoxClicker.spriteName == "check-unchecked")
                            {
                                m_UseIslandPlatformCheckBoxClicker.spriteName = "check-checked";
                                m_UseSidePlatformCheckBoxClicker.spriteName = "check-unchecked";
                                m_UseSingleTrackCheckBoxClicker.spriteName = "check-unchecked";
                            }

                            if (m_PrevTrackType != TrackType.IslandPlatform)
                            {
                                path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel Island");
                            }
                        }
                        break;
                    case TrackType.SingleTrack:
                        {
                            if (m_UseSingleTrackCheckBoxClicker.spriteName == "check-unchecked")
                            {
                                m_UseSingleTrackCheckBoxClicker.spriteName = "check-checked";
                                m_UseIslandPlatformCheckBoxClicker.spriteName = "check-unchecked";
                                m_UseSidePlatformCheckBoxClicker.spriteName = "check-unchecked";
                            }

                            if (m_PrevTrackType != TrackType.SingleTrack)
                            {
                                path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel Small");
                            }
                        }
                        break;
                }
            }
        }
        private void Activate(BuildingInfo bInfo)
        {
            m_oldAngle = 0;
            m_activated = true;
            m_currentBuilding = bInfo;
            isVisible = true;
            TunnelStationTrackToggleStyles(bInfo);
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
            var angleDelta = Math.PI / 180 * (m_setAngle - m_oldAngle);
            m_oldAngle = m_setAngle;
            SetStationCustomizations.ModifyStation(m_currentBuilding, m_setDepth, m_setLength, angleDelta, m_TrackType, m_PrevTrackType);
        }
    }
    public enum TrackType
    {
        SidePlatform,
        IslandPlatform,
        SingleTrack
    }
}
