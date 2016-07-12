using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using SingleTrainTrack.NEXT;
using SingleTrainTrack.NEXT.Extensions;
using Rail1LBuilder = SingleTrainTrack.Rail1L.Rail1LBuilder;
using Rail1LStationBuilder = SingleTrainTrack.Rail1LStation.Rail1LStationBuilder;
using Rail2LOWBuilder = DoubleTrainTrack.Rail2LOW.Rail2LOWBuilder;

namespace SingleTrainTrack
{
    public class Initializer : AbstractInitializer
    {

        public static List<KeyValuePair<NetInfo, NetInfoVersion>> Tracks;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> StationTracks;
        public static List<KeyValuePair<NetInfo, NetInfoVersion>> Tracks2Low;

        protected override void InitializeImpl()
        {
            InitializeByBuilder(new Rail1LBuilder(), Tracks);
            InitializeByBuilder(new Rail1LStationBuilder(), StationTracks);
            InitializeByBuilder(new Rail2LOWBuilder(), Tracks2Low);
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
                var buildUp = builder.GetType().GetMethod("BuildUp");
                buildUp.Invoke(builder, new object[] { newPrefab, version });
                //newPrefab.Setup10mMesh(version);
                if (builder is Rail1LBuilder)
                {
                    Tracks.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
                if (builder is Rail2LOWBuilder)
                {
                    Tracks2Low.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
                if (builder is Rail1LStationBuilder)
                {
                    StationTracks.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version));
                }
            };
        }

        private static Action<NetInfo, NetInfoVersion, List<KeyValuePair<NetInfo, NetInfoVersion>>> AddPairAction()
        {
            return (newPrefab, version, list) => { list.Add(new KeyValuePair<NetInfo, NetInfoVersion>(newPrefab, version)); };
        }
    }
}
