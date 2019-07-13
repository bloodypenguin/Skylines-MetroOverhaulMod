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

        private int maxColumns = 6;
        private int m_StationTrackPathCount;

        protected override PrefabInfo CurrentInfo { get => m_currentBuilding; set => m_currentBuilding = (BuildingInfo)value; }
        protected override void Activate(PrefabInfo info)
        {
            var newBuildingChosen = CurrentInfo == null || info.name != ((BuildingInfo)CurrentInfo).GetAnalogName();
            base.Activate(info);
            if (newBuildingChosen)
            {
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
                ColumnCount = 1
            });

            tsStationTypeSelector = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsStationTypeSelector",
                ParentComponent = pnlStationTypeSelectorContainer,
                ColumnCount = 2,
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
            pnlTrackSelectorContainer = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlTrackSelectorContainer",
                ParentComponent = pnlInnerContainer,
                ColumnCount = 1
            });
            var lblTrackSelectorTitle = CreateLabel(new UILabelParamProps()
            {
                Name = "lblTrackSelectorTitle",
                Text = "Customize Individual Tracks",
                ParentComponent = pnlTrackSelectorContainer,
                ColumnCount = 1,
                Margins = new Vector4(8, 8, 8, 8),
            });
            tsIndividualTrackSelector = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsIndividualTrackSelector",
                ParentComponent = pnlTrackSelectorContainer,
                Height = 50,
                ColumnCount = 1,
                Margins = new Vector4(8, 8, 8, 8),
            });
            var pnlStyleContainer = CreatePanel(new UIPanelParamProps()
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
            tsStyles = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsStyles",
                ParentComponent = pnlStyleContainer,
                ColumnCount = 1,
                Height = 40,
                Margins = new Vector4(8, 8, 8, 8),
            });
            AddTrackStyleButtons();
        }
        private void AddTrackStyleButtons()
        {
            btnModernStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnModernStyle",
                ToolTip = "Modern Style",
                ColumnCount = 3,
                ParentComponent = tsStyles,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ModernStyleAtlas", UIHelper.ModernStyle),
                Width = 59,
                Height = 52,
                StackWidths = true,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Modern;
                    SetNetToolPrefab();
                }
            });
            btnClassicStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnClassicStyle",
                ToolTip = "Classic Style",
                ColumnCount = 3,
                ParentComponent = tsStyles,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ClassicStyleAtlas", UIHelper.ClassicStyle),
                Width = 59,
                Height = 52,
                StackWidths = true,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Classic;
                    SetNetToolPrefab();
                }
            });
            btnNoStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnNoStyle",
                ColumnCount = 3,
                ParentComponent = tsStyles,
                Atlas = atlas,
                Width = 1,
                Height = 1,
                StackWidths = true,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.None;
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
                ParentComponent = tsStationTypeSelector,
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
                    var paths = m_currentBuilding.GetPaths();

                    for (int i = 0; i < paths.Count(); i++)
                    {
                        if (paths[i]?.m_netInfo != null)
                        {
                            if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                                if (ButtonArray.Length > i && ButtonArray[i].atlas != this.atlas)
                                    ButtonArray[i].atlas = UIHelper.GenerateLinearAtlas("MOM_MetroTrackToggleAtlas", UIHelper.MetroTrackToggle, 3);
                            }
                            else if (paths[i].m_netInfo.IsAbovegroundTrainStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                                if (ButtonArray.Length > i && ButtonArray[i].atlas != this.atlas)
                                    ButtonArray[i].atlas = UIHelper.GenerateLinearAtlas("MOM_TrainTrackToggleAtlas", UIHelper.TrainTrackToggle, 3);
                            }
                            if (ButtonArray[i]?.atlas != null)
                                RefreshSprites(ButtonArray[i]);
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
                ParentComponent = tsStationTypeSelector,
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
                    var paths = m_currentBuilding.GetPaths();
                    for (int i = 0; i < paths.Count(); i++)
                    {
                        if (paths[i]?.m_netInfo != null)
                        {
                            if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsAbovegroundTrainStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                                if (ButtonArray.Length > i && ButtonArray[i].atlas != this.atlas)
                                    ButtonArray[i].atlas = UIHelper.GenerateLinearAtlas("MOM_TrainTrackToggleAtlas", UIHelper.TrainTrackToggle, 3);
                                RefreshSprites(ButtonArray[i]);
                            }
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
                ParentComponent = tsStationTypeSelector,
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
                    var paths = m_currentBuilding.GetPaths();
                    for (int i = 0; i < paths.Count(); i++)
                    {
                        if (paths[i]?.m_netInfo != null)
                        {
                            if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsAbovegroundTrainStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                                if (ButtonArray.Length > i && ButtonArray[i].atlas != this.atlas)
                                    ButtonArray[i].atlas = UIHelper.GenerateLinearAtlas("MOM_MetroTrackToggleAtlas", UIHelper.MetroTrackToggle, 3);
                                RefreshSprites(ButtonArray[i]);
                            }
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
            if (ButtonArray.Length > 0)
            {
                for (int i = 0; i < ButtonArray.Length; i++)
                {
                    UIComponent uIComponent = ButtonArray[i];
                    if (uIComponent?.parent != null)
                    {
                        uIComponent.parent.RemoveUIComponent(uIComponent);
                        if (uIComponent != null)
                        {
                            DestroyImmediate(uIComponent.gameObject);
                        }
                    }
                }
                //tsIndividualTrackSelector.height = 0;
            }
        }
        private void AddTrackVehicleTypeArrayButtons()
        {
            if (m_currentBuilding != null)
            {
                var paths = m_currentBuilding.GetPaths();
                var trackStationPaths = paths.Where(p => p?.m_netInfo != null && (p.m_netInfo.IsAbovegroundMetroStationTrack() || p.m_netInfo.IsAbovegroundTrainStationTrack())).ToList();
                if (trackStationPaths != null && trackStationPaths.Count() > 0)
                {
                    m_StationTrackPathCount = 0;
                    var excludeList = new List<int>();
                    for (int i = 0; i < paths.Count(); i++)
                    {
                        var path = paths[i];
                        if (path != null)
                        {
                            if (trackStationPaths.Contains(path))
                            {
                                if (!excludeList.Contains(i))
                                {
                                    if (m_LinkedPathDict == null)
                                    {
                                        m_LinkedPathDict = new Dictionary<int, List<int>>();
                                    }
                                    AddTrackVehicleTypeArrayButton(i, path, trackStationPaths.Count());
                                    m_LinkedPathDict.Add(i, m_buildingTool.m_prefab.LinkedPaths(i));
                                    var linkedPathCount = m_LinkedPathDict[i].Count();
                                    if (linkedPathCount > 0)
                                    {
                                        for (int j = 0; j < linkedPathCount; j++)
                                        {
                                            var inx = m_LinkedPathDict[i][j];
                                            if (trackStationPaths.Contains(paths[inx]))
                                            {
                                                AddTrackVehicleTypeArrayButton(inx, path, trackStationPaths.Count(), false);
                                                excludeList.Add(inx);
                                                //lastLinkedStationPathIndex = j;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (path.m_finalNetInfo.IsMetroTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                            }
                            else if (path.m_finalNetInfo.IsTrainTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                            }
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
            ButtonArray[i] = CreateButton(new UIButtonParamProps()
            {
                Name = "btnTrackSelector" + i,
                ToolTip = tooltip,
                Atlas = addUIComponent ? atlas : this.atlas,
                ParentComponent = tsIndividualTrackSelector,
                ColumnCount = 8,
                //StackWidths = true,
                Height = 37,
                Width = 37,
                //ForceRowEnd = m_StationTrackPathCount == trackPathCount,
                AddUIComponent = addUIComponent,
                EventClick = (c, v) =>
                {
                    var inx = Array.IndexOf(ButtonArray, c);
                    if (trackVehicleType == TrackVehicleType.Default)
                    {
                        if (m_currentBuilding.IsMetroStation())
                        {
                            Debug.Log($"Building {m_currentBuilding} has triggered a tabchange to metro");
                            trackVehicleType = TrackVehicleType.Metro;
                            lblTabTitle.text = METRO_TAB_LABEL;
                            tsStationTypeSelector.selectedIndex = 2;
                            pnlOuterContainer.color = MetroColor;
                        }
                        else if (m_currentBuilding.IsTrainStation())
                        {
                            Debug.Log($"Building {m_currentBuilding} has triggered a tabchange to train");
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
                            ButtonArray[inx].atlas = UIHelper.GenerateLinearAtlas("MOM_MetroTrackToggleAtlas", UIHelper.MetroTrackToggle, 3);
                    }
                    else if (TrackVehicleTypeArray[inx] == TrackVehicleType.Metro)
                    {
                        TrackVehicleTypeArray[inx] = TrackVehicleType.Train;
                        if (ButtonArray[inx].atlas != this.atlas)
                            ButtonArray[inx].atlas = UIHelper.GenerateLinearAtlas("MOM_TrainTrackToggleAtlas", UIHelper.TrainTrackToggle, 3);
                    }
                    if (ButtonArray[i]?.atlas != null)
                        RefreshSprites(ButtonArray[i]);
                    HandleLinkedPathPropagations(inx);
                    SetNetToolPrefab();
                }
            });
        }
        private TrackVehicleType[] m_TrackStyleArray = null;
        private TrackVehicleType[] TrackVehicleTypeArray {
            get
            {
                if (m_TrackStyleArray == null)
                {
                    m_TrackStyleArray = new TrackVehicleType[m_currentBuilding.GetPaths().Count()];
                }
                return m_TrackStyleArray;
            }
        }
        private UIButton[] m_ButtonArray = null;
        private UIButton[] ButtonArray {
            get
            {
                if (m_ButtonArray == null)
                {
                    m_ButtonArray = new UIButton[m_currentBuilding.GetPaths().Count()];
                }
                return m_ButtonArray;
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
            if (hasMetroTrack)
            {
                btnClassicStyle.isInteractive = true;
                btnModernStyle.isInteractive = true;
                if (trackStyle == TrackStyle.None)
                {
                    btnModernStyle.SimulateClick();
                }
            }
            else
            {
                if (trackStyle != TrackStyle.None)
                {
                    btnNoStyle.SimulateClick();
                }

                btnClassicStyle.state = UIButton.ButtonState.Disabled;
                btnModernStyle.state = UIButton.ButtonState.Disabled;
                btnClassicStyle.isInteractive = false;
                btnModernStyle.isInteractive = false;
            }

            btnClassicStyle.isInteractive = hasMetroTrack;
            btnModernStyle.isInteractive = hasMetroTrack;

            var trainAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Train.ToString();
            var metroAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Metro.ToString();

            List<BuildingInfo.PathInfo> paths = null;
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
                            m_buildingTool.m_prefab = PrefabCollection<BuildingInfo>.FindLoaded(comeCorrectName);

                            paths = m_currentBuilding.GetPaths();

                            for (int i = 0; i < paths.Count(); i++)
                            {
                                if (paths[i]?.m_finalNetInfo != null)
                                {
                                    if (paths[i].m_finalNetInfo.IsAbovegroundMetroStationTrack() || paths[i].m_finalNetInfo.IsMetroTrack() || paths[i].m_finalNetInfo.IsAbovegroundTrainStationTrack() || paths[i].m_finalNetInfo.IsTrainTrack())
                                    {
                                        m_buildingTool.m_prefab.SetTrackVehicleType(i, trackVehicleType);
                                    }
                                    if (paths[i].m_finalNetInfo.IsAbovegroundMetroStationTrack() || paths[i].m_finalNetInfo.IsMetroTrack())
                                    {
                                        paths[i].SetMetroStyle(trackStyle);
                                    }
                                }
                            }
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

                                m_buildingTool.m_prefab = PrefabCollection<BuildingInfo>.FindLoaded(comeCorrectName);
                            }

                            paths = m_currentBuilding.GetPaths();

                            for (var i = 0; i < paths.Count; i++)
                            {
                                if (paths[i]?.m_netInfo != null)
                                {
                                    if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsMetroTrack() || paths[i].m_netInfo.IsAbovegroundTrainStationTrack() || paths[i].m_netInfo.IsTrainTrack())
                                    {
                                        m_buildingTool.m_prefab.SetTrackVehicleType(i, TrackVehicleTypeArray[i]);
                                        //HandleLinkedPathPropagations(i);
                                    }
                                    if (paths[i].m_finalNetInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsMetroTrack())
                                    {
                                        paths[i].SetMetroStyle(trackStyle);
                                    }
                                }
                            }
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

                        paths = m_currentBuilding.GetPaths();
                        for (var i = 0; i < paths.Count; i++)
                        {
                            if (paths[i]?.m_netInfo != null)
                            {
                                if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsMetroTrack() || paths[i].m_netInfo.IsAbovegroundTrainStationTrack() || paths[i].m_netInfo.IsTrainTrack())
                                {
                                    m_buildingTool.m_prefab.SetTrackVehicleType(i, TrackVehicleTypeArray[i]);
                                    //HandleLinkedPathPropagations(i);
                                }
                                if (paths[i].m_finalNetInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsMetroTrack())
                                {
                                    paths[i].SetMetroStyle(trackStyle);
                                }
                            }
                        }
                        break;
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
