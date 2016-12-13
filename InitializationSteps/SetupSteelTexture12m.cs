using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.NEXT.Texturing;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupSteelTexture
    {
        public static void Setup12mSteelTexture(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            if (segment.m_mesh.name.Contains("Pavement_Steel"))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Pavement_Steel__MainTex.png",
                                            @"Textures\Ground_Segment_Pavement_Steel__AlphaMap.png",
                                            @"Textures\Ground_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Ground_Segment_Pavement_Steel__MainTex_LOD.png",
                                            @"Textures\Ground_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                            @"Textures\Ground_Segment_Pavement_Steel__XYSMap_LOD.png"));
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
                            if (node.m_mesh.name.Contains("Pavement_Steel"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Node_Pavement_Steel__MainTex.png",
                                            @"Textures\Ground_Node_Pavement_Steel__AlphaMap.png",
                                            @"Textures\Ground_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Ground_Node_Pavement_Steel__MainTex_LOD.png",
                                            @"Textures\Ground_Node_Pavement_Steel__AlphaMap_LOD.png",
                                            @"Textures\Ground_Segment_Pavement_Steel__XYSMap_LOD.png"));
                            }
                            else if (node.m_mesh.name.Contains("Ground_Level_Crossing_Rail"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Level_Crossing_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Level_Crossing_Rail__XYSMap.png"));
                            }
                            else if(node.m_mesh.name.Contains("Ground_Level_Crossing"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Level_Crossing__MainTex.png",
                                            @"Textures\Ground_Level_Crossing__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                            else
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail_Steel__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail_Steel__XYSMap.png"));
                            }
                        }
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            var elevatedBridge = segment.m_mesh.name.Contains("Elevated") ? "Elevated" : "Bridge";
                            if (segment.m_mesh.name.Contains("Station"))
                            {
                               
                                segment.SetTextures(
                                    new TextureSet
                                        ($@"Textures\{elevatedBridge}_Station_Segment_Pavement_Steel__MainTex.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__AlphaMap.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        ($@"Textures\{elevatedBridge}_Segment_Pavement_Steel__MainTex_LOD.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__XYSMap_LOD.png"));
                            }
                            else if (segment.m_mesh.name.Contains("Pavement"))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        ($@"Textures\{elevatedBridge}_Segment_Pavement_Steel__MainTex.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__AlphaMap.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        ($@"Textures\{elevatedBridge}_Segment_Pavement_Steel__MainTex_LOD.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                            $@"Textures\{elevatedBridge}_Segment_Pavement_Steel__XYSMap_LOD.png"));
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
                            if (node.m_mesh.name.Contains("Pavement"))
                            {
                                if (node.m_mesh.name.Contains("Elevated"))
                                {
                                    node.SetTextures(
                                        new TextureSet
                                            (@"Textures\Elevated_Node_Pavement_Steel__MainTex.png",
                                                @"Textures\Elevated_Node_Pavement_Steel__AlphaMap.png",
                                                @"Textures\Ground_Segment_Pavement_Steel__XYSMap.png"),
                                        new LODTextureSet
                                            (@"Textures\Elevated_Segment_Pavement_Steel__MainTex_LOD.png",
                                                @"Textures\Elevated_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                                @"Textures\Ground_Segment_Pavement_Steel__XYSMap_LOD.png"));
                                }
                                else if (node.m_mesh.name.Contains("Bridge"))
                                {
                                    node.SetTextures(
                                        new TextureSet
                                            (@"Textures\Bridge_Segment_Pavement_Steel__MainTex.png",
                                                @"Textures\Bridge_Segment_Pavement_Steel__AlphaMap.png",
                                                @"Textures\Bridge_Segment_Pavement_Steel__XYSMap.png"),
                                        new LODTextureSet
                                            (@"Textures\Bridge_Segment_Pavement_Steel__MainTex_LOD.png",
                                                @"Textures\Bridge_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                                @"Textures\Bridge_Segment_Pavement_Steel__XYSMap_LOD.png"));
                                }
                            }
                            else if (node.m_mesh.name.Contains("Boosted"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Elevated_Segment_Steel_Rail__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Elevated_Segment_Steel_Rail__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Elevated_Segment_Steel_Rail__MainTex_LOD.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap_LOD.png",
                                            @"Textures\Elevated_Segment_Steel_Rail__XYSMap_LOD.png"));
                            }
                            else
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail_Steel__MainTex.png",
                                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                            @"Textures\Ground_Segment_Rail_Steel__XYSMap.png"));
                            }
                        }
                        break;
                    }
                case NetInfoVersion.Slope:
                    //case NetInfoVersion.Tunnel:
                    {
                        foreach (var segment in info.m_segments)
                        {
                            if (segment.m_mesh.name.Contains("Pavement_Steel"))
                            {
                                segment.SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Segment_Pavement_Steel__MainTex.png",
                                            @"Textures\Elevated_Segment_Pavement_Steel__AlphaMap.png",
                                            @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Segment_Pavement_Steel__MainTex_LOD.png",
                                            @"Textures\Elevated_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                            @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap_LOD.png"));
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
                            if (node.m_mesh.name.Contains("Node_Pavement_Steel"))
                            {
                                node.SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Node_Pavement_Steel__MainTex.png",
                                            @"Textures\Elevated_Node_Pavement_Steel__AlphaMap.png",
                                            @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Node_Pavement_Steel__MainTex_LOD.png",
                                            @"Textures\Elevated_Node_Pavement_Steel__AlphaMap_LOD.png",
                                            @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap_LOD.png"));
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
