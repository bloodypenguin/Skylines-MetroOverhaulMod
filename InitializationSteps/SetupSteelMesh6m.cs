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
            var ttInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            var defaultMaterial = ttInfo.m_nodes[0].m_material;
            var defaultLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var is10m = info.name.Contains("Two-Lane");
            var nodeList = new List<NetInfo.Node>();
            var isTwoWay = info.name.Contains("Two-Way") || info.name.Contains("Station") || is10m;
            var widthName = is10m ? "10m" : "6m";
            var mergeName = isTwoWay ? "Merge_" : "";
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var nodes6 = info.m_nodes[1].ShallowClone();
                        var nodes7 = info.m_nodes[1].ShallowClone();
                        nodeList.Add(nodes6);
                        nodeList.Add(nodes7);
                        nodes6
                            .SetMeshes
                                ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}Start.obj",
                               @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                        .SetConsistentUVs();
                        nodes7
                            .SetMeshes
                                ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}End.obj",
                                @"Meshes\6m\Ground_Rail_End_LOD.obj")
                        .SetConsistentUVs();
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        if (is10m)
                        {
                            var nodes8 = info.m_nodes[1].ShallowClone();
                            var nodes9 = info.m_nodes[1].ShallowClone();
                            var nodes10 = info.m_nodes[1].ShallowClone();
                            var nodes11 = info.m_nodes[1].ShallowClone();
                            nodeList.Add(nodes8);
                            nodeList.Add(nodes9);
                            nodeList.Add(nodes10);
                            nodeList.Add(nodes11);
                            nodes8
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                                  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                                 .SetConsistentUVs();
                            nodes9
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                                   @"Meshes\6m\Ground_Rail_End_LOD.obj")
                                  .SetConsistentUVs();
                            nodes10
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                                  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                                 .SetConsistentUVs();
                            nodes11
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                                   @"Meshes\6m\Ground_Rail_End_LOD.obj")
                                  .SetConsistentUVs();
                            nodes8.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                            nodes9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                            nodes8.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                            nodes9.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                        }
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {

                        var node12 = info.m_nodes[1].ShallowClone();
                        var node13 = info.m_nodes[1].ShallowClone();
                        var node14 = info.m_nodes[0].ShallowClone();
                        var node15 = info.m_nodes[0].ShallowClone();
                        var node16 = info.m_nodes[0].ShallowClone();
                        var node17 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node12);
                        nodeList.Add(node13);
                        nodeList.Add(node14);
                        nodeList.Add(node15);
                        nodeList.Add(node16);
                        nodeList.Add(node17);

                        node12.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        node13.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        node14.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        node15.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        node16.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        node17.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

                        node12
                            .SetMeshes
                            ($@"Meshes\{widthName}\Ground_Rail_{mergeName}Start.obj",
                            @"Meshes\6m\Ground_Rail_Start_LOD.obj");
                        node13
                            .SetMeshes
                            ($@"Meshes\{widthName}\Ground_Rail_{mergeName}End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj");
                        node14
                            .SetMeshes
                            ($@"Meshes\{widthName}\Elevated_Node_Pavement_Steel_Insert_{mergeName}Start.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node15
                            .SetMeshes
                            ($@"Meshes\{widthName}\Elevated_Node_Pavement_Steel_Insert_{mergeName}End.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node16
                            .SetMeshes
                            ($@"Meshes\{widthName}\Boosted_Rail_Steel_Insert_{mergeName}Start.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node17
                            .SetMeshes
                            ($@"Meshes\{widthName}\Boosted_Rail_Steel_Insert_{mergeName}End.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        if (is10m)
                        {
                            var node18 = info.m_nodes[1].ShallowClone();
                            var node19 = info.m_nodes[1].ShallowClone();
                            var node20 = info.m_nodes[0].ShallowClone();
                            var node21 = info.m_nodes[0].ShallowClone();
                            var node22 = info.m_nodes[0].ShallowClone();
                            var node23 = info.m_nodes[0].ShallowClone();

                            var node24 = info.m_nodes[1].ShallowClone();
                            var node25 = info.m_nodes[1].ShallowClone();
                            var node26 = info.m_nodes[0].ShallowClone();
                            var node27 = info.m_nodes[0].ShallowClone();
                            var node28 = info.m_nodes[0].ShallowClone();
                            var node29 = info.m_nodes[0].ShallowClone();

                            nodeList.Add(node18);
                            nodeList.Add(node19);
                            nodeList.Add(node20);
                            nodeList.Add(node21);
                            nodeList.Add(node22);
                            nodeList.Add(node23);

                            nodeList.Add(node24);
                            nodeList.Add(node25);
                            nodeList.Add(node26);
                            nodeList.Add(node27);
                            nodeList.Add(node28);
                            nodeList.Add(node29);

                            node18
                                .SetMeshes
                                ($@"Meshes\6m\Ground_Rail_Merge_Start.obj",
                                @"Meshes\6m\Ground_Rail_Start_LOD.obj");
                            node19
                                .SetMeshes
                                ($@"Meshes\6m\Ground_Rail_Merge_End.obj",
                                @"Meshes\6m\Ground_Rail_End_LOD.obj");
                            node20
                                .SetMeshes
                                ($@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert_Merge_Start.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                            node21
                                .SetMeshes
                                ($@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert_Merge_End.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                            node22
                                .SetMeshes
                                ($@"Meshes\6m\Boosted_Rail_Steel_Insert_Merge_Start.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                            node23
                                .SetMeshes
                                ($@"Meshes\6m\Boosted_Rail_Steel_Insert_Merge_End.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();

                            node24
                                .SetMeshes
                                ($@"Meshes\6m\Ground_Rail_Merge_Start.obj",
                                @"Meshes\6m\Ground_Rail_Start_LOD.obj");
                            node25
                                .SetMeshes
                                ($@"Meshes\6m\Ground_Rail_Merge_End.obj",
                                @"Meshes\6m\Ground_Rail_End_LOD.obj");
                            node26
                                .SetMeshes
                                ($@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert_Merge_Start.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                            node27
                                .SetMeshes
                                ($@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert_Merge_End.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                            node28
                                .SetMeshes
                                ($@"Meshes\6m\Boosted_Rail_Steel_Insert_Merge_Start.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();
                            node29
                                .SetMeshes
                                ($@"Meshes\6m\Boosted_Rail_Steel_Insert_Merge_End.obj", @"Meshes\6m\Blank.obj")
                                .SetConsistentUVs();

                            node18.m_connectGroup = NetInfo.ConnectGroup.CenterTram| NetInfo.ConnectGroup.OnewayEnd;
                            node19.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                            node20.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                            node21.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                            node22.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                            node23.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;

                            node24.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                            node25.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                            node26.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                            node27.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                            node28.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                            node29.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;

                            node20.m_directConnect = true;
                            node21.m_directConnect = true;
                            node22.m_directConnect = true;
                            node23.m_directConnect = true;

                            node26.m_directConnect = true;
                            node27.m_directConnect = true;
                            node28.m_directConnect = true;
                            node29.m_directConnect = true;
                        }
                        node14.m_directConnect = true;
                        node15.m_directConnect = true;
                        node16.m_directConnect = true;
                        node17.m_directConnect = true;
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var nodes6 = info.m_nodes[1].ShallowClone();
                        var nodes7 = info.m_nodes[1].ShallowClone();
                    
                    nodeList.Add(nodes6);
                    nodeList.Add(nodes7);
                    nodes6
                        .SetMeshes
                           ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}Start.obj",
                          @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                    .SetConsistentUVs();
                    nodes7
                        .SetMeshes
                           ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}End.obj",
                           @"Meshes\6m\Ground_Rail_End_LOD.obj")
                    .SetConsistentUVs();
                        if (is10m)
                        {
                            var nodes8 = info.m_nodes[1].ShallowClone();
                            var nodes9 = info.m_nodes[1].ShallowClone();
                            var nodes10 = info.m_nodes[1].ShallowClone();
                            var nodes11 = info.m_nodes[1].ShallowClone();
                            nodeList.Add(nodes8);
                            nodeList.Add(nodes9);
                            nodeList.Add(nodes10);
                            nodeList.Add(nodes11);
                            nodes8
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                                  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                                 .SetConsistentUVs();
                            nodes9
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                                   @"Meshes\6m\Ground_Rail_End_LOD.obj")
                                  .SetConsistentUVs();
                            nodes10
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                                  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                                 .SetConsistentUVs();
                            nodes11
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                                   @"Meshes\6m\Ground_Rail_End_LOD.obj")
                                  .SetConsistentUVs();
                            nodes8.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                            nodes9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                            nodes10.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                            nodes11.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                        }
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                    nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                    break;
            }
                case NetInfoVersion.Tunnel:
                    {
                        var nodes6 = info.m_nodes[0].ShallowClone();
                        var nodes7 = info.m_nodes[0].ShallowClone();

                        nodes6.m_material = defaultMaterial;
                        nodes6.m_lodMaterial = defaultLODMaterial;
                        nodes6.m_directConnect = true;
                        nodes7.m_material = defaultMaterial;
                        nodes7.m_lodMaterial = defaultLODMaterial;
                        nodes7.m_directConnect = true;

                        nodeList.Add(nodes6);
                        nodeList.Add(nodes7);
                        nodes6
                            .SetMeshes
                               ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}Start.obj",
                              @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                        .SetConsistentUVs();
                        nodes7
                            .SetMeshes
                               ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}End.obj",
                               @"Meshes\6m\Ground_Rail_End_LOD.obj")
                        .SetConsistentUVs();

                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        if (is10m)
                        {
                            var nodes8 = info.m_nodes[1].ShallowClone();
                            var nodes9 = info.m_nodes[1].ShallowClone();
                            var nodes10 = info.m_nodes[1].ShallowClone();
                            var nodes11 = info.m_nodes[1].ShallowClone();
                            nodeList.Add(nodes8);
                            nodeList.Add(nodes9);
                            nodeList.Add(nodes10);
                            nodeList.Add(nodes11);
                            nodes8
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                                  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                                 .SetConsistentUVs();
                            nodes9
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                                   @"Meshes\6m\Ground_Rail_End_LOD.obj")
                                  .SetConsistentUVs();
                            nodes10
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                                  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                                 .SetConsistentUVs();
                            nodes11
                                .SetMeshes
                                   ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                                   @"Meshes\6m\Ground_Rail_End_LOD.obj")
                                  .SetConsistentUVs();

                            nodes8.m_material = defaultMaterial;
                            nodes8.m_lodMaterial = defaultLODMaterial;
                            nodes8.m_directConnect = true;
                            nodes9.m_material = defaultMaterial;
                            nodes9.m_lodMaterial = defaultLODMaterial;
                            nodes9.m_directConnect = true;
                            nodes10.m_material = defaultMaterial;
                            nodes10.m_lodMaterial = defaultLODMaterial;
                            nodes10.m_directConnect = true;
                            nodes11.m_material = defaultMaterial;
                            nodes11.m_lodMaterial = defaultLODMaterial;
                            nodes11.m_directConnect = true;

                            nodes8.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                            nodes9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                            nodes10.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                            nodes11.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                        }
                        break;
                    }                            
            }
            return nodeList;
        }
        public static List<NetInfo.Node> GenerateLevelCrossing(NetInfo info)
        {
            var is10m = info.name.Contains("Two-Lane");
            var isMerge = info.name.Contains("Two-Way") || info.name.Contains("Station") || is10m;
            var mergeName = isMerge ? "Merge_" : "";
            var lcName = isMerge ? "" : "Level_Crossing_";
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
            var nodes3 = info.m_nodes[1].ShallowClone();
            var nodes4 = info.m_nodes[1].ShallowClone();
            var nodes5 = info.m_nodes[pavementIndex].ShallowClone();
            var nodes6 = info.m_nodes[pavementIndex].ShallowClone();

            nodeList.Add(nodes0);
            nodeList.Add(nodes1);
            nodeList.Add(nodes2);
            nodeList.Add(nodes3);
            nodeList.Add(nodes4);
            nodeList.Add(nodes5);
            nodeList.Add(nodes6);
            nodes0
                .SetMeshes
                (@"Meshes\6m\Ground_Level_Crossing.obj",
                @"Meshes\6m\Ground_Level_Crossing_LOD.obj");
            nodes1
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                (@"Meshes\6m\Ground_Level_Crossing_Rail.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes2
                .SetMeshes
                (@"Meshes\6m\Ground_Level_Crossing_Rail_Insert.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes3
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                ($@"Meshes\6m\Ground_{lcName}Rail_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes4
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                ($@"Meshes\6m\Ground_{lcName}Rail_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes5
                .SetMeshes
                ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes6
                .SetMeshes
                ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();

            if (is10m)
            {
                var nodes7 = info.m_nodes[1].ShallowClone();
                var nodes8 = info.m_nodes[1].ShallowClone();
                var nodes9 = info.m_nodes[pavementIndex].ShallowClone();
                var nodes10 = info.m_nodes[pavementIndex].ShallowClone();
                var nodes11 = info.m_nodes[1].ShallowClone();
                var nodes12 = info.m_nodes[1].ShallowClone();
                var nodes13 = info.m_nodes[pavementIndex].ShallowClone();
                var nodes14 = info.m_nodes[pavementIndex].ShallowClone();

                nodeList.Add(nodes7);
                nodeList.Add(nodes8);
                nodeList.Add(nodes9);
                nodeList.Add(nodes10);
                nodeList.Add(nodes11);
                nodeList.Add(nodes12);
                nodeList.Add(nodes13);
                nodeList.Add(nodes14);

                nodes7
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes8
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes9
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_Insert_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes10
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_Insert_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();

                nodes11
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes12
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes13
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_Insert_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes14
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_Insert_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();

                nodes7.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                nodes8.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                nodes9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                nodes10.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;

                nodes11.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                nodes12.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                nodes13.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                nodes14.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;

                nodes9.m_directConnect = true;
                nodes10.m_directConnect = true;
                nodes13.m_directConnect = true;
                nodes14.m_directConnect = true;
            }

            nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
            nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
            nodes3.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
            nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
            nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
            nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

            nodes5.m_directConnect = true;
            nodes6.m_directConnect = true;
            return nodeList;
        }
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

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

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

                        node1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        node10.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

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
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
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
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel_Trans.obj",
                                @"Meshes\6m\Elevated_Node_Pavement_Steel_LOD.obj")
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
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();

                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[1].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node9 = info.m_nodes[0].ShallowClone();
                        var node10 = info.m_nodes[0].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node9);
                        nodeList.Add(node10);

                        node1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        node9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

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
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_ThirdRail_Base.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();

                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
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

                        node9
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();

                        node10
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel_Trans.obj",
                                @"Meshes\6m\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node2.m_directConnect = true;
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;

                        node9.m_material = elevatedMaterial;
                        node9.m_lodMaterial = elevatedLODMaterial;
                        node9.m_directConnect = true;
                        node10.m_material = elevatedMaterial;
                        node10.m_lodMaterial = elevatedLODMaterial;
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        RoadHelper.HandleAsymSegmentFlags(segment2);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
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
                        node1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

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
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);

                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement_Steel.obj",
                                @"Meshes\6m\Tunnel_Pavement_LOD.obj")
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
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Node_Pavement_Steel.obj",
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
                        segment4.m_material = elevatedMaterial;
                        segment4.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;
                        RoadHelper.HandleAsymSegmentFlags(segment3);
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
                            .SetConsistentUVs();

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
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        node1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

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
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");

                        RoadHelper.HandleAsymSegmentFlags(segment2);

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

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
                        var node3 = prefab.m_nodes[3].ShallowClone();

                        var node10 = prefab.m_nodes[0].ShallowClone();
                        var node11 = prefab.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        nodeList.Add(node3);
                        nodeList.Add(node10);
                        nodeList.Add(node11);
                        node1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;

                        node10.m_connectGroup = NetInfo.ConnectGroup.CenterTram;

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
                            (@"Meshes\6m\Elevated_Node_Pavement_Steel_Trans.obj",
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
