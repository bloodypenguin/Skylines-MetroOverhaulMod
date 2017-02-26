using SingleTrainTrack.Common;
using SingleTrainTrack.NEXT;
using System.Linq;

namespace SingleTrainTrack.Rail1L.Rail1L2W
{
    class Rail1L2WBuilder : Rail1LBuilderBase
    {
        public int Order { get { return 4; } }
        public int UIOrder { get { return 4; } }

        public string BasedPrefabName { get { return Mod.TRAIN_TRACK; } }
        public string Name { get { return "Rail1L2W"; } }
        public string DisplayName { get { return "Single Two-Way Rail"; } }
        public string Description { get { return "A single two-way rail track that can be connected to conventional rail."; } }
        public string ShortDescription { get { return "Single Rail Track 2 Way"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"Textures\Rail1L\Rail1L2W\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Textures\Rail1L\infotooltip.png"; } }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup6mMesh(version);
            base.BuildUp(info, version);

            var railLane = info.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Vehicle);
            railLane.m_direction = NetInfo.Direction.AvoidBackward | NetInfo.Direction.AvoidForward;
        }
    }
}
