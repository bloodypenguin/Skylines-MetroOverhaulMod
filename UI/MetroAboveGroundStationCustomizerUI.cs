using System.Collections.Generic;
using MetroOverhaul.Extensions;
using UnityEngine;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.OptionsFramework;
using System.Linq;
using ColossalFramework.UI;
using System;

namespace MetroOverhaul.UI
{
    public class MetroAboveGroundStationCustomizerUI : MetroCustomizerBaseUI
    {
        protected override bool SatisfiesTrackSpecs(PrefabInfo info)
        {
            return OptionsWrapper<Options>.Options.ingameTrainMetroConverter && ((BuildingInfo)info).GetComponent<TransportStationAI>() != null && (((BuildingInfo)info).HasAbovegroundTrainStationTracks() || ((BuildingInfo)info).HasAbovegroundMetroStationTracks());
        }

        protected override ToolBase GetTheTool()
        {
            return m_buildingTool;
        }

        protected override PrefabInfo GetToolPrefab()
        {
            return ((BuildingTool)GetTheTool())?.m_prefab;
        }

        private List<BuildingInfo.PathInfo> m_AbovegroundStationPaths;
        protected List<BuildingInfo.PathInfo> AbovegroundStationPaths {
            get
            {
                if (m_AbovegroundStationPaths == null)
                    m_AbovegroundStationPaths = m_currentBuilding.AbovegroundStationPaths();
                return m_AbovegroundStationPaths;
            }
        }
        protected void ClearAbovegroundStationPaths()
        {
            if (m_AbovegroundStationPaths != null)
            {
                m_AbovegroundStationPaths.Clear();
                m_AbovegroundStationPaths = null;
            }
        }

        private int maxColumns = 6;
        private int m_StationTrackPathCount;

        protected override PrefabInfo CurrentInfo { get => m_currentBuilding; set => m_currentBuilding = (BuildingInfo)value; }
        protected override void Activate(PrefabInfo info)
        {
            var newBuildingChosen = CurrentInfo == null || info.name != ((BuildingInfo)CurrentInfo).GetAnalogName();
            base.Activate(info);
            if (newBuildingChosen)
            {
                ClearAbovegroundStationPaths();
                RemoveTrackVehicleTypeArrayButtons();
                m_LinkedPathDict = null;
                m_TrackStyleArray = null;
                m_ButtonArray = null;
                AddTrackVehicleTypeArrayButtons();
                if (btnDefault != null)
                    btnDefault.SimulateClick();
            }
        }
        protected override void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM ABOVE GROUND STATION TRACK GUI Created");
#endif
            base.CreateUI();
            width = 350;
            position = new Vector3(450, 0, 0);

