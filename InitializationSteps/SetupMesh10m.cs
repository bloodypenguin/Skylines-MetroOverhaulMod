using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using Next;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupMesh
    {
        public static void Setup10mMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroInfo)
        {
            var isOneWay = info.name.Contains("One-Way");
            var isLarge = info.name.Contains("Large");
            var width = isLarge ? "18m":"10m";
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var elevatedbrMaterial = brElInfo.m_segments[0].m_lodMaterial;
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
                                $@"Meshes\{width}\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\LevelCrossing_Pavement.obj",
                                $@"Meshes\{width}\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\LevelCrossing_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_LevelCrossing.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node6
                            .SetMeshes
                            ($@"Meshes\{width}\LevelCrossing_Rail_Insert.obj")
                            .SetConsistentUVs();

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node5.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node6.m_material = elevatedMaterial;
                        node6.m_lodMaterial = elevatedLODMaterial;
                        node6.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        if (isOneWay || isLarge)
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
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node3);
                        nodeList.Add(node4);

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Pavement.obj",
                                $@"Meshes\{width}\Elevated_Pavement_LOD.obj")
                            .SetConsistentUVs(); //ehem

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_RailGuards.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Node_Pavement.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj");

                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;

                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        if (isOneWay || isLarge)
                        {
                            nodeList.AddRange(GenerateLevelCrossing(info));
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
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
                        var node5 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node3);
                        nodeList.Add(node4);
                        nodeList.Add(node5);
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Pavement.obj",
                            $@"Meshes\{width}\Bridge_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj");
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_RailGuards.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Node_Pavement.obj",
                            $@"Meshes\{width}\Bridge_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj");
                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node5
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Node_Pavement_Trans.obj",
                            $@"Meshes\{width}\Bridge_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;
                        if (isOneWay || isLarge)
                        {
                            nodeList.AddRange(GenerateLevelCrossing(info));
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
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
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node4);
                        nodeList.Add(node5);
                        nodeList.Add(node6);
                        nodeList.Add(node7);
                        nodeList.Add(node8);
                        segment1
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Pavement.obj",
                                $@"Meshes\{width}\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Node_Pavement.obj",
                                $@"Meshes\{width}\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node3
                            .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\LevelCrossing_Rail.obj")
                            .SetConsistentUVs();

                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\LevelCrossing_Pavement.obj",
                                $@"Meshes\{width}\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node5
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_U_Node_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node6
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node7
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node8
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;
                        if (isOneWay || isLarge)
                        {
                            nodeList.AddRange(GenerateLevelCrossing(info));
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
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

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Node_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment1.m_material = elevatedbrMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedbrMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedbrMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_connectGroup = isOneWay ? (NetInfo.ConnectGroup)32 | NetInfo.ConnectGroup.NarrowTram : NetInfo.ConnectGroup.NarrowTram;
                        node2.m_directConnect = true;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        if (isOneWay || isLarge)
                        {
                            nodeList.AddRange(GenerateSplitTracks(info, version));
                        }
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
            }
        }

        //mind changed segment and node indices! (after Setup10mMesh)
        public static void Setup10mBarMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
        {
            var width = info.name.Contains("Large") ? "18m" : "10m";
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
                            ($@"Meshes\{width}\Ground_Fence.obj",
                            $@"Meshes\{width}\Ground_Fence_LOD.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Fence.obj",
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
                case NetInfoVersion.Bridge:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Fence.obj",
                            $@"Meshes\{width}\Blank.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Node_Fence.obj",
                            $@"Meshes\{width}\Blank.obj");

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
                            ($@"Meshes\{width}\Slope_Node_Fence.obj",
                            $@"Meshes\{width}\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
            }
        }

        public static void Setup10mStationMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var width = prefab.name.Contains("Large") ? "18m" : "10m";
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
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node4);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Station_Pavement.obj",
                                $@"Meshes\{width}\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\10m\Station_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\10m\Station_ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\LevelCrossing_Pavement.obj",
                                $@"Meshes\{width}\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\LevelCrossing_Station_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

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
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);

                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\10m\Station_Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\10m\Station_ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = new[] { node0, node1, node2 };
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
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);

                        segment1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\10m\Station_Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\10m\Station_ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Node_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node1.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
            }
        }
    }
}
