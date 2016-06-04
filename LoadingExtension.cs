using System.Collections.Generic;
using System.ComponentModel;
using ICities;
using Transit.Addon.RoadExtensions.PublicTransport.Rail1L;
using Transit.Addon.RoadExtensions.PublicTransport.Rail1LStation;
using Transit.Framework;
using UnityEngine;

namespace OneWayTrainTrack
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

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