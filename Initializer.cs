using System;
using UnityEngine;

namespace MetroOverhaul
{
    public class Initializer : AbstractInitializer
    {
        protected override void InitializeImpl()
        {
            CreatePrefab("Metro Track Ground", "Train Track", SetupMetroTrack().Chain(p =>
            {
                p.GetComponent<TrainTrackAI>().m_tunnelInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                CreatePrefab("Metro Track Slope", "Train Track Slope", SetupMetroTrack().Chain(p1 => p.GetComponent<TrainTrackAI>().m_slopeInfo = p1));
                CreatePrefab("Metro Track Bridge", "Train Track Bridge", SetupMetroTrack().Chain(p2 => p.GetComponent<TrainTrackAI>().m_bridgeInfo = p2));
                CreatePrefab("Metro Track Elevated", "Train Track Elevated", SetupMetroTrack().Chain(p3 => p.GetComponent<TrainTrackAI>().m_elevatedInfo = p3));
                //CreatePrefab("Metro Track Tunnel", "Train Track Tunnel", SetupMetroTrack().Chain(p4 => p.GetComponent<TrainTrackAI>().m_tunnelInfo = p4));
            }));
        }

        private static Action<NetInfo> SetupMetroTrack()
        {
            return (prefab) =>
            {
                var stationAI = prefab.GetComponent<TrainTrackBaseAI>();

                stationAI.m_createPassMilestone = LoadingExtension.milestone;

                prefab.m_class = ScriptableObject.CreateInstance<ItemClass>();
                prefab.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;
                prefab.m_class.m_service = ItemClass.Service.PublicTransport;
                prefab.m_class.m_level = ItemClass.Level.Level1;
                prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
                prefab.m_class.hideFlags = HideFlags.None;
                prefab.m_class.name = prefab.name;

                var track = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                prefab.m_UnlockMilestone = track.m_UnlockMilestone;

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
