using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Math;
using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT;
using MetroOverhaul.OptionsFramework;
using UnityEngine;

namespace MetroOverhaul
{
    public class AssetsUpdater
    {
        private static NetManager ninstance = Singleton<NetManager>.instance;
        private static BuildingManager binstance = Singleton<BuildingManager>.instance;
        private static TerrainManager tinstance = Singleton<TerrainManager>.instance;
        public void UpdateExistingAssets(LoadMode mode)
        {
            UpdateMetroTrainEffects();
            if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
                return;
            }
            try
            {
                UpdateVanillaMetroTracks();
                UpdateMetroStationsMeta();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        //        private static void UpdateTrainTracks()
        //        {
        //            var vanillaTracksNames = new[] { "Train Track", "Train Track Elevated", "Train Track Bridge", "Train Track Slope", "Train Track Tunnel" };
        //            var vanillaTracksCosts = vanillaTracksNames.ToDictionary(Initializer.DetectVersion, GetTrackCost);
        //            var toGroundMultipliers = vanillaTracksCosts.ToDictionary(keyValue => keyValue.Key,
        //                keyValue => keyValue.Value == vanillaTracksCosts[NetInfoVersion.Ground] ? 1f : keyValue.Value / (float)vanillaTracksCosts[NetInfoVersion.Ground]);

        //            var baseMultiplier = GetTrackCost("Metro Track Ground") / (float)GetTrackCost("Train Track");
        //            for (ushort i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
        //            {
        //                var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
        //                var ai = netInfo?.m_netAI as PlayerNetAI;
        //                if (ai == null || netInfo.m_class.m_service != ItemClass.Service.PublicTransport || netInfo.m_class.m_subService != ItemClass.SubService.PublicTransportTrain)
        //                {
        //                    continue;
        //                }
        //                var version = Initializer.DetectVersion(netInfo.name);
        //                var wasCost = GetTrackCost(netInfo);
        //                if (wasCost == 0)
        //                {
        //                    continue;
        //                }
        //                var newCost = wasCost / toGroundMultipliers[version] *
        //                                     Initializer.GetCostMultiplier(version) * GetAdditionalCostMultiplier(version) * baseMultiplier;
        //#if DEBUG
        //                UnityEngine.Debug.Log($"Updating asset {netInfo.name} cost. Was cost: {wasCost}. New cost: {newCost}");
        //#endif
        //                ai.m_constructionCost = (int)newCost;
        //                ai.m_maintenanceCost = (int)(newCost / 10f);
        //            }
        //        }

        //private static float GetAdditionalCostMultiplier(NetInfoVersion version)
        //{
        //    return (version == NetInfoVersion.Tunnel || version == NetInfoVersion.Slope || version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge) ? 1.5f : 1.0f;
        //}

        //private static int GetTrackCost(string prefabName)
        //{
        //    var netInfo = PrefabCollection<NetInfo>.FindLoaded(prefabName);
        //    return GetTrackCost(netInfo);
        //}

        //private static int GetTrackCost(NetInfo netInfo)
        //{
        //    return ((PlayerNetAI)netInfo.m_netAI).m_constructionCost;
        //}

        //this method is supposed to be called from LoadingExtension
        public static void UpdateBuildingsMetroPaths(LoadMode mode, bool toVanilla = false)
        {
#if !DEBUG
            if (mode == LoadMode.NewAsset || mode == LoadMode.NewAsset)
            {
                return;
            }
#endif

            try
            {
                for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
                {
                    var prefab = PrefabCollection<BuildingInfo>.GetPrefab(i);
                    if (prefab == null || !(prefab.m_buildingAI is DepotAI))
                    {
                        continue;
                    }
                    if (!toVanilla)
                    {
                        if (!OptionsWrapper<Options>.Options.metroUi)
                        {
                            SetStationCustomizations.ModifyStation(prefab, SetStationCustomizations.DEF_DEPTH, SetStationCustomizations.DEF_LENGTH, SetStationCustomizations.DEF_ANGLE, SetStationCustomizations.DEF_BEND_STRENGTH);
                        }
                    }
                    SetupTunnelTracks(prefab, toVanilla);
                }
                for (ushort i = 0; i < Singleton<NetManager>.instance.m_nodes.m_buffer.Count(); i++)
                {
                    NetNode n = Singleton<NetManager>.instance.m_nodes.m_buffer[i];
                    NetInfo info = n.Info;
                    if (info == null)
                    {
                        continue;
                    }
                    bool proceed = false;
                    if (m_NeedsConvert && (info.IsUndergroundMetroTrack() || info.name == "Metro Track"))
                    {
                        proceed = true;
                    }
                    else if (toVanilla && info.IsUndergroundMetroTrack())
                    {
                        proceed = true;
                    }
                    else if (!toVanilla)
                    {
                        if ((info.name == "Metro Track"))
                        {
                            proceed = true;
                        }
                        else if (info.IsUndergroundMetroTrack() && tinstance.SampleRawHeightSmooth(n.m_position) < n.m_position.y + 8)
                        {
                            proceed = true;
                        }
                    }
                    if (proceed)
                    {
                        m_NeedsConvert = true;
                        DipPath(i, n, toVanilla);
                    }
                }
                if (m_NeedsConvert)
                {
                    for (ushort i = 0; i < binstance.m_buildings.m_buffer.Count(); i++)
                    {
                        Building b = BuildingFrom(i);
                        BuildingInfo info = b.Info;
                        if (b.m_parentBuilding == 0)
                        {
                            if (info != null && info.m_buildingAI is DepotAI)
                            {
                                connectList = null;
                                if (info.m_subBuildings != null && info.m_subBuildings.Any(sb => sb != null && sb.m_buildingInfo != null && HasStationTracks(sb.m_buildingInfo)))
                                {
                                    ushort subBuildingID = b.m_subBuilding;
                                    while (subBuildingID > 0)
                                    {
                                        if (HasStationTracks(BuildingFrom(subBuildingID).Info))
                                            UpdateBuilding(subBuildingID, info);
                                        subBuildingID = BuildingFrom(subBuildingID).m_subBuilding;
                                    }
                                }
                                if (HasUndergroundMOMorVanilla(i, true))
                                {
                                    UpdateBuilding(i);
                                }
                            }
                        }
                    }

                    if (toVanilla)
                    {
                        for (ushort i = 0; i < ninstance.m_nodes.m_buffer.Count(); i++)
                        {
                            NetNode node = NodeFrom(i);
                            if (node.Info.IsMetroTrack())
                            {
                                ninstance.m_nodes.m_buffer[i].Info = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                                ninstance.UpdateNode(i);
                            }
                            else if (node.Info.IsMetroStationTrack())
                            {
                                ninstance.m_nodes.m_buffer[i].Info = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
                                ninstance.UpdateNode(i);
                            }
                        }
                        for (ushort i = 0; i < ninstance.m_segments.m_buffer.Count(); i++)
                        {
                            NetSegment segment = SegmentFrom(i);
                            if (segment.Info.IsMetroTrack())
                            {
                                ninstance.m_segments.m_buffer[i].Info = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                                ninstance.UpdateSegment(i);
                            }
                            else if (segment.Info.IsMetroStationTrack())
                            {
                                ninstance.m_segments.m_buffer[i].Info = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
                                ninstance.UpdateSegment(i);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
        private static bool HasStationTracks(BuildingInfo info)
        {
            bool retval = false;
            if (OptionsWrapper<Options>.Options.ghostMode)
            {
                retval = info.m_paths != null && info.m_paths.Any(p => p != null && p.m_netInfo.name == "Metro Station Track");
            }
            else
            {
                retval = info.HasUndergroundMetroStationTracks();
            }
            return retval;
        }
        private static bool IsStationTrack(NetInfo info)
        {
            bool retval = false;
            if (OptionsWrapper<Options>.Options.ghostMode)
            {
                retval = info != null && info.name == "Metro Station Track";
            }
            else
            {
                retval = info.IsUndergroundMetroStationTrack();
            }
            return retval;
        }
        private static bool HasUndergroundMOMorVanilla(ushort buildingID, bool isDeep = true)
        {
            Building building = BuildingFrom(buildingID);
            BuildingInfo info = building.Info;
            if (info != null)
            {
                if (info.m_paths != null)
                {
                    for (int i = 0; i < info.m_paths.Count(); i++)
                    {
                        NetInfo ninfo = info.m_paths[i]?.m_netInfo;
                        if (ninfo != null && (ninfo.IsUndergroundMetroStationTrack() || ninfo.name == "Metro Station Track"))
                        {
                            return true;
                        }
                    }
                }
                if (isDeep)
                {
                    if (info.m_subBuildings != null && info.m_subBuildings.Count() > 0)
                    {
                        for (int i = 0; i < info.m_subBuildings.Count(); i++)
                        {
                            BuildingInfo sinfo = info.m_subBuildings[i]?.m_buildingInfo;
                            if (sinfo != null && sinfo.m_paths != null)
                            {
                                for (int j = 0; j < sinfo.m_paths.Count(); j++)
                                {
                                    NetInfo sninfo = sinfo.m_paths[j]?.m_netInfo;
                                    if (sninfo != null && (sninfo.IsUndergroundMetroStationTrack() || sninfo.name == "Metro Station Track"))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private static void PrepareBuilding(ref BuildingInfo info)
        {
            m_CpmNetDict = null;
            RemoveCreatePassMileStone(ref info);
            for (int j = 0; j < info.m_paths.Count(); j++)
            {
                BuildingInfo.PathInfo path = info.m_paths[j];
                NetInfo nInfo = path.m_netInfo;
                if (nInfo.name == "Metro Station Track" || nInfo.IsUndergroundMetroStationTrack())
                {
                    if (nInfo.m_netAI == null)
                    {
                        var ai = nInfo.gameObject.AddComponent<MetroTrackAI>();
                        ai.m_info = nInfo;
                        nInfo.m_netAI = ai;
                    }
                    if (OptionsWrapper<Options>.Options.ghostMode)
                    {
                        info.m_paths[j].m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
                    }
                    else
                    {
                        info.m_paths[j].m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
                    }
                }
            }


        }
        private static void RemoveCreatePassMileStone(ref BuildingInfo info)
        {
            PlayerBuildingAI pbai = info.GetComponent<PlayerBuildingAI>();
            if (pbai != null)
            {
                if (pbai.m_createPassMilestone != null)
                {
                    if (m_CpmBuildingDict == null)
                    {
                        m_CpmBuildingDict = new Dictionary<string, List<ManualMilestone>>();
                    }
                    if (m_CpmBuildingDict.ContainsKey(info.name) == false)
                        m_CpmBuildingDict.Add(info.name, new List<ManualMilestone>());
                    m_CpmBuildingDict[info.name].Add(pbai.m_createPassMilestone);
                    m_CpmBuildingDict[info.name].Add(pbai.m_createPassMilestone2);
                    m_CpmBuildingDict[info.name].Add(pbai.m_createPassMilestone3);
                    pbai.m_createPassMilestone = null;
                    pbai.m_createPassMilestone2 = null;
                    pbai.m_createPassMilestone3 = null;
                    info.m_buildingAI = pbai;
                }
                foreach (BuildingInfo.PathInfo path in info.m_paths)
                {
                    if (path.m_netInfo != null)
                        RemoveCreatePassMileStone(path.m_netInfo);
                }
                if (info.m_subBuildings != null)
                {
                    foreach (var subBuilding in info.m_subBuildings)
                    {
                        if (subBuilding.m_buildingInfo != null)
                        {
                            RemoveCreatePassMileStone(ref subBuilding.m_buildingInfo);
                        }
                    }
                }

            }
        }
        private static void RemoveCreatePassMileStone(NetInfo info)
        {
            PlayerNetAI pnai = info.GetComponent<PlayerNetAI>();
            if (pnai != null)
            {
                if (pnai.m_createPassMilestone != null)
                {
                    if (m_CpmNetDict == null)
                    {
                        m_CpmNetDict = new Dictionary<string, ManualMilestone>();
                    }
                    if (m_CpmNetDict.ContainsKey(info.name) == false)
                        m_CpmNetDict.Add(info.name, pnai.m_createPassMilestone);
                    pnai.m_createPassMilestone = null;
                }
            }
        }
        private static void RevertBuilding(ref BuildingInfo info)
        {
            if (m_CpmNetDict != null)
            {
                for (int j = 0; j < info.m_paths.Count(); j++)
                {
                    NetInfo ninfo = info.m_paths[j].m_netInfo;
                    if (m_CpmNetDict.ContainsKey(ninfo.name))
                    {
                        if (ninfo.m_netAI != null)
                        {
                            PlayerNetAI pnai = ninfo.GetComponent<PlayerNetAI>();
                            if (pnai != null)
                            {
                                pnai.m_createPassMilestone = m_CpmNetDict[ninfo.name];
                            }
                        }
                    }
                }
            }
            if (m_CpmBuildingDict != null)
            {
                if (m_CpmBuildingDict.ContainsKey(info.name))
                {
                    PlayerBuildingAI pbai = info.GetComponent<PlayerBuildingAI>();
                    if (pbai != null)
                    {
                        pbai.m_createPassMilestone = m_CpmBuildingDict[info.name][0];
                        pbai.m_createPassMilestone2 = m_CpmBuildingDict[info.name][1];
                        pbai.m_createPassMilestone3 = m_CpmBuildingDict[info.name][2];
                        info.m_buildingAI = pbai;
                        if (info.m_subBuildings != null && info.m_subBuildings.Count() > 0)
                        {
                            foreach (BuildingInfo.SubInfo subBuilding in info.m_subBuildings)
                            {
                                if (subBuilding.m_buildingInfo != null)
                                    RevertBuilding(ref subBuilding.m_buildingInfo);
                            }
                        }
                    }
                }
            }
        }
        private static Dictionary<string, ManualMilestone> m_CpmNetDict = null;
        private static Dictionary<string, List<ManualMilestone>> m_CpmBuildingDict = null;
        private static bool m_NeedsConvert = false;
        private static void UpdateBuilding(ushort buildingID, BuildingInfo superInfo = null)
        {
            Building building = BuildingFrom(buildingID);
            BuildingInfo info = building.Info;

            PopulateDictionaries(buildingID);
            PrepareBuilding(ref info);
            if (!OptionsWrapper<Options>.Options.ghostMode && HasUndergroundMOMorVanilla(buildingID, false))
            {
                Debug.Log($"Info={info.name}");
                if (superInfo != null)
                {
                    Debug.Log($"Super={superInfo.name}");
                }
                else
                {
                    Debug.Log("Super=Info");
                }

                SetStationCustomizations.ModifyStation(info, SetStationCustomizations.DEF_DEPTH, SetStationCustomizations.MIN_LENGTH, SetStationCustomizations.DEF_ANGLE, SetStationCustomizations.DEF_BEND_STRENGTH, superInfo);
            }
            info = BuildingFrom(buildingID).Info;
            binstance.UpdateBuildingInfo(buildingID, info);
            RevertBuilding(ref info);
            ReconsileOrphanedSegments(buildingID);
        }

        private static Building BuildingFrom(ushort buildingID)
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
        }
        private static NetNode NodeFrom(ushort nodeID)
        {
            return Singleton<NetManager>.instance.m_nodes.m_buffer[nodeID];
        }
        private static NetSegment SegmentFrom(ushort segmentID)
        {
            return Singleton<NetManager>.instance.m_segments.m_buffer[segmentID];
        }
        private struct ConnectData
        {
            public ushort connectingSegment { get; set; }
            public ushort nonStationNodeID { get; set; }
            public Vector3 oldPosition { get; set; }
        }
        //private static Dictionary<ushort, KeyValuePair<Vector3, ushort>> ConnectMap { get; set; }

        private static List<ushort> GetMetroNodes(Building building)
        {
            ushort nodeID = building.m_netNode;
            List<ushort> stationNodeIDs = null;
            while (nodeID > 0)
            {
                NetNode node = NodeFrom(nodeID);
                if (node.Info != null)
                {
                    if (node.Info.IsUndergroundMetroStationTrack() || node.Info.name == "Metro Station Track" || node.Info.IsUndergroundMetroTrack() || node.Info.name == "Metro Track")
                    {
                        if (stationNodeIDs == null)
                        {
                            stationNodeIDs = new List<ushort>();
                        }
                        stationNodeIDs.Add(nodeID);
                    }
                    nodeID = NodeFrom(nodeID).m_nextBuildingNode;
                }
            }
            return stationNodeIDs;
        }
        private static List<ushort> GetStationNodes(Building building)
        {
            ushort nodeID = building.m_netNode;
            List<ushort> stationNodeIDs = null;
            while (nodeID > 0)
            {
                NetNode node = NodeFrom(nodeID);
                if (node.Info.IsUndergroundMetroStationTrack() || node.Info.name == "Metro Station Track")
                {
                    if (stationNodeIDs == null)
                    {
                        stationNodeIDs = new List<ushort>();
                    }
                    stationNodeIDs.Add(nodeID);
                }
                nodeID = NodeFrom(nodeID).m_nextBuildingNode;
            }
            return stationNodeIDs;
        }
        private static List<ConnectData> connectList;
        private static void PopulateDictionaries(ushort buildingID)
        {
            Building building = BuildingFrom(buildingID);
            List<ushort> nodeIDs = GetMetroNodes(building);
            if (nodeIDs != null)
            {
                for (int i = 0; i < nodeIDs.Count; i++)
                {
                    ushort nodeID = nodeIDs[i];
                    NetNode node = NodeFrom(nodeID);
                    var count = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        ushort segmentID = node.GetSegment(j);
                        if (segmentID > 0)
                        {
                            NetSegment segment = SegmentFrom(segmentID);
                            if (!nodeIDs.Contains(segment.m_startNode) || !nodeIDs.Contains(segment.m_endNode))
                            {
                                count++;
                                if (connectList == null)
                                {
                                    connectList = new List<ConnectData>();
                                }

                                ushort otherNodeID = segment.m_startNode == nodeID ? segment.m_endNode : segment.m_startNode;
                                if (otherNodeID > 0)
                                {
                                    ConnectData cd = new ConnectData()
                                    {
                                        nonStationNodeID = otherNodeID,
                                        oldPosition = node.m_position
                                    };
                                    if (!connectList.Contains(cd))
                                    {
                                        connectList.Add(cd);
                                    }
                                    ninstance.ReleaseSegment(segmentID, false);

                                }
                            }
                        }
                    }
                }
            }
        }
        private static void ReconsileOrphanedSegments(ushort buildingID)
        {
            if (connectList != null && connectList.Count > 0)
            {
                Building building = BuildingFrom(buildingID);
                List<ushort> stationNodeIDs = GetMetroNodes(building);
                if (stationNodeIDs != null)
                {
                    foreach (ushort sNodeID in stationNodeIDs)
                    {
                        NetNode sNode = NodeFrom(sNodeID);
                        for (var i = 0; i < connectList.Count(); i++)
                        {
                            ConnectData cd = connectList[i];
                            if (sNode.m_position.x == cd.oldPosition.x && /* node.m_position.y = kvp.key.y - DEF_Depth */sNode.m_position.z == cd.oldPosition.z)
                            {
                                ushort startNode = cd.nonStationNodeID;
                                ushort endNode = sNodeID;

                                ushort newSegmentID = 0;
                                if (startNode > 0 && CreateConnectionSegment(out newSegmentID, startNode, endNode))
                                    cd.connectingSegment = newSegmentID;
                                connectList[i] = cd;
                                ninstance.UpdateSegment(newSegmentID);
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < connectList.Count(); i++)
                    {
                        if (connectList[i].oldPosition != Vector3.zero && connectList[i].nonStationNodeID > 0 && connectList[i].connectingSegment == 0)
                        {
                            ushort closestNodeID = stationNodeIDs.OrderBy(snID => Vector3.Distance(NodeFrom(snID).m_position, connectList[i].oldPosition)).FirstOrDefault();
                            ushort startNode = connectList[i].nonStationNodeID;
                            ushort endNode = closestNodeID;

                            ushort newSegmentID = 0;
                            if (startNode > 0 && CreateConnectionSegment(out newSegmentID, startNode, endNode))
                                ninstance.UpdateSegment(newSegmentID);
                        }
                    }
                }

            }
        }
        private static bool CreateConnectionSegment(out ushort segment, ushort startNode, ushort endNode)
        {
            NetManager instance = Singleton<NetManager>.instance;
            Vector3 position = instance.m_nodes.m_buffer[(int)startNode].m_position;
            Vector3 startDirection = VectorUtils.NormalizeXZ(instance.m_nodes.m_buffer[(int)endNode].m_position - position);
            if (instance.CreateSegment(out segment, ref Singleton<SimulationManager>.instance.m_randomizer, PrefabCollection<NetInfo>.FindLoaded("Metro Track Tunnel"), startNode, endNode, startDirection, -startDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, false))
            {
                instance.UpdateSegment(segment);
                Singleton<SimulationManager>.instance.m_currentBuildIndex += 2U;
                return true;
            }
            segment = (ushort)0;
            return false;
        }

        private static void DipPath(ushort nodeID, NetNode node, bool isUndip = false)
        {
            float depth = 0;
            if (isUndip)
            {
                depth = -(SetStationCustomizations.DEF_DEPTH - 4);
            }
            else
            {
                depth = SetStationCustomizations.DEF_DEPTH - 4;
            }

            Vector3 location = new Vector3(node.m_position.x, node.m_position.y - depth, node.m_position.z);
            ninstance.MoveNode(nodeID, location);
            ninstance.UpdateNode(nodeID);
        }
        private static void SetupTunnelTracks(BuildingInfo info, bool toVanilla = false)
        {
            if (info?.m_paths == null)
            {
                return;
            }
            if (toVanilla)
            {
                foreach (var path in info.m_paths)
                {
                    if (path?.m_netInfo?.name == null)
                    {
                        continue;
                    }

                    if (path.m_netInfo.IsMetroStationTrack())
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
                    }
                    else if (path.m_netInfo.IsMetroTrack())
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                    }
                }
            }
        }

        private static void UpdateMetroStationsMeta()
        {
            var vanillaMetroStation = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");

            var infos = Resources.FindObjectsOfTypeAll<BuildingInfo>();
            if (infos == null)
            {
                return;
            }
            foreach (var info in infos)
            {
                try
                {
                    if (info == null || info.m_buildingAI == null || !info.IsMetroDepot())
                    {
                        continue;
                    }

                    var ai = info.m_buildingAI as TransportStationAI;
                    if (ai != null)
                    {
                        var transportStationAi = ai;
                        transportStationAi.m_maxVehicleCount = 0;
                    }
                    info.m_UnlockMilestone = vanillaMetroStation.m_UnlockMilestone;
                    ((DepotAI)info.m_buildingAI).m_createPassMilestone = ((DepotAI)vanillaMetroStation.m_buildingAI).m_createPassMilestone;
                    ((DepotAI)info.m_buildingAI).m_createPassMilestone2 = ((DepotAI)vanillaMetroStation.m_buildingAI).m_createPassMilestone2;
                    ((DepotAI)info.m_buildingAI).m_createPassMilestone3 = ((DepotAI)vanillaMetroStation.m_buildingAI).m_createPassMilestone3;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"MOM: Failed to update meta of {info?.name}:");
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        //this method is supposed to be called before level loading
        public static void PreventVanillaMetroTrainSpawning()
        {
            var metro = PrefabCollection<VehicleInfo>.FindLoaded("Metro");
            if (metro == null)
            {
                return;
            }
            metro.m_class = ScriptableObject.CreateInstance<ItemClass>();
        }

        //this method is supposed to be called before level loading
        public static void UpdateVanillaMetroTracks()
        {
            NetInfo metroTrack = null;
            NetInfo metroStationTrack = null;
            if (OptionsWrapper<Options>.Options.ghostMode)
            {
                metroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground");
                metroStationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground");
            }
            else
            {
                metroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                metroStationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
            }
            if (metroTrack != null)
            {
                metroTrack.m_availableIn = ItemClass.Availability.Editors;
            }
            if (metroStationTrack != null)
            {
                metroStationTrack.m_availableIn = ItemClass.Availability.Editors;
            }
        }

        private static void UpdateMetroTrainEffects()
        {
            var vanillaMetro = PrefabCollection<VehicleInfo>.FindLoaded("Metro");
            var arriveEffect = ((MetroTrainAI)vanillaMetro.m_vehicleAI).m_arriveEffect;
            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                try
                {
                    var info = PrefabCollection<VehicleInfo>.GetLoaded(i);
                    var metroTrainAI = info?.m_vehicleAI as MetroTrainAI;
                    if (metroTrainAI == null)
                    {
                        continue;
                    }
                    if (info.m_effects == null || info.m_effects.Length == 0)
                    {
                        info.m_effects = vanillaMetro.m_effects;
                    }
                    else
                    {
                        for (var j = 0; j < info.m_effects.Length; j++)
                        {
                            if (info.m_effects[j].m_effect?.name == "Train Movement")
                            {
                                info.m_effects[j] = vanillaMetro.m_effects[0];
                            }
                        }
                    }
                    var arriveEffectName = metroTrainAI.m_arriveEffect?.name;
                    if (arriveEffectName == null || arriveEffectName == "Transport Arrive")
                    {
                        metroTrainAI.m_arriveEffect = arriveEffect;
                    }
                }
                catch
                {
                    //swallow
                }
            }
        }
    }
}