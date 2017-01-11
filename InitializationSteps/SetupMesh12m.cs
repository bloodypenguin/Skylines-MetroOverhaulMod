using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupMesh
    {
        public static void Setup12mMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo,
            NetInfo metroInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var segment2 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = info.m_nodes[2];
                        var node3 = info.m_nodes[1].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();
                        var node5 = info.m_nodes[2].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_Pavement.obj",
                                @"Meshes\Ground_Pavement_LOD.obj");

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Ground_Node_Pavement.obj",
                                @"Meshes\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\LevelCrossing_Pavement.obj",
                                @"Meshes\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\LevelCrossing_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\ThirdRail_LevelCrossing.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node5.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2, node3, node4, node5 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node3 = info.m_nodes[3];
                        var node4 = info.m_nodes[0].ShallowClone();

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Pavement.obj",
                                @"Meshes\Elevated_Pavement_LOD.obj")
                            .SetConsistentUVs(); //ehem

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_RailGuards.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Node_Pavement.obj",
                                @"Meshes\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj");

                        node4
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2,segment3 };
                        info.m_nodes = new[] { node0, node1, node3, node4 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node3 = info.m_nodes[3];
                        var node4 = info.m_nodes[0].ShallowClone();

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Bridge_Pavement.obj",
                            @"Meshes\Bridge_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_RailGuards.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Bridge_Node_Pavement.obj",
                            @"Meshes\Bridge_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj");
                        node4
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2,segment3 };
                        info.m_nodes = new[] { node0, node1, node3, node4 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = metroInfo.m_segments[0];
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[3].ShallowClone();
                        var segment3 = info.m_segments[1].ShallowClone();
                        var node0 = metroInfo.m_nodes[0];
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = info.m_nodes[3].ShallowClone();
                        var node3 = info.m_nodes[1].ShallowClone();
                        var node4 = info.m_nodes[4].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        var node6 = info.m_nodes[1].ShallowClone();
                        var node7 = info.m_nodes[3].ShallowClone();
                        var node8 = info.m_nodes[3].ShallowClone();
                        segment1
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Slope_Pavement.obj",
                                @"Meshes\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Slope_Node_Pavement.obj",
                                @"Meshes\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node3
                            .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\LevelCrossing_Rail.obj")
                            .SetConsistentUVs();

                        node4
                            .SetMeshes
                            (@"Meshes\LevelCrossing_Pavement.obj",
                                @"Meshes\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node5
                            .SetMeshes
                            (@"Meshes\Slope_U_Node_Pavement.obj",
                                @"Meshes\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node6
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node7
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node8
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = new[] { node0, node1, node2, node3, node4, node5, node6, node7, node8 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = metroInfo.m_segments[0];
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = metroInfo.m_nodes[0];
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Tunnel_Pavement.obj",
                                @"Meshes\Tunnel_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Tunnel_Node_Pavement.obj",
                                @"Meshes\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
            }
        }

        //mind changed segment and node indices! (after Setup12mMesh)
        public static void Setup12mMeshBar(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
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
                            (@"Meshes\Ground_Fence.obj",
                            @"Meshes\Ground_Fence_LOD.obj");
                        node
                            .SetMeshes
                            (@"Meshes\Ground_Node_Fence.obj",
                            @"Meshes\Ground_Node_Fence_LOD.obj");

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Fence.obj",
                            @"Meshes\Blank.obj");
                        node
                            .SetMeshes
                            (@"Meshes\Elevated_Node_Fence.obj",
                            @"Meshes\Blank.obj");

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {

                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Fence.obj",
                            @"Meshes\Blank.obj");
                        node
                            .SetMeshes
                            (@"Meshes\Elevated_Node_Fence.obj",
                            @"Meshes\Blank.obj");

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var node = info.m_nodes[0].ShallowClone();
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Underground)
                            .SetMeshes
                            (@"Meshes\Slope_Node_Fence.obj",
                            @"Meshes\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
            }
            //if (version != NetInfoVersion.Ground)
            //{
            //    return;
            //}

            //var segment0 = info.m_segments[0];
            //var node0 = info.m_nodes[0];
            //var node2 = info.m_nodes[2];
            //segment0.m_material = elevatedMaterial;
            //segment0.m_lodMaterial = elevatedLODMaterial;
            //node0.m_material = elevatedMaterial;
            //node0.m_lodMaterial = elevatedLODMaterial;
            //node2.m_material = elevatedMaterial;
            //node2.m_lodMaterial = elevatedLODMaterial;
        }

        //mind changed segment and node indices! (after Setup12mMesh)
        public static void Setup12mMeshNoBar(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
        //    switch (version)
        //    {
        //        case NetInfoVersion.Ground:
        //            {
        //                var segment0 = info.m_segments[0];
        //                var node0 = info.m_nodes[0];
        //                segment0
        //                    .SetFlagsDefault()
        //                    .SetMeshes
        //                    (@"Meshes\Ground_NoBar_Pavement.obj",
        //                        @"Meshes\Ground_NoBar_Pavement_LOD.obj");

        //                node0
        //                    .SetMeshes
        //                    (@"Meshes\Ground_NoBar_Node_Pavement.obj",
        //                        @"Meshes\Ground_NoBar_Node_Pavement_LOD.obj")
        //                    .SetConsistentUVs(true);
        //                break;
        //            }
        //        case NetInfoVersion.Elevated:
        //            {
        //                var segment0 = info.m_segments[0];
        //                var node0 = info.m_nodes[0];
        //                segment0
        //                    .SetFlagsDefault()
        //                    .SetMeshes
        //                    (@"Meshes\Elevated_NoBar_Pavement.obj",
        //                        @"Meshes\Elevated_Pavement_LOD.obj");

        //                node0
        //                    .SetMeshes
        //                    (@"Meshes\Elevated_NoBar_Node_Pavement.obj",
        //                        @"Meshes\Elevated_Node_Pavement_LOD.obj")
        //                    .SetConsistentUVs(true);
        //                break;
        //            }
        //        case NetInfoVersion.Bridge:
        //            {
        //                var segment0 = info.m_segments[0];
        //                var node0 = info.m_nodes[0];
        //                segment0
        //                    .SetFlagsDefault()
        //                    .SetMeshes
        //                    (@"Meshes\Bridge_NoBar_Pavement.obj",
        //                        @"Meshes\Bridge_Pavement_LOD.obj");

        //                node0
        //                    .SetMeshes
        //                    (@"Meshes\Bridge_NoBar_Node_Pavement.obj",
        //                        @"Meshes\Bridge_Node_Pavement_LOD.obj")
        //                    .SetConsistentUVs(true);
        //                break;
        //            }
        //    }

        }


        public static void Setup12mMeshStation(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1];
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1];
                        var node2 = prefab.m_nodes[2];
                        var node3 = prefab.m_nodes[1].ShallowClone();
                        var node4 = prefab.m_nodes[0].ShallowClone();

                        segment0
                            .SetMeshes
                            (@"Meshes\Ground_Station_Pavement.obj",
                                @"Meshes\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Ground_Node_Pavement.obj",
                                @"Meshes\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\LevelCrossing_Pavement.obj",
                                @"Meshes\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\LevelCrossing_Station_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node2.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;

                        prefab.m_segments = new[] { segment0, segment1,segment2 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3,node4 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1];
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1];
                        var node2 = prefab.m_nodes[0].ShallowClone();
                        segment0
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Pavement.obj",
                             @"Meshes\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Node_Pavement.obj",
                             @"Meshes\Elevated_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1,segment2 };
                        prefab.m_nodes = new[] { node0, node1,node2 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = metroStationInfo.m_segments[0];    
                        var segment1 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment2 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment3 = metroStationInfo.m_segments[0].ShallowClone();
                        var node0 = metroStationInfo.m_nodes[0];
                        var node1 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node2 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node3 = metroStationInfo.m_nodes[0].ShallowClone();

                        segment1
                            .SetMeshes
                            (@"Meshes\Tunnel_Station_Pavement.obj",
                                @"Meshes\Tunnel_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\ThirdRail.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Tunnel_Station_Node_Pavement.obj",
                                @"Meshes\Tunnel_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\ThirdRail_Node.obj", @"Meshes\Blank.obj")
                            .SetConsistentUVs();
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2,segment3 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
            }
        }
    }
}
