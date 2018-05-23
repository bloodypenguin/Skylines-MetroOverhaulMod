using System;
using System.Collections.Generic;
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
        private const float BEND_STRENGTH_STEP = 0.5f;
        //private float m_setDepth;
        //private float m_setLength;
        //private float m_setBendStrength;
        //private float m_setAngle;
        private float m_oldAngle;
        private bool m_valueChanged = false;
        private ToggleType m_Toggle = ToggleType.None;
        private Dictionary<ToggleType, UIButton> toggleBtnDict = new Dictionary<ToggleType, UIButton>();
        private Dictionary<ToggleType, float> SetDict = new Dictionary<ToggleType, float>();
        private UIButton m_BtnToggleLength = new UIButton();
        private UIButton m_BtnToggleDepth = new UIButton();
        private UIButton m_BtnToggleAngle = new UIButton();
        private UIButton m_BtnToggleBend = new UIButton();
        private BulldozeTool m_bulldozeTool;
        private BuildingTool m_buildingTool;
        private NetTool m_netTool;
        private UIButton m_upgradeButtonTemplate;
        private BuildingInfo m_currentBuilding;
        private BuildingInfo m_currentSuperBuilding;
        private bool m_activated = false;
        private StationTrackType m_TrackType = StationTrackType.SidePlatform;
        private StationTrackType m_PrevTrackType = StationTrackType.SidePlatform;
        private Task m_T = null;
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
                BuildingInfo superFinalInfo = null;
                if (toolInfo != null)
                {
                    RestoreStationTrackStyles(toolInfo);
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
                            superFinalInfo = toolInfo;
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
                    Activate(finalInfo, superFinalInfo);
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

            SetDict.Add(ToggleType.Depth, SetStationCustomizations.DEF_DEPTH);
            SetDict.Add(ToggleType.Length, SetStationCustomizations.DEF_LENGTH);
            SetDict.Add(ToggleType.Angle, SetStationCustomizations.DEF_ANGLE);
            SetDict.Add(ToggleType.Bend, SetStationCustomizations.DEF_BEND_STRENGTH);

            m_oldAngle = 0;
        }
        private int m_SliderCount = 0;
        private void OnToggleValueChanged(UIComponent c, float v)
        {

        }
        private void CreateSlider(ToggleType type, int min, int max, int def, float step)
        {
            UIButton toggleBtn = toggleBtnDict[type];
            string typeString = type.ToString();
            UILabel TitleLabel = AddUIComponent<UILabel>();
            TitleLabel.relativePosition = new Vector3() { x = 8, y = 30 + m_SliderCount * 40, z = 0 };
            TitleLabel.text = "Station " + typeString;
            TitleLabel.isInteractive = false;

            UIPanel sliderPanel = AddUIComponent<UIPanel>();
            sliderPanel.atlas = atlas;
            sliderPanel.backgroundSprite = "GenericPanel";
            sliderPanel.color = new Color32(150, 150, 150, 255);
            sliderPanel.size = new Vector2(width - 16, 16);
            sliderPanel.relativePosition = new Vector2(8, 50 + m_SliderCount * 40);

            UIPanel sliderLeftPanel = sliderPanel.AddUIComponent<UIPanel>();
            sliderLeftPanel.name = typeString + " panel left";
            sliderLeftPanel.height = sliderPanel.height;
            sliderLeftPanel.width = (0.7f * sliderPanel.width) - 5;
            sliderLeftPanel.relativePosition = new Vector2(0, 0);

            UISlider slider = sliderLeftPanel.AddUIComponent<UISlider>();
            slider.name = typeString + " Slider";
            slider.maxValue = max;
            slider.minValue = min;
            slider.value = def;
            slider.stepSize = step;
            slider.relativePosition = new Vector2(0, 0);
            slider.size = sliderLeftPanel.size;
            slider.eventValueChanged += (c, v) =>
            {
                if (toggleBtn.text != v.ToString())
                {
                    m_valueChanged = true;
                    if (v >= min)
                    {
                        toggleBtn.text = v.ToString();
                        SetDict[type] = v;
                    }
                    else
                    {
                        toggleBtn.text = def.ToString();
                        SetDict[type] = def;
                    }
                }
            };
            slider.eventMouseUp += (c, e) =>
            {
                if (m_valueChanged)
                {
                    m_valueChanged = false;
                    m_T.Run();
                }

            };
            slider.eventClicked += (c, v) =>
            {
                if (m_Toggle != ToggleType.None)
                {
                    toggleBtnDict[m_Toggle].color = new Color32(150, 150, 150, 255);
                    toggleBtnDict[m_Toggle].normalBgSprite = "ButtonMenu";
                    toggleBtnDict[m_Toggle].useDropShadow = false;
                    toggleBtnDict[m_Toggle].opacity = 75;
                }
                if (m_Toggle != type)
                {
                    m_Toggle = type;
                    toggleBtn.color = new Color32(163, 255, 16, 255);
                    toggleBtn.normalBgSprite = "ButtonMenu";
                    toggleBtn.useDropShadow = true;
                    toggleBtn.opacity = 95;
                }
                else
                {
                    Focus();
                    m_Toggle = ToggleType.None;
                }
            };
            slider.eventKeyDown += (c, v) =>
            {
                switch (v.keycode)
                {
                    case KeyCode.LeftArrow:
                        slider.value = Math.Max(min, SetDict[type] - step);
                        break;
                    case KeyCode.RightArrow:
                        slider.value = Math.Min(max, SetDict[type] + step);
                        break;
                    case KeyCode.UpArrow:
                        slider.value = max;
                        break;
                    case KeyCode.DownArrow:
                        slider.value = min;
                        break;
                    case KeyCode.Alpha0:
                        slider.value = def;
                        break;
                }
                m_T.Run();
            };
            UISlicedSprite sliderBgSprite = sliderLeftPanel.AddUIComponent<UISlicedSprite>();
            sliderBgSprite.isInteractive = false;
            sliderBgSprite.atlas = atlas;
            sliderBgSprite.spriteName = "BudgetSlider";
            sliderBgSprite.size = sliderLeftPanel.size;
            sliderBgSprite.relativePosition = new Vector2(0, 0);

            UISlicedSprite sliderMkSprite = sliderLeftPanel.AddUIComponent<UISlicedSprite>();
            sliderMkSprite.atlas = atlas;
            sliderMkSprite.spriteName = "SliderBudget";
            sliderMkSprite.isInteractive = false;
            slider.thumbObject = sliderMkSprite;

            toggleBtn = sliderPanel.AddUIComponent<UIButton>();
            toggleBtn.normalBgSprite = "ButtonMenu";
            toggleBtn.text = def.ToString();
            toggleBtn.height = sliderPanel.height;
            toggleBtn.width = sliderPanel.size.x - sliderLeftPanel.size.x;
            toggleBtn.relativePosition = new Vector2(sliderLeftPanel.width, 0);
            toggleBtn.eventClicked += (c, v) =>
            {
                if (m_Toggle != ToggleType.None)
                {
                    toggleBtnDict[m_Toggle].color = new Color32(150, 150, 150, 255);
                    toggleBtnDict[m_Toggle].normalBgSprite = "ButtonMenu";
                    toggleBtnDict[m_Toggle].useDropShadow = false;
                    toggleBtnDict[m_Toggle].opacity = 75;
                }
                if (m_Toggle != type)
                {
                    m_Toggle = type;
                    toggleBtn.color = new Color32(163, 255, 16, 255);
                    toggleBtn.normalBgSprite = "ButtonMenu";
                    toggleBtn.useDropShadow = true;
                    toggleBtn.opacity = 95;
                }
                else
                {
                    Focus();
                    m_Toggle = ToggleType.None;
                }
            };
            toggleBtn.eventKeyDown += (c, v) =>
            {
                switch (v.keycode)
                {
                    case KeyCode.LeftArrow:
                        slider.value = Math.Max(min, SetDict[type] - step);
                        break;
                    case KeyCode.RightArrow:
                        slider.value = Math.Min(max, SetDict[type] + step);
                        break;
                    case KeyCode.UpArrow:
                        slider.value = max;
                        break;
                    case KeyCode.DownArrow:
                        slider.value = min;
                        break;
                    case KeyCode.Alpha0:
                        slider.value = def;
                        break;
                }
                m_T.Run();
            };
            m_SliderCount++;
        }

        private void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM UNDERGROUND STATION TRACK GUI Created");
