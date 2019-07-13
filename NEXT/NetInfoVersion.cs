using System;
using System.Collections.Generic;
using System.Linq;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.NEXT
{
    [Flags]
    public enum NetInfoVersion
    {
        None = 0,
        Ground = 1, //By default
        Elevated = 2,
        Bridge = 4,
        Tunnel = 8,
        Slope = 16,
        All = 31,
        Sunken = 32, 
        GroundGrass = 64,
        GroundTrees = 128,
        AllWithDecoration = 223,
    }

    public static class NetInfoVersionExtensions
    {
        public static IEnumerable<NetInfoVersion> ToCollection(this NetInfoVersion version)
        {
            return Enum
                .GetValues(typeof (NetInfoVersion))
                .OfType<NetInfoVersion>()
                .Where(niv => niv != NetInfoVersion.All && niv != NetInfoVersion.AllWithDecoration)
                .Where(niv => version.HasFlag(niv));
        }
    }
}
