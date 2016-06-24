using ICities;
using UnityEngine;

namespace MetroOverhaul
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;
        public static ManualMilestone milestone;
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (Container == null)
            {
                Container = new GameObject("MetroOverhaul").AddComponent<Initializer>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (Container == null)
            {
                return;
            }
            Object.Destroy(Container.gameObject);
            Container = null;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            foreach (var info in Resources.FindObjectsOfTypeAll<BuildingInfo>())
            {
                if (!(info.m_buildingAI is TransportStationAI) || info.m_class.m_subService != ItemClass.SubService.PublicTransportMetro)
                {
                    continue;
                }
                UpdateMetroStation(info);
            }


            var trainTrack = PrefabCollection<NetInfo>.FindLoaded("Train Track Tunnel");
            var track = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
            track.m_buildHeight = trainTrack.m_buildHeight;
            var ai = track.GetComponent<NetAI>();
            milestone = ((MetroTrackAI)ai).m_createPassMilestone;

            GameObject.Destroy(ai);
            var newAi = track.gameObject.AddComponent<TrainTrackTunnelAI>();
            track.m_netAI = newAi;
            track.m_netAI.m_info = track;
            track.m_maxHeight = trainTrack.m_maxHeight;
            track.m_minHeight = trainTrack.m_minHeight;
            track.m_surfaceLevel = trainTrack.m_surfaceLevel;
            track.m_terrainEndOffset = trainTrack.m_terrainEndOffset;
            track.m_terrainStartOffset = trainTrack.m_terrainStartOffset;
            track.m_followTerrain = false;
            newAi.m_createPassMilestone = milestone;


            var stationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
            var ai1 = stationTrack.GetComponent<NetAI>();
            GameObject.Destroy(ai1);
            var newAi1 = stationTrack.gameObject.AddComponent<TrainTrackTunnelAI>();
            stationTrack.m_netAI = newAi1;
            stationTrack.m_netAI.m_info = stationTrack;
            stationTrack.m_buildHeight = trainTrack.m_buildHeight;
            stationTrack.m_maxHeight = trainTrack.m_maxHeight;
            stationTrack.m_minHeight = trainTrack.m_minHeight;
            stationTrack.m_surfaceLevel = trainTrack.m_surfaceLevel;
            stationTrack.m_terrainEndOffset = trainTrack.m_terrainEndOffset;
            stationTrack.m_terrainStartOffset = trainTrack.m_terrainStartOffset;
            stationTrack.m_followTerrain = false;
            newAi1.m_createPassMilestone = milestone;

        }

        private static void UpdateMetroStation(BuildingInfo info)
        {
            var transportStationAi = ((TransportStationAI) info.m_buildingAI);
            transportStationAi.m_maxVehicleCount = 0;
            foreach (var path in info.m_paths)
            {
                if (path.m_netInfo.m_class.m_subService == ItemClass.SubService.PublicTransportMetro)
                {
                    for (var index = 0; index < path.m_nodes.Length; index++)
                    {
                        if (path.m_nodes[index].y > -12)
                        {
                            path.m_nodes[index] = new Vector3(path.m_nodes[index].x, -12, path.m_nodes[index].z);
                        }
                    }
                    for (var index = 0; index < path.m_curveTargets.Length; index++)
                    {
                        if (path.m_curveTargets[index].y > -12)
                        {
                            path.m_curveTargets[index] = new Vector3(path.m_curveTargets[index].x, -12,
                                path.m_curveTargets[index].z);
                        }
                    }
                }
            }
        }
    }
}