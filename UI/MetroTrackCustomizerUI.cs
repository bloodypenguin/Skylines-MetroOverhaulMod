using System;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;
using MetroOverhaul.NEXT.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace MetroOverhaul.UI
{
    public class MetroTrackCustomizerUI : MetroCustomizerBase
    {
        public override void Update()
        {
            if (m_netTool == null)
            {
                return;
            }
            try
            {
                var toolInfo = m_netTool.enabled ? m_netTool.m_prefab : null;
                if (toolInfo == m_currentNetInfo)
                {
                    return;
                }
                NetInfo finalInfo = null;
                if (toolInfo != null)
                {
                    if (toolInfo.IsMetroTrack())
                    {
                        finalInfo = toolInfo;
                    }
                }
                if (finalInfo == m_currentNetInfo)
                {
                    return;
                }
                if (finalInfo != null)
                {
                    Activate(finalInfo);
                }
                else
                {
                    Deactivate();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                Deactivate();
            }

        }
        public override void Start()
        {
            m_netTool = FindObjectOfType<NetTool>();
            if (m_netTool == null)
            {
#if DEBUG
                Next.Debug.Log("NetTool Not Found");
#endif
                enabled = false;
                return;
            }

            m_bulldozeTool = FindObjectOfType<BulldozeTool>();
            if (m_bulldozeTool == null)
            {
#if DEBUG
                Next.Debug.Log("BulldozeTool Not Found");
#endif
                enabled = false;
                return;
            }
            m_netTool = FindObjectOfType<NetTool>();
            if (m_netTool == null)
            {
#if DEBUG
                Next.Debug.Log("NetTool Not Found");
#endif
                enabled = false;
                return;
            }

            try
            {
                m_upgradeButtonTemplate = GameObject.Find("RoadsSmallPanel").GetComponent<GeneratedScrollPanel>().m_OptionsBar.Find<UIButton>("Upgrade");
            }
            catch
            {
#if DEBUG
                Next.Debug.Log("Upgrade button template not found");
#endif
            }

            CreateUI();
            trackStyle = 0;
            trackSize = 1;
            trackDirection = 1;
            ExecuteUiInstructions();
        }

        private void CreateUI()
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

            btnModernStyle = CreateButton("Modern", 2, (c, v) =>
            {
                trackStyle = 0;
                ExecuteUiInstructions();
            });

            btnClassicStyle = CreateButton("Classic", 2, (c, v) =>
               {
                   trackStyle = 1;
                   ExecuteUiInstructions();
               });
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

            CheckboxDict = new Dictionary<string, UICheckBox>();
            CreateCheckbox(ALT_BARRIER);
            CreateCheckbox(OVER_ROAD_FRIENDLY);
        }

        protected override void ExecuteUiInstructions()
        {
            ToggleButtonPairs(trackStyle, btnModernStyle, btnClassicStyle);
            ToggleButtonPairs(trackSize, btnSingleTrack, btnDoubleTrack, btnQuadTrack);
            ToggleButtonPairs(trackDirection, btnOneWay, btnTwoWay);

            NetInfo prefab = null;
            var fence = CheckboxDict[ALT_BARRIER].isChecked;
            switch (trackStyle)
            {
                case 0:
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
                                }
                                else
                                {
                                    prefab = fence ? concreteLargePrefab : concreteLargePrefabNoBar;
                                }
                            }
                            break;
                    }
                    break;
                case 1:
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
                var lanes = m_netTool.m_prefab.m_lanes.ToList();
                Next.Debug.Log($"MOM EE lane count: {lanes.Count()}");
                var lane = lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None);
                NetLaneProps.Prop prop = null;
                if (lane != null)
                {
                    var propList = lane.m_laneProps.m_props?.ToList();
                    if (propList != null)
                    {
                        Next.Debug.Log($"MOM EE lane found with {propList.Count()} props");
                        prop = propList.FirstOrDefault(p => p.m_prop.name.ToLower().Contains("l pillar ("));
                        if (prop != null)
                        {
                            Next.Debug.Log($"MOM EE Examining aLane");
                            var name = prop.m_prop.name;
                            //if (noCollisionPillars)
                            //{
                            //    prop.m_probability = 100;
                            //    Next.Debug.Log("MOM EE Enabled");
                            //}
                            //else
                            //{
                            //    prop.m_probability = 0;
                            //    Next.Debug.Log("MOM EE Disabled");
                            //}
                            var props = lane.m_laneProps.m_props?.ToList();
                            if (props != null)
                            {
                                var replacementPair = new KeyValuePair<string, PropInfo>(name, prop.m_prop);

                                if (props.Any(p => p.m_prop.name.ToLower().Contains(replacementPair.Key.ToLower())))
                                {
                                    var tempProp = new NetLaneProps.Prop();
                                    var propsToReplace = props.Where(p => p.m_prop.name.ToLower().Contains(replacementPair.Key.ToLower())).ToList();
                                    for (var i = 0; i < propsToReplace.Count; i++)
                                    {
                                        tempProp = propsToReplace[i].ShallowClone();
                                        props.Remove(propsToReplace[i]);
                                        tempProp.m_prop = replacementPair.Value;
                                        props.Add(tempProp);
                                    }
                                }
                                lane.m_laneProps.m_props = props.ToArray();
                            }
                        }
                    }
                }
                m_netTool.m_prefab.m_lanes = lanes.ToArray();
            }
        }

        private void Activate(NetInfo nInfo)
        {
            m_activated = true;
            m_currentNetInfo = nInfo;
            isVisible = true;
            ExecuteUiInstructions();
        }
        private void Deactivate()
        {
            if (!m_activated)
            {
                return;
            }
            CheckboxDict[OVER_ROAD_FRIENDLY].isChecked = false;
            ExecuteUiInstructions();
            m_currentNetInfo = null;
            isVisible = false;
            m_activated = false;
        }
    }
}
