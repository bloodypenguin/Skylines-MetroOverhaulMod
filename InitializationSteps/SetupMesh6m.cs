using System;
using System.Collections.Generic;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupMesh
    {
        public static List<NetInfo.Node> GenerateSplitTracks(NetInfo info, NetInfoVersion version)
        {
            //string[] variations = { "_Merge", "", "_Single", "_Single_Merge" };
            //NetInfo.ConnectGroup[] groups = { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)32, NetInfo.ConnectGroup.CenterTram, (NetInfo.ConnectGroup)16 };
            var ttInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            var defaultMaterial = ttInfo.m_nodes[0].m_material;
            var defaultLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var nodeList = new List<NetInfo.Node>();
            var is10m = info.name.Contains("Two-Lane");
            var is18m = info.name.Contains("Large");
            var isMerge = info.name.Contains("Two-Way") || info.name.Contains("Station");
            var isOneWay = info.name.Contains("One-Way");
            var mergeName = isMerge ? "Merge_" : "";
            var widthName = "";
            if (is18m)
            {
                widthName = "18m";
            }
            else if (is10m)
            {
                widthName = "10m";
            }
            else
            {
                widthName = "6m";
            }

            string[] variations = null;
            NetInfo.ConnectGroup[] groups = null;
            if (is10m)
            {
                variations = new List<string> { "_Merge", "" }.ToArray();
                groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.WideTram }.ToArray();
            }
            else if (is18m)
            {
                variations = new List<string> { "_Merge", "_Merge", "_Single_Merge", "_Single" }.ToArray();
                groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)64, (NetInfo.ConnectGroup)16, NetInfo.ConnectGroup.CenterTram }.ToArray();
            }
            else
            {
                if (isMerge)
                {
                    variations = new List<string> { "_Merge", "_Merge", "_Merge" }.ToArray();
                    groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)32, (NetInfo.ConnectGroup)64 }.ToArray();
                }
                else
                {
                    variations = new List<string> { "", "_Merge", "_Merge" }.ToArray();
                    groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)32, (NetInfo.ConnectGroup)64 }.ToArray();
                }
            }

            for (var i = 0; i < variations.Length; i++)
            {
                NetInfo.Node node1 = null;
                NetInfo.Node node2 = null;
                if (version == NetInfoVersion.Tunnel)
                {
                    node1 = info.m_nodes[0].ShallowClone();
                    node2 = info.m_nodes[0].ShallowClone();

                    node1.m_material = defaultMaterial;
                    node1.m_lodMaterial = defaultLODMaterial;
                    node1.m_directConnect = true;
                    node2.m_material = defaultMaterial;
                    node2.m_lodMaterial = defaultLODMaterial;
                    node2.m_directConnect = true;
                }
                else
                {
                    node1 = info.m_nodes[1].ShallowClone();
                    node2 = info.m_nodes[1].ShallowClone();
                }
                nodeList.Add(node1);
                nodeList.Add(node2);

                node1
                    .SetMeshes
                       ($@"Meshes\{widthName}\Boosted_Rail{variations[i]}_Start.obj",
                      @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                     .SetConsistentUVs();
                node2
                    .SetMeshes
                       ($@"Meshes\{widthName}\Boosted_Rail{variations[i]}_End.obj",
                       @"Meshes\6m\Ground_Rail_End_LOD.obj")
                      .SetConsistentUVs();

                node1.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                node2.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
            }
            return nodeList;
        }
        public static List<NetInfo.Node> GenerateLevelCrossing(NetInfo info)
        {
            var ttInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            var defaultMaterial = ttInfo.m_nodes[0].m_material;
            var defaultLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var is10m = info.name.Contains("Two-Lane");
            var is18m = info.name.Contains("Large");
            var isTwoWay = info.name.Contains("Two-Way") || info.name.Contains("Station");
            var isOneWay = info.name.Contains("One-Way");
            var mergeName = isTwoWay ? "Merge_" : "";
            var width = "";
            if (is10m)
            {
                width = "10m";
            }
            else if (is18m)
            {
                width = "18m";
            }
            else
            {
                width = "6m";
            }

            var nodeList = new List<NetInfo.Node>();
            var pavementIndex = -1;
            for (var i = 0; i < info.m_nodes.Length; i++)
            {
                if (info.m_nodes[i].m_flagsRequired == NetNode.Flags.LevelCrossing)
                {
                    if (info.m_nodes[i].m_directConnect == false)
                    {
                        pavementIndex = i;
                        break;
                    }
                }
            }
            var nodes0 = info.m_nodes[pavementIndex].ShallowClone();
            var nodes1 = info.m_nodes[1].ShallowClone();
            var nodes2 = info.m_nodes[pavementIndex].ShallowClone();

            nodeList.Add(nodes0);
            nodeList.Add(nodes1);
            nodeList.Add(nodes2);

            string[] variations = null;
            NetInfo.ConnectGroup myGroup;
            NetInfo.ConnectGroup[] groups = null;
            if (is10m)
            {
                variations = new List<string> { "_Merge", "" }.ToArray();
                groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.WideTram }.ToArray();
                if (isOneWay)
                {
                    myGroup = (NetInfo.ConnectGroup)32;
                }
                else
                {
                    myGroup = NetInfo.ConnectGroup.NarrowTram;
                }
            }
            else if (is18m)
            {
                variations = new List<string> { "_Merge", "_Single_Merge", "_Single" }.ToArray();
                groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)16, NetInfo.ConnectGroup.CenterTram }.ToArray();
                myGroup = NetInfo.ConnectGroup.WideTram;
            }
            else
            {
                if (isTwoWay)
                {
                    variations = new List<string> { "_Merge", "_Merge" }.ToArray();
                    groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)32 }.ToArray();
                    myGroup = (NetInfo.ConnectGroup)16;
                }
                else
                {
                    variations = new List<string> { "", "_Merge" }.ToArray();
                    groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)32 }.ToArray();
                    myGroup = NetInfo.ConnectGroup.CenterTram;
                }
            }
            nodes0
                .SetMeshes
                ($@"Meshes\{width}\LevelCrossing_Pavement.obj",
                @"Meshes\10m\LevelCrossing_Pavement_LOD.obj");
            nodes1
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                ($@"Meshes\{width}\LevelCrossing_Rail.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes2
                .SetMeshes
                ($@"Meshes\{width}\LevelCrossing_Rail_Insert.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes1.m_directConnect = true;
            nodes2.m_directConnect = true;
            nodes1.m_connectGroup = myGroup;
            nodes2.m_connectGroup = myGroup;
            for (var i = 0; i < variations.Length; i++)
            {
                if (is10m)
                {
                    if (groups[i] == NetInfo.ConnectGroup.NarrowTram || groups[i] == NetInfo.ConnectGroup.WideTram)
                    {
                        width = "10m";
                    }
                    else
                    {
                        width = "6m";
                    }
                }
                var node1 = info.m_nodes[1].ShallowClone();
                var node2 = info.m_nodes[1].ShallowClone();
                var node3 = info.m_nodes[1].ShallowClone();
                var node4 = info.m_nodes[1].ShallowClone();
                nodeList.Add(node1);
                nodeList.Add(node2);
                nodeList.Add(node3);
                nodeList.Add(node4);
                node1
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\{width}\LevelCrossing_Rail{variations[i]}_Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                node2
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\{width}\LevelCrossing_Rail{variations[i]}_End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                node3
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\{width}\LevelCrossing_Rail_Insert{variations[i]}_Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                node4
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\{width}\LevelCrossing_Rail_Insert{variations[i]}_End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();

                node3.m_directConnect = true;
                node3.m_material = defaultMaterial;
                node3.m_lodMaterial = defaultLODMaterial;
                node4.m_directConnect = true;
                node4.m_material = defaultMaterial;
                node4.m_lodMaterial = defaultLODMaterial;

                node1.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                node2.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
                node3.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                node4.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
            }

            return nodeList;
        }
        public static void Setup6mMesh(this NetInfo info, NetInfoVersion version)
        {
            var ttInfo = Prefabs.Find<NetInfo>("Train Track");
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var ttElInfo = Prefabs.Find<NetInfo>("Train Track Elevated");
            var defaultMaterial = brElInfo.m_segments[0].m_material;
            var defaultElMaterial = brElInfo.m_segments[0].m_lodMaterial;
            var defaultBrElMaterial = brElInfo.m_segments[0].m_material;
            var defaultLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var defaultElLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var defaultBrElLodMaterial = brElInfo.m_segments[0].m_lodMaterial;
            var isTwoWay = info.name.Contains("Two-Way");
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);

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
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");

                        RoadHelper.HandleAsymSegmentFlags(segments3);

                        nodes1.m_flagsForbidden = NetNode.Flags.LevelCrossing;

                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        nodeList.AddRange(GenerateLevelCrossing(info));

                        info.m_segments = new[] { segments0, segments1, /*segments2*/segments3 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }

                case NetInfoVersion.Elevated:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();

                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);
                        nodeList.Add(nodes2);
                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement.obj",
                            @"Meshes\6m\Elevated_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj",
                              @"Meshes\6m\Ground_Rail_LOD.obj");

                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Elevated_RailGuards.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Trans_Pavement.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj");
                        RoadHelper.HandleAsymSegmentFlags(segments3);
                        segments0.m_material = defaultMaterial;
                        segments0.m_lodMaterial = defaultLODMaterial;
                        segments3.m_material = defaultMaterial;
                        segments3.m_lodMaterial = defaultLODMaterial;
                        segments4.m_material = defaultMaterial;
                        segments4.m_lodMaterial = defaultLODMaterial;
                        nodes0.m_material = defaultMaterial;
                        nodes0.m_lodMaterial = defaultLODMaterial;
                        nodes2.m_material = defaultMaterial;
                        nodes2.m_lodMaterial = defaultLODMaterial;
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        info.m_segments = new[] { segments0, segments1 /*,segments2*/, segments3, segments4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);
                        nodeList.Add(nodes2);
                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Bridge_Pavement.obj",
                            @"Meshes\6m\Bridge_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj",
                              @"Meshes\6m\Ground_Rail_LOD.obj");

                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Elevated_RailGuards.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                              (@"Meshes\6m\Bridge_Node_Pavement.obj",
                              @"Meshes\6m\Elevated_Node_Pavement_LOD.obj")
                              .SetConsistentUVs();

                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj",
                              @"Meshes\6m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                              (@"Meshes\6m\Bridge_Trans_Pavement.obj",
                              @"Meshes\6m\Elevated_Node_Pavement_LOD.obj")
                              .SetConsistentUVs();
                        var segmentNormals = segments0.m_mesh.normals;
                        var segmentVertices = segments0.m_mesh.vertices;
                        var colors = new List<UnityEngine.Color>();
                        var colors32 = new List<UnityEngine.Color32>();

                        for (int i = 0; i < segments0.m_mesh.vertexCount; i++)
                        {
                            if (segmentNormals[i].y == 1 && segmentVertices[i].y == 0)
                            {
                                colors.Add(new UnityEngine.Color(255, 255, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 255, 255, 255));
                            }
                            else
                            {
                                colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                            }
                        }
                        segments0.m_mesh.colors = colors.ToArray();
                        segments0.m_mesh.colors32 = colors32.ToArray();

                        segmentNormals = segments0.m_lodMesh.normals;
                        segmentVertices = segments0.m_lodMesh.vertices;
                        colors = new List<UnityEngine.Color>();
                        colors32 = new List<UnityEngine.Color32>();

                        for (int i = 0; i < segments0.m_lodMesh.vertexCount; i++)
                        {
                            if (segmentNormals[i].y == 1 && Math.Abs(segmentVertices[i].x) <= 3 && segmentVertices[i].y < 3)
                            {
                                colors.Add(new UnityEngine.Color(255, 255, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 255, 255, 255));
                            }
                            else
                            {
                                colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                            }
                        }
                        segments0.m_lodMesh.colors = colors.ToArray();
                        segments0.m_lodMesh.colors32 = colors32.ToArray();
                        RoadHelper.HandleAsymSegmentFlags(segments3);
                        segments0.m_material = defaultMaterial;
                        segments0.m_lodMaterial = defaultLODMaterial;
                        segments4.m_material = defaultMaterial;
                        segments4.m_lodMaterial = defaultLODMaterial;
                        nodes0.m_material = defaultMaterial;
                        nodes0.m_lodMaterial = defaultLODMaterial;
                        nodes2.m_material = defaultMaterial;
                        nodes2.m_lodMaterial = defaultLODMaterial;
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        info.m_segments = new[] { segments0, segments1/*, segments2*/, segments3, segments4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }

                case NetInfoVersion.Slope:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        //var nodes2 wires
                        var nodes3 = info.m_nodes[3].ShallowClone();
                        var nodes9 = info.m_nodes[0].ShallowClone();

                        //var nodes10 = info.m_nodes[1].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);
                        nodeList.Add(nodes3);
                        nodeList.Add(nodes9);

                        segments0
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement.obj");
                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Slope_Pavement.obj",
                              @"Meshes\6m\Slope_Pavement_LOD.obj")
                              .SetConsistentUVs();

                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                          .SetMeshes
                            (@"Meshes\6m\Tunnel_Node_Pavement.obj",
                            @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();

                        nodes3
                            .SetMeshes
                              (@"Meshes\6m\Slope_Node_Pavement.obj",
                              @"Meshes\6m\Ground_Node_Pavement_LOD.obj");
                        nodes9
                            .SetMeshes
                            (@"Meshes\6m\Slope_U_Node_Pavement.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        RoadHelper.HandleAsymSegmentFlags(segments4);
                        nodes1.m_connectGroup = isTwoWay ? (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.CenterTram : NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        segments3.m_material = defaultBrElMaterial;
                        segments3.m_lodMaterial = defaultBrElLodMaterial;
                        nodes9.m_material = defaultBrElMaterial;
                        nodes9.m_lodMaterial = defaultBrElLodMaterial;

                        info.m_segments = new[] { segments0, segments1, segments3, segments4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        if (isTwoWay)
                        {
                            node2.m_connectGroup |= (NetInfo.ConnectGroup)16;
                        }
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        node1
                            .SetMeshes
                              (@"Meshes\6m\Tunnel_Node_Pavement.obj",
                                  @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                              .SetConsistentUVs();
                        node2
                            .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj")
                              .SetConsistentUVs();

                        segment1.m_material = defaultElMaterial;
                        segment1.m_lodMaterial = defaultElLODMaterial;
                        segment2.m_material = defaultElMaterial;
                        segment2.m_lodMaterial = defaultElLODMaterial;
                        segment3.m_material = defaultElMaterial;
                        segment3.m_lodMaterial = defaultElLODMaterial;
                        RoadHelper.HandleAsymSegmentFlags(segment1);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        node1.m_material = defaultBrElMaterial;
                        node1.m_lodMaterial = defaultBrElLodMaterial;
                        node2.m_connectGroup = isTwoWay ? (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.CenterTram : NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

                        node2.m_material = defaultMaterial;
                        node2.m_lodMaterial = defaultLODMaterial;

                        node2.m_directConnect = true;
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
                    }
                    break;
            }
        }
        public static void Setup6mMeshBar(NetInfo info, NetInfoVersion version)
        {
            var elevatedInfo = Prefabs.Find<NetInfo>("Train Track Elevated");
            var elevatedRInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var metroInfo = Prefabs.Find<NetInfo>("Metro Track");
            var elevatedRMaterial = elevatedRInfo.m_nodes[0].m_material;
            var elevatedRLODMaterial = elevatedRInfo.m_nodes[0].m_lodMaterial;
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
                            (@"Meshes\6m\Ground_Fence.obj",
                            @"Meshes\6m\Ground_Fence_LOD.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Fence.obj",
                            @"Meshes\6m\Ground_Node_Fence_LOD.obj");

                        segment.m_material = elevatedRMaterial;
                        segment.m_lodMaterial = elevatedRLODMaterial;
                        node.m_material = elevatedRMaterial;
                        node.m_lodMaterial = elevatedRLODMaterial;

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
                            (@"Meshes\6m\Elevated_Fence.obj",
                            @"Meshes\6m\Blank.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Fence.obj",
                            @"Meshes\6m\Blank.obj");

                        segment.m_material = elevatedRMaterial;
                        segment.m_lodMaterial = elevatedRLODMaterial;
                        node.m_material = elevatedRMaterial;
                        node.m_lodMaterial = elevatedRLODMaterial;

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
                            (@"Meshes\6m\Elevated_Fence.obj",
                            @"Meshes\6m\Blank.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Fence.obj",
                            @"Meshes\6m\Blank.obj");

                        segment.m_material = elevatedRMaterial;
                        segment.m_lodMaterial = elevatedRLODMaterial;
                        node.m_material = elevatedRMaterial;
                        node.m_lodMaterial = elevatedRLODMaterial;

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
                            (@"Meshes\6m\Slope_Node_Fence.obj",
                            @"Meshes\6m\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
            }
        }
        public static void Setup6mStationMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var trainTrackInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var trainTrackMaterial = trainTrackInfo.m_nodes[0].m_material;
            var trainTrackLODMaterial = trainTrackInfo.m_nodes[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);

                        node1.m_connectGroup = (NetInfo.ConnectGroup)16;
                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Station_Pavement.obj",
                                @"Meshes\6m\Ground_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Pavement.obj",
                                @"Meshes\6m\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        nodeList.AddRange(GenerateSplitTracks(prefab, version));
                        nodeList.AddRange(GenerateLevelCrossing(prefab));
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        node1.m_connectGroup = (NetInfo.ConnectGroup)16;

                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Pavement.obj",
                             @"Meshes\6m\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Node_Pavement.obj",
                             @"Meshes\6m\Elevated_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        nodeList.AddRange(GenerateSplitTracks(prefab, version));
                        nodeList.AddRange(GenerateLevelCrossing(prefab));
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = nodeList.ToArray();
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

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);

                        node2.m_connectGroup = (NetInfo.ConnectGroup)16;
                        segment1
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Pavement.obj",
                                @"Meshes\6m\Tunnel_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Node_Pavement.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
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
                        nodeList.AddRange(GenerateSplitTracks(prefab, version));
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        prefab.m_nodes = nodeList.ToArray();
                        break;
                    }
            }
        }
    }
}