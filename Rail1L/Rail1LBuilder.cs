using System;
using System.Linq;
using SingleTrainTrack.Common;
using SingleTrainTrack.NEXT;
using SingleTrainTrack.NEXT.Extensions;
using UnityEngine;

namespace SingleTrainTrack.Rail1L
{
    public abstract partial class Rail1LBuilder
    {
        public NetInfoVersion SupportedVersions
        {
            get { return NetInfoVersion.All; }
        }

        public virtual void BuildUp(NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
            var railVersionName = SharedHelpers.NameBuilder(SharedHelpers.TRAIN_TRACK, version);
            var railInfo = Prefabs.Find<NetInfo>(railVersionName);
            info.m_class = railInfo.m_class.Clone("APT" + railVersionName);

            ///////////////////////////
            // Texturing             //
            ///////////////////////////
            SetupTextures(info, version);

            ///////////////////////////
            // Set up                //
            ///////////////////////////
            info.m_hasParkingSpaces = false;
            //info.m_class = roadInfo.m_class.Clone(NetInfoClasses.NEXT_SMALL3L_ROAD);
            if (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel)
            {
                info.m_halfWidth = 4;
                info.m_pavementWidth = 2;
            }
            else
            {
                info.m_halfWidth = 3;
            }
            
            //if (version == NetInfoVersion.Tunnel)
            //{
            //    info.m_setVehicleFlags = Vehicle.Flags.Transition;
            //    info.m_setCitizenFlags = CitizenInstance.Flags.Transition;
            //}

            info.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = false,
                LanesToAdd = -1,
            });

            info.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
            info.m_nodeConnectGroups = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.NarrowTram;
            var owPlayerNetAI = railInfo.GetComponent<PlayerNetAI>();
            var playerNetAI = info.GetComponent<PlayerNetAI>();
            if (owPlayerNetAI != null && playerNetAI != null)
            {
                playerNetAI.m_constructionCost = owPlayerNetAI.m_constructionCost * 1 / 2; 
                playerNetAI.m_maintenanceCost = owPlayerNetAI.m_maintenanceCost * 1 / 2; 
            }

            var trainTrackAI = info.GetComponent<TrainTrackAI>();

            if (trainTrackAI != null)
            {

            }
        }

        public void LateBuildUp(NetInfo info, NetInfoVersion version)
        {
            var plPropInfo = PrefabCollection<PropInfo>.FindLoaded($"{Util.PackageName("Rail1LPowerLine")}.Rail1LPowerLine_Data");
            if (plPropInfo == null)
            {
                throw new Exception($"{info.name}: Rail1LPowerLine prop not found!");
            }
            var oldPlPropInfo = Prefabs.Find<PropInfo>("RailwayPowerline");
            NetInfoExtensions.ReplaceProps(info, plPropInfo, oldPlPropInfo);
            for (int i = 0; i < info.m_lanes.Count(); i++)
            {
                var powerLineProp = info.m_lanes[i].m_laneProps.m_props.Where(p => p.m_prop == plPropInfo).ToList();
                for (int j = 0; j < powerLineProp.Count(); j++)
                {
                    powerLineProp[j].m_position = new Vector3(2.4f, -0.15f, 0);
                    powerLineProp[j].m_angle = 180;
                }
            }

            if (version == NetInfoVersion.Elevated)
            {
                var epPropInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Rail1LElevatedPillar")}.Rail1LElevatedPillar_Data");

                if (epPropInfo == null)
                {
                    throw new Exception($"{info.name}: Rail1LElevatedPillar prop not found!");
                }

                if (epPropInfo != null)
                {
                    var bridgeAI = info.GetComponent<TrainTrackBridgeAI>();
                    if (bridgeAI != null)
                    {
                        bridgeAI.m_doubleLength = false;
                        bridgeAI.m_bridgePillarInfo = epPropInfo;
                        bridgeAI.m_bridgePillarOffset = 1;
                        bridgeAI.m_middlePillarInfo = null;
                    }
                }
            }
            else if (version == NetInfoVersion.Bridge)
            {
                var bpPropInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Rail1LBridgePillar")}.Rail1LBridgePillar_Data");

                if (bpPropInfo == null)
                {
                    throw new Exception($"{info.name}: Rail1LBridgePillar prop not found!");
                }

                if (bpPropInfo != null)
                {
                    var bridgeAI = info.GetComponent<TrainTrackBridgeAI>();
                    if (bridgeAI != null)
                    {
                        bridgeAI.m_bridgePillarInfo = bpPropInfo;
                    }
                }
            }
        }
    }
}