#endif
            Action stationMechanicsTask = DoStationMechanics;
            Task t = Task.Create(stationMechanicsTask);
            m_T = t;
            backgroundSprite = "GenericPanel";
            color = new Color32(68, 84, 68, 170);
            width = 280;
            height = 270;
            opacity = 60;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            UIPanel dragHandlePanel = AddUIComponent<UIPanel>();
            dragHandlePanel.atlas = atlas;
            dragHandlePanel.backgroundSprite = "GenericPanel";
            dragHandlePanel.width = width;
            dragHandlePanel.height = 30;
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
            titleLabel.text = "Subway Station Options";
            titleLabel.isInteractive = false;

            toggleBtnDict.Add(ToggleType.Depth, m_BtnToggleDepth);
            toggleBtnDict.Add(ToggleType.Length, m_BtnToggleLength);
            toggleBtnDict.Add(ToggleType.Angle, m_BtnToggleAngle);
            toggleBtnDict.Add(ToggleType.Bend, m_BtnToggleBend);

            CreateSlider(ToggleType.Length, SetStationCustomizations.MIN_LENGTH, SetStationCustomizations.MAX_LENGTH, SetStationCustomizations.DEF_LENGTH, LENGTH_STEP);
            CreateSlider(ToggleType.Depth, SetStationCustomizations.MIN_DEPTH, SetStationCustomizations.MAX_DEPTH, SetStationCustomizations.DEF_DEPTH, DEPTH_STEP);
            CreateSlider(ToggleType.Angle, SetStationCustomizations.MIN_ANGLE, SetStationCustomizations.MAX_ANGLE, SetStationCustomizations.DEF_ANGLE, ANGLE_STEP);
            CreateSlider(ToggleType.Bend, SetStationCustomizations.MIN_BEND_STRENGTH, SetStationCustomizations.MAX_BEND_STRENGTH, SetStationCustomizations.DEF_BEND_STRENGTH, BEND_STRENGTH_STEP);

            UICheckBox useIslandPlatformCheckBox = AddUIComponent<UICheckBox>();
            UICheckBox UseSidePlatformCheckBox = AddUIComponent<UICheckBox>();
            UICheckBox UseSingleTrackCheckBox = AddUIComponent<UICheckBox>();

            UseSidePlatformCheckBox.text = "Side Platform";
            UseSidePlatformCheckBox.size = new Vector2(width - 16, 16);
            UseSidePlatformCheckBox.relativePosition = new Vector2(8, 200);
            UseSidePlatformCheckBox.isInteractive = true;
            UseSidePlatformCheckBox.eventCheckChanged += (c, v) =>
            {
                if (UseSidePlatformCheckBox.isChecked)
                {
                    m_PrevTrackType = m_TrackType;
                    m_TrackType = StationTrackType.SidePlatform;
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
            useIslandPlatformCheckBox.relativePosition = new Vector2(8, 220);
            useIslandPlatformCheckBox.isInteractive = true;
            useIslandPlatformCheckBox.eventCheckChanged += (c, v) =>
            {
                if (useIslandPlatformCheckBox.isChecked)
                {
                    m_PrevTrackType = m_TrackType;
                    m_TrackType = StationTrackType.IslandPlatform;
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
            UseSingleTrackCheckBox.relativePosition = new Vector2(8, 240);
            UseSingleTrackCheckBox.isInteractive = true;
            UseSingleTrackCheckBox.eventCheckChanged += (c, v) =>
            {
                if (UseSingleTrackCheckBox.isChecked)
                {
                    m_PrevTrackType = m_TrackType;
                    m_TrackType = StationTrackType.SingleTrack;
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

        private void RestoreStationTrackStyles(BuildingInfo info)
        {
            for (var i = 0; i < info.m_paths.Length; i++)
            {
                var path = info.m_paths[i];
                if (path?.m_netInfo?.name != null && path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    if (m_PrevTrackType != StationTrackType.SidePlatform)
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
                    }
                }
            }
        }

        private void TunnelStationTrackToggleStyles(BuildingInfo info)
        {
            if (info?.m_paths == null)
            {
                return;
            }
            for (var i = 0; i < info.m_paths.Length; i++)
            {
                var path = info.m_paths[i];
                if (path?.m_netInfo?.name == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    continue;
                }
                switch (m_TrackType)
                {
                    case StationTrackType.SidePlatform:
                        {
                            if (m_UseSidePlatformCheckBoxClicker.spriteName == "check-unchecked")
                            {
                                m_UseSidePlatformCheckBoxClicker.spriteName = "check-checked";
                                m_UseIslandPlatformCheckBoxClicker.spriteName = "check-unchecked";
                                m_UseSingleTrackCheckBoxClicker.spriteName = "check-unchecked";
                            }

                            if (m_PrevTrackType != StationTrackType.SidePlatform)
                            {
                                path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
                            }
                        }
                        break;
                    case StationTrackType.IslandPlatform:
                        {
                            if (m_UseIslandPlatformCheckBoxClicker.spriteName == "check-unchecked")
                            {
                                m_UseIslandPlatformCheckBoxClicker.spriteName = "check-checked";
                                m_UseSidePlatformCheckBoxClicker.spriteName = "check-unchecked";
                                m_UseSingleTrackCheckBoxClicker.spriteName = "check-unchecked";
                            }

                            if (m_PrevTrackType != StationTrackType.IslandPlatform)
                            {
                                path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel Island");
                            }
                        }
                        break;
                    case StationTrackType.SingleTrack:
                        {
                            if (m_UseSingleTrackCheckBoxClicker.spriteName == "check-unchecked")
                            {
                                m_UseSingleTrackCheckBoxClicker.spriteName = "check-checked";
                                m_UseIslandPlatformCheckBoxClicker.spriteName = "check-unchecked";
                                m_UseSidePlatformCheckBoxClicker.spriteName = "check-unchecked";
                            }

                            if (m_PrevTrackType != StationTrackType.SingleTrack)
                            {
                                path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel Small");
                            }
                        }
                        break;
                }
            }
        }
        private void Activate(BuildingInfo bInfo, BuildingInfo superInfo = null)
        {
            m_oldAngle = 0;
            m_activated = true;
            m_currentBuilding = bInfo;
            m_currentSuperBuilding = superInfo;
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
            m_currentSuperBuilding = null;
            isVisible = false;
            m_activated = false;

        }

        private void DoStationMechanics()
        {
            var angleDelta = Math.PI / 180 * (SetDict[ToggleType.Angle] - m_oldAngle);
            m_oldAngle = SetDict[ToggleType.Angle];
            SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[ToggleType.Depth], SetDict[ToggleType.Length], angleDelta, SetDict[ToggleType.Bend], m_currentSuperBuilding);
        }
    }
    public enum StationTrackType
    {
        SidePlatform,
        IslandPlatform,
        SingleTrack
    }
    public enum ToggleType
    {
        None,
        Length,
        Depth,
        Angle,
        Bend
    }
}
