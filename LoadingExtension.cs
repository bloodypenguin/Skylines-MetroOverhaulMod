using ColossalFramework;
using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Redirection;
using MetroOverhaul.NEXT;
using System;
using System.Collections.Generic;
using Harmony;
using ColossalFramework.UI;
using MetroOverhaul.OptionsFramework;
using MetroOverhaul.UI;
using UnityEngine;
using MetroOverhaul.Extensions;
using System.Reflection;
using System.Linq;

namespace MetroOverhaul
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;
        private static AssetsUpdater _updater;
        private HarmonyInstance _harmony;
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

            _updater = null;
            LateBuildUpQueue.Clear();
            InstallAssets();
            if (Container == null)
            {
                Container = new GameObject("MetroOverhaul").AddComponent<Initializer>();
                Container.AppMode = loading.currentMode;
            }
            _harmony = HarmonyInstance.Create("andreharv.Skylines-MetroOverhaulMod");
            if (OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
            {
                PatchInitializePrefab.Apply(_harmony, ref Container);
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
                    var gameObject = new GameObject("MetroOverhaulUISetup");
                    gameObject.AddComponent<UpgradeSetup>();
                    //#if DEBUG
                    //                gameObject.AddComponent<StyleSelectionStationUI>();
                    //#else
                    //                gameObject.AddComponent<StyleSelectionUI>();
                    //#endif

                    if (OptionsWrapper<Options>.Options.metroUi)
                    {
                        var metroStationCustomizerUI = (MetroStationCustomizerUI)UIView.GetAView().AddUIComponent(typeof(MetroStationCustomizerUI));
                        UIView.GetAView().AddUIComponent(typeof(MetroTrackCustomizerUI));
                        UIView.GetAView().AddUIComponent(typeof(MetroAboveGroundStationCustomizerUI));
                        PatchCreateBuilding.Apply(_harmony, ref metroStationCustomizerUI);
                    }
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
            }
            GameObject.Destroy(UIView.GetAView().FindUIComponent<MetroStationCustomizerUI>("MetroStationCustomizerUI"));
            GameObject.Destroy(UIView.GetAView().FindUIComponent<MetroTrackCustomizerUI>("MetroTrackCustomizerUI"));
            GameObject.Destroy(UIView.GetAView().FindUIComponent<MetroAboveGroundStationCustomizerUI>("MetroAboveGroundStationCustomizerUI"));
        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
                PatchInitializePrefab.Revert(_harmony);
            PatchCreateBuilding.Revert(_harmony);
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
                Redirector<MetroTrainAIDetour>.Revert();
                Redirector<PassengerTrainAIDetour>.Revert();
            }
        }
    }
}