            CreateDragHandle("Surface Station Options");
            var pnlLabelContainer = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlLabelContainer",
                ColumnCount = 1,
                Margins = new Vector4(0, 0, 0, 8)
            });
            var lblStationTypeSelector = CreateLabel(new UILabelParamProps()
            {
                Name = "lblStationTypeSelector",
                Text = "Station Type Selector",
                ParentComponent = pnlLabelContainer,
                ColumnCount = 2
            });
            var pnlStationTypeSelectorInfo = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStationTypeSelectorInfo",
                ParentComponent = pnlLabelContainer,
                ColumnCount = 2
            });

            //var ttpStationTypeSelectorTooltip = CreateToolTipPanel(new UIToolTipPanelParamProps()
            //{
            //    Name = "ttpStationTypeSelectorTooltip",
            //    ToolTipPanelText = STATION_TYPE_SELECTOR_INFO
            //});

            //var btnStationTypeSelectorInfo = CreateButton(new UIButtonParamProps()
            //{
            //    Name = "btnStationTypeSelectorInfo",
            //    Atlas = atlas,
            //    ParentComponent = pnlStationTypeSelectorInfo,
            //    NormalBgSprite = "EconomyMoreInfo",
            //    HoveredBgSprite = "EconomyMoreInfoHovered",
            //    PressedBgSprite = "EconomyMoreInfoHovered",
            //    Height = 12,
            //    Width = 12,
            //    StackWidths = true
            //    //TooltipComponent = ttpStationTypeSelectorTooltip
            //});

            var pnlStationTypeSelectorContainer = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStationTypeSelectorInfoContainer",
                ColumnCount = 2
            });
            var pnlStationTypeSeparator = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStationTypeSeparator",
                ColumnCount = 2
            });

            pnlStationTypeSelector = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStationTypeSelector",
                ParentComponent = pnlStationTypeSelectorContainer,
                ColumnCount = 1,
                Margins = new Vector2(0, 0)
            });

            var pnlSeparator = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStationTypeSelectorSeparator",
                ParentComponent = pnlStationTypeSelectorContainer,
                ColumnCount = 2,
                Margins = new Vector2(0, 0)
            });
            AddTrackVehicleTypeButtons();
            pnlOuterContainer = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlOuterContainer",
                Atlas = atlas,
                ColumnCount = 1,
                BackgroundSprite = "GenericPanel",
                Margins = Vector4.zero
            });
            lblTabTitle = CreateLabel(new UILabelParamProps()
            {
                Name = "lblTabTitle",
                Text = "",
                ParentComponent = pnlOuterContainer,
                ColumnCount = 1,
                Margins = new Vector4(8, 0, 0, 5)
            });
            pnlInnerContainer = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlInnerContainer",
                ParentComponent = pnlOuterContainer,
                Atlas = atlas,
                ColumnCount = 1,
                Color = color,
                Margins = new Vector4(8, 8, 8, 8),
                BackgroundSprite = "GenericPanel"
            });
            pnlTrackSelectorContainerWrapper = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlTrackSelectorContainerWrapper",
                ParentComponent = pnlInnerContainer,
                ColumnCount = 1
            });
            var lblTrackSelectorTitle = CreateLabel(new UILabelParamProps()
            {
                Name = "lblTrackSelectorTitle",
                Text = "Customize Individual Tracks",
                ParentComponent = pnlTrackSelectorContainerWrapper,
                ColumnCount = 1,
            });
            pnlTrackSelectorContainer = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlTrackSelectorContainer",
                ParentComponent = pnlTrackSelectorContainerWrapper,
                Height = 60,
                ColumnCount = 1
            });
            pnlStyleContainer = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStyleContainer",
                ParentComponent = pnlInnerContainer,
                ColumnCount = 1
            });

            var lblStyles = CreateLabel(new UILabelParamProps()
            {
                Name = "lblStyles",
                Text = "Style Selector",
                ParentComponent = pnlStyleContainer,
                ColumnCount = 1
            });

            AddTrackStyleButtons();
        }
        private void AddTrackStyleButtons()
        {
            btnModernStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnModernStyle",
                ToolTip = "Modern Style",
                ColumnCount = 4,
                ParentComponent = pnlStyleContainer,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ModernStyleAtlas", UIHelper.ModernStyle),
                Width = 59,
                Height = 52,
                StackWidths = true,
                EventClick = (c, v) =>
                {
                    trackStyle = NetInfoTrackStyle.Modern;
                    SetNetToolPrefab();
                }
            });
            btnClassicStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnClassicStyle",
                ToolTip = "Classic Style",
                ColumnCount = 4,
                ParentComponent = pnlStyleContainer,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ClassicStyleAtlas", UIHelper.ClassicStyle),
                Width = 59,
                Height = 52,
                StackWidths = true,
                EventClick = (c, v) =>
                {
                    trackStyle = NetInfoTrackStyle.Classic;
                    SetNetToolPrefab();
                }
            });
            btnVanillaStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnVanillaStyle",
                ToolTip = "Vanilla Style",
                ColumnCount = 4,
                ParentComponent = pnlStyleContainer,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_VanillaStyleAtlas", UIHelper.VanillaStyle),
                Width = 59,
                Height = 52,
                StackWidths = true,
                EventClick = (c, v) =>
                {
                    trackStyle = NetInfoTrackStyle.Vanilla;
                    SetNetToolPrefab();
                }
            });

        }
        private void AddTrackVehicleTypeButtons()
        {
            btnDefault = CreateButton(new UIButtonParamProps()
            {
                Name = "btnDefault",
                ToolTip = "Default Station Configuration",
                ColumnCount = 3,
                ParentComponent = pnlStationTypeSelector,
                Margins = new Vector2(0, 0),
                Atlas = UIHelper.GenerateLinearAtlas("MOM_SteamTabAtlas", UIHelper.SteamTab, 10),
                Height = 35,
                Width = 57,
                StackWidths = true,
                HasFgBgSprites = true,
                EventClick = (c, v) =>
                {
                    if (m_currentBuilding.IsMetroStation())
                    {
                        lblTabTitle.text = METRO_DEFAULT_TAB_LABEL;
                    }
                    else if (m_currentBuilding.IsTrainStation())
                    {
                        lblTabTitle.text = TRAIN_DEFAULT_TAB_LABEL;
                    }

                    m_TrackStyleArray = null;
                    trackVehicleType = TrackVehicleType.Default;
                    pnlOuterContainer.color = DefaultColor;

                    for (int i = 0; i < AbovegroundStationPaths.Count(); i++)
                    {
                        if (AbovegroundStationPaths[i].m_netInfo.IsAbovegroundMetroStationTrack())
                        {
                            TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                            if (ButtonArray.Count > i)
                                RefreshSprites(ButtonArray[i], UIHelper.GenerateLinearAtlas("MOM_MetroTrackToggleAtlas", UIHelper.MetroTrackToggle, 3));
                        }
                        else if (AbovegroundStationPaths[i].m_netInfo.IsAbovegroundTrainStationTrack())
                        {
                            TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                            if (ButtonArray.Count > i)
                                RefreshSprites(ButtonArray[i], UIHelper.GenerateLinearAtlas("MOM_TrainTrackToggleAtlas", UIHelper.TrainTrackToggle, 3));
                        }
                        HandleLinkedPathPropagations(i);
                    }
                    SetNetToolPrefab();
                }
            });
            btnTrain = CreateButton(new UIButtonParamProps()
            {
                Name = "btnTrain",
                ToolTip = "Train Station Configuration",
                ColumnCount = 3,
                ParentComponent = pnlStationTypeSelector,
                Margins = new Vector2(0, 0),
                Atlas = UIHelper.GenerateLinearAtlas("MOM_TrainTabAtlas", UIHelper.TrainTab, 10),
                Height = 35,
                Width = 57,
                StackWidths = true,
                HasFgBgSprites = true,
                EventClick = (c, v) =>
                {
                    lblTabTitle.text = TRAIN_TAB_LABEL;
                    m_TrackStyleArray = null;
                    trackVehicleType = TrackVehicleType.Train;
                    pnlOuterContainer.color = TrainColor;
                    Debug.Log("Checkpoint Train");
                    for (int i = 0; i < AbovegroundStationPaths.Count(); i++)
                    {
                        Debug.Log("Path " + i + " is a station path and is being set to TRAIN");
                        TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                        if (ButtonArray.Count > i && ButtonArray[i].atlas != this.atlas)
                        {
                            Debug.Log("About to refresh sprite for Path " + i);
                            RefreshSprites(ButtonArray[i], UIHelper.GenerateLinearAtlas("MOM_TrainTrackToggleAtlas", UIHelper.TrainTrackToggle, 3));
                            Debug.Log("Sprite refreshed. They are " + ButtonArray[i].normalFgSprite.ToString() + " (fg) and " + ButtonArray[i].normalBgSprite.ToString() + " (bg).");
                        }
                        HandleLinkedPathPropagations(i);
                    }
                    SetNetToolPrefab();
                }
            });
            btnMetro = CreateButton(new UIButtonParamProps()
            {
                Name = "btnMetro",
                ToolTip = "Metro Station Configuration",
                ColumnCount = 3,
                ParentComponent = pnlStationTypeSelector,
                Margins = new Vector2(0, 0),
                Atlas = UIHelper.GenerateLinearAtlas("MOM_MetroTabAtlas", UIHelper.MetroTab, 10),
                Height = 35,
                Width = 57,
                StackWidths = true,
                HasFgBgSprites = true,
                EventClick = (c, v) =>
                {
                    lblTabTitle.text = METRO_TAB_LABEL;
                    m_TrackStyleArray = null;
                    trackVehicleType = TrackVehicleType.Metro;
                    pnlOuterContainer.color = MetroColor;
                    //var paths = m_currentBuilding.GetPaths();
                    Debug.Log("Checkpoint Metro");
                    for (int i = 0; i < AbovegroundStationPaths.Count(); i++)
                    {
                        Debug.Log("Path " + i + " is a station path and is being set to METRO");
                        TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                        if (ButtonArray.Count > i)
                        {
                            Debug.Log("About to refresh sprite for Path " + i);
                            RefreshSprites(ButtonArray[i], UIHelper.GenerateLinearAtlas("MOM_MetroTrackToggleAtlas", UIHelper.MetroTrackToggle, 3));
                            Debug.Log("Sprite refreshed. They are " + ButtonArray[i].normalFgSprite.ToString() + " (fg) and " + ButtonArray[i].normalBgSprite.ToString() + " (bg).");
                        }
                        HandleLinkedPathPropagations(i);
                    }
                    SetNetToolPrefab();
                }
            });

        }
        private void HandleLinkedPathPropagations(int i)
        {
            if (!m_InRecursion)
            {
                if (m_LinkedPathDict != null && m_LinkedPathDict.ContainsKey(i) && m_LinkedPathDict[i].Count > 0)
                {
                    foreach (var inx in m_LinkedPathDict[i])
                    {
                        if (m_buildingTool?.m_prefab?.m_paths[inx]?.m_netInfo != null)
                        {
                            if (m_buildingTool.m_prefab.m_paths[inx].m_netInfo.IsStationTrack())
                            {
                                m_InRecursion = true;
                                if (ButtonArray[inx] != null)
                                    ButtonArray[inx].SimulateClick();
                                m_InRecursion = false;
                            }
                            else
                            {
                                TrackVehicleTypeArray[inx] = TrackVehicleTypeArray[i];
                                SetNetToolPrefab();
                            }
                        }
                    }
                }
            }
        }
        private void RemoveTrackVehicleTypeArrayButtons()
        {
            RemoveChildren(pnlTrackSelectorContainer, false);
            pnlTrackSelectorContainer.height = 0;
            ClearButtonArray();
        }
        private void AddTrackVehicleTypeArrayButtons()
        {
            if (m_currentBuilding != null)
            {
                var paths = m_currentBuilding.GetPaths();
                m_StationTrackPathCount = 0;
                var excludeList = new List<int>();
                for (int i = 0; i < paths.Count(); i++)
                {
                    var path = paths[i];
                    if (path != null)
                    {
                        if (AbovegroundStationPaths.Contains(path))
                        {
                            var stationInx = AbovegroundStationPaths.IndexOf(path);
                            if (!excludeList.Contains(stationInx))
                            {
                                if (m_LinkedPathDict == null)
                                {
                                    m_LinkedPathDict = new Dictionary<int, List<int>>();
                                }
                                AddTrackVehicleTypeArrayButton(stationInx, path, AbovegroundStationPaths.Count());
                                m_LinkedPathDict.Add(stationInx, m_buildingTool.m_prefab.LinkedPaths(stationInx));
                                var linkedPathCount = m_LinkedPathDict[stationInx].Count();
                                if (linkedPathCount > 0)
                                {
                                    for (int j = 0; j < linkedPathCount; j++)
                                    {
                                        var inx = m_LinkedPathDict[stationInx][j];
                                        if (AbovegroundStationPaths.Contains(paths[inx]))
                                        {
                                            AddTrackVehicleTypeArrayButton(inx, path, AbovegroundStationPaths.Count(), false);
                                            excludeList.Add(inx);
                                            //lastLinkedStationPathIndex = j;
                                        }
                                    }
                                }
                            }
                        }
                        else if (path.m_finalNetInfo.IsMetroTrack())
                        {
                            //TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                        }
                        else if (path.m_finalNetInfo.IsTrainTrack())
                        {
                            //TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                        }
                    }
                }
            }
        }
        private void AddTrackVehicleTypeArrayButton(int i, BuildingInfo.PathInfo path, int trackPathCount, bool addUIComponent = true)
        {
            m_StationTrackPathCount++;
            UITextureAtlas atlas = null;
            string tooltip = "";
            if (path.m_finalNetInfo.IsAbovegroundMetroStationTrack())
            {
                TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                atlas = UIHelper.GenerateLinearAtlas("MOM_MetroTrackToggleAtlas", UIHelper.MetroTrackToggle, 3);
                tooltip = "Metro Track";
            }
            else if (path.m_finalNetInfo.IsAbovegroundTrainStationTrack())
            {
                TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                atlas = UIHelper.GenerateLinearAtlas("MOM_TrainTrackToggleAtlas", UIHelper.TrainTrackToggle, 3);
                tooltip = "Train Track";
            }
            ButtonArray.Add(CreateButton(new UIButtonParamProps()
            {
                Name = "btnTrackSelector" + i,
                ToolTip = tooltip,
                Atlas = addUIComponent ? atlas : this.atlas,
                ParentComponent = pnlTrackSelectorContainer,
                ColumnCount = 8,
                Margins = new Vector2(2, 2),
                Height = 37,
                Width = 37,
                //ForceRowEnd = m_StationTrackPathCount == trackPathCount,
                AddUIComponent = addUIComponent,
                EventClick = (c, v) => { btnTrackSelectorOnClick(c, v, i); }
            }));
        }
        private void btnTrackSelectorOnClick(UIComponent c, UIMouseEventParameter v, int index)
        {
            var inx = ButtonArray.IndexOf((UIButton)c);
            var tsStationTypeSelector = pnlStationTypeSelector.GetComponentsInChildren<UITabstrip>().FirstOrDefault(c2 => c2.name.EndsWith("0"));
            if (trackVehicleType == TrackVehicleType.Default)
            {
                if (m_currentBuilding.IsMetroStation())
                {
                    trackVehicleType = TrackVehicleType.Metro;
                    lblTabTitle.text = METRO_TAB_LABEL;
                    tsStationTypeSelector.selectedIndex = 2;
                    pnlOuterContainer.color = MetroColor;
                }
                else if (m_currentBuilding.IsTrainStation())
                {
                    trackVehicleType = TrackVehicleType.Train;
                    lblTabTitle.text = TRAIN_TAB_LABEL;
                    tsStationTypeSelector.selectedIndex = 1;
                    pnlOuterContainer.color = TrainColor;
                }
            }
            if (TrackVehicleTypeArray[inx] == TrackVehicleType.Train)
            {
                TrackVehicleTypeArray[inx] = TrackVehicleType.Metro;
                if (ButtonArray[inx].atlas != this.atlas)
                    RefreshSprites(ButtonArray[index], UIHelper.GenerateLinearAtlas("MOM_MetroTrackToggleAtlas", UIHelper.MetroTrackToggle, 3));
            }
            else if (TrackVehicleTypeArray[inx] == TrackVehicleType.Metro)
            {
                TrackVehicleTypeArray[inx] = TrackVehicleType.Train;
                if (ButtonArray[inx].atlas != this.atlas)
                    RefreshSprites(ButtonArray[index], UIHelper.GenerateLinearAtlas("MOM_TrainTrackToggleAtlas", UIHelper.TrainTrackToggle, 3));
            }

            HandleLinkedPathPropagations(inx);
            SetNetToolPrefab();
        }

        private TrackVehicleType[] m_TrackStyleArray = null;
        private TrackVehicleType[] TrackVehicleTypeArray {
            get
            {
                if (m_TrackStyleArray == null)
                {
                    m_TrackStyleArray = new TrackVehicleType[AbovegroundStationPaths.Count()];
                }
                return m_TrackStyleArray;
            }
        }

        private List<UIPanel> m_LinkedPathPanelArray = null;
        private List<UIPanel> LinkedPathPanelArray {
            get
            {
                if (m_LinkedPathPanelArray == null)
                {
                    m_LinkedPathPanelArray = new List<UIPanel>();
                }
                return m_LinkedPathPanelArray;
            }
        }

        private bool m_InRecursion = false;
        private Dictionary<int, List<int>> m_LinkedPathDict = null;
        private void SetNetToolPrefab()
        {
            btnTrain.isEnabled = OptionsWrapper<Options>.Options.ingameTrainMetroConverter;
            var hasMetroTrack = TrackVehicleTypeArray.Any(t => t == TrackVehicleType.Metro);
            var vanillaApplicable = m_currentBuilding.AbovegroundStationPaths().Any(p => p.m_netInfo.IsGroundSidePlatformMetroStationTrack() || p.m_netInfo.IsElevatedSidePlatformMetroStationTrack() || p.m_netInfo.IsGroundSidePlatformTrainStationTrack() || p.m_netInfo.IsElevatedSidePlatformTrainStationTrack());
            UIButton.ButtonState vanillaState;
            UIButton.ButtonState modernState;
            UIButton.ButtonState classicState;
            if (hasMetroTrack)
            {
                classicState = UIButton.ButtonState.Normal;
                modernState = UIButton.ButtonState.Normal;

                if (vanillaApplicable)
                    vanillaState = UIButton.ButtonState.Normal;
                else
                    vanillaState = UIButton.ButtonState.Disabled;

                switch (trackStyle)
                {
                    case NetInfoTrackStyle.None:
                    case NetInfoTrackStyle.Modern:
                        modernState = UIButton.ButtonState.Focused;
                        break;
                    case NetInfoTrackStyle.Classic:
                        classicState = UIButton.ButtonState.Focused;
                        break;
                    case NetInfoTrackStyle.Vanilla:
                        if (vanillaApplicable)
                            vanillaState = UIButton.ButtonState.Focused;
                        break;
                }
                if (trackStyle != NetInfoTrackStyle.None)
                {
                    btnModernStyle.parent.GetComponentsInChildren<UIButton>().FirstOrDefault(b => b.name.StartsWith(TS_STARTER)).SimulateClick();
                }

                btnClassicStyle.isInteractive = true;
                btnModernStyle.isInteractive = true;
                btnVanillaStyle.isInteractive = vanillaApplicable;
            }
            else
            {
                if (trackStyle != NetInfoTrackStyle.None)
                {
                    btnModernStyle.parent.GetComponentsInChildren<UIButton>().FirstOrDefault(b => b.name.StartsWith(TS_STARTER)).SimulateClick();
                }

                classicState = UIButton.ButtonState.Disabled;
                modernState = UIButton.ButtonState.Disabled;
                vanillaState = UIButton.ButtonState.Disabled;
                btnClassicStyle.isInteractive = false;
                btnModernStyle.isInteractive = false;
                btnVanillaStyle.isInteractive = false;
            }

            var trainAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Train.ToString();
            var metroAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Metro.ToString();

            switch (trackVehicleType)
            {
                case TrackVehicleType.Default:
                    {
                        if (OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
                        {
                            string comeCorrectName;
                            if (m_currentBuilding.name.Contains(ModTrackNames.ANALOG_PREFIX))
                            {
                                comeCorrectName = m_currentBuilding.name.Substring(0, m_currentBuilding.name.IndexOf(ModTrackNames.ANALOG_PREFIX));
                            }
                            else
                            {
                                comeCorrectName = m_currentBuilding.name;
                            }
                            Debug.Log("Setting " + m_currentBuilding.name + " to Default " + comeCorrectName);
                            m_buildingTool.m_prefab = PrefabCollection<BuildingInfo>.FindLoaded(comeCorrectName);

                            SetVehicleTypesAndStyles(true);
                        }
                        break;
                    }
                case TrackVehicleType.Train:
                    {
                        if (OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
                        {
                            if (m_currentBuilding.IsMetroStation())
                            {
                                string comeCorrectName;
                                if (m_currentBuilding.name.EndsWith(metroAnalogSuffix))
                                {
                                    comeCorrectName = m_currentBuilding.name.Substring(0, m_currentBuilding.name.IndexOf(metroAnalogSuffix));
                                }
                                else
                                {
                                    comeCorrectName = m_currentBuilding.name + trainAnalogSuffix;
                                }
                                Debug.Log("Setting " + m_currentBuilding.name + " to Train " + comeCorrectName);
                                m_buildingTool.m_prefab = PrefabCollection<BuildingInfo>.FindLoaded(comeCorrectName);
                            }
                            SetVehicleTypesAndStyles();
                        }
                        break;
                    }

                case TrackVehicleType.Metro:
                    {
                        if (m_currentBuilding.IsTrainStation())
                        {
                            string comeCorrectName;
                            if (m_currentBuilding.name.EndsWith(trainAnalogSuffix))
                            {
                                comeCorrectName = m_currentBuilding.name.Substring(0, m_currentBuilding.name.IndexOf(trainAnalogSuffix));
                            }
                            else
                            {
                                comeCorrectName = m_currentBuilding.name + metroAnalogSuffix;
                            }
                            m_buildingTool.m_prefab = PrefabCollection<BuildingInfo>.FindLoaded(comeCorrectName);
                        }
                        SetVehicleTypesAndStyles();

                        break;
                    }
            }
            btnVanillaStyle.state = vanillaState;
            btnModernStyle.state = modernState;
            btnClassicStyle.state = classicState;
        }
        private void SetVehicleTypesAndStyles(bool isDefault = false)
        {
            for (var i = 0; i < AbovegroundStationPaths.Count; i++)
            {
                var path = AbovegroundStationPaths[i];
                if (path.m_netInfo.IsAbovegroundMetroStationTrack() || path.m_netInfo.IsMetroTrack() || path.m_netInfo.IsAbovegroundTrainStationTrack() || path.m_netInfo.IsTrainTrack())
                {
                    m_buildingTool.m_prefab.SetTrackVehicleType(i, isDefault ? trackVehicleType : TrackVehicleTypeArray[i]);
                    //HandleLinkedPathPropagations(i);
                }
                if (path.m_finalNetInfo.IsAbovegroundMetroStationTrack() || path.m_netInfo.IsMetroTrack())
                {
                    //if (trackStyle == TrackStyle.None)
                    //    btnModernStyle.SimulateClick();
                    path.SetMetroStyle(trackStyle);
                }
            }
        }
        private UIComponent m_StationTypeSelectorTooltip = null;
        protected UIComponent StationTypeSelectorTooltip {
            get
            {
                if (m_StationTypeSelectorTooltip == null)
                {
                    m_StationTypeSelectorTooltip = UIView.Find("StationTypeSelectorTooltip");
                }
                return m_StationTypeSelectorTooltip;
            }
        }
    }
}
