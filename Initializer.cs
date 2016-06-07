using System;
using System.Collections.Generic;
using ColossalFramework;
using SingleTrainTrack.Meshes;
using Transit.Framework;
using Rail1LBuilder = SingleTrainTrack.Rail1L.Rail1LBuilder;
using Rail1LStationBuilder = SingleTrainTrack.Rail1LStation.Rail1LStationBuilder;

namespace SingleTrainTrack
{
    public class Initializer : AbstractInitializer
    {

        public static List<KeyValuePair<NetInfo, NetInfoVersion>> tracks;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> stationTracks;

        protected override void InitializeImpl()
        {
            InitializeByBuilder(new Rail1LBuilder(), tracks);
            InitializeByBuilder(new Rail1LStationBuilder(), tracks);
        }

        private void InitializeByBuilder(object trackBuilder, List<KeyValuePair<NetInfo, NetInfoVersion>> tracks)
        {
            TrainTrackAI mainAi = null;
            foreach (
                var version in
                    new[]
                    {
                        NetInfoVersion.Ground, NetInfoVersion.Elevated, NetInfoVersion.Bridge, NetInfoVersion.Slope,
                        NetInfoVersion.Tunnel
                    })
            {
                if (!(trackBuilder.GetPropery<NetInfoVersion>("SupportedVersions").IsFlagSet(version) || version == NetInfoVersion.Ground))
                {
                    continue;
                }
                var versionString = version == NetInfoVersion.Ground ? string.Empty : $" {version}";
                var newPrefabName = $"{trackBuilder.GetPropery<string>("Name")}{versionString}";
                var originalPrefabName = $"{trackBuilder.GetPropery<string>("BasedPrefabName")}{versionString}";

                UnityEngine.Debug.Log($"{originalPrefabName}=>{newPrefabName}");
                Action<NetInfo> action;
                switch (version)
                {
                    case NetInfoVersion.Ground:
                        action = arg => mainAi = arg.GetComponent<TrainTrackAI>();
                        break;
                    case NetInfoVersion.Elevated:
                        action = arg => mainAi.m_elevatedInfo = arg;
                        break;
                    case NetInfoVersion.Bridge:
                        action = arg => mainAi.m_bridgeInfo = arg;
                        break;
                    case NetInfoVersion.Tunnel:
                        action = arg => mainAi.m_tunnelInfo = arg;
                        break;
                    case NetInfoVersion.Slope:
                        action = arg => mainAi.m_slopeInfo = arg;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                CreatePrefab(newPrefabName, originalPrefabName,
                    SetupOneWayPrefabAction().Apply(trackBuilder).Apply(version)
                        .Chain(action)
                        .Chain(AddPairAction().Apply(version).Apply(tracks)));
                Util.AddLocale("NET", trackBuilder.GetPropery<string>("Name"), trackBuilder.GetPropery<string>("DisplayName"), trackBuilder.GetPropery<string>("Description"));

            }
        }

        private static Action<NetInfo, object, NetInfoVersion> SetupOneWayPrefabAction()
        {
            return (newPrefab, builder, version) =>
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
            };
        }

        private static Action<NetInfo, NetInfoVersion, List<KeyValuePair<NetInfo, NetInfoVersion>>> AddPairAction()
        {
            return (newPrefab, version, list) => { list.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version)); };
        }
    }
}
