namespace MetroOverhaul {
    public static class ModTrackNames {
        public const string TRAIN_TRACK = "Train Track";
        public const string TRAIN_SINGLE_TRACK = "Train Single Track";

        public const string TRAIN_STATION_TRACK_GROUND = "Train Station Track";
        public const string TRAIN_STATION_TRACK_GROUND_C = "Train Station Track (C)";
        public const string TRAIN_STATION_TRACK_GROUND_NP = "Train Station Track (NP)";
        public const string TRAIN_STATION_TRACK_GROUND_CNP = "Train Station Track (CNP)";
        public const string TRAIN_STATION_TRACK_GROUND_ISLAND = "1194290640.Wide Train Station Track_Data";
        public const string TRAIN_STATION_TRACK_GROUND_LARGE = "1577947171.Bypass Station Track_Ground_Data";
        public const string TRAIN_STATION_TRACK_GROUND_LARGE_DUALISLAND = "1698282173.DualIslandStationTrack_Ground_Data";
        //515489008
        public const string TRAIN_STATION_TRACK_ELEVA = "Station Track Eleva";
        public const string TRAIN_STATION_TRACK_ELEVATED_C = "Station Track Elevated (C)";
        public const string TRAIN_STATION_TRACK_ELEVATED_NP = "Station Track Elevated (NP)";
        public const string TRAIN_STATION_TRACK_ELEVATED_CNP = "Station Track Elevated (CNP)";
        public const string TRAIN_STATION_TRACK_ELEVATED_NARROW = "Station Track Elevated Narrow";
        public const string TRAIN_STATION_TRACK_ELEVATED_NARROW_C = "Station Track Elevated Narrow (C)";
        public const string TRAIN_STATION_TRACK_ELEVATED_NARROW_NP = "Station Track Elevated Narrow (NP)";
        public const string TRAIN_STATION_TRACK_ELEVATED_NARROW_CNP = "Station Track Elevated Narrow (CNP)";
        public const string TRAIN_STATION_TRACK_ELEVATED_ISLAND = "1194290640.ElevatedIslandPlatStationTrack_Data";
        public const string TRAIN_STATION_TRACK_ELEVATED_LARGE = "1577947171.Bypass Station Track_Elevated_Data";
        public const string TRAIN_STATION_TRACK_ELEVATED_LARGE_DUALISLAND = "1698282173.DualIslandStationTrack_Elevated_Data";

        public const string MOM_TRACK = "Metro Track";
        public const string MOM_TRACK_GROUND = "Metro Track Ground";
        public const string MOM_TRACK_NOBAR = "Metro Track Ground NoBar";

        public const string MOM_TRACK_TWOLANE_ONEWAY = "Metro Track Ground Two-Lane One-Way";
        public const string MOM_TRACK_TWOLANE_ONEWAY_NOBAR = "Metro Track Ground Two-Lane One-Way NoBar";

        public const string MOM_TRACK_LARGE = "Metro Track Ground Large";
        public const string MOM_TRACK_LARGE_NOBAR = "Metro Track Ground Large NoBar";

        public const string MOM_TRACK_SMALL = "Metro Track Ground Small";
        public const string MOM_TRACK_SMALL_NOBAR = "Metro Track Ground Small NoBar";

        public const string MOM_TRACK_SMALL_TWOWAY = "Metro Track Ground Small Two-Way";
        public const string MOM_TRACK_SMALL_TWOWAY_NOBAR = "Metro Track Ground Small Two-Way NoBar";

        public const string MOM_STATION_TRACK_GROUND = "Metro Station Track Ground";
        public const string MOM_STATION_TRACK_GROUND_ISLAND = "Metro Station Track Ground Island";
        public const string MOM_STATION_TRACK_GROUND_SMALL = "Metro Station Track Ground Small";
        public const string MOM_STATION_TRACK_GROUND_LARGE = "Metro Station Track Ground Large";
        public const string MOM_STATION_TRACK_GROUND_LARGE_DUALISLAND = "Metro Station Track Ground Large Dual Island";

