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
            var isLarge = info.name.Contains("Large");
            var width = isLarge ? "18m" : "10m";
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var defaultElMaterial = brElInfo.m_segments[0].m_material;
            var defaultElLODMaterial = brElInfo.m_segments[0].m_lodMaterial;

            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var trainTrackMaterial = trainTrackInfo.m_segments[0].m_material;
            var trainTrackLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;

            var nodeList = new List<NetInfo.Node>();
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        
                        var node4 = info.m_nodes[0].ShallowClone();
                        
                        nodeList.Add(node0);
                        nodeList.Add(node4);
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Pavement.obj",
                                $@"Meshes\{width}\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);
                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        
                        nodeList.Add(node0);
                        nodeList.Add(node4);
                        nodeList.Add(node5);

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Pavement_Steel.obj",
                                $@"Meshes\{width}\Elevated_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj")
                            .SetConsistentUVs();

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Node_Pavement_Steel.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node5
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Trans_Pavement_Steel.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        //var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        //var node0 = info.m_nodes[0].ShallowClone();
                        
                        var node3 = info.m_nodes[0].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();
                        
                        nodeList.Add(node3);
                        nodeList.Add(node4);


                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Trans_Pavement_Steel.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        info.m_segments = new[] { segment1, segment2 };
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
                        
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        var node6 = info.m_nodes[0].ShallowClone();
                        
                        nodeList.Add(node0);
                        nodeList.Add(node3);
                        nodeList.Add(node5);
                        nodeList.Add(node6);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
							$@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");
                        if (width == "18m")
                        {
                            segment1
                                .SetMeshes
                                ($@"Meshes\{width}\Rail.obj",
                                $@"Meshes\{width}\Rail_LOD.obj")
                                .SetConsistentUVs();
                        }
                        else
                        {
                            segment1
                                .SetMeshes
                                ($@"Meshes\{width}\Rail.obj")
                                .SetConsistentUVs();
                        }

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Pavement_Steel.obj",
                                $@"Meshes\{width}\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Pavement_Steel_Ground.obj",
                                $@"Meshes\{width}\Blank.obj");
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
							$@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");
                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Node_Pavement_Steel.obj",
                                $@"Meshes\{width}\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_U_Node_Pavement_Steel.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node6
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        //segment4.m_material = defaultElMaterial;
                        //segment4.m_lodMaterial = defaultElLODMaterial;
                        //node3.m_material = defaultElMaterial;
                        //node3.m_lodMaterial = defaultElLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = defaultElLODMaterial;
                        
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
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
                        var node4 = info.m_nodes[0].ShallowClone();
                        
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node4);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
							$@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Steel.obj",
                             $@"Meshes\{width}\Tunnel_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Steel_Ground.obj",
                                $@"Meshes\{width}\Blank.obj");
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
							$@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");
                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Node_Pavement_Steel.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                                ($@"Meshes\{width}\Tunnel_Trans_Pavement_Steel.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        segment1.m_material = defaultElMaterial;
                        segment1.m_lodMaterial = defaultElLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        segment4.m_material = defaultElMaterial;
                        segment4.m_lodMaterial = defaultElLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        node4.m_material = elevatedMaterial;
                        node4.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        break;
                    }
            }
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].m_flagsForbidden |= NetNode.Flags.LevelCrossing;
            }
            nodeList.AddRange(SetupMeshUtil.GenerateSplitTracksAndLevelCrossings(info, version));
            info.m_nodes = nodeList.ToArray();
        }

        //mind changed indices! (after Setup10mSteelMesh)
        public static void Setup10mSteelBarMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var isLarge = info.name.Contains("Large");
            var width = isLarge ? "18m" : "10m";
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Fence_Steel.obj",
                            $@"Meshes\{width}\Ground_Fence_LOD.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Fence_Steel.obj",
                            $@"Meshes\{width}\Ground_Node_Fence_LOD.obj");
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
                            ($@"Meshes\{width}\Elevated_Fence_Steel.obj",
                            $@"Meshes\{width}\Blank.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Node_Fence_Steel.obj",
                            $@"Meshes\{width}\Blank.obj");
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
                        var segment1 = info.m_segments[0].ShallowClone();
                        var node0 = elevatedInfo.m_nodes[0].ShallowClone();
                        var node1 = elevatedInfo.m_nodes[0].ShallowClone();
                        var node2 = elevatedInfo.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Fence_Steel.obj",
                            $@"Meshes\{width}\Blank.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Pavement_Steel.obj",
                            $@"Meshes\{width}\Bridge_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition | NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Node_Pavement_Steel.obj",
                            $@"Meshes\{width}\Bridge_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node1
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Trans_Pavement_Steel.obj",
                            $@"Meshes\{width}\Bridge_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node2
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Node_Fence_Steel.obj",
                            $@"Meshes\{width}\Blank.obj");

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment, segment1);
                        info.m_nodes = info.AddNodes(node0, node1, node2);
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment = info.m_segments[2].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        segment
                             .SetMeshes
                             ($@"Meshes\{width}\Slope_Pavement_Steel_Fence.obj",
                             $@"Meshes\{width}\Slope_Node_Fence_LOD.obj")
                             .SetConsistentUVs();
                        node
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Node_Fence.obj",
                            $@"Meshes\{width}\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_U_Node_Pavement_Steel_Fence.obj",
                            $@"Meshes\{width}\Slope_Node_Fence_LOD.obj")
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
                             ($@"Meshes\{width}\Tunnel_Pavement_Steel_Fence.obj",
                             $@"Meshes\{width}\Slope_Node_Fence_LOD.obj")
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
            var isLarge = info.name.Contains("Large");
            var width = isLarge ? "18m" : "10m";
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Bar_Steel.obj",
                                $@"Meshes\{width}\Blank.obj")
                                .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Bar_Steel.obj",
                                $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        info.m_segments = info.AddSegments(segment0);
                        info.m_nodes = info.AddNodes(node0);
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var node0 = elevatedInfo.m_nodes[0].ShallowClone();
                        var node1 = elevatedInfo.m_nodes[0].ShallowClone();
                        var node2 = elevatedInfo.m_nodes[0].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Pavement_Steel2.obj",
                            $@"Meshes\{width}\Bridge_Pavement_Steel2_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition | NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Node_Pavement_Steel2.obj",
                                $@"Meshes\{width}\Bridge_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Trans_Pavement_Steel2.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_Steel.obj")
                            .SetConsistentUVs();
                        node2
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Bar_Steel.obj",
                                $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        info.m_segments = info.AddSegments(segment0);
                        info.m_nodes = info.AddNodes(node0, node1, node2);
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[1].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Pavement_Steel_Bar.obj",
                                $@"Meshes\{width}\Blank.obj")
                                .SetConsistentUVs();
                        info.m_segments = info.AddSegments(segment0);
                        break;
                    }
            }
        }
        public static void Setup10mStationSteelMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var isLarge = prefab.name.Contains("Large");
            var width = isLarge ? "18m" : "10m";
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var nodeList = new List<NetInfo.Node>();
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node4 = prefab.m_nodes[0].ShallowClone();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node4);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Station_Pavement.obj",
                                $@"Meshes\{width}\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Station_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Station_Node_Rail.obj")
                            .SetConsistentUVs();

                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node4.m_material = elevatedMaterial;
                        node4.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1, segment2 };
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
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        segment0
                            .SetMeshes
                                ($@"Meshes\{width}\Elevated_Station_Pavement_Steel.obj",
                                $@"Meshes\{width}\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Node_Pavement_Steel.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();

                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Station_Node_Rail_Steel.obj",
                            $@"Meshes\{width}\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
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
                        break;
                    }
            }
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].m_flagsForbidden |= NetNode.Flags.LevelCrossing;
            }
            nodeList.AddRange(SetupMeshUtil.GenerateSplitTracksAndLevelCrossings(prefab, version));
            prefab.m_nodes = nodeList.ToArray();
        }
    }
}
