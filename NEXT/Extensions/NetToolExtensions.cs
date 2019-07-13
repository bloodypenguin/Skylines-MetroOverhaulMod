using System.Collections.Generic;

namespace MetroOverhaul.NEXT.Extensions {
    public static class NetToolExtensions
    {
        public static float GetElevation(this NetTool tool)
        {
            float elevation = -1;
            if (tool != null && tool.m_prefab != null)
            {
                var getElevationMethod = typeof(NetTool).GetMethod("GetElevation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (getElevationMethod != null)
                {
                    var param = new List<object>();
                    param.Add(tool.m_prefab);
                    object elevationObject = getElevationMethod.Invoke(tool, param.ToArray());
                    if (elevationObject != null)
                    {
                        elevation = (float)elevationObject;
                    }
                }
            }
            return elevation;
        }
    }
}