        public const string MOM_STATION_TRACK_ELEVATED = "Metro Station Track Elevated";
        public const string MOM_STATION_TRACK_ELEVATED_ISLAND = "Metro Station Track Elevated Island";
        public const string MOM_STATION_TRACK_ELEVATED_SMALL = "Metro Station Track Elevated Small";
        public const string MOM_STATION_TRACK_ELEVATED_LARGE = "Metro Station Track Elevated Large";
        public const string MOM_STATION_TRACK_ELEVATED_LARGE_DUALISLAND = "Metro Station Track Elevated Large Dual Island";

        public const string MOM_TRACK_STEEL = "Steel Metro Track Ground";
        public const string MOM_TRACK_NOBAR_STEEL = "Steel Metro Track Ground NoBar";

        public const string MOM_TRACK_TWOLANE_ONEWAY_STEEL = "Steel Metro Track Ground Two-Lane One-Way";
        public const string MOM_TRACK_TWOLANE_ONEWAY_NOBAR_STEEL = "Steel Metro Track Ground Two-Lane One-Way NoBar";

        public const string MOM_TRACK_SMALL_STEEL = "Steel Metro Track Ground Small";
        public const string MOM_TRACK_SMALL_NOBAR_STEEL = "Steel Metro Track Ground Small NoBar";

        public const string MOM_TRACK_SMALL_TWOWAY_STEEL = "Steel Metro Track Ground Small Two-Way";
        public const string MOM_TRACK_SMALL_TWOWAY_NOBAR_STEEL = "Steel Metro Track Ground Small Two-Way NoBar";

        public const string MOM_TRACK_LARGE_STEEL = "Steel Metro Track Ground Large";
        public const string MOM_TRACK_LARGE_NOBAR_STEEL = "Steel Metro Track Ground Large NoBar";

        public const string MOM_STATION_TRACK_GROUND_STEEL = "Steel Metro Station Track Ground";
        public const string MOM_STATION_TRACK_GROUND_ISLAND_STEEL = "Steel Metro Station Track Ground Island";
        public const string MOM_STATION_TRACK_GROUND_SMALL_STEEL = "Steel Metro Station Track Ground Small";
        public const string MOM_STATION_TRACK_GROUND_LARGE_STEEL = "Steel Metro Station Track Ground Large";
        public const string MOM_STATION_TRACK_GROUND_LARGE_DUALISLAND_STEEL = "Steel Metro Station Track Ground Large Dual Island";

        public const string MOM_STATION_TRACK_ELEVATED_STEEL = "Steel Metro Station Track Elevated";
        public const string MOM_STATION_TRACK_ELEVATED_ISLAND_STEEL = "Steel Metro Station Track Elevated Island";
        public const string MOM_STATION_TRACK_ELEVATED_SMALL_STEEL = "Steel Metro Station Track Elevated Small";
        public const string MOM_STATION_TRACK_ELEVATED_LARGE_STEEL = "Steel Metro Station Track Elevated Large";
        public const string MOM_STATION_TRACK_ELEVATED_LARGE_DUALISLAND_STEEL = "Steel Metro Station Track Elevated Large Dual Island";


        public const string ANALOG_PREFIX = "_XANALOGX_";

        public static string GetTrackAnalogName(string trackName)
        {
            var retval = "";
            if (trackName.IndexOf(TRAIN_TRACK) > -1)
            {
                if (trackName == TRAIN_TRACK)
                {
                    retval = MOM_TRACK_GROUND;
                }
                else
                {
                    retval = trackName.Replace(TRAIN_TRACK, MOM_TRACK);
                }

                return retval + " NoBar";
            }
            else if (trackName.IndexOf(MOM_TRACK) > -1)
            {
                trackName = trackName.Replace(" NoBar", "");
                if (trackName == MOM_TRACK_GROUND)
                {
                    retval = TRAIN_TRACK;
                }
                else
                {
                    retval = trackName.Replace(MOM_TRACK, TRAIN_TRACK);
                }
            }

            return "";
        }
    }
        public enum StationTrackType {
            None,
            SidePlatform,
            IslandPlatform,
            SingleTrack,
            QuadSidePlatform,
            //QuadSideIslandPlatform,
            QuadDualIslandPlatform
        }
        public enum TrackVehicleType {
            Default,
            Train,
            Metro
        }
        public enum TrackStyle {
            Modern,
            Classic
        }
}
