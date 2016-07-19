using SingleTrainTrack;
using SingleTrainTrack.NEXT;
using SingleTrainTrack.NEXT.Extensions;
using SingleTrainTrack.Common;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DoubleTrainTrack.Rail2L1W
{
    public partial class Rail2L1WBuilder
    {
        public int Order { get { return 7; } }
        public int UIOrder { get { return 9; } }

        public string BasedPrefabName { get { return SharedHelpers.TRAIN_TRACK; } }
        public string Name { get { return "Oneway Train Track"; } }
        public string DisplayName { get { return "Two Lane One-Way Rail"; } }
        public string Description { get { return "Dual one way rail tracks that can be connected to conventional rail."; } }
        public string ShortDescription { get { return "Double Rail Track"; } }
        public string UICategory { get { return "PublicTransportTrain"; } }

        public string ThumbnailsPath { get { return @"Textures\Rail2L1W\thumbnails.png"; } }
        public string InfoTooltipPath { get { return @"Textures\Rail2L1W\infotooltip.png"; } }

        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var railVersionName = SharedHelpers.NameBuilder(SharedHelpers.TRAIN_TRACK, version);
            var railInfo = Prefabs.Find<NetInfo>(railVersionName);

            ///////////////////////////
            // 3DModeling            //
            ///////////////////////////
            info.Setup10mMesh(version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_class = railInfo.m_class.Clone("APT" + railVersionName);
            info.m_halfWidth = 4.999f;

            //if (version == NetInfoVersion.Tunnel)
            //{
            //    info.m_setVehicleFlags = Vehicle.Flags.Transition;
            //    info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
            //}

            var lanes = new List<NetInfo.Lane>();
            lanes.AddRange(info.m_lanes.ToList());
            for (int i = 0; i < lanes.Count; i++)
            {
                if (lanes[i].m_direction == NetInfo.Direction.Backward)
                {
                    lanes[i].m_direction = NetInfo.Direction.Forward;
                }
            }
            info.m_lanes = lanes.ToArray();
            info.m_connectGroup = NetInfo.ConnectGroup.WideTram;
            info.m_nodeConnectGroups = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.NarrowTram;
            //var railInfos = new List<NetInfo>();
            //railInfos.Add(railInfo);
            //railInfos.Add(Prefabs.Find<NetInfo>(NetInfos.Vanilla.TRAIN_STATION_TRACK, false));
            //railInfos.Add(Prefabs.Find<NetInfo>("Train Cargo Track", false));
            //for (int i = 0; i < railInfos.Count; i++)
            //{
            //    var ri = railInfos[i];
            //    //info.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)9; 
            //    ri.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
            //    railInfo.m_nodeConnectGroups = NetInfo.ConnectGroup.NarrowTram;
            //    if (railInfo.m_nodes.Length > 1)
            //    {
            //        railInfo.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
            //    }

            //}

        }
    }
}