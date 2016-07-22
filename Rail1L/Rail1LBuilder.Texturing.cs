using SingleTrainTrack.NEXT;
using SingleTrainTrack.NEXT.Extensions;
using SingleTrainTrack.NEXT.Texturing;

namespace SingleTrainTrack.Rail1L
{
    public partial class Rail1LBuilder
    {
        private static void SetupTextures(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    for (var i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name.Contains("Ground_Pavement"))
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Textures\Rail1LStation\Ground_Segment_Pavement__MainTex.png",
                                    @"Textures\Rail1LStation\Ground_Segment_Pavement__AlphaMap.png"));
                        }
                    }
                    break;
                case NetInfoVersion.Slope:
                    for (var i = 0; i < info.m_segments.Length; i++)
                    {
                        if (info.m_segments[i].m_mesh.name == "pedestrian-tunnel-slope") 
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Textures\Rail1LStation\Slope_Segment__MainTex.png",
                                    @"Textures\Rail1LStation\Slope_Segment__AlphaMap.png"),
                                new LODTextureSet
                                    (@"Textures\Rail1LStation\Slope_Cover_LOD__MainTex.png",
                                    @"Textures\Rail1LStation\Slope_Cover_LOD__AlphaMap.png",
                                    @"Textures\Rail1LStation\Slope_Cover_LOD__XYSMap.png"));
                        }
                        else if (info.m_segments[i].m_mesh.name.Contains("Ground_Pavement"))
                        {
                            info.m_segments[i].SetTextures(
                                new TextureSet
                                    (@"Textures\Rail1LStation\Ground_Segment_Pavement__MainTex.png",
                                    @"Textures\Rail1LStation\Ground_Segment_Pavement__AlphaMap.png",
                                    null));
                        }
                    }
                    break;
                case NetInfoVersion.Tunnel:
                    {
                        info.SetAllSegmentsTexture(
                            new TextureSet
                                (@"Textures\Rail1LStation\Tunnel_Segment__MainTex.png",
                                @"Textures\Rail1LStation\Tunnel_Segment__AlphaMap.png"));
                        info.SetAllNodesTexture(
                            new TextureSet
                                (@"Textures\Rail1LStation\Tunnel_Segment__MainTex.png",
                                @"Textures\Rail1LStation\Tunnel_Segment__AlphaMap.png"));
                        break;
                    }
            }
        }
    }
}

