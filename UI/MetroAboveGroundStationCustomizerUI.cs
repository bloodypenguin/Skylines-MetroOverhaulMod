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
            return ((BuildingInfo)info).HasAbovegroundMetroStationTracks() || (Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter && ((BuildingInfo)info).HasAbovegroundTrainStationTracks());
        }

        protected override ToolBase GetTheTool()
        {
            return m_buildingTool;
        }

        protected override PrefabInfo GetToolPrefab()
        {
            return ((BuildingTool)GetTheTool())?.m_prefab;
        }

        private void AddTrackStyleButtons()
        {
            btnModernStyle = CreateButton(new UIButtonParamProps()
            {
                Text = "Modern",
                ColumnCount = 2,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Modern;
                    SetNetToolPrefab();
                }
            });

            btnClassicStyle = CreateButton(new UIButtonParamProps()
            {
                Text = "Classic",
                ColumnCount = 2,
                SameLine = true,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Classic;
                    SetNetToolPrefab();
                }
            });
        }

        private void RemoveTrackStyleButtons()
        {
            if (btnModernStyle != null || btnClassicStyle != null)
            {
                RemoveUIComponent(btnModernStyle);
                RemoveUIComponent(btnClassicStyle);
                btnModernStyle = null;
                btnClassicStyle = null;
            }
        }

        private void RemoveLinkedPathPanels()
        {
            if (m_LinkedPathPanelArray != null && m_LinkedPathPanelArray.Count > 0)
            {
                for (var i = 0; i < LinkedPathPanelArray.Count; i++)
                {
                    RemoveUIComponent(LinkedPathPanelArray[i]);
                }
                m_LinkedPathPanelArray = null;
            }
        }
        private void AddLinkedPathPanel(int inx, int lastInx)
        {
            var linkedPathPanel = this.AddUIComponent<UIPanel>();
            var lastButton = ButtonArray[m_LinkedPathDict[inx][lastInx]];
            linkedPathPanel.atlas = atlas;
            linkedPathPanel.backgroundSprite = "GenericPanel";
            linkedPathPanel.height = 10;
            linkedPathPanel.width = (lastButton.position.x + lastButton.width) - ButtonArray[inx].position.x;
            linkedPathPanel.relativePosition = new Vector3(ButtonArray[inx].relativePosition.x, ButtonArray[inx].relativePosition.y + 10);
            linkedPathPanel.color = new Color32(163, 255, 16, 255);
            linkedPathPanel.opacity = 50;
            linkedPathPanel.SendToBack();
            LinkedPathPanelArray.Add(linkedPathPanel);
        }
        private int maxColumns = 6;
        private int m_StationTrackPathCount;

        private void AddTrackVehicleTypeArrayButtons()
        {
            if (m_currentBuilding != null)
            {
                Debug.Log("Checkpoint3");
                var paths = m_currentBuilding.GetPaths();
                Debug.Log("Checkpoint4");
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
                                    Debug.Log("Checkpoint5");
                                    AddTrackVehicleTypeArrayButton(i, path, trackStationPaths.Count());
                                    Debug.Log("Getting linked paths for index " + i);
                                    m_LinkedPathDict.Add(i, m_buildingTool.m_prefab.LinkedPaths(i));
                                    Debug.Log("Checkpoint7");
                                    var linkedPathCount = m_LinkedPathDict[i].Count();
                                    if (linkedPathCount > 0)
                                    {
                                        //var lastLinkedStationPathIndex = -1;
                                        for (int j = 0; j < linkedPathCount; j++)
                                        {
                                            var inx = m_LinkedPathDict[i][j];
                                            if (trackStationPaths.Contains(paths[inx]))
                                            {
                                                Debug.Log("Checkpoint8");
                                                AddTrackVehicleTypeArrayButton(inx, path, trackStationPaths.Count(), false);
                                                Debug.Log("Checkpoint9");
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
            ButtonArray[i] = CreateButton(new UIButtonParamProps()
            {
                Text = i.ToString(),
                ColumnCount = maxColumns,
                ForceRowEnd = m_StationTrackPathCount == trackPathCount,
                AddUIComponent = addUIComponent,
                EventClick = (c, v) =>
                {
                    var inx = Array.IndexOf(ButtonArray, c);
                    if (trackVehicleType == TrackVehicleType.Default)
                    {
                        if (m_currentBuilding.IsMetroStation())
                        {
                            trackVehicleType = TrackVehicleType.Metro;
                        }
                        else if (m_currentBuilding.IsTrainStation())
                        {
                            trackVehicleType = TrackVehicleType.Train;
                        }
                    }
                    if (m_InRecursion)
                        Debug.Log("We are hitting the right place if we are in index " + inx);
                    if (TrackVehicleTypeArray[inx] == TrackVehicleType.Train)
                    {
                        TrackVehicleTypeArray[inx] = TrackVehicleType.Metro;
                        ButtonArray[inx].text = "M";
                    }
                    else if (TrackVehicleTypeArray[inx] == TrackVehicleType.Metro)
                    {
                        TrackVehicleTypeArray[inx] = TrackVehicleType.Train;
                        ButtonArray[inx].text = "T";
                    }
                    HandleLinkedPathPropagations(inx);
                    SetNetToolPrefab();
                }
            });

            if (path.m_finalNetInfo.IsAbovegroundMetroStationTrack())
            {
                TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                ButtonArray[i].text = "M";
            }
            else if (path.m_finalNetInfo.IsAbovegroundTrainStationTrack())
            {
                TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                ButtonArray[i].text = "T";
            }
        }
        private void RemoveTrackVehicleTypeArrayButtons()
        {
            var buttonArrayCount = ButtonArray.Count();
            for (int i = 0; i < buttonArrayCount; i++)
            {
                if (ButtonArray[i] != null)
                {
                    RemoveUIComponent(ButtonArray[i]);
                }
            }
        }

        private void AddTrackVehicleTypeButtons()
        {
            btnDefault = CreateButton(new UIButtonParamProps()
            {
                Text = "Default",
                ColumnCount = 3,
                EventClick = (c, v) =>
                {
                    m_TrackStyleArray = null;
                    trackVehicleType = TrackVehicleType.Default;
                    var paths = m_currentBuilding.GetPaths();

                    for (int i = 0; i < paths.Count(); i++)
                    {
                        if (paths[i]?.m_netInfo != null)
                        {
                            if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                                ButtonArray[i].text = "M";
                            }
                            else if (paths[i].m_netInfo.IsAbovegroundTrainStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                                ButtonArray[i].text = "T";
                            }
                        }
                        HandleLinkedPathPropagations(i);
                    }
                    SetNetToolPrefab();
                }
            });

            btnTrain = CreateButton(new UIButtonParamProps()
            {
                Text = "Train",
                NormalFgSprite = "SubBarPublicTransportTrain",
                ColumnCount = 3,
                EventClick = (c, v) =>
                {
                    m_TrackStyleArray = null;
                    trackVehicleType = TrackVehicleType.Train;
                    var paths = m_currentBuilding.GetPaths();
                    for (int i = 0; i < paths.Count(); i++)
                    {
                        if (paths[i]?.m_netInfo != null)
                        {
                            if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsAbovegroundTrainStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Train;
                                ButtonArray[i].text = "T";
                            }
                        }
                        HandleLinkedPathPropagations(i);
                    }
                    SetNetToolPrefab();
                }
            });

            btnMetro = CreateButton(new UIButtonParamProps()
            {
                Text = "Metro",
                NormalFgSprite = "SubBarPublicTransportMetro",
                ColumnCount = 3,
                EventClick = (c, v) =>
                {
                    m_TrackStyleArray = null;
                    trackVehicleType = TrackVehicleType.Metro;
                    var paths = m_currentBuilding.GetPaths();
                    for (int i = 0; i < paths.Count(); i++)
                    {
                        if (paths[i]?.m_netInfo != null)
                        {
                            if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsAbovegroundTrainStationTrack())
                            {
                                TrackVehicleTypeArray[i] = TrackVehicleType.Metro;
                                ButtonArray[i].text = "M";
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
                if (m_LinkedPathDict.ContainsKey(i) && m_LinkedPathDict[i].Count > 0)
                {
                    foreach (var inx in m_LinkedPathDict[i])
                    {
                        if (m_buildingTool.m_prefab.m_paths[inx].m_netInfo.IsStationTrack())
                        {
                            m_InRecursion = true;
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
                m_rowIndex = 2;
                AddTrackVehicleTypeArrayButtons();
                btnDefault.SimulateClick();
            }
        }
        protected override void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM ABOVE GROUND STATION TRACK GUI Created");
#endif

            backgroundSprite = "GenericPanel";
            color = new Color32(68, 84, 68, 170);
            width = 300;
            height = 300;
            opacity = 60;
            position = new Vector3(300, 0, 0);
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Surface Station Options");
            AddTrackVehicleTypeButtons();
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
            btnTrain.isEnabled = Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter;
            ToggleButtonPairs((int)trackVehicleType, btnDefault, btnTrain, btnMetro);
            if (btnModernStyle != null && btnClassicStyle != null)
            {
                ToggleButtonPairs((int)trackStyle, btnModernStyle, btnClassicStyle);
            }

            var trainAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Train.ToString();
            var metroAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Metro.ToString();

            List<BuildingInfo.PathInfo> paths = null;
            switch (trackVehicleType)
            {
                case TrackVehicleType.Default:
                    {
                        if (Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
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
                        if (Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
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

            if (TrackVehicleTypeArray.Any(t => t == TrackVehicleType.Metro))
            {
                if (btnClassicStyle == null && btnModernStyle == null)
                {
                    AddTrackStyleButtons();
                }
            }
            else
            {
                RemoveTrackStyleButtons();
            }
        }

    }
}
