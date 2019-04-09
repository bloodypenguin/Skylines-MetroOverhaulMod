namespace MetroOverhaul.Extensions {
    public static partial class BuildingInfoExtensions
    {
        public static void AssignNetInfo(this BuildingInfo.PathInfo path, string netInfoName)
        {
            NetInfo netInfo = PrefabCollection<NetInfo>.FindLoaded(netInfoName);
            path.AssignNetInfo(netInfo);
        }
        public static void AssignNetInfo(this BuildingInfo.PathInfo path, NetInfo netInfo)
        {
            path.m_netInfo = netInfo;
            path.m_finalNetInfo = netInfo;
        }
    }
}
