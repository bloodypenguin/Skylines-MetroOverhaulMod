using System;
using System.Linq;

namespace OneWayTrainTrack
{
    public class Initializer : AbstractInitializer
    {
        protected override void InitializeImpl()
        {
            CreatePrefab("Oneway Train Track", "Train Track", SetupOneWayPrefabAction().Chain(arg =>
            {
                var ai = arg.GetComponent<TrainTrackAI>();
                CreatePrefab("Oneway Train Track Tunnel", "Train Track Tunnel", SetupOneWayPrefabAction().Chain(arg1 => ai.m_tunnelInfo = arg1));
                CreatePrefab("Oneway Train Track Bridge", "Train Track Bridge", SetupOneWayPrefabAction().Chain(arg2 => ai.m_bridgeInfo = arg));
                CreatePrefab("Oneway Train Track Elevated", "Train Track Elevated", SetupOneWayPrefabAction().Chain(arg3 => ai.m_elevatedInfo = arg));
                CreatePrefab("Oneway Train Track Slope", "Train Track Slope", SetupOneWayPrefabAction().Chain(arg4 => ai.m_slopeInfo = arg4));
            }));
        }

        private static Action<NetInfo> SetupOneWayPrefabAction()
        {
            return newPrefab =>
            {
                foreach (var lane in newPrefab.m_lanes.Where(lane => lane.m_direction == NetInfo.Direction.Backward))
                {
                    lane.m_direction = NetInfo.Direction.None;
                }
                newPrefab.m_hasBackwardVehicleLanes = false;
            };
        }
    }
}
