using System.Linq;
using UnityEngine;

namespace OneWayTrainTrack
{
    public class Initializer : AbstractInitializer
    {
        protected override void InitializeImpl()
        {
            var oneWay = CreatePrefab("Oneway Train Track", "Train Track", SetupOneWayPrefab);
            var oneWayTunnel = CreatePrefab("Oneway Train Track Tunnel", "Train Track Tunnel", SetupOneWayPrefab);
            var oneWayBridge = CreatePrefab("Oneway Train Track Bridge", "Train Track Bridge", SetupOneWayPrefab);
            var oneWayElevated = CreatePrefab("Oneway Train Track Elevated", "Train Track Elevated", SetupOneWayPrefab);
            var oneWaySlope = CreatePrefab("Oneway Train Track Slope", "Train Track Slope", SetupOneWayPrefab);
            if (oneWay != null)
            {
                var ai = oneWay.GetComponent<TrainTrackAI>();
                ai.m_tunnelInfo = oneWayTunnel;
                ai.m_bridgeInfo = oneWayBridge;
                ai.m_elevatedInfo = oneWayElevated;
                ai.m_slopeInfo = oneWaySlope;
            }
            else
            {
                UnityEngine.Debug.LogError("OneWayTrainTrack - failed to create one way track!");
            }
        }

        private static void SetupOneWayPrefab(NetInfo newPrefab)
        {
            foreach (var lane in newPrefab.m_lanes)
            {
                if (lane.m_direction == NetInfo.Direction.Backward)
                {
                    lane.m_direction = NetInfo.Direction.None;
                }
            }
            newPrefab.m_hasBackwardVehicleLanes = false;
        }
    }
}
