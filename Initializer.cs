using System;
using System.Collections.Generic;
using System.Linq;
using Transit.Addon.RoadExtensions.PublicTransport.Rail1L;
using Transit.Addon.RoadExtensions.PublicTransport.Rail1LStation;
using Transit.Addon.RoadExtensions.PublicTransport.RailUtils;
using Transit.Framework;

namespace OneWayTrainTrack
{
    public class Initializer : AbstractInitializer
    {

        public static List<KeyValuePair<NetInfo, NetInfoVersion>> tracks;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> stationTracks;

        protected override void InitializeImpl()
        {
            var trackBuilder = new Rail1LBuilder();
            CreatePrefab(trackBuilder.Name, trackBuilder.BasedPrefabName, new Action<NetInfo, Rail1LBuilder, NetInfoVersion>(SetupOneWayPrefabAction)
                    .Apply(trackBuilder, NetInfoVersion.Ground).Chain(arg =>
            {
                var ai = arg.GetComponent<TrainTrackAI>();
                CreatePrefab($"{trackBuilder.Name} Tunnel", $"{trackBuilder.Name} Tunnel",
                    new Action<NetInfo, Rail1LBuilder, NetInfoVersion>(SetupOneWayPrefabAction)
                    .Apply(trackBuilder, NetInfoVersion.Tunnel)
                    .Chain(arg1 => ai.m_tunnelInfo = arg1));
                CreatePrefab($"{trackBuilder.Name} Bridge", $"{trackBuilder.Name} Bridge", new Action<NetInfo, Rail1LBuilder, NetInfoVersion>(SetupOneWayPrefabAction)
                    .Apply(trackBuilder, NetInfoVersion.Bridge)
                    .Chain(arg2 => ai.m_bridgeInfo = arg));
                CreatePrefab($"{trackBuilder.Name} Elevated", $"{trackBuilder.Name} Elevated", new Action<NetInfo, Rail1LBuilder, NetInfoVersion>(SetupOneWayPrefabAction)
                    .Apply(trackBuilder, NetInfoVersion.Elevated)
                    .Chain(arg3 => ai.m_elevatedInfo = arg));
                CreatePrefab($"{trackBuilder.Name} Slope", $"{trackBuilder.Name} Slope", new Action<NetInfo, Rail1LBuilder, NetInfoVersion>(SetupOneWayPrefabAction)
                    .Apply(trackBuilder, NetInfoVersion.Slope)
                    .Chain(arg4 => ai.m_slopeInfo = arg4));
            }));

            //TODO(earalov): init station track
        }

        private static void SetupOneWayPrefabAction(NetInfo newPrefab, object builder, NetInfoVersion version)
        {
            var buildUp = builder.GetType().GetMethod("BuildUp");
            buildUp.Invoke(builder, new object[] {newPrefab, version});
            newPrefab.Setup10mMesh(version);
            if (builder is Rail1LBuilder)
            {
                tracks.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
            }
            if (builder is Rail1LStationBuilder)
            {
                stationTracks.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
            }
        }
    }
}
