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

		private UIButton btnModernStyle;
		private UIButton btnClassicStyle;
		private UIButton btnSingleTrack;
		private UIButton btnDoubleTrack;
		private UIButton btnOneWay;
		private UIButton btnTwoWay;

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
			SetNetToolPrefab();
		}

		private void CreateUI()
		{
#if DEBUG
			Next.Debug.Log("MOM TRACK GUI Created");
#endif

			backgroundSprite = "GenericPanel";
			color = new Color32(73, 68, 84, 170);
			width = 200;
			height = 250;
			opacity = 90;
			position = Vector2.zero;
			isVisible = false;
			isInteractive = true;
			padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

			UIPanel dragHandlePanel = AddUIComponent<UIPanel>();
			dragHandlePanel.atlas = atlas;
			dragHandlePanel.backgroundSprite = "GenericPanel";
			dragHandlePanel.width = width;
			dragHandlePanel.height = 20;
			dragHandlePanel.opacity = 100;
			dragHandlePanel.color = new Color32(21, 140, 34, 255);
			dragHandlePanel.relativePosition = Vector3.zero;

			UIDragHandle dragHandle = dragHandlePanel.AddUIComponent<UIDragHandle>();
			dragHandle.width = width;
			dragHandle.height = 20;
			dragHandle.relativePosition = Vector3.zero;
			dragHandle.target = this;

			UILabel titleLabel = dragHandlePanel.AddUIComponent<UILabel>();
			titleLabel.relativePosition = new Vector3() { x = 5, y = 3, z = 0 };
			titleLabel.textAlignment = UIHorizontalAlignment.Center;
			titleLabel.text = "Track Attributes";
			titleLabel.isInteractive = false;

			btnModernStyle = CreateButton("Modern", new Vector3(8, 50), (c, v) =>
			{
				trackStyle = 0;
				SetNetToolPrefab();
			});

			btnClassicStyle = CreateButton("Classic", new Vector3(8 + (0.5f * width) - 16, 50), (c, v) =>
			  {
				  trackStyle = 1;
				  SetNetToolPrefab();
			  });
			btnSingleTrack = CreateButton("Single", new Vector3(8, 100), (c, v) =>
			  {
				  trackSize = 0;
				  SetNetToolPrefab();
			  });

			btnDoubleTrack = CreateButton("Double", new Vector3(8 + (0.5f * width) - 16, 100), (c, v) =>
			  {
				  trackSize = 1;
				  SetNetToolPrefab();
			  });

			btnOneWay = CreateButton("OneWay", new Vector3(8, 150), (c, v) =>
			 {
				 trackDirection = 0;
				 SetNetToolPrefab();
			 });

			btnTwoWay = CreateButton("TwoWay", new Vector3(8 + (0.5f * width) - 16, 150), (c, v) =>
			  {
				  trackDirection = 1;
				  SetNetToolPrefab();
			  });



			UICheckBox useFenceCheckBox = AddUIComponent<UICheckBox>();
			useFenceCheckBox.text = "Island Platform";
			useFenceCheckBox.size = new Vector2(width - 16, 16);
			useFenceCheckBox.relativePosition = new Vector2(8, 200);
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
				SetNetToolPrefab();
			};

			m_useFenceCheckBoxClicker = useFenceCheckBox.AddUIComponent<UISprite>();
			m_useFenceCheckBoxClicker.atlas = atlas;
			m_useFenceCheckBoxClicker.spriteName = "check-unchecked";
			m_useFenceCheckBoxClicker.relativePosition = new Vector2(0, 0);
			m_useFenceCheckBoxClicker.size = new Vector2(16, 16);
			m_useFenceCheckBoxClicker.isInteractive = true;

			UILabel useFenceLabel = useFenceCheckBox.AddUIComponent<UILabel>();
			useFenceLabel.relativePosition = new Vector2(20, 0);
			useFenceLabel.text = "Alt/Barrier";
			useFenceLabel.height = 16;
			useFenceLabel.isInteractive = true;

		}

		private UIButton CreateButton(string text, Vector3 pos, MouseEventHandler eventClick)
		{
			var button = this.AddUIComponent<UIButton>();
			button.width = 80;
			button.height = 30;
			button.normalBgSprite = "ButtonMenu";
			button.color = new Color32(150, 150, 150, 255);
			button.disabledBgSprite = "ButtonMenuDisabled";
			button.hoveredBgSprite = "ButtonMenuHovered";
			button.hoveredColor = new Color32(163, 255, 16, 255);
			button.focusedBgSprite = "ButtonMenu";
			button.focusedColor = new Color32(163, 255, 16, 255);
			button.pressedBgSprite = "ButtonMenuPressed";
			button.pressedColor = new Color32(163, 255, 16, 255);
			button.textColor = new Color32(255, 255, 255, 255);
			button.normalBgSprite = "ButtonMenu";
			button.focusedBgSprite = "ButtonMenuFocused";
			button.playAudioEvents = true;
			button.opacity = 75;
			button.dropShadowColor = new Color32(0, 0, 0, 255);
			button.dropShadowOffset = new Vector2(-1, -1);
			button.text = text;
			button.relativePosition = pos;
			button.eventClick += eventClick;

			return button;
		}

		private void SetNetToolPrefab()
		{
			if (trackStyle == 0)
			{
				btnModernStyle.color = new Color32(163, 255, 16, 255);
				btnModernStyle.normalBgSprite = "ButtonMenuFocused";
				btnModernStyle.useDropShadow = true;
				btnModernStyle.opacity = 95;
				btnClassicStyle.color = new Color32(150, 150, 150, 255);
				btnClassicStyle.normalBgSprite = "ButtonMenu";
				btnClassicStyle.useDropShadow = false;
				btnClassicStyle.opacity = 75;
			}
			else if (trackStyle == 1)
			{
				btnClassicStyle.color = new Color32(163, 255, 16, 255);
				btnClassicStyle.normalBgSprite = "ButtonMenuFocused";
				btnClassicStyle.useDropShadow = true;
				btnClassicStyle.opacity = 95;
				btnModernStyle.color = new Color32(150, 150, 150, 255);
				btnModernStyle.normalBgSprite = "ButtonMenu";
				btnModernStyle.useDropShadow = false;
				btnModernStyle.opacity = 75;
			}
			if (trackSize == 0)
			{
				btnSingleTrack.color = new Color32(163, 255, 16, 255);
				btnSingleTrack.normalBgSprite = "ButtonMenuFocused";
				btnSingleTrack.useDropShadow = true;
				btnSingleTrack.opacity = 95;
				btnDoubleTrack.color = new Color32(150, 150, 150, 255);
				btnDoubleTrack.normalBgSprite = "ButtonMenu";
				btnDoubleTrack.useDropShadow = false;
				btnDoubleTrack.opacity = 75;
			}
			else if (trackSize == 1)
			{
				btnDoubleTrack.color = new Color32(163, 255, 16, 255);
				btnDoubleTrack.normalBgSprite = "ButtonMenuFocused";
				btnDoubleTrack.useDropShadow = true;
				btnDoubleTrack.opacity = 95;
				btnSingleTrack.color = new Color32(150, 150, 150, 255);
				btnSingleTrack.normalBgSprite = "ButtonMenu";
				btnSingleTrack.useDropShadow = false;
				btnSingleTrack.opacity = 75;
			}
			if (trackDirection == 0)
			{
				btnOneWay.color = new Color32(163, 255, 16, 255);
				btnOneWay.normalBgSprite = "ButtonMenuFocused";
				btnOneWay.useDropShadow = true;
				btnOneWay.opacity = 95;
				btnTwoWay.color = new Color32(150, 150, 150, 255);
				btnTwoWay.normalBgSprite = "ButtonMenu";
				btnOneWay.useDropShadow = false;
				btnTwoWay.opacity = 75;
			}
			else if (trackDirection == 1)
			{
				btnTwoWay.color = new Color32(163, 255, 16, 255);
				btnTwoWay.normalBgSprite = "ButtonMenuFocused";
				btnTwoWay.useDropShadow = true;
				btnTwoWay.opacity = 95;
				btnOneWay.color = new Color32(150, 150, 150, 255);
				btnOneWay.normalBgSprite = "ButtonMenu";
				btnOneWay.useDropShadow = false;
				btnOneWay.opacity = 75;
			}
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
