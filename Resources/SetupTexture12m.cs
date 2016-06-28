using SubwayOverhaul.NEXT;
using SubwayOverhaul.NEXT.Extensions;
using SubwayOverhaul.NEXT.Texturing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul.SetupPrefab
{
    class SetupTexture
    {
        public static void Setup12mTexture(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                    for (var i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.Contains("Ground_Pavement"))
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Textures\Ground_Segment_Pavement__MainTex.png",
                                    @"Textures\Ground_Segment_Pavement__AlphaMap.png",
                                    @"Textures\Ground_Segment_Pavement__XYSMap.png"));
                        }
                    }
                    break;
                    }
            }
        }
    }
}
