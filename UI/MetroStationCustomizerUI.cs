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
        private static Dictionary<ToggleType, SliderData> SliderDataDict { get; set; }
        private static Dictionary<ToggleType, UISlider> SliderDict { get; set; }
        private static Dictionary<ToggleType, UIPanel> PanelDict { get; set; }
        private Dictionary<ToggleType, UIButton> toggleBtnDict = new Dictionary<ToggleType, UIButton>();
        private Dictionary<ToggleType, float> SetDict = new Dictionary<ToggleType, float>();
        //private UIButton m_lengthTextbox = new UIButton();
        //private UITextField m_depthTextbox = new UITextField();
        //private UITextField m_angleTextbox = new UITextField();
        //private UITextField m_bendStrengthTextbox = new UITextField();
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
            SetDict[ToggleType.Depth] = SetStationCustomizations.DEF_DEPTH;
            SetDict[ToggleType.Length] = SetStationCustomizations.DEF_LENGTH;
            SetDict[ToggleType.Angle] = SetStationCustomizations.DEF_ANGLE;
            SetDict[ToggleType.Bend] = SetStationCustomizations.DEF_BEND_STRENGTH;
            m_oldAngle = 0;
        }
        private int m_SliderCount = 0;
        private void OnToggleKeyDown(UIComponent c, UIKeyEventParameter e)
        {
            if (SliderDataDict != null && m_Toggle != ToggleType.None)
            {
                SliderData sData = SliderDataDict[m_Toggle];

                switch (e.keycode)
                {
                    case KeyCode.LeftArrow:
                        SliderDict[m_Toggle].value = Math.Max(sData.Min, SetDict[m_Toggle] - sData.Step);
                        break;
                    case KeyCode.RightArrow:
                        SliderDict[m_Toggle].value = Math.Min(sData.Max, SetDict[m_Toggle] + sData.Step);
                        break;
                    case KeyCode.UpArrow:
                        SliderDict[m_Toggle].value = sData.Max;
                        break;
                    case KeyCode.DownArrow:
                        SliderDict[m_Toggle].value = sData.Min;
                        break;
                    case KeyCode.RightControl:
                        SliderDict[m_Toggle].value = sData.Def;
                        break;
                }

                m_T.Run();
            }
        }
        private void OnToggleMouseDown(UIComponent c, UIMouseEventParameter e, ToggleType type)
        {
            m_Toggle = type;
            if (SliderDataDict != null && type != ToggleType.None)
            {
                foreach (UIPanel pnl in PanelDict.Values)
                {
                    pnl.color = new Color32(150, 150, 150, 210);
                }
                PanelDict[type].color = new Color32(30, 200, 50, 255);
                SliderDict[type].Focus();
                m_T.Run();
            }
        }
        private void CreateSlider(ToggleType type)
        {
            SliderData sData = SliderDataDict[type];
            string typeString = type.ToString();
            UILabel TitleLabel = AddUIComponent<UILabel>();
            TitleLabel.relativePosition = new Vector3() { x = 8, y = 30 + m_SliderCount * 40, z = 0 };
            TitleLabel.text = "Station " + typeString;
            TitleLabel.isInteractive = true;
            TitleLabel.name = "lbl" + typeString;
            TitleLabel.eventMouseDown += delegate (UIComponent sender, UIMouseEventParameter e) { OnToggleMouseDown(sender, e, type); };
            TitleLabel.eventKeyDown += delegate (UIComponent sender, UIKeyEventParameter e) { OnToggleKeyDown(sender, e); };

            UIPanel sliderPanel = AddUIComponent<UIPanel>();
            PanelDict[type] = sliderPanel;
            sliderPanel.atlas = atlas;
            sliderPanel.backgroundSprite = "GenericPanel";
            sliderPanel.name = "pnl" + typeString;
            sliderPanel.color = new Color32(150, 150, 150, 210);
            sliderPanel.playAudioEvents = true;
            sliderPanel.size = new Vector2(width - 16, 20);
            sliderPanel.relativePosition = new Vector2(8, 48 + m_SliderCount * 40);
            sliderPanel.eventMouseDown += delegate (UIComponent sender, UIMouseEventParameter e) { OnToggleMouseDown(sender, e, type); };
            sliderPanel.eventKeyDown += delegate (UIComponent sender, UIKeyEventParameter e) { OnToggleKeyDown(sender, e); };

            UIPanel sliderLeftPanel = sliderPanel.AddUIComponent<UIPanel>();
            sliderLeftPanel.name = "pnlLeft" + typeString;
            sliderLeftPanel.height = sliderPanel.height - 4;
            sliderLeftPanel.width = (0.7f * sliderPanel.width) - 5;
            sliderLeftPanel.relativePosition = new Vector2(2, 2);

            UITextField sliderTextField = sliderPanel.AddUIComponent<UITextField>();
            sliderTextField.isInteractive = false;
            sliderTextField.name = "tf" + typeString;
            sliderTextField.text = sData.Def.ToString();
            sliderTextField.height = sliderPanel.height;
            sliderTextField.width = sliderPanel.size.x - sliderLeftPanel.size.x;
            sliderTextField.relativePosition = new Vector2(sliderLeftPanel.width, 2);
            sliderTextField.eventMouseDown += delegate (UIComponent sender, UIMouseEventParameter e) { OnToggleMouseDown(sender, e, type); };

            UISlicedSprite sliderBgSprite = sliderLeftPanel.AddUIComponent<UISlicedSprite>();
            sliderBgSprite.isInteractive = false;
            sliderBgSprite.atlas = atlas;
            sliderBgSprite.name = "ssBgSprite" + typeString;
            sliderBgSprite.spriteName = "BudgetSlider";
            sliderBgSprite.size = sliderLeftPanel.size;
            sliderBgSprite.relativePosition = new Vector2(0, 0);
            sliderBgSprite.zOrder = 0;

            UISlicedSprite sliderMkSprite = sliderLeftPanel.AddUIComponent<UISlicedSprite>();
            sliderMkSprite.atlas = atlas;
            sliderMkSprite.name = "ssMkSprite" + typeString;
            sliderMkSprite.spriteName = "SliderBudget";
            sliderMkSprite.isInteractive = true;
            sliderMkSprite.zOrder = 1;

            UISlider slider = sliderLeftPanel.AddUIComponent<UISlider>();
            SliderDict[type] = slider;
            slider.name = typeString + " Slider";
            slider.isInteractive = true;
            slider.maxValue = sData.Max;
            slider.minValue = sData.Min;
            slider.value = sData.Def;
            slider.stepSize = sData.Step;
            slider.relativePosition = new Vector2(0, 0);
            slider.size = sliderLeftPanel.size;
            slider.thumbObject = sliderMkSprite;
            slider.zOrder = 2;
            slider.eventMouseWheel += (c, v) =>
            {
                slider.value += v.wheelDelta > 0 ? sData.Step : -sData.Step;
                OnToggleMouseDown(c, v, type);
            };
            slider.eventValueChanged += (c, v) =>
            {
                if (sliderTextField.text != v.ToString())
                {
                    m_valueChanged = true;
                    v = Math.Min(Math.Max(sData.Min, v), sData.Max);
                    sliderTextField.text = v.ToString();
                    SetDict[type] = v;
                    //m_T.Run();
                }
            };
            slider.eventMouseUp += (c, e) =>
            {
                OnToggleMouseDown(c, e, type);
                if (m_valueChanged)
                {
                    m_valueChanged = false;
                }
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

            SliderDataDict = new Dictionary<ToggleType, SliderData>();
            SliderDataDict.Add(ToggleType.Depth, new SliderData()
            {
                Def = SetStationCustomizations.DEF_DEPTH,
                Max = SetStationCustomizations.MAX_DEPTH,
                Min = SetStationCustomizations.MIN_DEPTH,
                SetVal = SetStationCustomizations.DEF_DEPTH,
                Step = DEPTH_STEP
            });
            SliderDataDict.Add(ToggleType.Length, new SliderData()
            {
                Def = SetStationCustomizations.DEF_LENGTH,
                Max = SetStationCustomizations.MAX_LENGTH,
                Min = SetStationCustomizations.MIN_LENGTH,
                SetVal = SetStationCustomizations.DEF_LENGTH,
                Step = LENGTH_STEP
            });
            SliderDataDict.Add(ToggleType.Angle, new SliderData()
            {
                Def = SetStationCustomizations.DEF_ANGLE,
                Max = SetStationCustomizations.MAX_ANGLE,
                Min = SetStationCustomizations.MIN_ANGLE,
                SetVal = SetStationCustomizations.DEF_ANGLE,
                Step = ANGLE_STEP
            });
            SliderDataDict.Add(ToggleType.Bend, new SliderData()
            {
                Def = SetStationCustomizations.DEF_BEND_STRENGTH,
                Max = SetStationCustomizations.MAX_BEND_STRENGTH,
                Min = SetStationCustomizations.MIN_BEND_STRENGTH,
                SetVal = SetStationCustomizations.DEF_BEND_STRENGTH,
                Step = BEND_STRENGTH_STEP
            });
            SliderDict = new Dictionary<ToggleType, UISlider>();
            PanelDict = new Dictionary<ToggleType, UIPanel>();

            CreateSlider(ToggleType.Length);
            CreateSlider(ToggleType.Depth);
            CreateSlider(ToggleType.Angle);
            CreateSlider(ToggleType.Bend);

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
            if (info.m_paths != null && info.m_paths.Length > 0)
            {
                for (var i = 0; i < info.m_paths.Length; i++)
                {
                    var path = info.m_paths[i];
                    if (path?.m_netInfo?.name != null && path.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        if (m_PrevTrackType != StationTrackType.SidePlatform)
                        {
                            path.AssignNetInfo("Metro Station Track Tunnel");
                        }
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
                                path.AssignNetInfo("Metro Station Track Tunnel");
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
                                path.AssignNetInfo("Metro Station Track Tunnel Island");
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
                                path.AssignNetInfo("Metro Station Track Tunnel Small");
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
            foreach (UIPanel pnl in PanelDict.Values)
            {
                pnl.color = new Color32(150, 150, 150, 210);
            }
            m_currentBuilding = null;
            m_currentSuperBuilding = null;
            isVisible = false;
            m_activated = false;

        }

        private void DoStationMechanics()
        {
            //var angleDelta = Math.PI / 180 * (SetDict[ToggleType.Angle] - m_oldAngle);
            //m_oldAngle = SetDict[ToggleType.Angle];
            SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[ToggleType.Depth], SetDict[ToggleType.Length], (int)SetDict[ToggleType.Angle], SetDict[ToggleType.Bend], m_currentSuperBuilding);
        }
    }
    public struct SliderData
    {
        public float Max { get; set; }
        public float Min { get; set; }
        public float Def { get; set; }
        public float Step { get; set; }
        public float SetVal { get; set; }
        public void SetTheVal(float val)
        {
            SetVal = val;
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
