using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ICities;
using Transit.Addon.RoadExtensions.PublicTransport.Rail1L;
using Transit.Addon.RoadExtensions.PublicTransport.Rail1LStation;
using Transit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OneWayTrainTrack
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

            var path = Util.AssemblyDirectory;
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
                Container = new GameObject("OneWayTrainTrack").AddComponent<Initializer>();
            }

            Initializer.tracks = new List<KeyValuePair<NetInfo, NetInfoVersion>>();
            Initializer.stationTracks = new List<KeyValuePair<NetInfo, NetInfoVersion>>();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (Initializer.tracks == null || Initializer.stationTracks == null)
            {
                return;
            }
            var railInfos = Resources.FindObjectsOfTypeAll<NetInfo>();
            foreach (var ri in railInfos.Where(ri => ri?.m_netAI is TrainTrackBaseAI))
            {
                if (Initializer.tracks.Select(p => p.Key).Contains(ri) || Initializer.stationTracks.Select(p => p.Key).Contains(ri))
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

            var trackBuilder = new Rail1LBuilder();
            foreach (var pair in Initializer.tracks)
            {
                trackBuilder.LateBuildUp(pair.Key, pair.Value);
            }
            Initializer.tracks = null;
            var stationTrackBuilder = new Rail1LStationBuilder();;
            foreach (var pair in Initializer.stationTracks)
            {
                stationTrackBuilder.LateBuildUp(pair.Key, pair.Value);
            }
            Initializer.stationTracks = null;
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
    }
}