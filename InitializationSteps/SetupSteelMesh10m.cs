using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupSteelMesh
    {

        public static void Setup10mSteelMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            var isOneWay = info.name.Contains("One-Way");
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var trainTrackMaterial = trainTrackInfo.m_segments[0].m_material;
            var trainTrackLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
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
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node4);
                        nodeList.Add(node5);
                        nodeList.Add(node6);
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Ground_Pavement_Steel.obj",
                                @"Meshes\10m\Ground_Pavement_LOD.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\10m\Ground_Node_Pavement_Steel.obj",
                                @"Meshes\10m\Ground_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);

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
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Pavement.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\10m\LevelCrossing_Rail.obj");
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
                        if (isOneWay)
                        {
                            nodeList.AddRange(GenerateLevelCrossing(info));
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = nodeList.ToArray();
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
                        var node5 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node4);
                        nodeList.Add(node5);

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Pavement_Steel.obj",
                                @"Meshes\10m\Elevated_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

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
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Node_Pavement_Steel.obj",
                                @"Meshes\10m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node1
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj");

                        node2
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail_Steel.obj",
                            @"Meshes\10m\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node4
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node5
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Node_Pavement_Steel_Trans.obj",
                                @"Meshes\10m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;
                        if (isOneWay)
                        {
                            nodeList.AddRange(GenerateLevelCrossing(info));
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }
                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = nodeList.ToArray();
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
                        var node4 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node4);

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Bridge_Pavement_Steel.obj",
                            @"Meshes\10m\Bridge_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();

                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Meshes\10m\Bridge_Node_Pavement_Steel.obj",
                            @"Meshes\10m\Bridge_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node1
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj");

                        node2
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Rail_Steel.obj",
                            @"Meshes\10m\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node4
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Node_Pavement_Steel_Trans.obj",
                                @"Meshes\10m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        if (elevatedMaterial != null)
                        {
                            segment0.m_material = elevatedMaterial;
                            segment0.m_lodMaterial = elevatedLODMaterial;
                            node0.m_material = elevatedMaterial;
                            node0.m_lodMaterial = elevatedLODMaterial;
                            node2.m_directConnect = true;
                            node4.m_material = elevatedMaterial;
                            node4.m_lodMaterial = elevatedLODMaterial;

                            //segment1.m_material = railMaterial;
                            //node1.m_material = railMaterial;
                        }
                        if (isOneWay)
                        {
                            nodeList.AddRange(GenerateLevelCrossing(info));
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[3].ShallowClone();
                        var segment3 = info.m_segments[3].ShallowClone();
                        var segment4 = info.m_segments[3].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node4 = info.m_nodes[4].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        var node6 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node3);
                        nodeList.Add(node4);
                        nodeList.Add(node5);
                        nodeList.Add(node6);

                        segment1
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Slope_Pavement_Steel.obj",
                                @"Meshes\10m\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Slope_Pavement_Steel_Ground.obj",
                                @"Meshes\10m\Blank.obj");

                        node1
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj");
                        node3
                            .SetMeshes
                            (@"Meshes\10m\Slope_Node_Pavement_Steel.obj",
                                @"Meshes\10m\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\10m\Slope_U_Node_Pavement_Steel.obj",
                                @"Meshes\10m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node6
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        //segment3.m_material = elevatedMaterial;

                        //node3.m_material = elevatedMaterial;
                        node5.m_material = elevatedMaterial;
                        if (isOneWay)
                        {
                            nodeList.AddRange(GenerateLevelCrossing(info));
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }

                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var metroInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                        var segment0 = metroInfo.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var segment4 = info.m_segments[0].ShallowClone();
                        var node0 = metroInfo.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Tunnel_Pavement_Steel.obj",
                                @"Meshes\10m\Tunnel_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Tunnel_Pavement_Steel_Ground.obj",
                                @"Meshes\10m\Blank.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Tunnel_Node_Pavement_Steel.obj",
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
                        segment4.m_material = elevatedMaterial;
                        segment4.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                        node2.m_directConnect = true;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        if (isOneWay)
                        {
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
            }

        }

        //mind changed indices! (after Setup10mSteelMesh)
        public static void Setup10mSteelBarMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
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
                            (@"Meshes\10m\Ground_Fence_Steel.obj",
                            @"Meshes\10m\Ground_Fence_LOD.obj");
                        node
                            .SetMeshes
                            (@"Meshes\10m\Ground_Node_Fence_Steel.obj",
                            @"Meshes\10m\Ground_Node_Fence_LOD.obj");
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
                            (@"Meshes\10m\Elevated_Fence_Steel.obj",
                            @"Meshes\10m\Blank.obj");
                        node
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Node_Fence_Steel.obj",
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
                            (@"Meshes\10m\Bridge_Fence_Steel.obj",
                            @"Meshes\10m\Blank.obj");
                        node
                            .SetMeshes
                            (@"Meshes\10m\Bridge_Node_Fence_Steel.obj",
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
                        var segment = info.m_segments[2].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        segment
                             .SetMeshes
                             (@"Meshes\10m\Slope_Pavement_Steel_Fence.obj",
                             @"Meshes\10m\Slope_Node_Fence_LOD.obj")
                             .SetConsistentUVs();
                        node
                            .SetMeshes
                            (@"Meshes\10m\Slope_Node_Fence.obj",
                            @"Meshes\10m\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;
                        node2
                            .SetMeshes
                            (@"Meshes\10m\Slope_U_Node_Pavement_Steel_Fence.obj",
                            @"Meshes\10m\Slope_Node_Fence_LOD.obj")
                            .SetConsistentUVs();
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node, node2);
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment = info.m_segments[2].ShallowClone();
                        segment
                             .SetMeshes
                             (@"Meshes\10m\Tunnel_Pavement_Steel_Fence.obj",
                             @"Meshes\10m\Slope_Node_Fence_LOD.obj")
                             .SetConsistentUVs();

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        break;
                    }
            }
        }
        private static void AddToSegments(this NetInfo.Segment[] segments, params NetInfo.Segment[] toAdd)
        {
            var theSegments = segments.ToList();
            theSegments.AddRange(toAdd);
            segments = theSegments.ToArray();
        }
        private static void AddToNodes(this NetInfo.Node[] nodes, params NetInfo.Node[] toAdd)
        {
            var thenodes = nodes.ToList();
            thenodes.AddRange(toAdd);
            nodes = thenodes.ToArray();
        }
        //mind changed indices! (after Setup10mSteelMesh)
        public static void Setup10mSteelNoBarMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
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
                            (@"Meshes\10m\Elevated_Bar_Steel.obj",
                                @"Meshes\10m\Blank.obj")
                                .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Bar_Steel.obj",
                                @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs(true);

                        info.m_segments = info.AddSegments(segment0);
                        info.m_nodes = info.AddNodes(node0);
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[1].ShallowClone();
                        //var node0 = info.m_nodes[1].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\10m\Slope_Pavement_Steel_Bar.obj",
                                @"Meshes\10m\Blank.obj")
                                .SetConsistentUVs();
                        //node0
                        //    .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                        //    .SetMeshes
                        //    (@"Meshes\10m\Tunnel_Node_Pavement_Steel_Bar.obj",
                        //        @"Meshes\10m\Blank.obj")
                        //    .SetConsistentUVs(true);
                        //node0.m_material = elevatedMaterial;
                        info.m_segments = info.AddSegments(segment0);
                        //info.m_nodes = info.AddNodes(node0);
                        break;
                    }
                    //case NetInfoVersion.Tunnel:
                    //    {
                    //        var segment0 = info.m_segments[0].ShallowClone();
                    //        var node0 = info.m_nodes[0].ShallowClone();
                    //        segment0
                    //            .SetFlagsDefault()
                    //            .SetMeshes
                    //            (@"Meshes\10m\Tunnel_Pavement_Steel_Bar.obj",
                    //                @"Meshes\10m\Blank.obj")
                    //                .SetConsistentUVs();
                    //        node0
                    //            .SetMeshes
                    //            (@"Meshes\10m\Tunnel_Node_Pavement_Steel_Bar.obj",
                    //                @"Meshes\10m\Blank.obj")
                    //            .SetConsistentUVs(true);
                    //        segment0.m_material = elevatedMaterial;
                    //        segment0.m_lodMaterial = elevatedLODMaterial;
                    //        node0.m_material = elevatedMaterial;
                    //        node0.m_lodMaterial = elevatedLODMaterial;
                    //        info.m_segments = info.AddSegments(segment0);
                    //        info.m_nodes = info.AddNodes(node0);
                    //        break;
                    //    }
            }
        }
        public static void Setup10mStationSteelMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
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
                            (@"Meshes\10m\Ground_Station_Pavement_Steel.obj",
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
                            (@"Meshes\10m\Ground_Station_Node_Pavement_Steel.obj",
                                @"Meshes\10m\Ground_Station_Node_Pavement_LOD.obj");
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
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node4.m_material = elevatedMaterial;
                        node4.m_lodMaterial = elevatedLODMaterial;
                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node2.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;

                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3, node4 };
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
                        var node3 = prefab.m_nodes[0].ShallowClone();
                        segment0
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Station_Pavement_Steel.obj",
                                @"Meshes\10m\Elevated_Station_Pavement_LOD.obj");
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
                            (@"Meshes\10m\Elevated_Station_Node_Pavement_Steel.obj",
                                @"Meshes\10m\Elevated_Station_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\10m\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();

                        node2
                            .SetMeshes
                            (@"Meshes\10m\Boosted_Station_Node_Rail_Steel.obj",
                            @"Meshes\10m\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\10m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
            }
        }
    }
}
