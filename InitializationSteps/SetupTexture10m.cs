using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.NEXT.Texturing;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupTexture
    {
        public static void Setup10mTexture(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            if (segment.m_mesh.name.Contains("Pavement") || (segment.m_mesh.name.Contains("Fence")))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Pavement__MainTex.png",
                                            @"Textures\Ground_Segment_Pavement__AlphaMap.png",
                                            @"Textures\Ground_Segment_Pavement__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Ground_Segment_Pavement__MainTex_LOD.png",
                                            @"Textures\Ground_Segment_Pavement__AlphaMap_LOD.png",
                                            @"Textures\Ground_Segment_Pavement__XYSMap_LOD.png"));

                            }
                            else if (segment.m_mesh.name.Contains("ThirdRail"))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\ThirdRail__MainTex.png",
                                            @"Textures\ThirdRail__AlphaMap.png",
                                            @"Textures\ThirdRail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\ThirdRail__MainTex_LOD.png",
                                            @"Textures\ThirdRail__AlphaMap_LOD.png",
                                            @"Textures\ThirdRail__XYSMap_LOD.png"));
                            }
                            else
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        foreach (var node in info.m_nodes)
                        {
                            if (node.m_mesh.name.Contains("LevelCrossing_Pavement"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Level_Crossing__MainTex.png",
                                            @"Textures\Ground_Level_Crossing__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Ground_Level_Crossing__MainTex_LOD.png",
                                            @"Textures\Ground_Level_Crossing__AlphaMap_LOD.png",
                                            @"Textures\Ground_Segment_Pavement__XYSMap_LOD.png"));
                            }
                            
                            else if (node.m_mesh.name.Contains("LevelCrossing_Rail"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Level_Crossing_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Level_Crossing_Rail__XYSMap.png"));
                            }
                            else if (node.m_mesh.name.Contains("Pavement") || (node.m_mesh.name.Contains("Fence")))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Node_Pavement__MainTex.png",
                                            @"Textures\Ground_Node_Pavement__AlphaMap.png",
                                            @"Textures\Ground_Segment_Pavement__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Ground_Node_Pavement__MainTex_LOD.png",
                                            @"Textures\Ground_Node_Pavement__AlphaMap_LOD.png",
                                            @"Textures\Ground_Segment_Pavement__XYSMap_LOD.png"));
                            }
                            else if (node.m_mesh.name.Contains("ThirdRail"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\ThirdRail__MainTex.png",
                                            @"Textures\ThirdRail__AlphaMap.png",
                                            @"Textures\ThirdRail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\ThirdRail__MainTex_LOD.png",
                                            @"Textures\ThirdRail__AlphaMap_LOD.png",
                                            @"Textures\ThirdRail__XYSMap_LOD.png"));
                            }
                            else
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            if (segment.m_mesh.name.Contains("Pavement") || segment.m_mesh.name.Contains("Fence") || segment.m_mesh.name.Contains("Guard"))
                            {
                                var isElevated = segment.m_mesh.name.Contains("Elevated");

                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Elevated_Segment_Pavement__MainTex.png",
                                            @"Textures\Elevated_Segment_Pavement__AlphaMap.png",
                                            @"Textures\Elevated_Segment_Pavement__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Elevated_Segment_Pavement__MainTex_LOD.png",
                                            (isElevated ? @"Textures\Elevated_Segment_Pavement__AlphaMap_LOD.png"
                                                : @"Textures\Bridge_Pavement__AlphaMap_LOD.png"),
                                            @"Textures\Elevated_Segment_Pavement__XYSMap_LOD.png"));
                            }
                            else if (segment.m_mesh.name.Contains("ThirdRail"))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\ThirdRail__MainTex.png",
                                            @"Textures\ThirdRail__AlphaMap.png",
                                            @"Textures\ThirdRail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\ThirdRail__MainTex_LOD.png",
                                            @"Textures\ThirdRail__AlphaMap_LOD.png",
                                            @"Textures\ThirdRail__XYSMap_LOD.png"));
                            }
                            else
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        foreach (var node in info.m_nodes)
                        {
                            if (node.m_mesh.name.Contains("Pavement") || (node.m_mesh.name.Contains("Fence")))
                            {
                                if (node.m_mesh.name.Contains("Elevated"))
                                {
                                    node.SetTextures(
                                        new TextureSet
                                            (@"Textures\Elevated_Node_Pavement__MainTex.png",
                                                @"Textures\Elevated_Node_Pavement__AlphaMap.png",
                                                @"Textures\Ground_Segment_Pavement__XYSMap.png"),
                                        new LODTextureSet
                                            (@"Textures\Elevated_Node_Pavement__MainTex_LOD.png",
                                                @"Textures\Elevated_Node_Pavement__AlphaMap_LOD.png",
                                                @"Textures\Ground_Segment_Pavement__XYSMap_LOD.png"));
                                }
                                else if (node.m_mesh.name.Contains("Bridge"))
                                {
                                    node.SetTextures(
                                        new TextureSet
                                            (@"Textures\Bridge_Node_Pavement__MainTex.png",
                                                @"Textures\Elevated_Node_Pavement__AlphaMap.png",
                                                @"Textures\Bridge_Node_Pavement__XYSMap.png"),
                                        new LODTextureSet
                                            (@"Textures\Bridge_Node_Pavement__MainTex_LOD.png",
                                                @"Textures\Elevated_Node_Pavement__AlphaMap_LOD.png",
                                                @"Textures\Bridge_Node_Pavement__XYSMap_LOD.png"));
                                }
                            }
                            else if (node.m_mesh.name.Contains("ThirdRail"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\ThirdRail__MainTex.png",
                                            @"Textures\ThirdRail__AlphaMap.png",
                                            @"Textures\ThirdRail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\ThirdRail__MainTex_LOD.png",
                                            @"Textures\ThirdRail__AlphaMap_LOD.png",
                                            @"Textures\ThirdRail__XYSMap_LOD.png"));
                            }
                            else
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        break;
                    }
                case NetInfoVersion.Slope:
                case NetInfoVersion.Tunnel:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            if (segment.m_mesh.name.Contains("Station"))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Station_Segment_Pavement__MainTex.png",
                                            @"Textures\Elevated_Segment_Pavement__AlphaMap.png",
                                            @"Textures\Tunnel_Station_Segment_Pavement__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Station_Segment_Pavement__MainTex_LOD.png",
                                            @"Textures\Elevated_Segment_Pavement__AlphaMap_LOD.png",
                                            @"Textures\Tunnel_Station_Segment_Pavement__XYSMap_LOD.png"));
                            }
                            else if (segment.m_mesh.name.Contains("Pavement") || (segment.m_mesh.name.Contains("Fence")))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Segment_Pavement__MainTex.png",
                                            @"Textures\Elevated_Segment_Pavement__AlphaMap.png",
                                            @"Textures\Tunnel_Segment_Pavement__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Segment_Pavement__MainTex_LOD.png",
                                            @"Textures\Elevated_Segment_Pavement__AlphaMap_LOD.png",
                                            @"Textures\Tunnel_Segment_Pavement__XYSMap_LOD.png"));
                            }
                            else if (segment.m_mesh.name.Contains("ThirdRail"))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\ThirdRail__MainTex.png",
                                            @"Textures\ThirdRail__AlphaMap.png",
                                            @"Textures\ThirdRail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\ThirdRail__MainTex_LOD.png",
                                            @"Textures\ThirdRail__AlphaMap_LOD.png",
                                            @"Textures\ThirdRail__XYSMap_LOD.png"));
                            }
                            else
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        foreach (var node in info.m_nodes)
                        {
                            if (node.m_mesh.name.Contains("Station_Node_Pavement"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Station_Node_Pavement__MainTex.png",
                                            @"Textures\Elevated_Segment_Pavement__AlphaMap.png",
                                            @"Textures\Tunnel_Station_Segment_Pavement__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Station_Segment_Pavement__MainTex_LOD.png",
                                            @"Textures\Elevated_Segment_Pavement__AlphaMap_LOD.png",
                                            @"Textures\Tunnel_Station_Segment_Pavement__XYSMap_LOD.png"));
                            }
                            else if (node.m_mesh.name.Contains("Pavement") || (node.m_mesh.name.Contains("Fence")))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Node_Pavement__MainTex.png",
                                            @"Textures\Elevated_Node_Pavement__AlphaMap.png",
                                            @"Textures\Tunnel_Segment_Pavement__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Node_Pavement__MainTex_LOD.png",
                                            @"Textures\Elevated_Node_Pavement__AlphaMap_LOD.png",
                                            @"Textures\Tunnel_Segment_Pavement__XYSMap_LOD.png"));
                            }
                            else if (node.m_mesh.name.Contains("ThirdRail"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\ThirdRail__MainTex.png",
                                            @"Textures\ThirdRail__AlphaMap.png",
                                            @"Textures\ThirdRail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\ThirdRail__MainTex_LOD.png",
                                            @"Textures\ThirdRail__AlphaMap_LOD.png",
                                            @"Textures\ThirdRail__XYSMap_LOD.png"));
                            }
                            else 
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        break;
                    }
            }
        }
    }
}
