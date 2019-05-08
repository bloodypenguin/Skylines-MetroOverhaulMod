using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace MetroOverhaul.UI
{
    public class MetroTrackCustomizerUI : MetroCustomizerBaseUI
    {
        protected override bool SatisfiesTrackSpecs(PrefabInfo info)
        {
            return ((NetInfo)info).IsMetroTrack();
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
            ExecuteUiInstructions();
        }

        protected override void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM TRACK GUI Created");
#endif

            backgroundSprite = "GenericPanel";
            color = new Color32(73, 68, 84, 170);
            width = 250;
            height = 270;
            opacity = 90;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Track Options");

            btnModernStyle = CreateButton(new UIButtonParamProps()
            {
                Text = "Modern",
                ColumnCount = 2,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Modern;
                    ExecuteUiInstructions();
                }
            });

            btnClassicStyle = CreateButton(new UIButtonParamProps()
            {
                Text = "Classic",
                ColumnCount = 2,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Classic;
                    ExecuteUiInstructions();
                }
            });
            btnSingleTrack = CreateButton(new UIButtonParamProps()
            {
                Text = "Single",
                ColumnCount = 3,
                EventClick = (c, v) =>
                {
                    trackSize = 0;
                    ExecuteUiInstructions();
                }
            });

            btnDoubleTrack = CreateButton(new UIButtonParamProps()
            {
                Text = "Double",
                ColumnCount = 3,
                EventClick = (c, v) =>
                {
                    trackSize = 1;
                    ExecuteUiInstructions();
                }
            });

            btnQuadTrack = CreateButton(new UIButtonParamProps()
            {
                Text = "Quad",
                ColumnCount = 3,
                EventClick = (c, v) =>
                {
                    trackSize = 2;
                    ExecuteUiInstructions();
                }
            });

            btnOneWay = CreateButton(new UIButtonParamProps()
            {
                Text = "OneWay",
                ColumnCount = 2,
                EventClick = (c, v) =>
                {
                    trackDirection = 0;
                    ExecuteUiInstructions();
                }
            });

            btnTwoWay = CreateButton(new UIButtonParamProps()
            {
                Text = "TwoWay",
                ColumnCount = 2,
                EventClick = (c, v) =>
                {
                    trackDirection = 1;
                    ExecuteUiInstructions();
                }
            });

            CheckboxDict = new Dictionary<string, UICheckBox>();
            CreateCheckbox(ALT_BARRIER);
            CreateCheckbox(OVER_ROAD_FRIENDLY);
            //CreateCheckbox(EXTRA_PILLARS);
        }

        protected override void ExecuteUiInstructions()
        {
            ToggleButtonPairs((int)trackStyle, btnModernStyle, btnClassicStyle);
            ToggleButtonPairs(trackSize, btnSingleTrack, btnDoubleTrack, btnQuadTrack);
            ToggleButtonPairs(trackDirection, btnOneWay, btnTwoWay);

            NetInfo prefab = null;
            var fence = CheckboxDict[ALT_BARRIER].isChecked;
            btnOneWay.enabled = true;
            switch (trackStyle)
            {
                case TrackStyle.Modern:
                    switch (trackSize)
                    {
                        case 0:
                            {
                                if (trackDirection == 0)
                                {
                                    prefab = fence ? concreteSmallPrefab : concreteSmallPrefabNoBar;
                                }
                                else
                                {
                                    prefab = fence ? concreteSmallTwoWayPrefab : concreteSmallTwoWayPrefabNoBar;

                                }
                            }
                            break;
                        case 1:
                            {
                                if (trackDirection == 0)
                                {
                                    prefab = fence ? concreteTwoLaneOneWayPrefab : concreteTwoLaneOneWayPrefabNoBar;
                                }
                                else
                                {
                                    prefab = fence ? concretePrefab : concretePrefabNoBar;
                                }
                            }
                            break;

                        case 2:
                            {
                                if (trackDirection == 0)
                                {
                                    btnOneWay.enabled = false;
                                    btnTwoWay.SimulateClick();
                                }
                                else
                                {
                                    prefab = fence ? concreteLargePrefab : concreteLargePrefabNoBar;
                                    btnOneWay.enabled = false;
                                }
                            }
                            break;
                    }
                    break;
                case TrackStyle.Classic:
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
                                    btnOneWay.enabled = false;
                                    btnTwoWay.SimulateClick();
                                }
                                else
                                {
                                    prefab = fence ? steelLargePrefab : steelLargePrefabNoBar;
                                    btnOneWay.enabled = false;
                                }
                            }
                            break;
                    }
                    break;
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
            CheckboxDict[OVER_ROAD_FRIENDLY].isChecked = false;
            ExecuteUiInstructions();
        }
        protected override void SubDeactivate()
        {
            //CheckboxDict[OVER_ROAD_FRIENDLY].isChecked = false;
            //ExecuteUiInstructions();
        }
    }
}
