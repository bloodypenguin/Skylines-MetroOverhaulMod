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
                p.GetComponent<TrainTrackAI>().m_connectedElevatedInfo = null;
                p.GetComponent<TrainTrackAI>().m_connectedInfo = null;
            }));
            CreatePrefab("Metro Station Track Ground", "Train Station Track", SetupMetroTrack().Apply(mTunnelInfo));
            CreatePrefab("Metro Station Track Elevated", "Train Station Track", SetupMetroTrack().Apply(mTunnelInfo).Chain(SetupElevatedStationTrack()));

        }

        public static Action<NetInfo> SetupElevatedStationTrack()
        {
            return elevatedPrefab =>
            {
                var trackAi = elevatedPrefab.GetComponent<TrainTrackAI>();
                trackAi.m_elevatedInfo = elevatedPrefab;

                elevatedPrefab.m_followTerrain = false;
                elevatedPrefab.m_flattenTerrain = false;
                elevatedPrefab.m_createGravel = false;
                elevatedPrefab.m_createPavement = false;
                elevatedPrefab.m_createRuining = false;
                elevatedPrefab.m_requireSurfaceMaps = false;
                elevatedPrefab.m_clipTerrain = false;
                elevatedPrefab.m_snapBuildingNodes = false;
                elevatedPrefab.m_placementStyle = ItemClass.Placement.Procedural;
                elevatedPrefab.m_useFixedHeight = true;
                elevatedPrefab.m_lowerTerrain = true;
                elevatedPrefab.m_availableIn = ItemClass.Availability.GameAndAsset;
            };
        }

        private static Action<NetInfo, NetInfo> SetupMetroTrack()
        {
            return (prefab, metroTunnel) =>
            {
                var milestone = metroTunnel.GetComponent<MetroTrackAI>().m_createPassMilestone;
                metroTunnel.m_placementStyle = ItemClass.Placement.Procedural;
                PrefabCollection<VehicleInfo>.FindLoaded("Metro").m_class =
                    ScriptableObject.CreateInstance<ItemClass>();

                prefab.GetComponent<TrainTrackBaseAI>().m_createPassMilestone = milestone;
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

                foreach (var lane in prefab.m_lanes)
                {
                    if (lane.m_vehicleType == VehicleInfo.VehicleType.None)
                    {
                        lane.m_stopType = VehicleInfo.VehicleType.Metro;
                    }
                    else
                    {
                        lane.m_vehicleType = VehicleInfo.VehicleType.Metro;
                    }
                }

                Modifiers.RemoveElectricityPoles(prefab);
            };
        }
    }
}
