using System.Collections.Generic;
using MetroOverhaul.Extensions;
using UnityEngine;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul.UI {
    public class MetroAboveGroundStationCustomizerUI: MetroCustomizerBaseUI {
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
            return ((BuildingTool)GetTheTool()).m_prefab;
        }

        protected override PrefabInfo CurrentInfo { get => m_currentBuilding; set => m_currentBuilding = (BuildingInfo)value; }

        protected override void SubStart()
        {
            trackVehicleType = TrackVehicleType.Default;
            trackStyle = TrackStyle.Modern;
        }

        protected override void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM ABOVE GROUND STATION TRACK GUI Created");
#endif

            backgroundSprite = "GenericPanel";
            color = new Color32(68, 84, 68, 170);
            width = 200;
            height = 300;
            opacity = 60;
            position = Vector3.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Surface Station Options");

            btnTrain = CreateButton("Train", 3, (c, v) =>
            {
                trackVehicleType = TrackVehicleType.Train;
                SetNetToolPrefab();
            });
            btnMetro = CreateButton("Metro", 3, (c, v) =>
            {
                trackVehicleType = TrackVehicleType.Metro;
                SetNetToolPrefab();
            });
            btnDefault = CreateButton("Default", 3, (c, v) =>
            {
                trackVehicleType = TrackVehicleType.Default;
                SetNetToolPrefab();
            });
            btnModernStyle = CreateButton("Modern", 2, (c, v) =>
            {
                trackStyle = TrackStyle.Modern;
                SetNetToolPrefab();
            });
            btnClassicStyle = CreateButton("Classic", 2, (c, v) =>
            {
                trackStyle = TrackStyle.Classic;
                SetNetToolPrefab();
            });

        }

        private void SetNetToolPrefab()
        {
            btnTrain.isEnabled = Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter;
            ToggleButtonPairs((int)trackVehicleType, btnDefault, btnTrain, btnMetro);
            ToggleButtonPairs((int)trackStyle, btnModernStyle, btnClassicStyle);
            btnClassicStyle.isVisible = trackVehicleType == TrackVehicleType.Metro || (trackVehicleType == TrackVehicleType.Default && m_currentBuilding.IsMetroStation());
            btnModernStyle.isVisible = trackVehicleType == TrackVehicleType.Metro || (trackVehicleType == TrackVehicleType.Default && m_currentBuilding.IsMetroStation());
            var trainAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Train.ToString();
            var metroAnalogSuffix = ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Metro.ToString();
            switch (trackVehicleType)
            {
                case TrackVehicleType.Default:
                    break;
                case TrackVehicleType.Train:
                    {
                        if (Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter && m_currentBuilding.IsMetroStation())
                        {
                            var comeCorrectName = "";
                            if (m_currentBuilding.name.EndsWith(metroAnalogSuffix))
                            {
                                comeCorrectName = m_currentBuilding.name.Substring(0, m_currentBuilding.name.IndexOf(metroAnalogSuffix));
                            }
                            else
                            {
                                comeCorrectName = m_currentBuilding.name + trainAnalogSuffix;
                            }
                            m_buildingTool.m_prefab = PrefabCollection<BuildingInfo>.FindLoaded(comeCorrectName);
                            if (m_buildingTool.m_prefab.HasAbovegroundMetroStationTracks())
                            {
                                var paths = m_buildingTool.m_prefab.m_paths;
                                for (var i = 0; i < paths.Length; i++)
                                {
                                    if (paths[i]?.m_netInfo != null)
                                    {
                                        if (paths[i].m_netInfo.IsAbovegroundMetroStationTrack() || paths[i].m_netInfo.IsMetroTrack())
                                        {
                                            m_buildingTool.m_prefab.SetTrackVehicleType(i, trackVehicleType);
                                        }
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
                            var comeCorrectName = "";
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

                        var paths = m_buildingTool.m_prefab.m_paths;
                        for (var i = 0; i < paths.Length; i++)
                        {
                            if (paths[i]?.m_netInfo != null)
                            {
                                if (paths[i].m_netInfo.IsAbovegroundTrainStationTrack() || paths[i].m_netInfo.IsTrainTrack())
                                {
                                    m_buildingTool.m_prefab.SetTrackVehicleType(i, trackVehicleType);
                                }
                                paths[i].SetMetroStyle(trackStyle);
                            }
                        }
                        break;
                    }
            }
        }
        private void Activate(BuildingInfo bInfo)
        {
            m_activated = true;
            m_currentBuilding = bInfo;
            isVisible = true;
            if (bInfo.IsTrainStation())
            {
                trackVehicleType = TrackVehicleType.Train;
            }
            else if (bInfo.IsMetroStation())
            {
                trackVehicleType = TrackVehicleType.Metro;
                foreach (var path in bInfo.m_paths)
                {
                    if (path.m_netInfo.IsAbovegroundMetroStationTrack())
                    {
                        trackStyle = path.m_netInfo.name.ToLower().StartsWith("steel") ? TrackStyle.Classic : TrackStyle.Modern;
                        break;
                    }
                }
            }
            SetNetToolPrefab();
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
    }
}
