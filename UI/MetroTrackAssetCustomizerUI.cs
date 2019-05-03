using System;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace MetroOverhaul.UI {
    public class MetroTrackAssetCustomizerUI : MetroCustomizerBaseUI
    {
        protected override bool SatisfiesTrackSpecs(PrefabInfo info)
        {
            return ((NetInfo)info).IsMetroTrack() || ((NetInfo)info).IsMetroStationTrack();
        }

        protected override ToolBase GetTheTool()
        {
            return m_netTool;
        }

        protected override PrefabInfo GetToolPrefab()
        {
            return ((NetTool)GetTheTool())?.m_prefab;
        }

        protected override PrefabInfo CurrentInfo { get => m_currentNetInfo; set => m_currentNetInfo = (NetInfo)value; }

        protected override void SubStart()
        {
            trackStyle = TrackStyle.Modern;
            trackSize = 1;
            trackDirection = 1;
            isStation = 1;
            stationType = 0;
            ExecuteUiInstructions();
        }

        protected override void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM TRACK GUI Created");
#endif

            backgroundSprite = "GenericPanel";
            color = new Color32(73, 68, 84, 170);
            width = 300;
            height = 300;
            opacity = 90;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Track/Station Options");

            btnModernStyle = CreateButton("Modern", 2, (c, v) =>
            {
                trackStyle = TrackStyle.Modern;
                ExecuteUiInstructions();
            });

            btnClassicStyle = CreateButton("Classic", 2, (c, v) =>
              {
                  trackStyle = TrackStyle.Classic;
                  ExecuteUiInstructions();
              });
            btnStation = CreateButton("Stn Trk", 2, (c, v) =>
            {
                isStation = 1;
                ExecuteUiInstructions();
            });
            btnTrack = CreateButton("Track", 2, (c, v) =>
            {
                isStation = 0;
                ExecuteUiInstructions();
            });

            var rowIndex2 = m_rowIndex;

            buttonStationDict = new Dictionary<StationTrackType, UIButton>();
            buttonStationDict[StationTrackType.SidePlatform] = CreateButton(StationTrackType.SidePlatform, 3, (c, v) =>
            {
                stationType = StationTrackType.SidePlatform;
                ExecuteUiInstructions();
            });
            buttonStationDict[StationTrackType.IslandPlatform] = CreateButton(StationTrackType.IslandPlatform, 3, (c, v) =>
            {
                stationType = StationTrackType.IslandPlatform;
                ExecuteUiInstructions();
            });
            buttonStationDict[StationTrackType.SingleTrack] = CreateButton(StationTrackType.SingleTrack, 3, (c, v) =>
            {
                stationType = StationTrackType.SingleTrack;
                ExecuteUiInstructions();
            });
            buttonStationDict[StationTrackType.QuadSidePlatform] = CreateButton(StationTrackType.QuadSidePlatform, 2, (c, v) =>
            {
                stationType = StationTrackType.QuadSidePlatform;
                ExecuteUiInstructions();
            });
            buttonStationDict[StationTrackType.QuadDualIslandPlatform] = CreateButton(StationTrackType.QuadDualIslandPlatform, 2, (c, v) =>
            {
                stationType = StationTrackType.QuadDualIslandPlatform;
                ExecuteUiInstructions();
            });

            var rowIndexTemp = m_rowIndex;
            m_rowIndex = rowIndex2;

            btnSingleTrack = CreateButton("Single", 3, (c, v) =>
              {
                  trackSize = 0;
                  ExecuteUiInstructions();
              });

            btnDoubleTrack = CreateButton("Double", 3, (c, v) =>
              {
                  trackSize = 1;
                  ExecuteUiInstructions();
              });

            btnQuadTrack = CreateButton("Quad", 3, (c, v) =>
            {
                trackSize = 2;
                ExecuteUiInstructions();
            });

            btnOneWay = CreateButton("OneWay", 2, (c, v) =>
              {
                  trackDirection = 0;
                  ExecuteUiInstructions();
              });

            btnTwoWay = CreateButton("TwoWay", 2, (c, v) =>
              {
                  trackDirection = 1;
                  ExecuteUiInstructions();
              });
            m_height = (Math.Max(rowIndexTemp, m_rowIndex) * 60) + 20;
            CheckboxDict = new Dictionary<string, UICheckBox>();
            CreateCheckbox(ALT_BARRIER);
            CreateCheckbox(OVER_ROAD_FRIENDLY);

        }
        private UITextureAtlas m_InGameAtlas = null;
        private UITextureAtlas InGameAtlas()
        {
            if (m_InGameAtlas == null)
            {
                var atlases = Resources.FindObjectsOfTypeAll<UITextureAtlas>();
                foreach (UITextureAtlas atlas in atlases)
                {
                    if (atlas.name == "Ingame")
                    {
                        m_InGameAtlas = atlas;
                    }
                }
            }
            return m_InGameAtlas;
        }

        protected override void ExecuteUiInstructions()
        {
            btnSingleTrack.isVisible = isStation == 0;
            btnDoubleTrack.isVisible = isStation == 0;
            btnQuadTrack.isVisible = isStation == 0;
            btnOneWay.isVisible = isStation == 0;
            btnTwoWay.isVisible = isStation == 0;

            foreach (var kvp in CheckboxDict)
            {
                kvp.Value.isVisible = isStation == 0;
            }
            foreach (var kvp in buttonStationDict)
            {
                kvp.Value.isVisible = isStation == 1;
            }

            ToggleButtonPairs((int)trackStyle, btnModernStyle, btnClassicStyle);
            ToggleButtonPairs(isStation, btnTrack, btnStation);

            if (isStation == 0)
            {
                ToggleButtonPairs(trackSize, btnSingleTrack, btnDoubleTrack, btnQuadTrack);
                ToggleButtonPairs(trackDirection, btnOneWay, btnTwoWay);
            }
            else if (isStation == 1)
            {
                ToggleButtonPairs(stationType);
            }
            NetInfo prefab = null;
            var fence = CheckboxDict[ALT_BARRIER].isChecked;
            switch (trackStyle)
            {
                case TrackStyle.Modern:
                    {
                        switch (isStation)
                        {
                            case 0:
                                switch (trackSize)
                                {
                                    case 0:
                                        if (trackDirection == 0)
                                            prefab = fence ? concreteSmallPrefab : concreteSmallPrefabNoBar;
                                        else
                                            prefab = fence ? concreteSmallTwoWayPrefab : concreteSmallTwoWayPrefabNoBar;
                                        break;
                                    case 1:
                                        if (trackDirection == 0)
                                            prefab = fence ? concreteTwoLaneOneWayPrefab : concreteTwoLaneOneWayPrefabNoBar;
                                        else
                                            prefab = fence ? concretePrefab : concretePrefabNoBar;
                                        break;
                                    case 2:
                                        if (trackDirection == 0) { }
                                        else
                                            prefab = fence ? concreteLargePrefab : concreteLargePrefabNoBar;
                                        break;
                                }
                                break;
                            case 1:
                                switch (stationType)
                                {
                                    case StationTrackType.SidePlatform:
                                        prefab = concreteSideStationPrefab;
                                        break;
                                    case StationTrackType.IslandPlatform:
                                        prefab = concreteIslandStationPrefab;
                                        break;
                                    case StationTrackType.SingleTrack:
                                        prefab = concreteSingleStationPrefab;
                                        break;
                                    case StationTrackType.QuadSidePlatform:
                                        prefab = concreteQuadSideStationPrefab;
                                        break;
                                    case StationTrackType.QuadDualIslandPlatform:
                                        prefab = concreteQuadDualIslandStationPrefab;
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case TrackStyle.Classic:
                    {
                        switch (isStation)
                        {
                            case 0:
                                {
                                    switch (trackSize)
                                    {
                                        case 0:
                                            {
                                                if (trackDirection == 0)
                                                {
                                                    prefab = fence ? steelSmallPrefab : steelSmallPrefabNoBar;
                                                }
                                                else
                                                {
                                                    prefab = fence ? steelSmallTwoWayPrefab : steelSmallTwoWayPrefabNoBar;
                                                }
                                            }
                                            break;
                                        case 1:
                                            {
                                                if (trackDirection == 0)
                                                {
                                                    prefab = fence ? steelTwoLaneOneWayPrefab : steelTwoLaneOneWayPrefabNoBar;
                                                }
                                                else
                                                {
                                                    prefab = fence ? steelPrefab : steelPrefabNoBar;
                                                }
                                            }
                                            break;
                                        case 2:
                                            {
                                                if (trackDirection == 0)
                                                {
                                                    //prefab = fence ? steelTwoLaneOneWayPrefab : steelTwoLaneOneWayPrefabNoBar;
                                                }
                                                else
                                                {
                                                    prefab = fence ? steelLargePrefab : steelLargePrefabNoBar;
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    switch (stationType)
                                    {
                                        case StationTrackType.SidePlatform:
                                            prefab = steelSideStationPrefab;
                                            break;
                                        case StationTrackType.IslandPlatform:
                                            prefab = steelIslandStationPrefab;
                                            break;
                                        case StationTrackType.SingleTrack:
                                            prefab = steelSingleStationPrefab;
                                            break;
                                        case StationTrackType.QuadSidePlatform:
                                            prefab = steelQuadSideStationPrefab;
                                            break;
                                        case StationTrackType.QuadDualIslandPlatform:
                                            prefab = steelQuadDualIslandStationPrefab;
                                            break;
                                    }
                                }
                                break;
                        }
                        break;
                    }
            }
            if (prefab != null)
            {
                if (CheckboxDict.ContainsKey(OVER_ROAD_FRIENDLY))
                {
                    var noCollisionPillars = CheckboxDict[OVER_ROAD_FRIENDLY].isChecked;
                    var elevatedPrefab = PrefabCollection<NetInfo>.FindLoaded(prefab.name.Replace("Ground", "Elevated"));
                    var ttbai = elevatedPrefab?.GetComponent<TrainTrackBridgeAIMetro>();
                    if (ttbai != null)
                    {
                        ttbai.NoPillarCollision = noCollisionPillars;
                    }

                    var bridgePrefab = PrefabCollection<NetInfo>.FindLoaded(prefab.name.Replace("Ground", "Bridge"));
                    var ttbai2 = bridgePrefab?.GetComponent<TrainTrackBridgeAIMetro>();
                    if (ttbai2 != null)
                    {
                        ttbai2.NoPillarCollision = noCollisionPillars;
                    }
                }

                m_netTool.m_prefab = prefab;
                m_currentNetInfo = prefab;
            }
        }

        protected override void Activate(PrefabInfo info)
        {
            base.Activate(info);
            ExecuteUiInstructions();
        }
        protected override void SubDeactivate()
        {
            if (m_TheIntersectClass != null)
            {
                m_currentNetInfo.m_intersectClass = m_TheIntersectClass;
            }
        }
    }
}
