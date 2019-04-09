using MetroOverhaul.Extensions;


namespace MetroOverhaul.NEXT.Extensions {
    public static partial class BuildingInfoExtensions {
        public static void SetMetroStyle(this BuildingInfo.PathInfo info, TrackStyle style)
        {
            if (info.m_netInfo.IsAbovegroundMetroStationTrack())
            {
                var trackName = info.m_netInfo.name;
                if (style == TrackStyle.Modern)
                {
                    if (trackName.ToLower().StartsWith("steel"))
                    {
                        trackName = trackName.Substring(6);
                    }

                    info.AssignNetInfo(trackName);
                }
                else if (style == TrackStyle.Classic)
                {
                    if (trackName.ToLower().StartsWith("steel") == false)
                    {
                        trackName = "Steel " + trackName;
                    }

                    info.AssignNetInfo(trackName);
                }
            }
        }
    }
}

