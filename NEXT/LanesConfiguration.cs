
namespace MetroOverhaul.NEXT
{
    public class LanesConfiguration
    {
        public int VehicleLanesToAdd { get; set; }
        public int PedestrianLanesToAdd { get; set; }
        public float PedLaneOffset { get; set; }
        public float? PedPropOffsetX { get; set; }
        public float? SpeedLimit { get; set; }
        public float VehicleLaneWidth { get; set; }
        public float PedestrianLaneWidth { get; set; }
        public float LanePositionOffst { get; set; }
        public bool IsTwoWay { get; set; }
        public bool HasBusStop { get; set; }
        public float BusStopOffset { get; set; }
        public CenterLaneType CenterLane { get; set; }
        public float CenterLaneWidth { get; set; }
        public float CenterLanePosition { get; set; }
        public LanesLayoutStyle LayoutStyle { get; set; }

        public LanesConfiguration()
        {
            VehicleLanesToAdd = 0;
            PedestrianLanesToAdd = 0;
            PedLaneOffset = 0.0f;
            PedPropOffsetX = null;
            SpeedLimit = null;
            VehicleLaneWidth = 3.0f;
            PedestrianLaneWidth = -1;
            LanePositionOffst = 0;
            IsTwoWay = true;
            HasBusStop = true;
            BusStopOffset = 1.5f;
            CenterLane = CenterLaneType.None;
            CenterLaneWidth = 3.0f;
            CenterLanePosition = 0;
            LayoutStyle = LanesLayoutStyle.Symmetrical;
        }
    }
}
