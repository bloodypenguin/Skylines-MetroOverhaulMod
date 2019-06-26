using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
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
        protected BuildingTool m_buildingTool = null;
        protected UIButton m_upgradeButtonTemplate;
        protected NetInfo m_currentNetInfo;
        protected BuildingInfo m_currentBuilding;
        protected BuildingInfo m_currentCloneBuilding;
        protected bool m_activated = false;
        public static MetroTrackCustomizerUI instance;

        protected NetInfo trainTrackPrefab;
        protected NetInfo trainStationTrackPrefab;

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
        protected UITabstrip tsPillarChooser;
        protected UITabstrip tsTrackVsStation;
        protected UITabstrip tsStationTypeSelector;
        protected UITabstrip tsIndividualTrackSelector;
        protected UITabstrip tsStyles;
        protected UIPanel pnlOuterContainer;
        protected UIPanel pnlInnerContainer;
        protected UIPanel pnlTrackSelectorContainer;
        protected UILabel lblTabTitle;
        protected UITextField tfStationLengthPanelTooltip;
        protected UITextField tfStationDepthPanelTooltip;
        protected UITextField tfStationAnglePanelTooltip;
        protected UITextField tfStationBendPanelTooltipTooltip;
        protected Task m_T = null;

        protected bool m_valueChanged = false;
        protected MetroStationTrackAlterType m_Toggle = MetroStationTrackAlterType.None;
        protected static Dictionary<MetroStationTrackAlterType, SliderData> SliderDataDict { get; set; }
        protected static Dictionary<MetroStationTrackAlterType, UISlider> SliderDict { get; set; }
        protected static Dictionary<MetroStationTrackAlterType, UIPanel> PanelDict { get; set; }
        protected static Dictionary<StationTrackType, UICheckBox> CheckboxStationDict { get; set; }
        protected static Dictionary<StationTrackType, UIButton> buttonStationDict { get; set; }
        protected static Dictionary<string, UICheckBox> CheckboxDict { get; set; }
        protected Dictionary<MetroStationTrackAlterType, UIButton> toggleBtnDict = new Dictionary<MetroStationTrackAlterType, UIButton>();
        protected Dictionary<MetroStationTrackAlterType, float> SetDict = new Dictionary<MetroStationTrackAlterType, float>();

        protected StationTrackType m_TrackType = StationTrackType.None;

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
        protected string Station_Length_Info { get { return $"Adjusts the length of your station track. It is recommended to set this to about the length of your Metro Trains. Default is {SetStationCustomizations.DEF_LENGTH}m with a maximum length of {SetStationCustomizations.MAX_LENGTH}m and a minimum length of {SetStationCustomizations.MIN_LENGTH}m."; } }
        protected string Station_Depth_Info { get { return $"Adjusts the depth of your station track. It is recommended you set your tracks deeper on uneven terrain. Default depth is {SetStationCustomizations.DEF_DEPTH}m with a maximum depth of {SetStationCustomizations.MAX_DEPTH}m and a minimum depth of {SetStationCustomizations.MIN_DEPTH}m."; } }
        protected string Station_Angle_Info { get { return $"Adjusts the angle of your station track. This allows you to spin your tracks to align to your metro layout. Default angle is {SetStationCustomizations.DEF_ANGLE}° with a maximum angle of {SetStationCustomizations.MAX_ANGLE}° and a minimum angle of {SetStationCustomizations.MIN_ANGLE}°."; } }
        protected string Station_Bend_Info { get { return $"Adjusts the bend in the station track. This is useful for situations where your station connects tracks arriving from different angles. Default bend is { SetStationCustomizations.DEF_BEND_STRENGTH }° (no bend) with a maximum bend of { SetStationCustomizations.MAX_BEND_STRENGTH}° and a minimum bend of { SetStationCustomizations.MIN_BEND_STRENGTH}°."; } }
        protected Color32 DefaultColor = new Color32(90, 115, 217, 255);
        protected Color32 TrainColor = new Color32(233, 85, 38, 255);
        protected Color32 MetroColor = new Color32(6, 213, 73, 255);
        protected ItemClass m_TheIntersectClass = null;

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
            m_netTool = FindObjectOfType<NetTool>();
            if (m_netTool == null)
            {
#if DEBUG
                Next.Debug.Log("NetTool Not Found");
#endif
                enabled = false;
                return;
            }
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

        private float m_SetWidth = 250;
        protected virtual void CreateUI()
        {
            backgroundSprite = "GenericPanel";
            color = new Color32(73, 68, 84, 170);
            width = m_SetWidth;
            height = 0;
            opacity = 90;
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

            concretePrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground");
            concretePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground NoBar");

            concreteTwoLaneOneWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Two-Lane One-Way");
            concreteTwoLaneOneWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Two-Lane One-Way NoBar");

            concreteLargePrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Large");
            concreteLargePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Large NoBar");

            concreteSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small");
            concreteSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small NoBar");

            concreteSmallTwoWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small Two-Way");
            concreteSmallTwoWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small Two-Way NoBar");

            concreteSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground");
            concreteIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground Island");
            concreteSingleStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground Small");
            concreteQuadSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground Large");
            concreteQuadDualIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground Large Dual Island");

            steelPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground");
            steelPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground NoBar");

            steelTwoLaneOneWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Two-Lane One-Way");
            steelTwoLaneOneWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Two-Lane One-Way NoBar");

            steelSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small");
            steelSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small NoBar");

            steelSmallTwoWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small Two-Way");
            steelSmallTwoWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small Two-Way NoBar");

            steelLargePrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Large");
            steelLargePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Large NoBar");

            steelSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground");
            steelIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground Island");
            steelSingleStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground Small");
            steelQuadSideStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground Large");
            steelQuadDualIslandStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground Large Dual Island");
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

        protected UIPanel CreatePanel(UIPanelParamProps properties, bool isFixed = true)
        {
            var parentComponent = GetParentComponent(properties);
            var panel = (UIPanel)InitComponent(parentComponent.AddUIComponent<UIPanel>(), properties, isFixed);
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
            var parentComponent = GetParentComponent(properties);
            var tabstrip = (UITabstrip)InitComponent(parentComponent.AddUIComponent<UITabstrip>(), properties);
            tabstrip.navigateWithArrowTabKeys = true;
            tabstrip.startSelectedIndex = properties.StartSelectedIndex;
            tabstrip.zOrder = 5;
            return tabstrip;
        }
        protected UILabel CreateLabel(UILabelParamProps properties)
        {
            var parentComponent = GetParentComponent(properties);
            var label = (UILabel)InitComponent(parentComponent.AddUIComponent<UILabel>(), properties);
            label.textColor = properties.TextColor;
            label.textAlignment = properties.TextAlignment;
            label.textScale = properties.TextScale;
            if (!string.IsNullOrEmpty(properties.Text))
                label.text = properties.Text;
            return label;
        }
        protected void RefreshSprites(UIButton button)
        {
            button.normalBgSprite = button.atlas.name + "Bg";
            button.disabledBgSprite = button.atlas.name + "BgDisabled";
            button.focusedBgSprite = button.atlas.name + "BgFocused";
            button.hoveredBgSprite = button.atlas.name + "BgHovered";
            button.pressedBgSprite = button.atlas.name + "BgPressed";

            button.normalFgSprite = button.atlas.name + "Fg";
            button.disabledFgSprite = button.atlas.name + "FgDisabled";
            button.focusedFgSprite = button.atlas.name + "FgFocused";
            button.hoveredFgSprite = button.atlas.name + "FgHovered";
            button.pressedFgSprite = button.atlas.name + "FgPressed";
        }
        protected UIButton CreateButton(UIButtonParamProps properties)
        {
            var parentComponent = GetParentComponent(properties);
            var button = (UIButton)InitComponent(parentComponent.AddUIComponent<UIButton>(), properties);
            if (!string.IsNullOrEmpty(properties.Text))
                button.text = properties.Text;
            if (properties.Atlas != null)
            {
                button.atlas = properties.Atlas;
                if (properties.HasFgBgSprites)
                {
                    RefreshSprites(button);
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

            //button.gameObject.GetComponent<TutorialUITag>().tutorialTag = name;

            //button.tooltipAnchor = UITooltipAnchor.Anchored;
            //button.horizontalAlignment = UIHorizontalAlignment.Center;
            //button.verticalAlignment = UIVerticalAlignment.Middle;
            //button.pivot = UIPivotPoint.TopCenter;
            button.playAudioEvents = true;
            button.opacity = 95;
            button.dropShadowColor = new Color32(0, 0, 0, 255);
            button.dropShadowOffset = new Vector2(-1, -1);

            return button;
        }
        protected UISlicedSprite CreateUISlicedSprite(UIParamPropsBase properties)
        {
            var parentComponent = GetParentComponent(properties);
            UISlicedSprite sprite = (UISlicedSprite)InitComponent(parentComponent.AddUIComponent<UISlicedSprite>(), properties);
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
            var parentComponent = GetParentComponent(properties);

            UILabel titleLabel = CreateLabel(new UILabelParamProps()
            {
                Name = "lblTitle" + typeString,
                Text = "Station " + typeString,
                ParentComponent = parentComponent,
                ColShare = 4,
                Margins = new Vector4(8, 8, 8, 8)
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
                ColShare = 8,
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
                ColShare = 9,
                Margins = new Vector4(2, 0, 0, 2)
            });
            //sliderLeftPanel.height = sliderPanel.height - 4;
            UIPanel sliderRightPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "sliderRightPanel" + typeString,
                ParentComponent = sliderPanel,
                ColShare = 3
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
            sliderBgSprite.zOrder = 0;

            UISlicedSprite sliderMkSprite = sliderLeftPanel.AddUIComponent<UISlicedSprite>();
            sliderMkSprite.atlas = atlas;
            sliderMkSprite.name = "ssMkSprite" + typeString;
            sliderMkSprite.spriteName = "SliderBudget";
            sliderMkSprite.isInteractive = true;
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
                slider.value += v.wheelDelta > 0 ? sData.Step : -sData.Step;
                OnToggleMouseDown(c, v, properties.TrackAlterType);
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
            slider.eventMouseUp += (c, e) =>
            {
                OnToggleMouseDown(c, e, properties.TrackAlterType);
                if (m_valueChanged)
                {
                    m_valueChanged = false;
                }
            };
            return slider;
        }
        protected UICheckBox CreateCheckbox(UICheckboxParamProps properties)
        {
            var parentComponent = GetParentComponent(properties);
            UICheckBox checkbox = (UICheckBox)InitComponent(parentComponent.AddUIComponent<UICheckBox>(), properties);
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

        protected int m_rowIndex = 1;
        private int m_colIndex = 0;
        private float m_stackedWidths = 0;
        private int m_totalColShare = 0;
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
            if (m_colIndex * properties.Component.width > ParentTreeDict[properties.Component].width)
            {
                //var diff = ((m_colIndex + 1) * properties.Component.width) - ParentTreeDict[properties.Component].width;
                //PropagateParentComponentWidthUpdates(ParentTreeDict[properties.Component], diff);
            }
            if ((m_colIndex == properties.ColumnCount && properties.ColShare == -1) || properties.ForceRowEnd || m_totalColShare >= 12)
            {
                m_colIndex = 0;
                m_totalColShare = 0;
                m_stackedWidths = 0;
                if (properties.SameLine == false)
                {
                    m_rowIndex++;
                    var rowHeightAdditive = properties.Component.height + properties.Margins.y + properties.Margins.w;
                    PropagateParentComponentHeightUpdates(properties.Component, rowHeightAdditive);
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
            if (target != this && ParentTreeDict[target] != null)
            {
                var targetHeight = target.height + target.absolutePosition.y;
                var parentHeight = ParentTreeDict[target].height + ParentTreeDict[target].absolutePosition.y;
                if (targetHeight > parentHeight)
                {
                    ParentTreeDict[target].height += heightAdditive;
                    PropagateParentComponentHeightUpdates(ParentTreeDict[target], heightAdditive);
                }
            }
        }
        private UIComponent InitComponent(UIComponent component, UIParamPropsBase properties, bool isFixed = true)
        {
            var parentComponent = GetParentComponent(properties);
            ParentTreeDict.Add(component, parentComponent);
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
        protected UIComponent GetParentComponent(UIParamPropsBase properties)
        {
            if (properties.ParentComponent == null)
            {
                properties.ParentComponent = this;
            }
            return properties.ParentComponent;
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
        protected virtual void OnToggleKeyDown(UIComponent c, UIKeyEventParameter e)
        {

        }

        protected float m_height = 50;

        protected virtual void TunnelStationTrackToggleStyles(StationTrackType type)
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
                TunnelStationTrackToggleStyles(type);
            };

            UILabel cbLabel = checkBox.AddUIComponent<UILabel>();
            cbLabel.relativePosition = new Vector2(20, 0);
            cbLabel.text = typeName;
            cbLabel.height = 16;
            cbLabel.isInteractive = true;
            m_height += 20;
            m_CheckboxCount++;
        }


        protected virtual void Activate(PrefabInfo info)
        {
            CurrentInfo = info;
            m_activated = true;
            isVisible = true;
        }

        protected virtual void SubDeactivate() { }
        private void Deactivate()
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
