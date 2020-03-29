using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static MetroOverhaul.StationTrackCustomizations;
//using StationTrackType = MetroOverhaul.ModTrackNames.StationTrackType;

namespace MetroOverhaul.UI
{
    public class MetroCustomizerBaseUI : UIPanel
    {
        public TrackVehicleType trackVehicleType = TrackVehicleType.Default;
        public TrackStyle trackStyle = TrackStyle.Modern;
        public PillarType pillarType = PillarType.None;
        public int trackSize = 1;
        public int trackDirection = 1;
        protected BulldozeTool m_bulldozeTool;
        protected NetTool m_netTool;
        protected BuildingTool m_buildingTool;
        protected UIButton m_upgradeButtonTemplate;
        protected NetInfo m_currentNetInfo;
        protected BuildingInfo m_currentBuilding;
        protected BuildingInfo m_currentCloneBuilding;
        protected bool m_activated = false;
        public static MetroTrackCustomizerUI instance;

        protected NetInfo trainTrackPrefab;
        protected NetInfo trainStationTrackPrefab;

        protected NetInfo vanillaPrefab;

        protected NetInfo concretePrefab;
        protected NetInfo concretePrefabNoBar;

        protected NetInfo concreteTwoLaneOneWayPrefab;
        protected NetInfo concreteTwoLaneOneWayPrefabNoBar;

        protected NetInfo concreteLargePrefab;
        protected NetInfo concreteLargePrefabNoBar;

        protected NetInfo concreteSmallPrefab;
        protected NetInfo concreteSmallPrefabNoBar;

        protected NetInfo concreteSmallTwoWayPrefab;
        protected NetInfo concreteSmallTwoWayPrefabNoBar;

        protected NetInfo steelPrefab;
        protected NetInfo steelPrefabNoBar;

        protected NetInfo steelTwoLaneOneWayPrefab;
        protected NetInfo steelTwoLaneOneWayPrefabNoBar;

        protected NetInfo steelLargePrefab;
        protected NetInfo steelLargePrefabNoBar;

        protected NetInfo steelSmallPrefab;
        protected NetInfo steelSmallPrefabNoBar;

        protected NetInfo steelSmallTwoWayPrefab;
        protected NetInfo steelSmallTwoWayPrefabNoBar;

        protected UIButton btnMetro;
        protected UIButton btnTrain;
        protected UIButton btnDefault;
        protected UIButton btnModernStyle;
        protected UIButton btnClassicStyle;
        protected UIButton btnVanillaStyle;
        protected UIButton btnNoStyle;
        protected UIButton btnSingleTrack;
        protected UIButton btnDoubleTrack;
        protected UIButton btnQuadTrack;
        protected UIButton btnOneWay;
        protected UIButton btnTwoWay;
        protected UIButton btnStation;
        protected UIButton btnTrack;
        protected UIButton btnWideMedianPillar;
        protected UIButton btnWidePillar;
        protected UIButton btnNarrowMedianPillar;
        protected UIButton btnNarrowPillar;
        protected UIPanel pnlPillarChooser;
        protected UIPanel pnlTrackVsStation;
        protected UIPanel pnlStationTypeSelector;
        protected UIPanel pnlStyles;
        protected UIPanel pnlOuterContainer;
        protected UIPanel pnlInnerContainer;
        protected UIPanel pnlTrackSelectorContainerWrapper;
        protected UIPanel pnlTrackSelectorContainer;
        protected UIPanel pnlStyleContainer;
        protected UILabel lblTabTitle;
        protected UITextField tfStationXPanelTooltip;
        protected UITextField tfStationYPanelTooltip;
        protected UITextField tfStationLengthPanelTooltip;
        protected UITextField tfStationDepthPanelTooltip;
        protected UITextField tfStationRotationPanelTooltip;
        protected UITextField tfStationCurvePanelTooltipTooltip;
        protected Task m_T = null;

        protected bool m_valueChanged = false;
        protected MetroStationTrackAlterType m_Toggle = MetroStationTrackAlterType.None;
        public Dictionary<MetroStationTrackAlterType, SliderData> SliderDataDict { get; set; }
        public static Dictionary<MetroStationTrackAlterType, UISlider> SliderDict { get; set; }
        protected static Dictionary<MetroStationTrackAlterType, UIPanel> PanelDict { get; set; }
        protected static Dictionary<StationTrackType, UICheckBox> CheckboxStationDict { get; set; }
        protected static Dictionary<StationTrackType, UIButton> buttonStationDict { get; set; }
        protected static Dictionary<string, UICheckBox> CheckboxDict { get; set; }
        protected Dictionary<MetroStationTrackAlterType, UIButton> toggleBtnDict = new Dictionary<MetroStationTrackAlterType, UIButton>();
        public Dictionary<MetroStationTrackAlterType, float> SetDict = new Dictionary<MetroStationTrackAlterType, float>();

        public StationTrackType m_TrackType = StationTrackType.None;

        public int isStation = 1;
        public StationTrackType stationType;

        protected NetInfo concreteSideStationPrefab;
        protected NetInfo concreteIslandStationPrefab;
        protected NetInfo concreteSingleStationPrefab;
        protected NetInfo concreteQuadSideStationPrefab;
        protected NetInfo concreteQuadDualIslandStationPrefab;

        protected NetInfo steelSideStationPrefab;
        protected NetInfo steelIslandStationPrefab;
        protected NetInfo steelSingleStationPrefab;
        protected NetInfo steelQuadSideStationPrefab;
        protected NetInfo steelQuadDualIslandStationPrefab;

