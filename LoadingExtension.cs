using ColossalFramework;
using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Redirection;
using SubwayOverhaul.NEXT;
using System;
using UnityEngine;

namespace MetroOverhaul
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;
        public static bool Done { get; private set; } // Only one Assets installation throughout the application

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (Done) // Only one Assets installation throughout the application
            {
                return;
            }

            var path = Util.AssemblyPath;
            foreach (var action in AssetManager.instance.CreateLoadingSequence(path))
            {
                var localAction = action;

                Loading.QueueLoadingAction(() =>
                {
                    try
                    {
                        localAction();
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogException(ex);
                    }
                });
            }

            Done = true;
            if (Container == null)
            {
                Container = new GameObject("MetroOverhaul").AddComponent<Initializer>();
            }
            Redirector<DepotAIDetour>.Deploy();
            //Redirector<MetroTrainAIDetour>.Deploy(); //don't deploy this! For some reason that causes citizens not boarding trains
        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (Container == null)
            {
                return;
            }
            UnityEngine.Object.Destroy(Container.gameObject);
            Container = null;
            Redirector<DepotAIDetour>.Revert();
            Redirector<MetroTrainAI>.Revert();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            UpdateMetroStations();
            var vehicles = Singleton<VehicleManager>.instance.m_vehicles;
            for (ushort i = 0; i < vehicles.m_size; i++)
            {
                var vehicle = vehicles.m_buffer[i];
                if (vehicle.m_flags == ~Vehicle.Flags.All || vehicle.Info == null)
                {
                    continue;
                }
                if (vehicle.Info.name == "Metro")
                {
                    Singleton<VehicleManager>.instance.ReleaseVehicle(i);
                }
            }
            UpdateEffect();
        }

        private static void UpdateEffect()
        {
            var effect = ((MetroTrainAI)PrefabCollection<VehicleInfo>.FindLoaded("Metro").m_vehicleAI).m_arriveEffect;
            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                var info = PrefabCollection<VehicleInfo>.GetLoaded(i);
                var metroTrainAI = info?.m_vehicleAI as MetroTrainAI;
                if (metroTrainAI == null)
                {
                    continue;
                }
                metroTrainAI.m_arriveEffect = effect;
            }
        }

        private static void UpdateMetroStations()
        {
            foreach (var info in Resources.FindObjectsOfTypeAll<BuildingInfo>())
            {
                if (!(info.m_buildingAI is TransportStationAI) ||
                    info.m_class.m_subService != ItemClass.SubService.PublicTransportMetro)
                {
                    continue;
                }
                var transportStationAi = ((TransportStationAI)info.m_buildingAI);
                transportStationAi.m_maxVehicleCount = 0;
                foreach (var path in info.m_paths)
                {
                    if (path.m_netInfo.name != "Metro Station Track")
                    {
                        continue;
                    }
                    var start = path.m_nodes[0];
                    var end = path.m_nodes[1];

                    if (!(Vector3.Distance(start, end) < 144.0f))
                    {
                        UnityEngine.Debug.Log($"Station {info.name}: can't make longer tracks: track is already long enough");
                        continue;
                    }
                    var middle = new Vector3
                    {
                        x = (start.x + end.x) / 2.0f,
                        y = (start.y + end.y) / 2.0f,
                        z = (start.z + end.z) / 2.0f
                    };
                    if (path.m_curveTargets.Length > 1 || (path.m_curveTargets.Length == 1 && Vector3.Distance(middle, path.m_curveTargets[0]) > 0.1))
                    {
                        UnityEngine.Debug.Log($"Station {info.name}: can't make longer tracks: unprocessable curve points");
                        continue;
                    }

                    var toStart = Vector3.Distance(start, middle);
                    start.x = middle.x + 72.0f * (start.x - middle.x) / toStart;
                    start.y = middle.y + 72.0f * (start.y - middle.y) / toStart;
                    start.z = middle.z + 72.0f * (start.z - middle.z) / toStart;
                    path.m_nodes[0] = start;
                    var toEnd = Vector3.Distance(end, middle);
                    end.x = middle.x - 72.0f * (middle.x - end.x) / toEnd;
                    end.y = middle.y - 72.0f * (middle.y - end.y) / toEnd;
                    end.z = middle.z - 72.0f * (middle.z - end.z) / toEnd;
                    path.m_nodes[1] = end;
                    //TODO(earalov): shift connected segments
                }
            }
        }
    }
}