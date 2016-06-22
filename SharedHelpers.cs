using SingleTrainTrack.NEXT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingleTrainTrack
{
    class SharedHelpers
    {
        public const string TRAIN_STATION_TRACK = "Train Station Track";
        public const string TRAIN_TRACK = "Train Track";

        public static string NameBuilder(string baseName, NetInfoVersion version)
        {
            return $"{baseName}{(version == NetInfoVersion.Ground ? "" : " " + version.ToString())}".Trim();
        }
    }
}
