using ColossalFramework;
using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Redirection;
using MetroOverhaul.NEXT;
using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using MetroOverhaul.OptionsFramework;
using MetroOverhaul.UI;
using UnityEngine;

namespace MetroOverhaul
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;
        private static AssetsUpdater _updater;
        public static bool Done { get; private set; } // Only one Assets installation throughout the application

        private static readonly Queue<Action> LateBuildUpQueue = new Queue<Action>();

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            _updater = null;
            LateBuildUpQueue.Clear();
            InstallAssets();
            if (Container == null)
            {
                Container = new GameObject("MetroOverhaul").AddComponent<Initializer>();
            }
            Redirector<DepotAIDetour>.Deploy();
            if (OptionsWrapper<Options>.Options.improvedMetroTrainAi)
            {
                Redirector<MetroTrainAIDetour>.Deploy();
            }
            if (OptionsWrapper<Options>.Options.improvedPassengerTrainAi)
            {
                Redirector<PassengerTrainAIDetour>.Deploy();
            }
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
            _updater = null;
            if (Container == null)
            {
                return;
            }
            UnityEngine.Object.Destroy(Container.gameObject);
            Container = null;
            Redirector<DepotAIDetour>.Revert();
            Redirector<MetroTrainAI>.Revert();
            Redirector<PassengerTrainAIDetour>.Revert();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            while (LateBuildUpQueue.Count > 0)
            {
                LateBuildUpQueue.Dequeue().Invoke();
            }
            if (_updater == null)
            {
                _updater = new AssetsUpdater();
                _updater.UpdateExistingAssets(mode);
            }
            AssetsUpdater.UpdateBuildingsMetroPaths(mode, false);
            SimulationManager.instance.AddAction(DespawnVanillaMetro);
            if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame)
            {
                var gameObject = new GameObject("MetroOverhaulUISetup");
                gameObject.AddComponent<UpgradeSetup>();
                gameObject.AddComponent<StyleSelectionUI>();

                if (OptionsWrapper<Options>.Options.metroUi)
                {
                    UIView.GetAView().AddUIComponent(typeof(MetroStationCustomizerUI));
                }
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            //it appears, the game caches vanilla prefabs even when exiting to main menu, and stations won't load properly on reloading from main menu
            AssetsUpdater.UpdateBuildingsMetroPaths(LoadMode.LoadMap, true);
            var go = GameObject.Find("MetroOverhaulUISetup");
            if (go != null)
            {
                GameObject.Destroy(go);
            }
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
    }
}