using ColossalFramework.UI;
using UnityEngine;

namespace SingleTrainTrack.UI
{
    public class UpgradeSetup : MonoBehaviour
    {
        public void Update()
        {
            var panel = GameObject.Find("TracksOptionPanel(PublicTransportPanel)");
            if (panel == null)
            {
                return;
            }
            var toolModeGo = panel.transform.FindChild("ToolMode");
            var tabstrip = toolModeGo.GetComponent<UITabstrip>();
            if (tabstrip.tabCount > 3)
            {
                return;
            }
            var rop = panel.GetComponent<RoadsOptionPanel>();
            rop.m_Modes = new[]
            {
            NetTool.Mode.Straight,
            NetTool.Mode.Curved,
            NetTool.Mode.Freeform,
            NetTool.Mode.Upgrade
            };
            var button = tabstrip.AddTab("Upgrade");
            button.size = new Vector2(36, 36);
            var upgrade = GameObject.Find("Upgrade").GetComponent<UIButton>();
            button.atlas = upgrade.atlas;
            button.normalFgSprite = upgrade.normalFgSprite;
            button.pressedFgSprite = upgrade.pressedFgSprite;
            button.disabledFgSprite = upgrade.disabledFgSprite;
            button.focusedFgSprite = upgrade.focusedFgSprite;
            button.hoveredFgSprite = upgrade.hoveredFgSprite;
            button.normalBgSprite = upgrade.normalBgSprite;
            button.pressedBgSprite = upgrade.pressedBgSprite;
            button.disabledBgSprite = upgrade.disabledBgSprite;
            button.focusedBgSprite = upgrade.focusedBgSprite;
            button.hoveredBgSprite = upgrade.hoveredBgSprite;
            button.text = "";
            Destroy(this);
        }
    }
}