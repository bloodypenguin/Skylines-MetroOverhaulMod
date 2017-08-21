using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupMesh
    {
        public static void Setup10mMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo,
            NetInfo metroInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0].ShallowClone(); 
                        var segment1 = info.m_segments[1].ShallowClone(); 
                        var segment2 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = info.m_nodes[2].ShallowClone();
                        var node3 = info.m_nodes[1].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();
                        var node5 = info.m_nodes[2].ShallowClone();
                        var node6 = info.m_nodes[1].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Ground_Pavement.obj",
                                @"Meshes\10m\Ground_Pavement_LOD.obj");

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\10m\Ground_Node_Pavement.obj",
                                @"Meshes\10m\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Pavement.obj",
                                @"Meshes\10m\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_LevelCrossing.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node6
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Rail_Insert.obj")
                            .SetConsistentUVs();

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node5.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node6.m_material = elevatedMaterial;
                        node6.m_lodMaterial = elevatedLODMaterial;
                        node6.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2, node3, node4, node5, node6 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Pavement.obj",
                                @"Meshes\10m\Elevated_Pavement_LOD.obj")
                            .SetConsistentUVs(); //ehem

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Elevated_RailGuards.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Node_Pavement.obj",
                                @"Meshes\10m\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj");

                        node4
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
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
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Bridge_Pavement.obj",
                            @"Meshes\10m\Bridge_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Elevated_RailGuards.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\10m\Bridge_Node_Pavement.obj",
                            @"Meshes\10m\Bridge_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj");
                        node4
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
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
                        var segment0 = metroInfo.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[3].ShallowClone();
                        var segment3 = info.m_segments[1].ShallowClone();
                        var node0 = metroInfo.m_nodes[0].ShallowClone();
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
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Slope_Pavement.obj",
                                @"Meshes\10m\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\10m\Slope_Node_Pavement.obj",
                                @"Meshes\10m\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node3
                            .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Rail.obj")
                            .SetConsistentUVs();

                        node4
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Pavement.obj",
                                @"Meshes\10m\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node5
                            .SetMeshes
                            (@"Meshes\10m\Slope_U_Node_Pavement.obj",
                                @"Meshes\10m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node6
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node7
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node8
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
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
                        var segment0 = metroInfo.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = metroInfo.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Tunnel_Pavement.obj",
                                @"Meshes\10m\Tunnel_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Tunnel_Node_Pavement.obj",
                                @"Meshes\10m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
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

        //mind changed segment and node indices! (after Setup10mMesh)
        public static void Setup10mBarMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
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
                            (@"Meshes\10m\Ground_Fence.obj",
                            @"Meshes\10m\Ground_Fence_LOD.obj");
                        node
                            .SetMeshes
                            (@"Meshes\10m\Ground_Node_Fence.obj",
                            @"Meshes\10m\Ground_Node_Fence_LOD.obj");

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
                            (@"Meshes\10m\Elevated_Fence.obj",
                            @"Meshes\10m\Blank.obj");
                        node
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Node_Fence.obj",
                            @"Meshes\10m\Blank.obj");

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
                            (@"Meshes\10m\Elevated_Fence.obj",
                            @"Meshes\10m\Blank.obj");
                        node
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Node_Fence.obj",
                            @"Meshes\10m\Blank.obj");

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
                            (@"Meshes\10m\Slope_Node_Fence.obj",
                            @"Meshes\10m\Slope_Node_Fence_LOD.obj");

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

            //var segment0 = info.m_segments[0].ShallowClone();
            //var node0 = info.m_nodes[0].ShallowClone();
            //var node2 = info.m_nodes[2].ShallowClone();
            //segment0.m_material = elevatedMaterial;
            //segment0.m_lodMaterial = elevatedLODMaterial;
            //node0.m_material = elevatedMaterial;
            //node0.m_lodMaterial = elevatedLODMaterial;
            //node2.m_material = elevatedMaterial;
            //node2.m_lodMaterial = elevatedLODMaterial;
        }

        public static void Setup10mStationMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[2].ShallowClone();
                        var node3 = prefab.m_nodes[1].ShallowClone();
                        var node4 = prefab.m_nodes[0].ShallowClone();

                        segment0
                            .SetMeshes
                            (@"Meshes\10m\Ground_Station_Pavement.obj",
                                @"Meshes\10m\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\10m\Ground_Node_Pavement.obj",
                                @"Meshes\10m\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Pavement.obj",
                                @"Meshes\10m\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Station_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
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
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[0].ShallowClone();
                        segment0
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Station_Pavement.obj",
                             @"Meshes\10m\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Station_Node_Pavement.obj",
                             @"Meshes\10m\Elevated_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
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
                        var segment0 = metroStationInfo.m_segments[0].ShallowClone();    
                        var segment1 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment2 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment3 = metroStationInfo.m_segments[0].ShallowClone();
                        var node0 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node1 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node2 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node3 = metroStationInfo.m_nodes[0].ShallowClone();

                        segment1
                            .SetMeshes
                            (@"Meshes\10m\Tunnel_Station_Pavement.obj",
                                @"Meshes\10m\Tunnel_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Tunnel_Station_Node_Pavement.obj",
                                @"Meshes\10m\Tunnel_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
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
                        prefab.m_segments = new[] { segment0, segment1, segment2,segment3 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
            }
        }
    }
}
