using System.Linq;
using SingleTrainTrack.NEXT;

namespace SingleTrainTrack.Rail1LStation.Rail1L1SidedStation
{
    public class Rail1LStationBuilder : Rail1LStationBuilderBase
    {
        public int Order { get { return 1; } }
        public int UIOrder { get { return 1; } }

        public string BasedPrefabName { get { return Mod.TRAIN_STATION_TRACK; } }
        public string Name { get { return "Rail1LStation"; } }
        public string DisplayName { get { return "Single Rail Station Track: 1 Sided Boarding"; } }
        public string Description { get { return "Single Rail Station Track. Cims can enter from the left side in perspective to the direction the track is drawn."; } }
        public string ShortDescription { get { return "Single Rail Station Track"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"Textures\Rail1LStation\Rail1L1SidedStation\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Textures\Rail1LStation\infotooltip.png"; } }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
            base.BuildUp(info, version);
            var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();

            for (int i = 0; i < pedLanes.Count; i++)
            {
                pedLanes[i].m_position = -4;
            }
        }
    }
}
