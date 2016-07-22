using SingleTrainTrack.Common;
using SingleTrainTrack.NEXT;

namespace SingleTrainTrack.Rail1L.Rail1L1W
{
    class Rail1LBuilder : Rail1LBuilderBase
    {
        public int Order { get { return 3; } }
        public int UIOrder { get { return 3; } }

        public string BasedPrefabName { get { return Mod.TRAIN_TRACK; } }
        public string Name { get { return "Rail1L"; } }
        public string DisplayName { get { return "Single One-Way Rail"; } }
        public string Description { get { return "A single one-way rail track that can be connected to conventional rail."; } }
        public string ShortDescription { get { return "Single Rail Track"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"Textures\Rail1L\Rail1L1W\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Textures\Rail1L\infotooltip.png"; } }

        public override void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup6mMesh(version);
            base.BuildUp(info, version);
        }

    }
}
