using System;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;
using System.Collections.Generic;

namespace MetroOverhaul.UI
{
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
            trackDirection = 1;
            pillarType = PillarType.WideMedian;
            ExecuteUiInstructions();
        }

        protected override void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM TRACK GUI Created");
#endif
            base.CreateUI();
            width = 250;
            CreateDragHandle("Track Options");
            var pnlStyles = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStyles",
                ColShare = 6,
                Margins = new Vector2(10, 0)
            });
            var pnlDirections = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlDirections",
                ColShare = 5,
                ColOffset = 1,
            });
            var lblStyles = CreateLabel(new UILabelParamProps()
            {
                Name = "lblStyles",
                Text = "Style Selector",
                ParentComponent = pnlStyles,
                ColumnCount = 1
            });
            var tsStyles = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsStyles",
                ParentComponent = pnlStyles,
                Margins = new Vector2(9, 0),
                ColumnCount = 1
            });
            btnModernStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnModernStyle",
                ToolTip = "Modern Style",
                ColumnCount = 3,
                ParentComponent = tsStyles,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ModernStyleAtlas", UIHelper.ModernStyle),
                Width = 59,
                Height = 52,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Modern;
                    ExecuteUiInstructions();
                }
            });
            btnClassicStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnClassicStyle",
                ToolTip = "Classic Style",
                ColumnCount = 3,
                ParentComponent = tsStyles,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ClassicStyleAtlas", UIHelper.ClassicStyle),
                Width = 59,
                Height = 52,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Classic;
                    ExecuteUiInstructions();
                }
            });
            btnVanillaStyle = CreateButton(new UIButtonParamProps()
            {
                Name = "btnVanillaStyle",
                ToolTip = "Vanilla Style",
                ColumnCount = 3,
                ParentComponent = tsStyles,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_VanillaStyleAtlas", UIHelper.VanillaStyle),
                Width = 59,
                Height = 52,
                EventClick = (c, v) =>
                {
                    trackStyle = TrackStyle.Vanilla;
                    ExecuteUiInstructions();
                }
            });
            var lblDirections = CreateLabel(new UILabelParamProps()
            {
                Name = "lblDirections",
                Text = "Direction",
                ParentComponent = pnlDirections,
                Margins = new Vector2(8, 16),
                ColumnCount = 1
            });
            var tsDirections = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsStyles",
                ParentComponent = pnlDirections,
                Margins = new Vector2(9, 0),
                StartSelectedIndex = 1,
                ColumnCount = 1
            });
            btnOneWay = CreateButton(new UIButtonParamProps()
            {
                Name = "btnOneWay",
                ColumnCount = 2,
                ParentComponent = tsDirections,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_OnewayDirectionAtlas", UIHelper.OnewayDirection),
                Width = 36,
                Height = 33,
                Margins = new Vector2(8, 27),
                EventClick = (c, v) =>
                {
                    trackDirection = 0;
                    ExecuteUiInstructions();
                }
            });
            btnTwoWay = CreateButton(new UIButtonParamProps()
            {
                Name = "btnTwoWay",
                ColumnCount = 2,
                ParentComponent = tsDirections,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_TwowayDirectionAtlas", UIHelper.TwowayDirection),
                Width = 36,
                Height = 33,
                Margins = new Vector2(8, 27),
                EventClick = (c, v) =>
                {
                    trackDirection = 1;
                    ExecuteUiInstructions();
                }
            });
            var pnlTrackVsStation = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlTrackVsStation",
                ColShare = 10,
                ColOffset = 2,
                Margins = new Vector2(0, 20)
            });
            var lblTrackVsStation = CreateLabel(new UILabelParamProps()
            {
                Name = "lblTrackVsStation",
                Text = "Track / Station Track",
                ParentComponent = pnlTrackVsStation,
                ColumnCount = 1
            });
            tsTrackVsStation = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsTrackVsStation",
                ColumnCount = 1,
                ParentComponent = pnlTrackVsStation
            });
            btnTrack = CreateButton(new UIButtonParamProps()
            {
                Text = "Track",
                ColumnCount = 2,
                EventClick = (c, v) =>
                {
                    isStation = 0;
                    ExecuteUiInstructions();
                }
            });
            btnStation = CreateButton(new UIButtonParamProps()
            {
                Text = "Stn Trk",
                ColumnCount = 2,
                EventClick = (c, v) =>
                {
                    isStation = 1;
                    ExecuteUiInstructions();
                }
            });
            var pnlPillarChooser = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlPillarChooser",
                ColShare = 11,
                ColOffset = 1,
                Margins = new Vector2(0, 20)
            });
            var lblPillarChooser = CreateLabel(new UILabelParamProps()
            {
                Name = "lblPillarChooser",
                Text = "Pillar Selector",
                ParentComponent = pnlPillarChooser,
                ColumnCount = 1
            });
            tsPillarChooser = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsPillarChooser",
                ColumnCount = 1,
                ParentComponent = pnlPillarChooser
            });
            btnWideMedianPillar = CreateButton(new UIButtonParamProps()
            {
                Name = "btnWideMedianPillar",
                ParentComponent = tsPillarChooser,
                ColumnCount = 4,
                Width = 50,
                Height = 50,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_WideMedianPillarAtlas", UIHelper.WideMedianPillar),
                EventClick = (c, v) =>
                {
                    pillarType = PillarType.WideMedian;
                    ExecuteUiInstructions();
                }
            });
            btnWidePillar = CreateButton(new UIButtonParamProps()
            {
                Name = "btnWidePillar",
                ParentComponent = tsPillarChooser,
                ColumnCount = 4,
                Width = 50,
                Height = 50,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_WidePillarAtlas", UIHelper.WidePillar),
                EventClick = (c, v) =>
                {
                    pillarType = PillarType.Wide;
                    ExecuteUiInstructions();
                }
            });
            btnNarrowPillar = CreateButton(new UIButtonParamProps()
            {
                Name = "btnNarrowPillar",
                ParentComponent = tsPillarChooser,
                ColumnCount = 4,
                Width = 50,
                Height = 50,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_NarrowPillarAtlas", UIHelper.NarrowPillar),
                EventClick = (c, v) =>
                {
                    pillarType = PillarType.Narrow;
                    ExecuteUiInstructions();
                }
            });
            btnNarrowMedianPillar = CreateButton(new UIButtonParamProps()
            {
                Name = "btnNarrowMedianPillar",
                ParentComponent = tsPillarChooser,
                ColumnCount = 4,
                Width = 50,
                Height = 50,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_NarrowMedianPillarAtlas", UIHelper.NarrowMedianPillar),
                EventClick = (c, v) =>
                {
                    pillarType = PillarType.NarrowMedian;
                    ExecuteUiInstructions();
                }
            });

            var pnlCheckboxOptions = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlCheckboxOptions",
                ColumnCount = 1,
                Margins = new Vector2(0, 10)
            });
            CheckboxDict = new Dictionary<string, UICheckBox>();

            CheckboxDict[ALT_BARRIER] = CreateCheckbox(new UICheckboxParamProps()
            {
                Text = ALT_BARRIER,
                ColumnCount = 1,
                ParentComponent = pnlCheckboxOptions,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_CheckboxAtlas", UIHelper.Checkbox),
            });
            CheckboxDict[OVER_ROAD_FRIENDLY] = CreateCheckbox(new UICheckboxParamProps()
            {
                Text = OVER_ROAD_FRIENDLY,
                ColumnCount = 1,
                ParentComponent = pnlCheckboxOptions,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_CheckboxAtlas", UIHelper.Checkbox),
            });
            //CreateCheckbox(EXTRA_PILLARS);
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

            ToggleButtonPairs((int)trackStyle, btnModernStyle, btnClassicStyle, btnVanillaStyle);
            ToggleButtonPairs(isStation, btnTrack, btnStation);

            if (isStation == 0)
            {
                ToggleButtonPairs(trackDirection, btnOneWay, btnTwoWay);
            }
            else if (isStation == 1)
            {
                ToggleButtonPairs(stationType);
            }
            NetInfo prefab = null;
            var fence = CheckboxDict[ALT_BARRIER].isChecked;
            if (!btnOneWay.isInteractive)
            {
                btnOneWay.isInteractive = true;
                if (trackDirection == 0)
                {
                    btnOneWay.state = UIButton.ButtonState.Focused;
                }
                else
                {
                    btnOneWay.state = UIButton.ButtonState.Normal;
                }
            }
            if (!btnNarrowMedianPillar.isInteractive)
            {
                btnNarrowMedianPillar.isInteractive = true;
                if (pillarType == PillarType.NarrowMedian)
                {
                    btnNarrowMedianPillar.state = UIButton.ButtonState.Focused;
                }
                else
                {
                    btnNarrowMedianPillar.state = UIButton.ButtonState.Normal;
                }
            }
            switch (trackStyle)
            {
                case TrackStyle.Vanilla:
                    switch (trackSize)
                    {
                        case 0:
                            {
                            }
                            break;
                        case 1:
                            if (trackDirection == 0)
                            {

                            }
                            else
                            {
                                prefab = vanillaPrefab;
                            }
                            break;
                    }
                    break;
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
                                        {
                                            if (trackDirection == 0)
                                            {
                                                btnOneWay.enabled = false;
                                                btnTwoWay.SimulateClick();
                                            }
                                            else
                                            {
                                                prefab = fence ? concreteLargePrefab : concreteLargePrefabNoBar;
                                                btnOneWay.state = UIButton.ButtonState.Disabled;
                                                btnOneWay.isInteractive = false;
                                                btnNarrowMedianPillar.state = UIButton.ButtonState.Disabled;
                                                btnNarrowMedianPillar.isInteractive = false;
                                            }
                                        }
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
                                    case StationTrackType.SinglePlatform:
                                        prefab = concreteSingleStationPrefab;
                                        break;
                                    case StationTrackType.ExpressSidePlatform:
                                        prefab = concreteQuadSideStationPrefab;
                                        break;
                                    case StationTrackType.DualIslandPlatform:
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
                                                    btnOneWay.enabled = false;
                                                    btnTwoWay.SimulateClick();
                                                }
                                                else
                                                {
                                                    prefab = fence ? steelLargePrefab : steelLargePrefabNoBar;
                                                    btnOneWay.state = UIButton.ButtonState.Disabled;
                                                    btnOneWay.isInteractive = false;
                                                    btnNarrowMedianPillar.state = UIButton.ButtonState.Disabled;
                                                    btnNarrowMedianPillar.isInteractive = false;
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
                                        case StationTrackType.SinglePlatform:
                                            prefab = steelSingleStationPrefab;
                                            break;
                                        case StationTrackType.ExpressSidePlatform:
                                            prefab = steelQuadSideStationPrefab;
                                            break;
                                        case StationTrackType.DualIslandPlatform:
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
                    var ttbai = elevatedPrefab?.GetComponent<MOMMetroTrackBridgeAI>();
                    if (ttbai != null)
                    {
                        ttbai.NoPillarCollision = noCollisionPillars;
                    }

                    var bridgePrefab = PrefabCollection<NetInfo>.FindLoaded(prefab.name.Replace("Ground", "Bridge"));
                    var ttbai2 = bridgePrefab?.GetComponent<MOMMetroTrackBridgeAI>();
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
            if (info.name.Contains("Large"))
            {
                trackSize = 2;
            }
            else if (info.name.Contains("Small"))
            {
                trackSize = 0;
            }
            else
            {
                trackSize = 1;
            }
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
