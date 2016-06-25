using System;
using UnityEngine;

namespace MetroOverhaul
{
    public class Initializer : AbstractInitializer
    {
        protected override void InitializeImpl()
        {
            var mTunnelInfo = FindOriginalPrefab("Metro Track");
            CreatePrefab("Metro Track Ground", "Train Track", SetupMetroTrack().Apply(mTunnelInfo).Chain(p =>
            {
                p.GetComponent<TrainTrackAI>().m_tunnelInfo = mTunnelInfo;
                p.m_InfoTooltipAtlas = mTunnelInfo.m_InfoTooltipAtlas;
                p.m_InfoTooltipThumbnail = mTunnelInfo.m_InfoTooltipThumbnail;
                p.m_Thumbnail = mTunnelInfo.m_Thumbnail;
                CreatePrefab("Metro Track Slope", "Train Track Slope", SetupMetroTrack().Apply(mTunnelInfo).Chain(p1 => p.GetComponent<TrainTrackAI>().m_slopeInfo = p1));
                CreatePrefab("Metro Track Bridge", "Train Track Bridge", SetupMetroTrack().Apply(mTunnelInfo).Chain(p2 => p.GetComponent<TrainTrackAI>().m_bridgeInfo = p2));
                CreatePrefab("Metro Track Elevated", "Train Track Elevated", SetupMetroTrack().Apply(mTunnelInfo).Chain(p3 => p.GetComponent<TrainTrackAI>().m_elevatedInfo = p3));
            }));
        }

        private static Action<NetInfo, NetInfo> SetupMetroTrack()
        {
            return (prefab, metroTunnel) =>
            {
                var stationAI = prefab.GetComponent<TrainTrackBaseAI>();

                stationAI.m_createPassMilestone = LoadingExtension.milestone;

                prefab.m_class = ScriptableObject.CreateInstance<ItemClass>();
                prefab.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;
                prefab.m_class.m_service = ItemClass.Service.PublicTransport;
                prefab.m_class.m_level = ItemClass.Level.Level1;
                prefab.m_class.m_layer = ItemClass.Layer.Default;
                prefab.m_class.hideFlags = HideFlags.None;
                prefab.m_class.name = prefab.name;
                prefab.m_maxBuildAngle = metroTunnel.m_maxBuildAngle;
                prefab.m_maxBuildAngleCos = metroTunnel.m_maxBuildAngleCos;
                prefab.m_maxTurnAngle = metroTunnel.m_maxTurnAngle;
                prefab.m_maxTurnAngleCos = metroTunnel.m_maxTurnAngleCos;
                prefab.m_averageVehicleLaneSpeed = metroTunnel.m_averageVehicleLaneSpeed;
                prefab.m_UnlockMilestone = metroTunnel.m_UnlockMilestone;

                foreach (var VARIABLE in prefab.m_lanes)
                {
                    if (VARIABLE.m_vehicleType != VehicleInfo.VehicleType.None)
                    {
                        VARIABLE.m_vehicleType = VehicleInfo.VehicleType.Metro;
                        VARIABLE.m_stopType = VehicleInfo.VehicleType.Metro;
                    }
                }
            };
        }
    }
}
