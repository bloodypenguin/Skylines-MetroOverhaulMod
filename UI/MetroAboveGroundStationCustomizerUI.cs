using System.Linq;
using System.Collections.Generic;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.UI {
    public class MetroAboveGroundStationCustomizerUI : MetroCustomizerBase {
        private Dictionary<NetInfo, NetInfo[]> m_StationDict = null;
        private Dictionary<NetInfo, NetInfo[]> StationDict {
            get {
                if (m_StationDict == null) {
                    m_StationDict = new Dictionary<NetInfo, NetInfo[]>();
                    m_StationDict.Add(trainStationTrackPrefab, new[] { concreteSideStationPrefab, steelSideStationPrefab });
                }
                return m_StationDict;
            }
            set {
                m_StationDict = value;
            }
        }
        protected override bool SatisfiesTrackSpecs(PrefabInfo info) {
            return ((BuildingInfo)info).HasAbovegroundMetroStationTracks() || ((BuildingInfo)info).HasAbovegroundTrainStationTracks();
        }

        protected override ToolBase GetTheTool() {
            return m_buildingTool;
        }

        protected override PrefabInfo GetToolPrefab() {
            return ((BuildingTool)GetTheTool()).m_prefab;
        }

        protected override PrefabInfo CurrentInfo { get => m_currentBuilding; set => m_currentBuilding = (BuildingInfo)value; }

        protected override void SubStart() {
            stationClass = 1;
            trackStyle = 0;
        }

        protected override void CreateUI() {
#if DEBUG
            Next.Debug.Log("MOM ABOVE GROUND STATION TRACK GUI Created");
#endif

            backgroundSprite = "GenericPanel";
            color = new Color32(68, 84, 68, 170);
            width = 200;
            height = 300;
            opacity = 60;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Surface Station Options");

            btnTrain = CreateButton("Passenger Rail", 2, (c, v) => {
                stationClass = 0;
                SetNetToolPrefab();
            });
            btnTrain = CreateButton("Metro", 2, (c, v) => {
                stationClass = 1;
                SetNetToolPrefab();
            });

            btnModernStyle = CreateButton("Modern", 2, (c, v) => {
                trackStyle = 0;
                SetNetToolPrefab();
            });
            btnClassicStyle = CreateButton("Classic", 2, (c, v) => {
                trackStyle = 1;
                SetNetToolPrefab();
            });

        }
        private BuildingInfo m_TrainBuilding = null;
        private BuildingInfo m_Metrobuilding = null;
        private void SetNetToolPrefab() {
            //if (trackStyle == 0)
            //{
            //	btnModernStyle.color = new Color32(163, 255, 16, 255);
            //	btnModernStyle.normalBgSprite = "ButtonMenuFocused";
            //	btnModernStyle.useDropShadow = true;
            //	btnModernStyle.opacity = 95;
            //	btnClassicStyle.color = new Color32(150, 150, 150, 255);
            //	btnClassicStyle.normalBgSprite = "ButtonMenu";
            //	btnClassicStyle.useDropShadow = false;
            //	btnClassicStyle.opacity = 75;
            //}
            //else if (trackStyle == 1)
            //{
            //	btnClassicStyle.color = new Color32(163, 255, 16, 255);
            //	btnClassicStyle.normalBgSprite = "ButtonMenuFocused";
            //	btnClassicStyle.useDropShadow = true;
            //	btnClassicStyle.opacity = 95;
            //	btnModernStyle.color = new Color32(150, 150, 150, 255);
            //	btnModernStyle.normalBgSprite = "ButtonMenu";
            //	btnModernStyle.useDropShadow = false;
            //	btnModernStyle.opacity = 75;
            //}

            btnClassicStyle.isVisible = stationClass == 1;
            btnModernStyle.isVisible = stationClass == 1;
            //if (m_currentBuilding.m_class.m_subService == ItemClass.SubService.PublicTransportTrain) {
            //    if (m_TrainBuilding == null) {
            //        m_TrainBuilding = m_currentBuilding;
            //    }
            //    if (m_Metrobuilding == null) {
            //        m_Metrobuilding = m_currentBuilding.Clone(m_currentBuilding.name + " Analog");
            //        m_Metrobuilding.m_class = m_currentBuilding.m_class.Clone(m_currentBuilding.m_class.name + " Analog");
            //        m_Metrobuilding.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;

            //        //var ai = m_currentBuilding.GetComponent<TransportStationAI>().CloneBuildingAI(m_currentBuilding.GetComponent<TransportStationAI>().name + " Analog");
            //        //if (ai != null) {
            //        //    ai.m_info = m_Metrobuilding;
            //        //    ai.m_transportLineInfo = PrefabCollection<NetInfo>.FindLoaded("Metro");
            //        //    ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Metro Line");
            //        //    m_Metrobuilding.m_buildingAI = ai;
            //        //}
            //        var paths = m_Metrobuilding.m_paths;
            //        for (var i = 0; i < paths.Length; i++) {
            //            if (paths[i]?.m_netInfo != null) {
            //                var trackName = paths[i].m_netInfo.name;
            //                switch (stationClass) {
            //                    case 0:
            //                        if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack()) {
            //                            var match = StationDict.Where(s => s.Value.Contains(paths[i].m_netInfo));
            //                            if (match.Count() > 0) {
            //                                paths[i].AssignNetInfo(match.First().Key);
            //                            }
            //                        }
            //                        break;
            //                    case 1:
            //                        if (paths[i].m_netInfo.IsAbovegroundTrainStationTrack()) {
            //                            var track = StationDict[paths[i].m_netInfo][trackStyle];
            //                            paths[i].AssignNetInfo(track);
            //                        } else if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack()) {
            //                            var match = StationDict.Where(s => s.Value.Contains(paths[i].m_netInfo));
            //                            if (match.Count() > 0) {
            //                                paths[i].AssignNetInfo(match.First().Value[trackStyle]);
            //                            }
            //                        }
            //                        break;
            //                }

            //            }
            //        }

            //    }
                switch (stationClass) {
                    case 0:
                        //m_buildingTool.m_prefab = m_TrainBuilding;
                        break;
                    case 1:
                        //m_buildingTool.m_prefab = m_Metrobuilding;
                        break;
                }
            //}
        }
        private void Activate(BuildingInfo bInfo) {
            m_activated = true;
            m_currentBuilding = bInfo;
            isVisible = true;
            foreach (var path in bInfo.m_paths) {
                if (path.m_netInfo.IsAbovegroundMetroStationTrack()) {
                    trackStyle = path.m_netInfo.name.ToLower().StartsWith("steel") ? 1 : 0;
                    break;
                }
            }
            SetNetToolPrefab();
        }
        private void Deactivate() {
            if (!m_activated) {
                return;
            }
            m_currentBuilding = null;
            isVisible = false;
            m_activated = false;

        }
    }
}
