using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ICities;
using Transit.Framework;
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
                Container = new GameObject("Rail1L").AddComponent<Initializer>();
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
            try
            {
                var trackBuilder = new Rail1LBuilder();
                foreach (var pair in Initializer.tracks)
                {
                    trackBuilder.LateBuildUp(pair.Key, pair.Value);
                }
                Initializer.tracks = null;
                var stationTrackBuilder = new Rail1LStationBuilder();
                ;
                foreach (var pair in Initializer.stationTracks)
                {
                    stationTrackBuilder.LateBuildUp(pair.Key, pair.Value);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}\nMake sure the required prop is installed and is enabled in Content Manager!");
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