        protected const string ALT_BARRIER = "Alternate Design";
        protected const string OVER_ROAD_FRIENDLY = "Over-Road Friendly";
        protected const string EXTRA_PILLARS = "Extra Pillars";

        protected const string TRAIN_DEFAULT_TAB_LABEL = "PLOPS A TRAIN STATION (DEFAULT)";
        protected const string METRO_DEFAULT_TAB_LABEL = "PLOPS A METRO STATION (DEFAULT)";
        protected const string TRAIN_TAB_LABEL = "PLOPS A TRAIN STATION";
        protected const string METRO_TAB_LABEL = "PLOPS A METRO STATION";

        protected const string STATION_TYPE_SELECTOR_INFO = "Convert your station from Default, to Train or to Metro.\n Default tab leaves the station as normal, Train tab converts all tracks to accept train vehicles and allows intercity trains.\n Metro tab converts all tracks to accept metro vehicles. After selecting a station type you can further customize individual track types below.";

        protected const string TS_STARTER = "btnTsStarter";
        protected const string TS_ENDER = "btnTsEnder";

        protected string Station_X_Info { get { return $"Adjusts the horizontal alignment of your station track. Default is {SetStationCustomizations.DEF_LENGTH}m with a maximum length of {SetStationCustomizations.MAX_LENGTH}m and a minimum length of {SetStationCustomizations.MIN_LENGTH}m."; } }
        protected string Station_Y_Info { get { return $"Adjusts the vertical alignment of your station track. Default is {SetStationCustomizations.DEF_LENGTH}m with a maximum length of {SetStationCustomizations.MAX_LENGTH}m and a minimum length of {SetStationCustomizations.MIN_LENGTH}m."; } }
        protected string Station_Length_Info { get { return $"Adjusts the length of your station track. It is recommended to set this to about the length of your Metro Trains. Default is {SetStationCustomizations.DEF_LENGTH}m with a maximum length of {SetStationCustomizations.MAX_LENGTH}m and a minimum length of {SetStationCustomizations.MIN_LENGTH}m."; } }
        protected string Station_Depth_Info { get { return $"Adjusts the depth of your station track. It is recommended you set your tracks deeper on uneven terrain. Default depth is {SetStationCustomizations.DEF_DEPTH}m with a maximum depth of {SetStationCustomizations.MAX_DEPTH}m and a minimum depth of {SetStationCustomizations.MIN_DEPTH}m."; } }
        protected string Station_Rotation_Info { get { return $"Adjusts the angle of your station track. This allows you to spin your tracks to align to your metro layout. Default angle is {SetStationCustomizations.DEF_ROTATION}° with a maximum angle of {SetStationCustomizations.MAX_ROTATION}° and a minimum angle of {SetStationCustomizations.MIN_ROTATION}°."; } }
        protected string Station_Curve_Info { get { return $"Adjusts the curve in the station track. This is useful for situations where your station connects tracks arriving from different angles. Default curve is { SetStationCustomizations.DEF_CURVE }° (no curve) with a maximum curve of { SetStationCustomizations.MAX_CURVE}° and a minimum curve of { SetStationCustomizations.MIN_CURVE}°."; } }
        protected Color32 DefaultColor = new Color32(90, 115, 217, 255);
        protected Color32 TrainColor = new Color32(233, 85, 38, 255);
        protected Color32 MetroColor = new Color32(6, 213, 73, 255);
        protected ItemClass m_TheIntersectClass = null;

        public bool IsActivated()
        {
            return m_activated;
        }
        protected virtual bool SatisfiesTrackSpecs(PrefabInfo info)
        {
            return false;
        }
        protected virtual ToolBase GetTheTool()
        {
            return null;
        }
        protected virtual PrefabInfo GetToolPrefab()
        {
            return null;
        }
        protected virtual PrefabInfo CurrentInfo { get; set; }

