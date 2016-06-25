using System;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class Initializer : AbstractInitializer
    {
        protected override void InitializeImpl()
        {
            CreatePrefab("Station Track Eleva", "Train Station Track", 
                SetupElevatedPrefab); //for compatibility, never change this prefab's name
            CreatePrefab("Station Track Elevated (C)", "Train Station Track",
                new Action<NetInfo, bool>(SetupElevatedPrefab).Apply(true));
            CreatePrefab("Station Track Elevated (NP)", "Train Station Track",
                new Action<NetInfo>(SetupElevatedPrefab).Chain(Modifiers.RemoveElectricityPoles));
            CreatePrefab("Station Track Elevated (CNP)", "Train Station Track",
                new Action<NetInfo, bool>(SetupElevatedPrefab).Apply(true).Chain(Modifiers.RemoveElectricityPoles));

            CreatePrefab("Station Track Elevated Narrow", "Train Station Track",
                new Action<NetInfo>(SetupElevatedPrefab).Chain(Modifiers.MakePedestrianLanesNarrow));
            CreatePrefab("Station Track Elevated Narrow (C)", "Train Station Track",
                new Action<NetInfo, bool>(SetupElevatedPrefab).Apply(true).Chain(Modifiers.MakePedestrianLanesNarrow));
            CreatePrefab("Station Track Elevated Narrow (NP)", "Train Station Track",
                new Action<NetInfo>(SetupElevatedPrefab).Chain(Modifiers.RemoveElectricityPoles).Chain(Modifiers.MakePedestrianLanesNarrow));
            CreatePrefab("Station Track Elevated Narrow (CNP)", "Train Station Track",
                new Action<NetInfo, bool>(SetupElevatedPrefab).Apply(true).Chain(Modifiers.RemoveElectricityPoles).Chain(Modifiers.MakePedestrianLanesNarrow));
            
            CreatePrefab("Station Track Sunken", "Train Station Track", 
                SetupSunkenPrefab); //for compatibility, never change this prefab's name
            CreatePrefab("Station Track Sunken (NP)", "Train Station Track",
                new Action<NetInfo>(SetupSunkenPrefab).Chain(Modifiers.RemoveElectricityPoles));
            CreatePrefab("Train Station Track (C)", "Train Station Track",
                Modifiers.CreatePavement);
            CreatePrefab("Train Station Track (NP)", "Train Station Track",
                Modifiers.RemoveElectricityPoles);
            CreatePrefab("Train Station Track (CNP)", "Train Station Track",
                new Action<NetInfo>(Modifiers.CreatePavement).Chain(Modifiers.RemoveElectricityPoles));
            
            CreatePrefab("Station Track Tunnel", "Train Station Track", 
                SetupTunnelPrefab); //for compatibility, never change this prefab's name
        }

        private static void SetupElevatedPrefab(NetInfo elevatedPrefab)
        {
            SetupElevatedPrefab(elevatedPrefab, false);
        }

        private static void SetupElevatedPrefab(NetInfo elevatedPrefab, bool concrete)
        {
            var stationAI = elevatedPrefab.GetComponent<TrainTrackAI>();
            stationAI.m_elevatedInfo = elevatedPrefab;

            elevatedPrefab.m_followTerrain = false;
            elevatedPrefab.m_flattenTerrain = false;
            elevatedPrefab.m_createGravel = false;
            elevatedPrefab.m_createPavement = false;
            elevatedPrefab.m_createRuining = false;
            elevatedPrefab.m_requireSurfaceMaps = false;
            elevatedPrefab.m_clipTerrain = false;
            elevatedPrefab.m_snapBuildingNodes = false;
            elevatedPrefab.m_placementStyle = ItemClass.Placement.Procedural;
            elevatedPrefab.m_useFixedHeight = true;
            elevatedPrefab.m_lowerTerrain = true;
            elevatedPrefab.m_availableIn = ItemClass.Availability.GameAndAsset;
            var elevatedTrack = FindOriginalPrefab("Train Track Elevated");
            if (elevatedTrack == null)
            {
                return;
            }
            var etstMesh = Util.LoadMesh(string.Concat(Util.AssemblyDirectory, "/TTNR.obj"), "ETST ");
            var etstSegmentLodMesh = Util.LoadMesh(string.Concat(Util.AssemblyDirectory, "/TTNR_LOD.obj"), "ETST_SLOD");
            var etstNodeLodMesh = Util.LoadMesh(string.Concat(Util.AssemblyDirectory, "/TTNR_Node_LOD.obj"), "ETST_NLOD");
            elevatedPrefab.m_segments[0].m_segmentMaterial = ModifyRailMaterial(elevatedTrack.m_segments[0].m_segmentMaterial, concrete);
            elevatedPrefab.m_segments[0].m_material = ModifyRailMaterial(elevatedTrack.m_segments[0].m_material, concrete);
            elevatedPrefab.m_segments[0].m_mesh = etstMesh;
            elevatedPrefab.m_segments[0].m_segmentMesh = etstMesh;
            elevatedPrefab.m_segments[0].m_lodMaterial = ModifyRailMaterial(elevatedTrack.m_segments[0].m_lodMaterial, concrete);
            elevatedPrefab.m_segments[0].m_lodMesh = etstSegmentLodMesh;
            elevatedPrefab.m_nodes[0].m_material = ModifyRailMaterial(elevatedTrack.m_nodes[0].m_material, concrete);
            elevatedPrefab.m_nodes[0].m_nodeMaterial = ModifyRailMaterial(elevatedTrack.m_nodes[0].m_nodeMaterial, concrete);
            elevatedPrefab.m_nodes[0].m_lodMaterial = ModifyRailMaterial(elevatedTrack.m_nodes[0].m_lodMaterial, concrete);
            elevatedPrefab.m_nodes[0].m_lodMesh = etstNodeLodMesh;
            elevatedPrefab.m_nodes[0].m_nodeMesh = etstMesh;
            elevatedPrefab.m_nodes[0].m_mesh = etstMesh;
        }

        private static Material ModifyRailMaterial(Material material, bool concrete)
        {
            if (!concrete)
            {
                return material;
            }
            var newMaterial = new Material(material)
            {
                name = $"{material.name}-concrete",
                shader = Shader.Find("Custom/Net/RoadBridge")
            };
            return newMaterial;
        }

        private static void SetupTunnelPrefab(NetInfo tunnelPrefab)
        {
            SetupSunkenPrefab(tunnelPrefab);

            tunnelPrefab.m_canCollide = false;

            foreach (var lane in tunnelPrefab.m_lanes)
            {
                lane.m_laneProps = null;
            }
            var metroStation = FindOriginalPrefab("Metro Station Track");
            if (metroStation != null)
            {
                tunnelPrefab.m_segments = new[] { metroStation.m_segments[0] };
                //TODO(earalov): make a shallow copy of segment and change some properties
                tunnelPrefab.m_nodes = new[] { metroStation.m_nodes[0] };
                //TODO(earalov): make a shallow copy of segment and change some properties
            }
            else
            {
                Debug.LogWarning("ElevatedTrainStationTrack - Couldn't find metro station track");
            }
        }

        private static void SetupSunkenPrefab(NetInfo sunkenPrefab)
        {

            
            var stationAI = sunkenPrefab.GetComponent<TrainTrackAI>();
            stationAI.m_tunnelInfo = sunkenPrefab;

            sunkenPrefab.m_clipTerrain = false;

            sunkenPrefab.m_createGravel = false;
            sunkenPrefab.m_createPavement = false;
            sunkenPrefab.m_createRuining = false;

            sunkenPrefab.m_flattenTerrain = false;
            sunkenPrefab.m_followTerrain = false;

            sunkenPrefab.m_intersectClass = null;

            sunkenPrefab.m_maxHeight = -1;
            sunkenPrefab.m_minHeight = -3;

            sunkenPrefab.m_requireSurfaceMaps = false;
            sunkenPrefab.m_snapBuildingNodes = false;

            sunkenPrefab.m_placementStyle = ItemClass.Placement.Procedural;
            sunkenPrefab.m_useFixedHeight = true;
            sunkenPrefab.m_lowerTerrain = false;
            sunkenPrefab.m_availableIn = ItemClass.Availability.GameAndAsset;
        }
    }
}
