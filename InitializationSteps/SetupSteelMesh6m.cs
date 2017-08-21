using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
     public static partial class SetupSteelMesh
    {

        public static void Setup6mSteelMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var trainTrackMaterial = trainTrackInfo.m_nodes[0].m_material;
            var trainTrackLODMaterial = elevatedInfo.m_nodes[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();

                        var segments3 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[2].ShallowClone();
                        var nodes3 = info.m_nodes[2].ShallowClone();
                        var nodes4 = info.m_nodes[1].ShallowClone();
                        var nodes5 = info.m_nodes[1].ShallowClone();
                        var nodes6 = info.m_nodes[2].ShallowClone();
                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Pavement.obj",
                            @"Meshes\6m\Ground_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj",
                            @"Meshes\6m\Ground_Rail_LOD.obj");

                        segments3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();

                        nodes0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Pavement.obj",
                            @"Meshes\6m\Ground_Node_Pavement_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");

                        nodes2
                            .SetMeshes
                            (@"Meshes\6m\Ground_Level_Crossing.obj",
                            @"Meshes\6m\Ground_Level_Crossing_LOD.obj");
                        nodes3
                            .SetMeshes
                            (@"Meshes\6m\Ground_Level_Crossing_Rail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        nodes4
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Start.obj",
                            @"Meshes\6m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj");
                        nodes6
                            .SetMeshes
                            (@"Meshes\6m\Ground_Level_Crossing_Rail_Insert.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();

                        RoadHelper.HandleAsymSegmentFlags(segments3);

                        nodes1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        nodes3.m_material = trainTrackMaterial;
                        nodes3.m_lodMaterial = trainTrackLODMaterial;
                        nodes3.m_directConnect = true;
                        nodes6.m_directConnect = true;
                        info.m_segments = new[] { segments0, segments1, /*segments2*/segments3 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5 , nodes6 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs(); //ehem

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");
                        node2
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_directConnect = true;
                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2, node3, node4 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Bridge_Pavement_Steel.obj",
                            @"Meshes\6m\Bridge_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node1
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");

                        node2
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        if (elevatedMaterial != null)
                        {
                            segment0.m_material = elevatedMaterial;
                            segment0.m_lodMaterial = elevatedLODMaterial;
                            node0.m_material = elevatedMaterial;
                            node0.m_lodMaterial = elevatedLODMaterial;
                            node2.m_directConnect = true;
                            //segment1.m_material = railMaterial;
                            //node1.m_material = railMaterial;
                        }

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[3].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node4 = info.m_nodes[4].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        var node6 = info.m_nodes[0].ShallowClone();

                        segment1
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Slope_Pavement.obj",
                                @"Meshes\6m\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");
                        node3
                            .SetMeshes
                            (@"Meshes\6m\Slope_Node_Pavement.obj",
                                @"Meshes\6m\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Node_Pavement.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node6
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment3.m_material = elevatedMaterial;
                        node3.m_material = elevatedMaterial;
                        node5.m_material = elevatedMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = new[] { node0, node1, node3, node4, node5, node6 };
                        break;
                    }
            }

        }

        //mind changed indices! (after Setup6mSteelMesh)
        public static void Setup6mSteelMeshBar(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Fence_Steel.obj",
                            @"Meshes\6m\Ground_Fence_LOD.obj");
                        node
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Fence_Steel.obj",
                            @"Meshes\6m\Ground_Node_Fence_LOD.obj");
                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;
                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        //var segments = info.m_segments.ToList();
                        //segments.Add(segment);
                        //info.m_segments = segments.ToArray();
                        //var nodes = info.m_nodes.ToList();
                        //nodes.Add(node);
                        //info.m_nodes = nodes.ToArray();
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Fence_Steel.obj",
                            @"Meshes\6m\Blank.obj");
                        node
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Fence_Steel.obj",
                            @"Meshes\6m\Blank.obj");
                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                //case NetInfoVersion.Bridge:
                //    {

                //        var segment = info.m_segments[0].ShallowClone();
                //        var node = info.m_nodes[0].ShallowClone();
                //        segment
                //            .SetFlagsDefault()
                //            .SetMeshes
                //            (@"Meshes\6m\Bridge_Fence_Steel.obj",
                //            @"Meshes\6m\Blank.obj");
                //        node
                //            .SetMeshes
                //            (@"Meshes\6m\Bridge_Node_Fence_Steel.obj",
                //            @"Meshes\6m\Blank.obj");

                //        segment.m_material = elevatedMaterial;
                //        segment.m_lodMaterial = elevatedLODMaterial;
                //        node.m_material = elevatedMaterial;
                //        node.m_lodMaterial = elevatedLODMaterial;

                //        info.m_segments = info.AddSegments(segment);
                //        info.m_nodes = info.AddNodes(node);
                //        break;
                //    }
                case NetInfoVersion.Slope:
                    {
                        var node = info.m_nodes[0].ShallowClone();
                        node
                            .SetMeshes
                            (@"Meshes\6m\Slope_Node_Fence.obj",
                            @"Meshes\6m\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
            }
        }

        public static void Setup6mSteelMeshNoBar(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Bar_Steel.obj",
                                @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Bar_Steel.obj",
                                @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs(true);

                        info.m_segments = info.AddSegments(segment0);
                        info.m_nodes = info.AddNodes(node0);
                        break;
                    }
            }
        }
        public static void Setup6mStationSteelMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[2].ShallowClone();
                        var node3 = prefab.m_nodes[1].ShallowClone();

                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Station_Pavement_Steel.obj",
                                @"Meshes\6m\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Station_Node_Pavement_Steel.obj",
                                @"Meshes\6m\Ground_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\6m\LevelCrossing_Pavement.obj",
                                @"Meshes\6m\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node2.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Node_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Station_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();


                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1 };
                        break;
                    }
            }
        }
    }
}
