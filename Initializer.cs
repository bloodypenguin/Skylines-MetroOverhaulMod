using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using SingleTrainTrack.NEXT;
using SingleTrainTrack.NEXT.Extensions;
using Rail1L1WBuilder = SingleTrainTrack.Rail1L.Rail1L1W.Rail1L1WBuilder;
using Rail1L2WBuilder = SingleTrainTrack.Rail1L.Rail1L2W.Rail1L2WBuilder;
using Rail1L2SidedStationBuilder = SingleTrainTrack.Rail1LStation.Rail1L2SidedStation.Rail1L2SidedStationBuilder;
using Rail1L1SidedStationBuilder = SingleTrainTrack.Rail1LStation.Rail1L1SidedStation.Rail1L1SidedStationBuilder;
using Rail2L1WBuilder = DoubleTrainTrack.Rail2L1W.Rail2L1WBuilder;

namespace SingleTrainTrack
{
    public class Initializer : AbstractInitializer
    {

        public static List<KeyValuePair<NetInfo, NetInfoVersion>> Tracks1W;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> Tracks2W;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> Station2SidedTracks;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> Station1SidedTracks;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> Tracks2L1W;

        protected override void InitializeImpl()
        {
            InitializeByBuilder(new Rail1L1WBuilder(), Tracks1W);
            InitializeByBuilder(new Rail1L2WBuilder(), Tracks2W);
            InitializeByBuilder(new Rail1L2SidedStationBuilder(), Station2SidedTracks);
            InitializeByBuilder(new Rail1L1SidedStationBuilder(), Station1SidedTracks);
            InitializeByBuilder(new Rail2L1WBuilder(), Tracks2L1W);
        }

        private void InitializeByBuilder(object trackBuilder, List<KeyValuePair<NetInfo, NetInfoVersion>> tracks)
        {
            TrainTrackAI mainAi = null;
            new[]{
                NetInfoVersion.Ground, NetInfoVersion.Elevated, NetInfoVersion.Bridge, NetInfoVersion.Slope,
                NetInfoVersion.Tunnel
            }.ForEach(version =>
                {
                    ModifyExistingNetInfos.ModifyExistingIcons();
                    if (!(trackBuilder.GetPropery<NetInfoVersion>("SupportedVersions").IsFlagSet(version) || version == NetInfoVersion.Ground))
                    {
                        return;
                    }
                    var newPrefabName = SharedHelpers.NameBuilder(trackBuilder.GetPropery<string>("Name"), version);
                    var originalPrefabName =
                        SharedHelpers.NameBuilder(trackBuilder.GetPropery<string>("BasedPrefabName"), version);
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
                            .Chain(AddPairAction().Apply(version).Apply(tracks))
                            .Chain(SetupUIAction().Apply(trackBuilder))
                        );
                });
        }


        private static Action<NetInfo, object> SetupUIAction()
        {
            return (info, trackBuilder) =>
            {
                info.m_UIPriority = trackBuilder.GetPropery<int>("UIOrder");
                info.SetUICategory(trackBuilder.GetPropery<string>("UICategory"));

                if (!trackBuilder.GetPropery<string>("ThumbnailsPath").IsNullOrWhiteSpace())
                {
                    var thumbnails = AssetManager.instance.GetThumbnails(trackBuilder.GetPropery<string>("Name"), trackBuilder.GetPropery<string>("ThumbnailsPath"));
                    info.m_Atlas = thumbnails;
                    info.m_Thumbnail = thumbnails.name;
                }

                if (!trackBuilder.GetPropery<string>("InfoTooltipPath").IsNullOrWhiteSpace())
                {
                    var infoTips = AssetManager.instance.GetInfoTooltip(trackBuilder.GetPropery<string>("Name"), trackBuilder.GetPropery<string>("InfoTooltipPath"));
                    info.m_InfoTooltipAtlas = infoTips;
                    info.m_InfoTooltipThumbnail = infoTips.name;

                }
            };
        }


        private static Action<NetInfo, object, NetInfoVersion> SetupOneWayPrefabAction()
        {
            return (newPrefab, builder, version) =>
            {
                newPrefab.m_isCustomContent = true;
                var buildUp = builder.GetType().GetMethod("BuildUp");
                buildUp.Invoke(builder, new object[] { newPrefab, version });
                //newPrefab.Setup10mMesh(version);
                if (builder is Rail1L1WBuilder)
                {
                    Tracks1W.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
                if (builder is Rail1L2WBuilder)
                {
                    Tracks2W.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
                if (builder is Rail2L1WBuilder)
                {
                    Tracks2L1W.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
                if (builder is Rail1L2SidedStationBuilder)
                {
                    Station2SidedTracks.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
                if (builder is Rail1L1SidedStationBuilder)
                {
                    Station1SidedTracks.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
            };
        }

        private static Action<NetInfo, NetInfoVersion, List<KeyValuePair<NetInfo, NetInfoVersion>>> AddPairAction()
        {
            return (newPrefab, version, list) => { list.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version)); };
        }
    }
}
