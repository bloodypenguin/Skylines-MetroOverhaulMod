using SingleTrainTrack.NEXT;
using System.Linq;

namespace SingleTrainTrack.Rail1LStation.Rail1L2SidedStation
{
    public class Rail1L2SidedStationBuilder : Rail1LStationBuilderBase
    {
        public int Order { get { return 2; } }
        public int UIOrder { get { return 2; } }

        public string BasedPrefabName { get { return Mod.TRAIN_STATION_TRACK; } }
        public string Name { get { return "Rail1L2SidedStation"; } }
        public string DisplayName { get { return "Single Rail Station Track: 2 Sided Boarding"; } }
        public string Description { get { return "Single Rail Station Track. Cims can enter from either side of the track."; } }
        public string ShortDescription { get { return "Single Rail Station Track"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"Textures\Rail1LStation\Rail1L2SidedStation\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Textures\Rail1LStation\infotooltip.png"; } }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
            base.BuildUp(info, version);
            var pedLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();

            for (int i = 0; i < pedLanes.Count; i++)
            {
                pedLanes[i].m_position = (((i - 1) * 2) + 1) * 4;
            }
        }      
    }
}
