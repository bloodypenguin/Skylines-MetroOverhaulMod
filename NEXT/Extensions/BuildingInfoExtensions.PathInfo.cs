using MetroOverhaul.Extensions;


namespace MetroOverhaul.NEXT.Extensions
{
    public static partial class BuildingInfoExtensions
    {
        public static void SetMetroStyle(this BuildingInfo.PathInfo info, NetInfoTrackStyle style)
        {
            if (info.m_finalNetInfo.IsAbovegroundMetroStationTrack())
            {
                var trackName = info.m_finalNetInfo.name;
                if (style == NetInfoTrackStyle.Vanilla)
                {
                    if (info.m_finalNetInfo.IsGroundSidePlatformMetroStationTrack() || info.m_finalNetInfo.IsElevatedSidePlatformMetroStationTrack())
                    {
                        trackName = trackName.Substring(trackName.IndexOf("Metro")).Replace(" Ground", " Ground 01").Replace(" Tunnel", "");
                        info.AssignNetInfo(trackName, false);
                    }
                }
                else if (style == NetInfoTrackStyle.Modern)
                {
                    trackName = trackName.Substring(trackName.IndexOf("Metro")).Replace(" Ground 01", " Ground");
                    if (trackName.EndsWith("Track"))
                    {
                        trackName += " Tunnel";
                    }
                    info.AssignNetInfo(trackName, false);
                }
                else if (style == NetInfoTrackStyle.Classic)
                {
                    trackName = "Steel " + trackName.Substring(trackName.IndexOf("Metro")).Replace(" Ground 01", " Ground");
                    if (trackName.EndsWith("Track"))
                    {
                        trackName += " Tunnel";
                    }
                    info.AssignNetInfo(trackName, false);
                }
            }
        }
    }
}

