using ColossalFramework;
using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Redirection;
using MetroOverhaul.NEXT;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetroOverhaul
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;
        public static bool Done { get; private set; } // Only one Assets installation throughout the application

        private static readonly Queue<Action> LateBuildUpQueue = new Queue<Action>();

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            LateBuildUpQueue.Clear();
            InstallAssets();
            if (Container == null)
            {
                Container = new GameObject("MetroOverhaul").AddComponent<Initializer>();
            }
            Redirector<DepotAIDetour>.Deploy();
            //Redirector<MetroTrainAIDetour>.Deploy(); //don't deploy this! For some reason that causes citizens not boarding trains
        }

        public static void EnqueueLateBuildUpAction(Action action)
        {
            LateBuildUpQueue.Enqueue(action);
        }

        private static void InstallAssets()
        {
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
            while (LateBuildUpQueue.Count > 0)
            {
                LateBuildUpQueue.Dequeue().Invoke();
            }
            MetroStations.UpdateMetroStation(0, 0);
            DespawnVanillaMetro();
            UpdateEffect();
        }

        private static void DespawnVanillaMetro()
        {
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
        }

        private static void UpdateEffect()
        {
            var metro = PrefabCollection<VehicleInfo>.FindLoaded("Metro");
            var arriveEffect = ((MetroTrainAI)metro.m_vehicleAI).m_arriveEffect;
            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                var info = PrefabCollection<VehicleInfo>.GetLoaded(i);
                var metroTrainAI = info?.m_vehicleAI as MetroTrainAI;
                if (metroTrainAI == null)
                {
                    continue;
                }
                info.m_effects = metro.m_effects;
                metroTrainAI.m_arriveEffect = arriveEffect;
            }
        }


    }
}