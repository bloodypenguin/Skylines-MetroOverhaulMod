using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using DoubleTrainTrack.Rail2LOW;
using ICities;
using SingleTrainTrack.NEXT;
using SingleTrainTrack.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Rail1LBuilder = SingleTrainTrack.Rail1L.Rail1LBuilder;
using Rail1LStationBuilder = SingleTrainTrack.Rail1LStation.Rail1LStationBuilder;

namespace SingleTrainTrack
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;
        public static bool Done { get; private set; } // Only one Assets installation throughout the application

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            InstallAssets();
            if (Container == null)
            {
                Container = new GameObject("Rail1L").AddComponent<Initializer>();
            }
            Initializer.Tracks = new List<KeyValuePair<NetInfo, NetInfoVersion>>();
            Initializer.Tracks2Low = new List<KeyValuePair<NetInfo, NetInfoVersion>>();
            Initializer.StationTracks = new List<KeyValuePair<NetInfo, NetInfoVersion>>();

            new object[]
            {
               new Rail2LOWBuilder(),
               new Rail1LBuilder(),
               new Rail1LStationBuilder()
            }.ForEach(trackBuilder =>
            {
                Util.AddLocale("NET", trackBuilder.GetPropery<string>("Name"), trackBuilder.GetPropery<string>("DisplayName"), trackBuilder.GetPropery<string>("Description"));
            });
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
            if (Initializer.Tracks == null || Initializer.StationTracks == null || Initializer.Tracks2Low == null)
            {
                return; //that assures that following code gets executed only on the first loading
            }
            var railInfos = Resources.FindObjectsOfTypeAll<NetInfo>();
            foreach (var ri in railInfos.Where(ri => ri?.m_netAI is TrainTrackBaseAI && ri.m_class.m_subService == ItemClass.SubService.PublicTransportTrain))
            {
                if (Initializer.Tracks.Select(p => p.Key).Contains(ri) ||
                    Initializer.StationTracks.Select(p => p.Key).Contains(ri) ||
                    Initializer.Tracks2Low.Select(p => p.Key).Contains(ri))
                {
                    continue;
                }
                ri.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                ri.m_nodeConnectGroups = NetInfo.ConnectGroup.NarrowTram;
                if (ri.m_nodes.Length > 1)
                {
                    ri.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                }
            }
            try
            {
                var trackBuilder = new Rail1LBuilder();
                foreach (var pair in Initializer.Tracks)
                {
                    if (pair.Key.m_halfWidth < 4)
                        trackBuilder.LateBuildUp(pair.Key, pair.Value);
                }
                var stationTrackBuilder = new Rail1LStationBuilder();
                foreach (var pair in Initializer.StationTracks)
                {
                    stationTrackBuilder.LateBuildUp(pair.Key, pair.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                Initializer.StationTracks = null;
                Initializer.Tracks2Low = null;
                Initializer.Tracks = null;
            }
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            var gameObject = new GameObject("SingleTrainTrackUISetup");
            gameObject.AddComponent<UpgradeSetup>();
            if (Util.IsModActive("One-Way Street Arrows"))
            {
                gameObject.AddComponent<ArrowsButtonSetup>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            var gameObject = GameObject.Find("SingleTrainTrackUISetup");
            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
            }

            if (Container == null)
            {
                return;
            }
            Object.Destroy(Container.gameObject);
            Container = null;
            ModifyExistingNetInfos.Reset();
        }
    }
}