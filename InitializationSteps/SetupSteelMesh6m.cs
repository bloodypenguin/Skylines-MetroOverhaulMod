using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupSteelMesh
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
            //var is16m = info.name.Contains("Island");
            var is18m = info.name.Contains("Large");
            var isMerge = info.name.Contains("Two-Way") || info.name.Contains("Station") || is10m || is18m;
            var mergeName = isMerge ? "Merge_" : "";
            var widthName = "";
            var isElevatedBridge = version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge;
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
            //else if (is16m)
            //{
            //    variations = new List<string> { "_Merge" }.ToArray();
            //    groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)16}.ToArray();
            //}
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
                NetInfo.Node node1a = null;
                NetInfo.Node node2 = null;
                NetInfo.Node node2a = null;

                if (isElevatedBridge)
                {
                    node1 = info.m_nodes[1].ShallowClone();
                    node1a = info.m_nodes[1].ShallowClone();
                    node2 = info.m_nodes[1].ShallowClone();
                    node2a = info.m_nodes[1].ShallowClone();
                    var node3 = info.m_nodes[0].ShallowClone(); ;
                    var node4 = info.m_nodes[0].ShallowClone(); ;
                    var node5 = info.m_nodes[0].ShallowClone();
                    var node6 = info.m_nodes[0].ShallowClone();

                    nodeList.Add(node1);
                    nodeList.Add(node1a);
                    nodeList.Add(node2);
                    nodeList.Add(node2a);
                    nodeList.Add(node3);
                    nodeList.Add(node4);
                    nodeList.Add(node5);
                    nodeList.Add(node6);

                    node1
                        .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                        .SetMeshes
                           ($@"Meshes\{widthName}\Ground_Rail{variations[i]}_Start.obj",
                          @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                         .SetConsistentUVs();
                    node1a
                        .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                        .SetMeshes
                           ($@"Meshes\{widthName}\Boosted_Rail{variations[i]}_Start.obj",
                           @"Meshes\6m\Ground_Rail_End_LOD.obj")
                          .SetConsistentUVs();
                    node2
                        .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                        .SetMeshes
                           ($@"Meshes\{widthName}\Ground_Rail{variations[i]}_End.obj",
                          @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                         .SetConsistentUVs();
                    node2a
                        .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                        .SetMeshes
                            ($@"Meshes\{widthName}\Boosted_Rail{variations[i]}_End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj")
                            .SetConsistentUVs();
                    node3
                        .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                        .SetMeshes
                           ($@"Meshes\{widthName}\Boosted_Rail_Steel_Insert{variations[i]}_Start.obj",
                          @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                         .SetConsistentUVs();
                    node4
                        .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                        .SetMeshes
                           ($@"Meshes\{widthName}\Boosted_Rail_Steel_Insert{variations[i]}_End.obj",
                           @"Meshes\6m\Ground_Rail_End_LOD.obj")
                          .SetConsistentUVs();
                    node5
                        .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                        .SetMeshes
                           ($@"Meshes\{widthName}\Elevated_Node_Pavement_Steel_Insert{variations[i]}_Start.obj",
                          @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                         .SetConsistentUVs();
                    node6
                        .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                        .SetMeshes
                           ($@"Meshes\{widthName}\Elevated_Node_Pavement_Steel_Insert{variations[i]}_End.obj",
                           @"Meshes\6m\Ground_Rail_End_LOD.obj")
                          .SetConsistentUVs();

                    node3.m_directConnect = true;
                    node4.m_directConnect = true;
                    node5.m_directConnect = true;
                    node6.m_directConnect = true;

                    node1.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                    node1a.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                    node2.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
                    node2a.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
                    node3.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                    node4.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
                    node5.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                    node6.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
                    var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
                    node3.m_material = brElInfo.m_segments[0].m_material;
                    node3.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;
                    node4.m_material = brElInfo.m_segments[0].m_material;
                    node4.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;
                    node5.m_material = brElInfo.m_segments[0].m_material;
                    node5.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;
                    node6.m_material = brElInfo.m_segments[0].m_material;
                    node6.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;
                }
                else
                {
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
            }
            return nodeList;
        }

        public static List<NetInfo.Node> GenerateLevelCrossing(NetInfo info)
        {
            var is10m = info.name.Contains("Two-Lane");
            var is18m = info.name.Contains("Large");
            var isMerge = info.name.Contains("Two-Way") || info.name.Contains("Station") || is10m;
            var isOneWay = info.name.Contains("One-Way");
            var mergeName = isMerge ? "Merge_" : "";
            var LevelCrossing = isMerge ? "" : "LevelCrossing_";
            var nodeList = new List<NetInfo.Node>();
            var pavementIndex = -1;
            var width = "";
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

            if (is18m)
            {
                width = "18m";
            }
            else if (is10m)
            {
                width = "10m";
            }
            else
            {
                width = "6m";
            }

            var nodes0 = info.m_nodes[pavementIndex].ShallowClone();
            var nodes1 = info.m_nodes[1].ShallowClone();
            var nodes2 = info.m_nodes[pavementIndex].ShallowClone();

            nodeList.Add(nodes0);
            nodeList.Add(nodes1);
            nodeList.Add(nodes2);

            nodes0
                .SetMeshes
                ($@"Meshes\{width}\LevelCrossing_Pavement.obj",
                @"Meshes\6m\LevelCrossing_LOD.obj");
            nodes1
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                ($@"Meshes\{width}\LevelCrossing_Rail.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes2
                .SetMeshes
                ($@"Meshes\{width}\LevelCrossing_Rail_Insert.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();

            string[] variations = null;
            NetInfo.ConnectGroup myGroup;
            NetInfo.ConnectGroup[] groups = null;
            if (is10m)
            {
                if (isOneWay)
                {
                    myGroup = (NetInfo.ConnectGroup)32;
                }
                else
                {
                    myGroup = NetInfo.ConnectGroup.NarrowTram;
                }
                variations = new List<string> { "_Merge", "" }.ToArray();
                groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.WideTram }.ToArray();
            }
            else if (is18m)
            {
                myGroup = NetInfo.ConnectGroup.WideTram;
                variations = new List<string> { "_Merge", "_Single_Merge", "_Single" }.ToArray();
                groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)16, NetInfo.ConnectGroup.CenterTram }.ToArray();
            }
            else
            {
                if (isMerge)
                {
                    myGroup = (NetInfo.ConnectGroup)16;
                    variations = new List<string> { "_Merge", "_Merge" }.ToArray();
                    groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)32 }.ToArray();
                }
                else
                {
                    myGroup = NetInfo.ConnectGroup.CenterTram;
                    variations = new List<string> { "", "_Merge" }.ToArray();
                    groups = new List<NetInfo.ConnectGroup> { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)32 }.ToArray();
                }
            }
            nodes1.m_directConnect = true;
            nodes2.m_directConnect = true;
            nodes1.m_connectGroup = myGroup;
            nodes2.m_connectGroup = myGroup;
            for (var i = 0; i < variations.Length; i++)
            {
                var node1 = info.m_nodes[1].ShallowClone();
                var node2 = info.m_nodes[1].ShallowClone();
                var node3 = info.m_nodes[pavementIndex].ShallowClone();
                var node4 = info.m_nodes[pavementIndex].ShallowClone();
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
                    .SetMeshes
                    ($@"Meshes\{width}\LevelCrossing_Rail_Insert{variations[i]}_Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                node4
                    .SetMeshes
                    ($@"Meshes\{width}\LevelCrossing_Rail_Insert{variations[i]}_End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                node1.m_directConnect = true;
                node2.m_directConnect = true;
                node3.m_directConnect = true;
                node4.m_directConnect = true;
                node1.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                node2.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
                node3.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayStart;
                node4.m_connectGroup = groups[i] | NetInfo.ConnectGroup.OnewayEnd;
                var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
                node3.m_material = brElInfo.m_segments[0].m_material;
                node3.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;
                node4.m_material = brElInfo.m_segments[0].m_material;
                node4.m_lodMaterial = brElInfo.m_segments[0].m_lodMaterial;

            }
            return nodeList;
        }
        public static void Setup6mSteelMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var trainTrackMaterial = trainTrackInfo.m_nodes[0].m_material;
            var trainTrackLODMaterial = elevatedInfo.m_nodes[0].m_lodMaterial;
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var defaultElMaterial = brElInfo.m_segments[0].m_material;
            var defaultElLODMaterial = brElInfo.m_segments[0].m_lodMaterial;
            var isTwoWay = info.name.Contains("Two-Way");
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();

                        var segments3 = info.m_segments[1].ShallowClone();
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
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        nodeList.AddRange(GenerateLevelCrossing(info));

                        info.m_segments = new[] { segments0, segments1, /*segments2*/segments3 };
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
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node10 = info.m_nodes[0].ShallowClone();
                        var node11 = info.m_nodes[0].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();

                        if (isTwoWay)
                        {
                            node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | (NetInfo.ConnectGroup)16;
                            node10.m_connectGroup = NetInfo.ConnectGroup.CenterTram | (NetInfo.ConnectGroup)16;
                        }
                        else
                        {
                            node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                            node10.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        }

                        if (isTwoWay)
                        {
                            node1.m_connectGroup |= (NetInfo.ConnectGroup)16;

                        }
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Pavement_Steel_LOD.obj")
                                .SetConsistentUVs();

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

                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_ThirdRail_Base.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();

                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition | NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");
                        node2
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node10
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node11
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Trans_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node10);
                        nodeList.Add(node11);

                        RoadHelper.HandleAsymSegmentFlags(segment2);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;

                        node10.m_material = elevatedMaterial;
                        node10.m_lodMaterial = elevatedLODMaterial;
                        node10.m_directConnect = true;
                        node11.m_material = elevatedMaterial;
                        node11.m_lodMaterial = elevatedLODMaterial;
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();

                        //var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();
                        var node9 = info.m_nodes[0].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        //nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node9);

                        if (isTwoWay)
                        {
                            node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | (NetInfo.ConnectGroup)16;
                            node9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | (NetInfo.ConnectGroup)16;
                        }
                        else
                        {
                            node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                            node9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        }

                        segment1
                            .SetFlagsDefault()
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
                            (@"Meshes\6m\Elevated_ThirdRail_Base.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();

                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");
                        node2
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node3
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Trans_Pavement_Steel.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node9
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();

                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        node9.m_material = elevatedMaterial;
                        node9.m_lodMaterial = elevatedLODMaterial;
                        node9.m_directConnect = true;
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        RoadHelper.HandleAsymSegmentFlags(segment2);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        info.m_segments = new[] { segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[3].ShallowClone();
                        var segment4 = info.m_segments[3].ShallowClone();
                        var segment5 = info.m_segments[3].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = info.m_nodes[1].ShallowClone();
                        var node3 = info.m_nodes[3].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node5);

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
                            (@"Meshes\6m\Slope_Pavement_Steel.obj",
                                @"Meshes\6m\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Slope_Pavement_Steel_Ground.obj",
                                @"Meshes\10m\Blank.obj");
                        segment5
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\6m\Slope_Node_Pavement_Steel.obj",
                                @"Meshes\6m\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Node_Pavement_Steel.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        segment4.m_material = elevatedMaterial;
                        segment4.m_lodMaterial = elevatedLODMaterial;
                        segment5.m_material = elevatedMaterial;
                        segment5.m_lodMaterial = elevatedLODMaterial;
                        node2.m_connectGroup = isTwoWay ? (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.CenterTram : NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;
                        RoadHelper.HandleAsymSegmentFlags(segment5);
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4, segment5 };
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
                        node3.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        if (isTwoWay)
                        {
                            node3.m_connectGroup |= (NetInfo.ConnectGroup)16;
                        }
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement_Steel.obj",
                                @"Meshes\6m\Tunnel_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement_Steel_Ground.obj",
                                @"Meshes\6m\Blank.obj");
                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Node_Pavement_Steel.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                                ($@"Meshes\6m\Tunnel_Trans_Pavement_Steel.obj",
                                $@"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
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
                        node3.m_connectGroup = isTwoWay ? (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.CenterTram : NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        node3.m_directConnect = true;
                        RoadHelper.HandleAsymSegmentFlags(segment1);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        RoadHelper.HandleAsymSegmentFlags(segment4);
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        info.m_nodes = nodeList.ToArray();
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
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
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
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
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
                case NetInfoVersion.Bridge:
                    {

                        var segment = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var node0 = elevatedInfo.m_nodes[0].ShallowClone();
                        var node1 = elevatedInfo.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\6m\Bridge_Fence_Steel.obj",
                            $@"Meshes\6m\Blank.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\6m\Bridge_Pavement_Steel.obj",
                            $@"Meshes\6m\Bridge_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\6m\Bridge_Node_Fence_Steel.obj",
                            $@"Meshes\6m\Blank.obj");
                        node1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            ($@"Meshes\6m\Bridge_Node_Pavement_Steel.obj",
                            $@"Meshes\6m\Bridge_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment, segment1);
                        info.m_nodes = info.AddNodes(node0, node1);
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var node = info.m_nodes[0].ShallowClone();
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Underground)
                            .SetMeshes
                            (@"Meshes\6m\Slope_Node_Pavement_Steel_Fence.obj",
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
                            (@"Meshes\6m\Elevated_Bar_Steel.obj",
                                @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Bar_Steel.obj",
                                @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();

                        info.m_segments = info.AddSegments(segment0);
                        info.m_nodes = info.AddNodes(node0);
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var node0 = trainTrackInfo.m_nodes[0].ShallowClone();
                        var node1 = trainTrackInfo.m_nodes[0].ShallowClone();
                        var node2 = trainTrackInfo.m_nodes[0].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\6m\Bridge_Pavement_Steel2.obj",
                            $@"Meshes\6m\Bridge_Pavement_Steel2_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition | NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\6m\Bridge_Node_Pavement_Steel2.obj",
                            $@"Meshes\6m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node1
                           .SetFlags(NetNode.Flags.Transition, NetNode.Flags.LevelCrossing)
                           .SetMeshes
                           ($@"Meshes\6m\Elevated_Trans_Pavement_Steel.obj",
                           $@"Meshes\6m\Bridge_Node_Pavement_Steel_LOD.obj")
                           .SetConsistentUVs();
                        node2
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Bar_Steel.obj",
                                @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment0);
                        info.m_nodes = info.AddNodes(node0, node1, node2);
                        break;
                    }
            }
        }
        public static void Setup6mStationSteelMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var isTwoWay = prefab.name.Contains("Two-Way");
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
                                @"Meshes\6m\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();

                        node0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Pavement.obj",
                                @"Meshes\6m\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj")
                            .SetConsistentUVs();

                        RoadHelper.HandleAsymSegmentFlags(segment2);

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        nodeList.AddRange(GenerateSplitTracks(prefab, version));
                        nodeList.AddRange(GenerateLevelCrossing(prefab));

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        prefab.m_segments = new[] { segment0, segment1, segment2 };

                        prefab.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {

                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var segment3 = prefab.m_segments[0].ShallowClone();

                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[0].ShallowClone();
                        //var node3 = prefab.m_nodes[3].ShallowClone();

                        var node10 = prefab.m_nodes[0].ShallowClone();
                        var node11 = prefab.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        //nodeList.Add(node3);
                        nodeList.Add(node10);
                        nodeList.Add(node11);
                        node1.m_connectGroup = (NetInfo.ConnectGroup)16;
                        node2.m_connectGroup = (NetInfo.ConnectGroup)16;
                        node10.m_connectGroup = (NetInfo.ConnectGroup)16;
                        
                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Station_Pavement_LOD.obj");
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

                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_ThirdRail_Base.obj", @"Meshes\6m\Blank.obj")
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
                        node2
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node10
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();

                        node11
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Trans_Pavement_Steel.obj",
                                @"Meshes\6m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        RoadHelper.HandleAsymSegmentFlags(segment2);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;

                        node10.m_material = elevatedMaterial;
                        node10.m_lodMaterial = elevatedLODMaterial;
                        node10.m_directConnect = true;

                        node11.m_material = elevatedMaterial;
                        node11.m_lodMaterial = elevatedLODMaterial;

                        nodeList.AddRange(GenerateSplitTracks(prefab, version));
                        nodeList.AddRange(GenerateLevelCrossing(prefab));
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        prefab.m_nodes = nodeList.ToArray();
                        break;
                    }
            }
        }
    }
}
