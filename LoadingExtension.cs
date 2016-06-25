using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Redirection;
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
            Redirector<DepotAIDetour>.Deploy();
            Redirector<MetroTrainAIDetour>.Deploy();
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
            Redirector<DepotAIDetour>.Revert();
            Redirector<MetroTrainAI>.Revert();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            DetroyRefresher();
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
            milestone = track.GetComponent<MetroTrackAI>().m_createPassMilestone;
            SetupMetroTrack(track, trainTrack);
            var stationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
            SetupMetroTrack(stationTrack, trainTrack);

            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                RefreshPanelInGame();
            }        
        }

        private static void SetupMetroTrack(NetInfo track, NetInfo trainTrack)
        {
            var ai = track.GetComponent<NetAI>();
            GameObject.Destroy(ai);
            var newAi = track.gameObject.AddComponent<TrainTrackTunnelAI>();
            track.m_placementStyle = ItemClass.Placement.Procedural;
            track.m_netAI = newAi;
            track.m_netAI.m_info = track;
            track.m_buildHeight = trainTrack.m_buildHeight;
            track.m_maxHeight = trainTrack.m_maxHeight;
            track.m_minHeight = trainTrack.m_minHeight;
            track.m_surfaceLevel = trainTrack.m_surfaceLevel;
            track.m_terrainEndOffset = trainTrack.m_terrainEndOffset;
            track.m_terrainStartOffset = trainTrack.m_terrainStartOffset;
            track.m_followTerrain = false;
            newAi.m_createPassMilestone = milestone;
        }

        private static void UpdateMetroStation(BuildingInfo info)
        {
            var transportStationAi = ((TransportStationAI) info.m_buildingAI);
            transportStationAi.m_maxVehicleCount = 0;
        }

        private static void RefreshPanelInGame()
        {
            new GameObject("MetroOverhaulPanelRefresher").AddComponent<PanelRefresher>();
        }

        private class PanelRefresher : MonoBehaviour
        {
            private void Update()
            {
                var go = GameObject.Find("PublicTransportMetroPanel");
                var panel = go?.GetComponent<PublicTransportPanel>();
                if (panel == null)
                {
                    return;
                }
                panel?.RefreshPanel();
                Destroy(this);
            }
        }

        private static void DetroyRefresher()
        {
            var refresher = GameObject.Find("MetroOverhaulPanelRefresherr");
            if (refresher != null)
            {
                Object.Destroy(refresher);
            }
        }
    }
}