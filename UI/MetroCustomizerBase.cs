using ColossalFramework.Threading;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class MetroCustomizerBase : UIPanel
    {
        public int trackStyle = 0;
        public int trackSize = 1;
        public int trackDirection = 1;
        protected BulldozeTool m_bulldozeTool;
        protected NetTool m_netTool;
        protected UIButton m_upgradeButtonTemplate;
        protected NetInfo m_currentNetInfo;
        protected bool m_activated = false;
        public static MetroTrackCustomizerUI instance;

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

        protected UIButton btnModernStyle;
        protected UIButton btnClassicStyle;
        protected UIButton btnSingleTrack;
        protected UIButton btnDoubleTrack;
        protected UIButton btnQuadTrack;
        protected UIButton btnOneWay;
        protected UIButton btnTwoWay;
        protected UIButton btnStation;
        protected UIButton btnTrack;

        protected Task m_T = null;

        protected bool m_valueChanged = false;
        protected ToggleType m_Toggle = ToggleType.None;
        protected static Dictionary<ToggleType, SliderData> SliderDataDict { get; set; }
        protected static Dictionary<ToggleType, UISlider> SliderDict { get; set; }
        protected static Dictionary<ToggleType, UIPanel> PanelDict { get; set; }
        protected static Dictionary<StationTrackType, UICheckBox> CheckboxStationDict { get; set; }
        protected static Dictionary<StationTrackType, UIButton> buttonStationDict { get; set; }
        protected static Dictionary<string, UICheckBox> CheckboxDict { get; set; }
        protected Dictionary<ToggleType, UIButton> toggleBtnDict = new Dictionary<ToggleType, UIButton>();
        protected Dictionary<ToggleType, float> SetDict = new Dictionary<ToggleType, float>();
        protected BuildingTool m_buildingTool = null;
        protected BuildingInfo m_currentBuilding;
        protected BuildingInfo m_currentSuperBuilding;
        protected StationTrackType m_TrackType = StationTrackType.SidePlatform;
        protected StationTrackType m_PrevTrackType = StationTrackType.SidePlatform;

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

        protected const string ALT_BARRIER = "Alt/Barrier";
        protected const string OVER_ROAD_FRIENDLY = "Over-Road Friendly";

        protected ItemClass m_TheIntersectClass = null;

        public override void Awake()
        {
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
            titleLabel.relativePosition = new Vector3() { x = 5, y = 5, z = 0 };
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.text = title;
            titleLabel.isInteractive = false;
        }

        protected void ToggleButtonPairs(int activeIndex, params UIButton[] buttons)
        {
            var active = buttons[activeIndex];
            active.color = new Color32(163, 255, 16, 255);
            active.normalBgSprite = "ButtonMenuFocused";
            active.useDropShadow = true;
            active.opacity = 95;
            var inactives = buttons.Except(new List<UIButton>() { active });
            foreach (UIButton inactive in inactives)
            {
                inactive.color = new Color32(150, 150, 150, 255);
                inactive.normalBgSprite = "ButtonMenu";
                inactive.useDropShadow = false;
                inactive.opacity = 75;
            }
        }

        protected void ToggleButtonPairs(StationTrackType activeStationtype)
        {
            foreach (var kvp in buttonStationDict)
            {
                kvp.Value.color = new Color32(150, 150, 150, 255);
                kvp.Value.normalBgSprite = "ButtonMenu";
                kvp.Value.useDropShadow = false;
                kvp.Value.opacity = 75;
            }
            buttonStationDict[activeStationtype].color = new Color32(163, 255, 16, 255);
            buttonStationDict[activeStationtype].normalBgSprite = "ButtonMenuFocused";
            buttonStationDict[activeStationtype].useDropShadow = true;
            buttonStationDict[activeStationtype].opacity = 95;
        }

        protected int m_rowIndex = 1;
        private int m_colIndex = 0;

        private int m_SliderCount = 0;
        private int m_CheckboxCount = 0;
        protected UIButton CreateButton(StationTrackType type, int columnCount, MouseEventHandler eventClick, bool sameLine = false)
        {
            return CreateButton(GetNameFromStationType(type), columnCount, eventClick, sameLine);
        }

        protected UIButton CreateButton(string text, int columnCount, MouseEventHandler eventClick, bool sameLine = false)
        {
            var button = this.AddUIComponent<UIButton>();
            button.relativePosition = new Vector3(8 + (((float)m_colIndex / columnCount) * width), m_rowIndex * 50);
            button.width = (width / columnCount) - 16;
            button.height = 30;
            button.normalBgSprite = "ButtonMenu";
            button.color = new Color32(150, 150, 150, 255);
            button.disabledBgSprite = "ButtonMenuDisabled";
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
            button.opacity = 75;
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
        protected virtual void OnToggleMouseDown(UIComponent c, UIMouseEventParameter e, ToggleType type)
        {

        }
        protected virtual void OnToggleKeyDown(UIComponent c, UIKeyEventParameter e)
        {

        }
        
        protected float m_height = 50;
        protected void CreateSlider(ToggleType type)
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
            m_height += 40;
            m_SliderCount++;
        }
        protected virtual void TunnelStationTrackToggleStyles(BuildingInfo info, StationTrackType type)
        {

        }
        protected virtual void ExecuteUiInstructions()
        {

        }
        protected void CreateCheckbox(string label)
        {
            UICheckBox checkbox = AddUIComponent<UICheckBox>();
            checkbox.text = label;
            checkbox.size = new Vector2(width - 16, 16);
            checkbox.relativePosition = new Vector2(8, 200 + (30 * CheckboxDict.Count));
            CheckboxDict[label] = checkbox;
            checkbox.isInteractive = true;

            UISprite cbClicker = checkbox.AddUIComponent<UISprite>();
            cbClicker.atlas = atlas;
            cbClicker.spriteName = "check-unchecked";
            cbClicker.relativePosition = new Vector2(0, 0);
            cbClicker.size = new Vector2(16, 16);
            cbClicker.isInteractive = true;
            checkbox.eventCheckChanged += (c, v) =>
            {
                if (checkbox.isChecked)
                {
                    cbClicker.spriteName = "check-checked";
                }
                else
                {
                    cbClicker.spriteName = "check-unchecked";
                }
                ExecuteUiInstructions();
            };

            UILabel checkboxLabel = checkbox.AddUIComponent<UILabel>();
            checkboxLabel.relativePosition = new Vector2(20, 0);
            checkboxLabel.text = label;
            checkboxLabel.height = 16;
            checkboxLabel.isInteractive = true;

            m_height = Math.Max(m_height + 30, 230);
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
            checkBox.eventCheckChanged += (c, v) =>
            {
                if (checkBox.isChecked)
                {
                    m_PrevTrackType = m_TrackType;
                    m_TrackType = type;
                    foreach (var kvp in CheckboxStationDict)
                    {
                        if (kvp.Key != type)
                        {
                            kvp.Value.isChecked = false;
                        }
                    }
                    TunnelStationTrackToggleStyles(m_currentBuilding, type);
                    m_T.Run();
                }
                else
                {
                    var allUnchecked = true;
                    foreach (var cb in CheckboxDict.Values)
                    {
                        if (cb.isChecked)
                        {
                            allUnchecked = false;
                            break;
                        }
                    }
                    if (allUnchecked)
                    {
                        checkBox.isChecked = true;
                    }
                }
            };

            UISprite cbClicker = checkBox.AddUIComponent<UISprite>();
            cbClicker.atlas = atlas;
            cbClicker.spriteName = "check-unchecked";
            cbClicker.relativePosition = new Vector2(0, 0);
            cbClicker.size = new Vector2(16, 16);
            cbClicker.isInteractive = true;

            UILabel cbLabel = checkBox.AddUIComponent<UILabel>();
            cbLabel.relativePosition = new Vector2(20, 0);
            cbLabel.text = typeName;
            cbLabel.height = 16;
            cbLabel.isInteractive = true;
            m_height += 20;
            m_CheckboxCount++;
        }
        public enum StationTrackType
        {
            SidePlatform,
            IslandPlatform,
            SingleTrack,
            QuadSidePlatform,
            //QuadSideIslandPlatform,
            QuadDualIslandPlatform
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
}
