using ColossalFramework;
using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Redirection;
using MetroOverhaul.NEXT;
using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using MetroOverhaul.OptionsFramework;
using MetroOverhaul.UI;
using PrefabHook;
using UnityEngine;
using MetroOverhaul.Extensions;

namespace MetroOverhaul
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;
        private static AssetsUpdater _updater;
        public static bool Done { get; private set; } // Only one Assets installation throughout the application

        private static readonly Queue<Action> LateBuildUpQueue = new Queue<Action>();
#if DEBUG
        private bool indebug = true;
#else
        private bool indebug = false;
#endif

        private LoadMode _cachedMode;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (Util.IsHooked())
            {
                if (OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
                {
                    BuildingInfoHook.OnPreInitialization += info =>
                    {
                        try
                        {
                            if (Container.RegisterWid(info,true))
                            {
                                var ai = info.GetComponent<PlayerBuildingAI>();
                                if (ai != null)
                                {
                                    if (ai is TransportStationAI)
                                    {
                                        if (info.m_class.m_subService == ItemClass.SubService.PublicTransportTrain || info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro)
                                        {
                                            if (info.name.IndexOf(ModTrackNames.ANALOG_PREFIX) == -1)
                                            {
                                                if (info.HasAbovegroundTrainStationTracks() || info.HasAbovegroundMetroStationTracks())
                                                {
                                                    Container.InitializeBuildingInfoImpl(info);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(e);
                        }
                    };
                    BuildingInfoHook.OnPostInitialization += info =>
                    {
                        try
                        {
                            if (Container.RegisterWid(info, false))
                            {
                                var ai = info.GetComponent<PlayerBuildingAI>();
                                if (ai != null)
                                {
                                    if (ai is TransportStationAI)
                                    {
                                        if (info.m_class.m_subService == ItemClass.SubService.PublicTransportTrain || info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro)
                                        {
                                            if (info.name.IndexOf(ModTrackNames.ANALOG_PREFIX) == -1)
                                            {
                                                if (info.HasAbovegroundTrainStationTracks() || info.HasAbovegroundMetroStationTracks())
                                                {
                                                    Container.InitializeBuildingInfoImpl(info);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(e);
                        }
                    };
                    BuildingInfoHook.Deploy();
                }
            }

            _updater = null;
            LateBuildUpQueue.Clear();
            InstallAssets();
            if (Container == null)
            {
                Container = new GameObject("MetroOverhaul").AddComponent<Initializer>();
                Container.AppMode = loading.currentMode;
            }

            if (loading.currentMode == AppMode.AssetEditor)
            {
                Redirector<RoadsGroupPanelDetour>.Deploy();
            }
            if (loading.currentMode == AppMode.Game)
            {
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

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            bool isVanilla = OptionsWrapper<Options>.Options.ghostMode;
            if (!isVanilla)
            {
                _cachedMode = mode;
                while (LateBuildUpQueue.Count > 0)
                {
                    try
                    {
                        LateBuildUpQueue.Dequeue().Invoke();
                    }
                    catch (Exception e)
                    {
                        UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Enable asset in Content Manager!", e.Message, false);
                    }
                }
                if (_updater == null)
                {
                    _updater = new AssetsUpdater();
                    _updater.UpdateExistingAssets(mode);
                }
            }
            AssetsUpdater.UpdateBuildingsMetroPaths(mode, isVanilla);
            if (!isVanilla)
            {
                if (mode == LoadMode.NewGame || mode == LoadMode.LoadGame || mode == LoadMode.NewGameFromScenario)
                {
                    SimulationManager.instance.AddAction(DespawnVanillaMetro);
                    var gameObject = new GameObject("MetroOverhaulUISetup");
                    gameObject.AddComponent<UpgradeSetup>();
                    //#if DEBUG
                    //                gameObject.AddComponent<StyleSelectionStationUI>();
                    //#else
                    //                gameObject.AddComponent<StyleSelectionUI>();
                    //#endif

                    if (OptionsWrapper<Options>.Options.metroUi)
                    {
                        UIView.GetAView().AddUIComponent(typeof(MetroStationCustomizerUI));
                        UIView.GetAView().AddUIComponent(typeof(MetroTrackCustomizerUI));
                        UIView.GetAView().AddUIComponent(typeof(MetroAboveGroundStationCustomizerUI));
                    }

                    var transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Metro");
                    transportInfo.m_netLayer = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                    transportInfo.m_stationLayer = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                    transportInfo.m_secondaryLayer = 9;
                    transportInfo.m_secondaryLineMaterial = transportInfo.m_lineMaterial;
                    transportInfo.m_secondaryLineMaterial2 = transportInfo.m_lineMaterial2;
                }
                else
                {
                    var gameObject = new GameObject("MetroOverhaulUISetup");
                    //gameObject.AddComponent<StyleSelectionStationUI>();
#if DEBUG
                    UIView.GetAView().AddUIComponent(typeof(MetroStationCustomizerUI));
                    UIView.GetAView().AddUIComponent(typeof(MetroTrackAssetCustomizerUI));
                    UIView.GetAView().AddUIComponent(typeof(MetroAboveGroundStationCustomizerUI));
#endif
                }
                //Container.DoSomething();
            }

        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (!OptionsWrapper<Options>.Options.ghostMode)
            {
                //it appears, the game caches vanilla prefabs even when exiting to main menu, and stations won't load properly on reloading from main menu
                //AssetsUpdater.UpdateBuildingsMetroPaths(_cachedMode, true);
                var go = GameObject.Find("MetroOverhaulUISetup");
                if (go != null)
                {
                    GameObject.Destroy(go);
                }

                var transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Metro");
                transportInfo.m_netLayer = ItemClass.Layer.MetroTunnels;
                transportInfo.m_stationLayer = ItemClass.Layer.MetroTunnels;
            }

        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (Util.IsHooked() && OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
                BuildingInfoHook.Revert();

            if (!OptionsWrapper<Options>.Options.ghostMode)
            {
                _updater = null;
                if (Container == null)
                {
                    return;
                }
                UnityEngine.Object.Destroy(Container);
                Container = null;
                Redirector<RoadsGroupPanelDetour>.Revert();
                Redirector<DepotAIDetour>.Revert();
                Redirector<MetroTrainAI>.Revert();
                Redirector<PassengerTrainAIDetour>.Revert();
            }
        }
        private static void DespawnVanillaMetro()
        {
            var vehicles = Singleton<VehicleManager>.instance.m_vehicles;
            for (ushort i = 0; i < vehicles.m_size; i++)
            {
                var vehicle = vehicles.m_buffer[i];
                if (vehicle.m_flags == (Vehicle.Flags)0 || vehicle.Info == null)
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