        public override void Update()
        {
            try
            {
                if (GetTheTool() != null && GetTheTool().enabled)
                {
                    var toolInfo = GetToolPrefab();
                    if (toolInfo != null)
                    {
                        if (CurrentInfo?.name == null || toolInfo.name != CurrentInfo.name)
                        {
                            if (SatisfiesTrackSpecs(toolInfo))
                            {
                                Activate(toolInfo);
                            }
                            else
                            {
                                Deactivate();
                            }
                        }
                    }
                    else
                    {
                        Deactivate();
                    }
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
            m_netTool = GetExactTool<NetTool>();
            if (m_netTool == null)
            {
#if DEBUG
                Next.Debug.Log("NetTool Not Found");
#endif
                enabled = false;
                return;
            }
            m_buildingTool = GetExactTool<BuildingTool>();
            if (m_buildingTool == null)
            {
#if DEBUG
                Next.Debug.Log("BuildingTool Not Found");
#endif
                enabled = false;
                return;
            }
            m_bulldozeTool = GetExactTool<BulldozeTool>();
            if (m_bulldozeTool == null)
            {
#if DEBUG
                Next.Debug.Log("BuildingTool Not Found");
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
            SubStart();
        }

        private T GetExactTool<T>() where T : UnityEngine.Object => FindObjectsOfType<T>().Where(x => x.GetType() == typeof(T)).FirstOrDefault();

        private float m_SetWidth = 250;
        protected virtual void CreateUI()
        {
            backgroundSprite = "GenericPanel";
            color = new Color32(73, 68, 84, 170);
            width = m_SetWidth;
            height = 0;
            opacity = 90;
            name = GetType().ToString();
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };
        }
        protected virtual void SubStart() { }

        public override void Awake()
        {
            trainTrackPrefab = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            trainStationTrackPrefab = PrefabCollection<NetInfo>.FindLoaded("Train Station Track");

            vanillaPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Vanilla));

            concretePrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern));
            concretePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.TwowayTwoLane, true));

            concreteTwoLaneOneWayPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.OnewayTwoLane));
            concreteTwoLaneOneWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.OnewayTwoLane, true));

            concreteLargePrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.Quad));
            concreteLargePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.Quad, true));

            concreteSmallPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.OnewayOneLane));
            concreteSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.OnewayOneLane, true));

            concreteSmallTwoWayPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.TwowayOneLane));
            concreteSmallTwoWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Modern, TrackType.TwowayOneLane, true));

            concreteSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Modern));
            concreteIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Modern, StationTrackType.IslandPlatform));
            concreteSingleStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Modern, StationTrackType.SinglePlatform));
            concreteQuadSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Modern, StationTrackType.ExpressSidePlatform));
            concreteQuadDualIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Modern, StationTrackType.DualIslandPlatform));

            steelPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic));
            steelPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.TwowayTwoLane, true));

            steelTwoLaneOneWayPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.OnewayTwoLane));
            steelTwoLaneOneWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.OnewayTwoLane, true));

            steelSmallPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.OnewayOneLane));
            steelSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.OnewayOneLane, true));

            steelSmallTwoWayPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.TwowayOneLane));
            steelSmallTwoWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.TwowayOneLane, true));

            steelLargePrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.Quad));
            steelLargePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Ground, TrackStyle.Classic, TrackType.Quad, true));

            steelSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Classic));
            steelIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Classic, StationTrackType.IslandPlatform));
            steelSingleStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Classic, StationTrackType.SinglePlatform));
            steelQuadSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Classic, StationTrackType.ExpressSidePlatform));
            steelQuadDualIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroStationTrackName(NetInfoVersion.Ground, TrackStyle.Classic, StationTrackType.DualIslandPlatform));
        }

        protected List<UIButton> m_ButtonArray = null;
        protected List<UIButton> ButtonArray {
            get
            {
                if (m_ButtonArray == null)
                {
                    m_ButtonArray = new List<UIButton>();
                }
                return m_ButtonArray;
            }
        }
        protected void ClearButtonArray()
        {
            if (m_ButtonArray != null)
            {
                m_ButtonArray.Clear();
                m_ButtonArray = null;
            }
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
            titleLabel.relativePosition = new Vector3() { x = 50, y = 3, z = 0 };
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.text = title;
            titleLabel.isInteractive = false;

            UIButton headerIcon = dragHandlePanel.AddUIComponent<UIButton>();
            headerIcon.atlas = UIHelper.GenerateLinearAtlas("MOM_HeaderIconAtlas", UIHelper.HeaderIconTexture, 1, new string[] { UIHelper.HeaderIconTexture.name });
            headerIcon.relativePosition = new Vector3() { x = 5, y = -10 };
            headerIcon.width = 40;
            headerIcon.height = 40;
            headerIcon.normalFgSprite = UIHelper.HeaderIconTexture.name;

            height += 2 * dragHandle.height;
        }
        protected T CreateUIComponent<T>(UIParamPropsBase properties, bool isFixed = true) where T : UIComponent
        {
            properties.ParentComponent = properties.ParentComponent ?? this;
            return (T)InitComponent(properties.ParentComponent.AddUIComponent<T>(), properties, isFixed);
        }
        protected UIPanel CreatePanel(UIPanelParamProps properties, bool isFixed = true)
        {
            var panel = CreateUIComponent<UIPanel>(properties, isFixed);
            panel.backgroundSprite = properties.BackgroundSprite;
            if (properties.Color != null)
            {
                panel.color = (Color32)properties.Color;
            }
            panel.zOrder = 10;
            return panel;
        }
        protected UITabstrip CreateTabStrip(UITabstripParamProps properties)
        {
            var tabstrip = CreateUIComponent<UITabstrip>(properties);
            tabstrip.navigateWithArrowTabKeys = true;
            tabstrip.startSelectedIndex = properties.StartSelectedIndex;

            return tabstrip;
        }

        protected UILabel CreateLabel(UILabelParamProps properties)
        {
            var label = CreateUIComponent<UILabel>(properties);
            label.textColor = properties.TextColor;
            label.textAlignment = properties.TextAlignment;
            label.textScale = properties.TextScale;
            if (!string.IsNullOrEmpty(properties.Text))
                label.text = properties.Text;
            return label;
        }
        protected void RefreshSprites(UIButton button, UITextureAtlas newAtlas, bool includeFgSprites = true)
        {
            if (button != null && newAtlas != null)
            {
                button.atlas = newAtlas;
                button.normalBgSprite = button.atlas.name + "Bg";
                button.disabledBgSprite = button.atlas.name + "BgDisabled";
                button.focusedBgSprite = button.atlas.name + "BgFocused";
                button.hoveredBgSprite = button.atlas.name + "BgHovered";
                button.pressedBgSprite = button.atlas.name + "BgPressed";
                if (includeFgSprites)
                {
                    button.normalFgSprite = button.atlas.name + "Fg";
                    button.disabledFgSprite = button.atlas.name + "FgDisabled";
                    button.focusedFgSprite = button.atlas.name + "FgFocused";
                    button.hoveredFgSprite = button.atlas.name + "FgHovered";
                    button.pressedFgSprite = button.atlas.name + "FgPressed";
                }
            }
        }
        public int GetMaxButtonFit(UITabstrip tabstrip, float buttonWidth)
        {
            return (int)Math.Floor(tabstrip.width / buttonWidth);
        }
        private bool TsCanFitButton(UITabstrip tabStrip, UIButtonParamProps buttonProperties)
        {
            var childButtons = tabStrip.GetComponentsInChildren<UIButton>();
            var totalWidth = childButtons.Sum(b => b.width) + buttonProperties.Width;
            return totalWidth <= tabStrip.width;
        }
        protected UITabstrip[] FindUITabStripsInPanel(UIPanel panel)
        {
            return panel?.GetComponentsInChildren<UITabstrip>();
        }
        protected UITabstrip FindUITabStripInPanel(UIPanel panel, int index = 0)
        {
            var tabStrip = panel?.GetComponentsInChildren<UITabstrip>();
            if (tabStrip != null)
            {
                return tabStrip[index];
            }
            return null;
        }
        protected UIButton CreateButton(UIButtonParamProps properties)
        {
            var tsCount = 0;
            properties.ParentComponent = properties.ParentComponent ?? this;
            UITabstrip tabStrip = null;
            if (properties.ParentComponent is UIPanel)
            {
                var childTabStrips = properties.ParentComponent.GetComponentsInChildren<UITabstrip>();
                List<int> removalIndices = null;
                for (var i = childTabStrips.Count() - 1; i >= 0; i--)
                {
                    var childButtons = childTabStrips[i].GetComponentsInChildren<UIButton>();
                    if (childButtons.Count() == 0 || childButtons.All(b => b.name == TS_STARTER || b.name == TS_ENDER))
                    {
                        if (removalIndices == null)
                            removalIndices = new List<int>();
                        removalIndices.Add(i);
                    }
                }
                if (removalIndices != null)
                {
                    for (var i = 0; i < removalIndices.Count(); i++)
                    {
                        RemoveComponent(childTabStrips[removalIndices[i]]);
                    }
                }
                var lastTabstrip = childTabStrips.LastOrDefault();
                if (childTabStrips == null || childTabStrips.Count() == 0 || !TsCanFitButton(lastTabstrip, properties))
                {
                    if (childTabStrips != null)
                    {
                        tsCount = childTabStrips.Count();
                        if (childTabStrips.Count() > 0)
                        {

                            var enderProperties = new UIButtonParamProps()
                            {
                                Name = TS_ENDER + tsCount,
                                ColumnCount = 2,
                                ParentComponent = lastTabstrip,
                                ForceRowEnd = true,
                                Width = 1,
                                Height = 1,
                            };
                            CreateUIComponent<UIButton>(enderProperties);
                            lastTabstrip.startSelectedIndex = 0;
                            lastTabstrip.selectedIndex = 0;
                        }
                    }
                    m_colIndex = 0;
                    var tsProperties = new UITabstripParamProps()
                    {
                        Name = properties.ParentComponent.name.Replace("pnl", "ts") + tsCount,
                        ColumnCount = 1,
                        ParentComponent = properties.ParentComponent
                    };
                    tabStrip = CreateUIComponent<UITabstrip>(tsProperties);
                }
                else
                {
                    tabStrip = childTabStrips.LastOrDefault();
                    if (properties.SelectOnCreate)
                        tabStrip.startSelectedIndex = tabStrip.GetComponentsInChildren<UIButton>().ToList().Count();
                }

                properties.ParentComponent = tabStrip;
                if (tabStrip.childCount == 0)
                {
                    var starterProperties = new UIButtonParamProps()
                    {
                        Name = TS_STARTER + tsCount,
                        ColumnCount = 2,
                        ParentComponent = tabStrip,
                        ForceRowEnd = true,
                        Width = 1,
                        Height = 1,
                    };
                    if (properties.SelectOnCreate)
                        tabStrip.startSelectedIndex = 1;
                    CreateUIComponent<UIButton>(starterProperties);
                }
            }
            var button = CreateUIComponent<UIButton>(properties);
            if (properties.SelectOnCreate)
                tabStrip.selectedIndex = tabStrip.GetComponentsInChildren<UIButton>().ToList().IndexOf(button);
            if (!string.IsNullOrEmpty(properties.Text))
                button.text = properties.Text;
            if (properties.Atlas != null)
            {
                button.atlas = properties.Atlas;
                if (properties.HasFgBgSprites)
                {
                    RefreshSprites(button, button.atlas);
                }
                else
                {
                    button.normalBgSprite = properties.NormalBgSprite != null ? properties.NormalBgSprite : properties.Atlas.name + "Bg";
                    button.disabledBgSprite = properties.DisabledBgSprite != null ? properties.DisabledBgSprite : properties.Atlas.name + "BgDisabled";
                    button.focusedBgSprite = properties.FocusedBgSprite != null ? properties.FocusedBgSprite : properties.Atlas.name + "BgFocused";
                    button.hoveredBgSprite = properties.HoveredBgSprite != null ? properties.HoveredBgSprite : properties.Atlas.name + "BgHovered";
                    button.pressedBgSprite = properties.PressedBgSprite != null ? properties.PressedBgSprite : properties.Atlas.name + "BgPressed";
                }
            }
            else
            {
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
                if (!string.IsNullOrEmpty(properties.NormalFgSprite))
                    button.normalFgSprite = properties.NormalFgSprite;
                button.focusedBgSprite = "ButtonMenuFocused";
            }
            if (properties.TooltipComponent != null)
            {
                button.tooltip = "-";
                button.tooltipBox = properties.TooltipComponent;
            }

            if (properties.DisableState)
            {
                button.enabled = false;
                button.isInteractive = false;
                button.state = UIButton.ButtonState.Disabled;
            }
            button.playAudioEvents = true;
            button.opacity = 95;
            button.dropShadowColor = new Color32(0, 0, 0, 255);
            button.dropShadowOffset = new Vector2(-1, -1);
            button.eventKeyDown += delegate (UIComponent sender, UIKeyEventParameter e)
            { OnToggleKeyDown(sender, e); };
            return button;
        }
        protected UISlicedSprite CreateUISlicedSprite(UIParamPropsBase properties)
        {
            var sprite = CreateUIComponent<UISlicedSprite>(properties);
            if (properties.Atlas != null)
                sprite.atlas = properties.Atlas;
            sprite.zOrder = 1;

            return sprite;
        }
        protected UIPanel CreateToolTipPanel(UIToolTipPanelParamProps properties)
        {
            var pnlStationTypeSelectorTooltip = CreatePanel(new UIPanelParamProps()
            {
                Name = properties.Name.Replace("ttp", "pnl"),
                Atlas = atlas,
                ParentComponent = properties.ParentComponent,
                BackgroundSprite = "GenericPanel"
            }, false);
            pnlStationTypeSelectorTooltip.isVisible = false;
            pnlStationTypeSelectorTooltip.autoLayoutDirection = LayoutDirection.Vertical;
            pnlStationTypeSelectorTooltip.autoLayout = true;
            pnlStationTypeSelectorTooltip.pivot = UIPivotPoint.BottomLeft;
            pnlStationTypeSelectorTooltip.opacity = 0;
            pnlStationTypeSelectorTooltip.relativePosition = new Vector3(420, -50);
            UILabel tfStationTypeSelectorTooltip = CreateLabel(new UILabelParamProps()
            {
                Name = "tfStationTypeSelectorTooltip",
                Text = properties.ToolTipPanelText,
                ParentComponent = pnlStationTypeSelectorTooltip
            });
            tfStationTypeSelectorTooltip.wordWrap = true;
            return pnlStationTypeSelectorTooltip;
        }
        protected UISlider CreateSlider(UISliderParamProps properties)
        {
            SliderData sData = SliderDataDict[properties.TrackAlterType];
            string typeString = properties.TrackAlterType.ToString();
            var parentComponent = properties.ParentComponent ?? this;

            UILabel titleLabel = CreateLabel(new UILabelParamProps()
            {
                Name = "lblTitle" + typeString,
                Text = typeString,
                ParentComponent = parentComponent,
                ColShare = 3.25f,
                Margins = new Vector4(28, 2, 0, 0)
            });

            titleLabel.isInteractive = true;
            titleLabel.eventMouseDown += delegate (UIComponent sender, UIMouseEventParameter e)
            { OnToggleMouseDown(sender, e, properties.TrackAlterType); };
            titleLabel.eventKeyDown += delegate (UIComponent sender, UIKeyEventParameter e)
            { OnToggleKeyDown(sender, e); };

            UIPanel sliderPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "sldPanel" + typeString,
                ParentComponent = parentComponent,
                ColShare = 8.75f,
                Atlas = atlas,
                BackgroundSprite = "GenericPanel",
                Color = new Color32(150, 150, 150, 210)
            });
            PanelDict[properties.TrackAlterType] = sliderPanel;
            sliderPanel.playAudioEvents = true;
            sliderPanel.eventMouseDown += delegate (UIComponent sender, UIMouseEventParameter e)
            { OnToggleMouseDown(sender, e, properties.TrackAlterType); };
            sliderPanel.eventKeyDown += delegate (UIComponent sender, UIKeyEventParameter e)
            { OnToggleKeyDown(sender, e); };
            sliderPanel.eventKeyUp += delegate (UIComponent sender, UIKeyEventParameter e)
            { OnToggleKeyUp(sender, e); };

            //var pnlStationTypeSelectorInfo = CreatePanel(new UIPanelParamProps()
            //{
            //    Name = "pnlStationTypeSelectorInfo" + typeString,
            //    ParentComponent = parentComponent,
            //    ColShare = 1
            //});
            //var btnStationTypeSelectorInfo = CreateButton(new UIButtonParamProps()
            //{
            //    Name = "btnStationTypeSelectorInfo" + typeString,
            //    Atlas = atlas,
            //    ParentComponent = pnlStationTypeSelectorInfo,
            //    NormalBgSprite = "EconomyMoreInfo",
            //    HoveredBgSprite = "EconomyMoreInfoHovered",
            //    PressedBgSprite = "EconomyMoreInfoHovered",
            //    Height = 12,
            //    Width = 12,
            //    TooltipComponent = properties.TooltipComponent,
            //    StackWidths = true
            //});

            UIPanel sliderLeftPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "sliderLeftPanel" + typeString,
                ParentComponent = sliderPanel,
                Height = 12,
                ColShare = 10,
                Margins = new Vector4(2, 0, 0, 2)
            });
            //sliderLeftPanel.height = sliderPanel.height - 4;
            UIPanel sliderRightPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "sliderRightPanel" + typeString,
                ParentComponent = sliderPanel,
                ColShare = 2
            });

            UILabel sliderValueLabel = CreateLabel(new UILabelParamProps()
            {
                Name = "sliderValueLabel" + typeString,
                Text = sData.Def.ToString(),
                ParentComponent = sliderRightPanel,
                ColumnCount = 1,

            });
            sliderValueLabel.isInteractive = false;
            sliderValueLabel.eventMouseDown += delegate (UIComponent sender, UIMouseEventParameter e)
            { OnToggleMouseDown(sender, e, properties.TrackAlterType); };

            UISlicedSprite sliderBgSprite = sliderLeftPanel.AddUIComponent<UISlicedSprite>();
            sliderBgSprite.isInteractive = false;
            sliderBgSprite.atlas = atlas;
            sliderBgSprite.name = "ssBgSprite" + typeString;
            sliderBgSprite.spriteName = "BudgetSlider";
            sliderBgSprite.size = sliderLeftPanel.size;
            sliderBgSprite.relativePosition = new Vector2(0, 0);
            //sliderBgSprite.eventMouseDown += (c, v) => { OnToggleMouseDown(c, v, properties.TrackAlterType); };
            //sliderBgSprite.eventMouseUp += (c, v) => { OnToggleMouseUp(c, v, properties.TrackAlterType); };
            sliderBgSprite.zOrder = 0;

            UISlicedSprite sliderMkSprite = sliderLeftPanel.AddUIComponent<UISlicedSprite>();
            sliderMkSprite.atlas = atlas;
            sliderMkSprite.name = "ssMkSprite" + typeString;
            sliderMkSprite.spriteName = "SliderBudget";
            sliderMkSprite.isInteractive = true;
            //sliderMkSprite.eventMouseDown += (c, v) => { OnToggleMouseDown(c, v, properties.TrackAlterType); };
            //sliderMkSprite.eventMouseUp += (c, v) => { OnToggleMouseUp(c, v, properties.TrackAlterType); };
            sliderMkSprite.zOrder = 1;

            UISlider slider = sliderLeftPanel.AddUIComponent<UISlider>();
            SliderDict[properties.TrackAlterType] = slider;
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
                Debug.Log("Slider did this.  Wheel Delta " + v.wheelDelta);
                SliderDict[properties.TrackAlterType].value += v.wheelDelta > 0 ? sData.Step : -sData.Step;
                OnToggleMouseDown(c, v, properties.TrackAlterType);
                OnToggleMouseUp(c, v, properties.TrackAlterType);
                //OnToggleMouseUp(c, v, properties.TrackAlterType);
            };
            sliderValueLabel.eventMouseWheel += (c, v) =>
            {
                Debug.Log("SliderValueLabel did this.  Wheel Delta " + v.wheelDelta);
                SliderDict[properties.TrackAlterType].value += v.wheelDelta > 0 ? sData.Step : -sData.Step;
                OnToggleMouseDown(c, v, properties.TrackAlterType);
                OnToggleMouseUp(c, v, properties.TrackAlterType);
                //OnToggleMouseUp(c, v, properties.TrackAlterType);
            };
            sliderPanel.eventMouseWheel += (c, v) =>
            {
                Debug.Log("SliderPanel did this.  Wheel Delta " + v.wheelDelta);
                SliderDict[properties.TrackAlterType].value += v.wheelDelta > 0 ? sData.Step : -sData.Step;
                OnToggleMouseDown(c, v, properties.TrackAlterType);
                OnToggleMouseUp(c, v, properties.TrackAlterType);
                //OnToggleMouseUp(c, v, properties.TrackAlterType);
            };
            sliderLeftPanel.eventMouseWheel += (c, v) =>
            {
                Debug.Log("SliderPanel did this.  Wheel Delta " + v.wheelDelta);
                SliderDict[properties.TrackAlterType].value += v.wheelDelta > 0 ? sData.Step : -sData.Step;
                OnToggleMouseDown(c, v, properties.TrackAlterType);
                OnToggleMouseUp(c, v, properties.TrackAlterType);
                //OnToggleMouseUp(c, v, properties.TrackAlterType);
            };
            slider.eventValueChanged += (c, v) =>
            {
                if (sliderValueLabel.text != v.ToString())
                {
                    m_valueChanged = true;
                    v = Math.Min(Math.Max(sData.Min, v), sData.Max);
                    sliderValueLabel.text = v.ToString();
                    SetDict[properties.TrackAlterType] = v;
                    //m_T.Run();
                }
            };
            //slider.eventMouseDown += (c, e) =>
            //{
            //    OnToggleMouseDown(c, e, properties.TrackAlterType);
            //};
            //slider.eventMouseDown += (c, e) =>
            // {

            // };
            slider.eventMouseUp += (c, e) =>
            {
                OnToggleMouseDown(c, e, properties.TrackAlterType);
                OnToggleMouseUp(c, e, properties.TrackAlterType);
                if (m_valueChanged)
                {
                    m_valueChanged = false;
                }
            };
            return slider;
        }
        protected UICheckBox CreateCheckbox(UICheckboxParamProps properties)
        {
            var checkbox = CreateUIComponent<UICheckBox>(properties);
            if (!string.IsNullOrEmpty(properties.Text))
                checkbox.text = properties.Text;
            checkbox.isInteractive = true;

            UISprite cbClicker = checkbox.AddUIComponent<UISprite>();
            cbClicker.atlas = properties.Atlas;
            cbClicker.spriteName = properties.Atlas.name + "Bg";
            cbClicker.relativePosition = new Vector2(0, 0);
            cbClicker.size = new Vector2(16, 16);
            cbClicker.isInteractive = true;
            checkbox.eventCheckChanged += (c, v) =>
            {
                if (checkbox.isChecked)
                {
                    cbClicker.spriteName = properties.Atlas.name + "BgFocused";
                }
                else
                {
                    cbClicker.spriteName = properties.Atlas.name + "Bg";
                }
                ExecuteUiInstructions();
            };

            UILabel checkboxLabel = checkbox.AddUIComponent<UILabel>();
            checkboxLabel.relativePosition = new Vector2(20, 0);
            checkboxLabel.text = properties.Text;
            checkboxLabel.height = 16;
            checkboxLabel.isInteractive = true;
            return checkbox;
        }

        private int m_colIndex = 0;
        private float m_stackedWidths = 0;
        private float m_totalColShare = 0;
        private Dictionary<UIComponent, UIComponent> m_ParentTreeDict;
        private Dictionary<UIComponent, UIComponent> ParentTreeDict {
            get
            {
                if (m_ParentTreeDict == null)
                {
                    m_ParentTreeDict = new Dictionary<UIComponent, UIComponent>();
                }
                return m_ParentTreeDict;
            }
        }
        private void HandleNextPositioning(UIParamPropsBase properties)
        {
            m_colIndex++;
            if (properties.StackWidths)
            {
                m_stackedWidths += properties.Width + properties.Margins.x;
            }
            if (properties.ColShare > -1)
            {
                m_totalColShare += properties.ColShare + properties.ColOffset;
            }
            var target = properties.Component;
            if (m_colIndex * target.width > ParentTreeDict[target].width)
            {
                //var diff = ((m_colIndex + 1) * properties.Component.width) - ParentTreeDict[properties.Component].width;
                //PropagateParentComponentWidthUpdates(ParentTreeDict[properties.Component], diff);
            }
            if (m_colIndex == 1 && properties.ColShare == -1)
                if (properties.SameLine == false)
                {
                    var rowHeightAdditive = target.height + properties.Margins.y + properties.Margins.w;
                    Debug.Log("About to adjust reheight for " + target.name);
                    PropagateParentComponentHeightUpdates(target, rowHeightAdditive);
                }
            if ((m_colIndex == properties.ColumnCount && properties.ColShare == -1) || properties.ForceRowEnd || m_totalColShare >= 12)
            {
                m_colIndex = 0;
                m_totalColShare = 0;
                m_stackedWidths = 0;

                if (properties.SameLine == false)
                {                    
                    foreach (var kvp in ParentTreeDict)
                    {
                        if (kvp.Key.parent != this && kvp.Key.parent != null)
                        {
                            if (properties.Component?.parent?.parent != null)
                            {
                                if (kvp.Key.parent == properties.Component.parent.parent)
                                {

                                }
                            }

                        }
                    }
                }

            }
        }
        private void PropagateParentComponentWidthUpdates(UIComponent target, float widthAdditive)
        {
            target.width += widthAdditive;
            if (target != this && ParentTreeDict[target] != null)
            {
                PropagateParentComponentWidthUpdates(ParentTreeDict[target], widthAdditive);
            }
        }
        private void PropagateParentComponentHeightUpdates(UIComponent target, float heightAdditive)
        {
            if (target?.parent != null)
            {
                var targetVert = target.height + target.absolutePosition.y;
                var parentVert = target.parent.height + target.parent.absolutePosition.y;
                if (targetVert > parentVert)
                {
                    target.parent.height += heightAdditive;
                    PropagateParentComponentHeightUpdates(target.parent, heightAdditive);
                }
                //PropagateSiblingDisplacement(target);
            }
        }
        //private void PropagateSiblingDisplacement(UIComponent target)
        //{
        //    var allSiblings = target?.parent?.GetComponentsInChildren<UIComponent>();
        //    List<UIComponent> allRealSiblings = new List<UIComponent>();
        //    if (allSiblings != null)
        //    {
        //        for (int i = 0; i < allSiblings.Count(); i++)
        //        {
        //            if (allSiblings[i]?.parent != null && allSiblings[i].name != target.name && allSiblings[i].parent.name == target.parent.name)
        //            {
        //                allRealSiblings.Add(allSiblings[i]);
        //            }
        //        }
        //    }

        //    var allSibsCount = 0;
        //    if (allRealSiblings != null)
        //        allSibsCount = allRealSiblings.Count();
        //    var relevantSiblings = allRealSiblings.Where(c => c.absolutePosition.y > target.absolutePosition.y).OrderBy(c2 => c2.absolutePosition.y).ToList();
        //    var siblingsCount = 0;
        //    if (relevantSiblings != null)
        //        siblingsCount = relevantSiblings.Count();
        //    if (relevantSiblings != null && relevantSiblings.Count() > 0)
        //    {
        //        if (target.absolutePosition.y + target.height > relevantSiblings[0].absolutePosition.y)
        //        {
        //            var diff = target.absolutePosition.y + target.height - relevantSiblings[0].absolutePosition.y;
        //            foreach (var sibling in relevantSiblings)
        //                sibling.absolutePosition = new Vector3(sibling.absolutePosition.x, sibling.absolutePosition.y + diff, sibling.absolutePosition.z);
        //            PropagateParentComponentHeightUpdates(relevantSiblings.Last(), diff);
        //        }
        //    }
        //}
        private UIComponent InitComponent(UIComponent component, UIParamPropsBase properties, bool isFixed = true)
        {
            ParentTreeDict.Add(component, properties.ParentComponent);
            properties.Component = component;
            component.tooltip = properties.ToolTip;
            var addUIComponent = !(properties is UIButtonParamProps) || (properties as UIButtonParamProps).AddUIComponent;
            if (!string.IsNullOrEmpty(properties.Name))
                component.name = properties.Name;
            component.width = addUIComponent ? properties.GetWidth() : 0;
            component.height = addUIComponent ? properties.Height : 0;
            component.eventClick += properties.EventClick;
            if (isFixed)
            {
                if (properties.StackWidths)
                {
                    component.relativePosition = properties.GetRelativePositionByStackedWidths(m_stackedWidths);
                }
                else if (properties.ColShare > -1)
                {
                    component.relativePosition = properties.GetRelativePositionByColumnShare(m_totalColShare);
                }
                else if (addUIComponent)
                {
                    component.relativePosition = properties.GetRelativePositionByColumnCount(m_colIndex);
                }
                HandleNextPositioning(properties);
            }
            return component;
        }

        protected void ToggleButtonPairs(int activeIndex, params UIButton[] buttons)
        {
            var active = buttons[activeIndex];
            if (active.isEnabled)
            {
                active.color = new Color32(163, 255, 16, 255);
                active.normalBgSprite = "ButtonMenuFocused";
                active.useDropShadow = true;
                active.opacity = 95;
                var inactives = buttons.Except(new List<UIButton>() { active });
                foreach (UIButton inactive in inactives)
                {
                    if (inactive.isEnabled)
                    {
                        inactive.color = new Color32(150, 150, 150, 255);
                        inactive.normalBgSprite = "ButtonMenu";
                        inactive.useDropShadow = false;
                        inactive.opacity = 95;
                    }
                }
            }
        }
        protected void ToggleCustomAtlasButtonPairs(int activeIndex, params UIButton[] buttons)
        {
            var active = buttons[activeIndex];
            if (active.isEnabled)
            {
                active.hoveredBgSprite = active.atlas.name + "Focused";
                var inactives = buttons.Except(new List<UIButton>() { active });
                foreach (UIButton inactive in inactives)
                {
                    if (inactive.isEnabled)
                    {
                        inactive.hoveredBgSprite = inactive.atlas.name + "Hovered";
                    }
                }
            }
        }
        protected void ToggleButtonPairs(StationTrackType activeStationtype)
        {
            foreach (var kvp in buttonStationDict)
            {
                kvp.Value.color = new Color32(150, 150, 150, 255);
                kvp.Value.normalBgSprite = "ButtonMenu";
                kvp.Value.useDropShadow = false;
                kvp.Value.opacity = 95;
            }
            buttonStationDict[activeStationtype].color = new Color32(163, 255, 16, 255);
            buttonStationDict[activeStationtype].normalBgSprite = "ButtonMenuFocused";
            buttonStationDict[activeStationtype].useDropShadow = true;
            buttonStationDict[activeStationtype].opacity = 95;
        }

        private int m_CheckboxCount = 0;
        protected UIButton CreateButton(StationTrackType type, int columnCount, MouseEventHandler eventClick, bool sameLine = false)
        {
            return CreateButton(new UIButtonParamProps()
            {
                Text = GetNameFromStationType(type),
                ColumnCount = columnCount,
                EventClick = eventClick,
                SameLine = sameLine
            });
        }

        protected virtual void OnToggleMouseDown(UIComponent c, UIMouseEventParameter e, MetroStationTrackAlterType type)
        {

        }
        protected virtual void OnToggleMouseUp(UIComponent c, UIMouseEventParameter e, MetroStationTrackAlterType type)
        {

        }
        protected virtual void OnToggleKeyDown(UIComponent c, UIKeyEventParameter e)
        {

        }
        protected virtual void OnToggleKeyUp(UIComponent c, UIKeyEventParameter e)
        {

        }
        protected float m_height = 50;

        protected virtual void TunnelStationTrackToggleStyles()
        {

        }
        protected virtual void ExecuteUiInstructions()
        {

        }

        protected string GetNameFromStationType(StationTrackType type)
        {
            return Regex.Replace(type.ToString(), "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }
        protected void CreateCheckbox(StationTrackType type)
        {
            string typeName = GetNameFromStationType(type);
            var checkBox = AddUIComponent<UICheckBox>();
            CheckboxStationDict[type] = checkBox;
            checkBox.text = typeName;
            checkBox.size = new Vector2(width - 16, 16);
            checkBox.relativePosition = new Vector2(8, 200 + (m_CheckboxCount * 20));
            checkBox.isInteractive = true;
            UISprite cbClicker = checkBox.AddUIComponent<UISprite>();

            cbClicker.atlas = atlas;
            cbClicker.spriteName = "check-unchecked";
            cbClicker.relativePosition = new Vector2(0, 0);
            cbClicker.size = new Vector2(16, 16);
            cbClicker.isInteractive = true;
            checkBox.eventCheckChanged += (c, v) =>
            {
                TunnelStationTrackToggleStyles();
            };

            UILabel cbLabel = checkBox.AddUIComponent<UILabel>();
            cbLabel.relativePosition = new Vector2(20, 0);
            cbLabel.text = typeName;
            cbLabel.height = 16;
            cbLabel.isInteractive = true;
            m_height += 20;
            m_CheckboxCount++;
        }

        protected void BlurAllButtonsInPanel(UIPanel panel)
        {
            var tsGroup = FindUITabStripsInPanel(panel);
            for (var i = 0; i < tsGroup.Count(); i++)
            {
                tsGroup[i].GetComponentsInChildren<UIButton>().FirstOrDefault().SimulateClick();
            }
        }

        protected void RemoveChildren(UIComponent uIComponent, bool removeThis = false)
        {
            var children = uIComponent.GetComponentsInChildren<UIComponent>();
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child != null && child != uIComponent)
                    {
                        RemoveChildren(child, true);
                    }
                }
            }
            if (removeThis)
                DestroyImmediate(uIComponent.gameObject);
        }
        protected void RemoveComponent(UIComponent uIComponent)
        {
            if (uIComponent?.parent != null)
            {
                uIComponent.parent.RemoveUIComponent(uIComponent);
                if (uIComponent != null)
                {
                    DestroyImmediate(uIComponent.gameObject);
                }
            }
        }
        protected virtual void Activate(PrefabInfo info)
        {
            CurrentInfo = info;
            m_activated = true;
            isVisible = true;
        }

        protected virtual void SubDeactivate() { }
        protected void Deactivate()
        {
            if (!m_activated)
            {
                return;
            }
            SubDeactivate();
            CurrentInfo = null;
            isVisible = false;
            m_activated = false;
        }
    }
}
