using System;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class MetroTrackCustomizerUI : UIPanel
    {
        public int trackStyle = 0;
        public int trackSize = 1;
        public int trackDirection = 1;
        public bool fence = false;
        private BulldozeTool m_bulldozeTool;
        private NetTool m_netTool;
        private UIButton m_upgradeButtonTemplate;
        private NetInfo m_currentNetInfo;
        private bool m_activated = false;
        public static MetroTrackCustomizerUI instance;

        UISprite m_useFenceCheckBoxClicker = null;

        private NetInfo concretePrefab;
        private NetInfo concretePrefabNoBar;

        private NetInfo concreteTwoLaneOneWayPrefab;
        private NetInfo concreteTwoLaneOneWayPrefabNoBar;

        private NetInfo concreteLargePrefab;
        private NetInfo concreteLargePrefabNoBar;

        private NetInfo concreteSmallPrefab;
        private NetInfo concreteSmallPrefabNoBar;

        private NetInfo concreteSmallTwoWayPrefab;
        private NetInfo concreteSmallTwoWayPrefabNoBar;

        private NetInfo steelPrefab;
        private NetInfo steelPrefabNoBar;

        private NetInfo steelTwoLaneOneWayPrefab;
        private NetInfo steelTwoLaneOneWayPrefabNoBar;

        private NetInfo steelLargePrefab;
        private NetInfo steelLargePrefabNoBar;

        private NetInfo steelSmallPrefab;
        private NetInfo steelSmallPrefabNoBar;

        private NetInfo steelSmallTwoWayPrefab;
        private NetInfo steelSmallTwoWayPrefabNoBar;

        public override void Awake()
        {
            concretePrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground");
            concretePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground NoBar");

            concreteTwoLaneOneWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Two-Lane One-Way");
            concreteTwoLaneOneWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Two-Lane One-Way NoBar");

            concreteLargePrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Large");
            concreteLargePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Large NoBar");

            concreteSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small");
            concreteSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small NoBar");

            concreteSmallTwoWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small Two-Way");
            concreteSmallTwoWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small Two-Way NoBar");

            steelPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground");
            steelPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground NoBar");

            steelTwoLaneOneWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Two-Lane One-Way");
            steelTwoLaneOneWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Two-Lane One-Way NoBar");

            steelSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small");
            steelSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small NoBar");

            steelSmallTwoWayPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small Two-Way");
            steelSmallTwoWayPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small Two-Way NoBar");

            steelLargePrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Large");
            steelLargePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Large NoBar");
        }

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
                    //RestoreStationTrackStyle(toolInfo);
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
            fence = false;
        }

        private void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM TRACK GUI Created");
#endif

            backgroundSprite = "GenericPanel";
            color = new Color32(68, 84, 68, 170);
            width = 200;
            height = 270;
            position = Vector2.zero;
            isVisible = false;
            isInteractive = true;
            padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.width = width;
            dragHandle.height = 20;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            UILabel titleLabel = AddUIComponent<UILabel>();
            titleLabel.relativePosition = new Vector3() { x = 5, y = 0, z = 0 };
            titleLabel.text = "Track Attributes";
            titleLabel.isInteractive = false;

            UILabel styleTitleLabel = AddUIComponent<UILabel>();
            styleTitleLabel.relativePosition = new Vector3() { x = 8, y = 20, z = 0 };
            styleTitleLabel.text = "Track Style";
            styleTitleLabel.isInteractive = false;

            UILabel trackSizeTitleLabel = AddUIComponent<UILabel>();
            trackSizeTitleLabel.relativePosition = new Vector3() { x = 8, y = 70, z = 0 };
            trackSizeTitleLabel.text = "Track Size";
            trackSizeTitleLabel.isInteractive = false;

            UILabel lengthTitleLabel = AddUIComponent<UILabel>();
            lengthTitleLabel.relativePosition = new Vector3() { x = 8, y = 120, z = 0 };
            lengthTitleLabel.text = "Track Style";
            lengthTitleLabel.isInteractive = false;

            UIButton btnModernStyle = AddUIComponent<UIButton>();
            btnModernStyle.atlas = atlas;
            btnModernStyle.color = new Color32(206, 206, 206, 255);
            btnModernStyle.size = new Vector2(0.5f * (width - 16), 16);
            btnModernStyle.relativePosition = new Vector2(8, 40);
            btnModernStyle.eventClicked += (c, v) =>
            {
                trackStyle = 0;
            };
            UIButton btnClassicStyle = AddUIComponent<UIButton>();
            btnClassicStyle.atlas = atlas;
            btnClassicStyle.color = new Color32(206, 206, 206, 255);
            btnClassicStyle.size = new Vector2(0.5f * (width - 16), 16);
            btnClassicStyle.relativePosition = new Vector2(8 + (0.5f * width) - 16, 40);
            btnModernStyle.eventClicked += (c, v) =>
            {
                trackStyle = 1;
            };
            UIButton btnSingleTrack = AddUIComponent<UIButton>();
            btnModernStyle.atlas = atlas;
            btnModernStyle.color = new Color32(206, 206, 206, 255);
            btnModernStyle.size = new Vector2(0.5f * (width - 16), 16);
            btnModernStyle.relativePosition = new Vector2(8, 90);
            btnModernStyle.eventClicked += (c, v) =>
            {
                trackSize = 0;
            };
            UIButton btnDoubleTrack = AddUIComponent<UIButton>();
            btnClassicStyle.atlas = atlas;
            btnClassicStyle.color = new Color32(206, 206, 206, 255);
            btnClassicStyle.size = new Vector2(0.5f * (width - 16), 16);
            btnClassicStyle.relativePosition = new Vector2(8 + (0.5f * width) - 16, 90);
            btnModernStyle.eventClicked += (c, v) =>
            {
                trackSize = 1;
            };
            UIButton btnOneWay = AddUIComponent<UIButton>();
            btnModernStyle.atlas = atlas;
            btnModernStyle.color = new Color32(206, 206, 206, 255);
            btnModernStyle.size = new Vector2(0.5f * (width - 16), 16);
            btnModernStyle.relativePosition = new Vector2(8, 140);
            btnModernStyle.eventClicked += (c, v) =>
            {
                trackDirection = 0;
            };
            UIButton btnTwoWay = AddUIComponent<UIButton>();
            btnClassicStyle.atlas = atlas;
            btnClassicStyle.color = new Color32(206, 206, 206, 255);
            btnClassicStyle.size = new Vector2(0.5f * (width - 16), 16);
            btnClassicStyle.relativePosition = new Vector2(8 + (0.5f * width) - 16, 140);
            btnModernStyle.eventClicked += (c, v) =>
            {
                trackDirection = 1;
            };

        UICheckBox useFenceCheckBox = AddUIComponent<UICheckBox>();
            useFenceCheckBox.text = "Island Platform";
            useFenceCheckBox.size = new Vector2(width - 16, 16);
            useFenceCheckBox.relativePosition = new Vector2(8, 220);
            useFenceCheckBox.isInteractive = true;
            useFenceCheckBox.eventCheckChanged += (c, v) =>
            {
                fence = useFenceCheckBox.isChecked;
                if (fence)
                {
                    m_useFenceCheckBoxClicker.spriteName = "check-checked";
                }
                else
                {
                    m_useFenceCheckBoxClicker.spriteName = "check-unchecked";
                }
            };

            m_useFenceCheckBoxClicker = useFenceCheckBox.AddUIComponent<UISprite>();
            m_useFenceCheckBoxClicker.atlas = atlas;
            m_useFenceCheckBoxClicker.spriteName = "check-unchecked";
            m_useFenceCheckBoxClicker.relativePosition = new Vector2(0, 0);
            m_useFenceCheckBoxClicker.size = new Vector2(16, 16);
            m_useFenceCheckBoxClicker.isInteractive = true;

            UILabel useFenceLabel = useFenceCheckBox.AddUIComponent<UILabel>();
            useFenceLabel.relativePosition = new Vector2(20, 0);
            useFenceLabel.text = "Island Platform";
            useFenceLabel.height = 16;
            useFenceLabel.isInteractive = true;

        }

        //private void RestoreStationTrackStyles(BuildingInfo info)
        //{
        //    for (var i = 0; i < info.m_paths.Length; i++)
        //    {
        //        var path = info.m_paths[i];
        //        if (path?.m_netInfo?.name != null && path.m_netInfo.IsUndergroundMetroStationTrack())
        //        {
        //            if (m_PrevTrackType != StationTrackType.SidePlatform)
        //            {
        //                path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
        //            }
        //        }
        //    }
        //}
        private void SetNetToolPrefab()
        {
            NetInfo prefab = null;
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
            if (prefab != null)
            {
                m_netTool.m_prefab = prefab;
            }
        }

        private void Activate(NetInfo nInfo)
        {
            m_activated = true;
            m_currentNetInfo = nInfo;
            isVisible = true;
            SetNetToolPrefab();
        }
        private void Deactivate()
        {
            if (!m_activated)
            {
                return;
            }
            m_currentNetInfo = null;
            isVisible = false;
            m_activated = false;
        }
    }
}
