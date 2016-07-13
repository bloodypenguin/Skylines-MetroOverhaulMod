using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace SingleTrainTrack.UI
{
    public class ArrowsButtonSetup : MonoBehaviour
    {
        public void Update()
        {
            var panel = GameObject.Find("TracksOptionPanel(PublicTransportPanel)");
            if (panel == null)
            {
                return;
            }
            var arrowsButton = GameObject.Find("StreetDirectionViewerButton");
            if (arrowsButton == null)
            {
                return;
            }
            var clone = GameObject.Instantiate(arrowsButton);
            var button = clone.GetComponent<UIMultiStateButton>();
            var uip = panel.GetComponent<UIPanel>();
            uip.AttachUIComponent(clone);
            var originalMultiStateButton = arrowsButton.GetComponent<UIMultiStateButton>();
            button.relativePosition = originalMultiStateButton.relativePosition;

            var extensions = (List<IThreadingExtension>)SimulationManager.instance.m_ThreadingWrapper.GetType()
                .GetField("m_ThreadingExtensions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(SimulationManager.instance.m_ThreadingWrapper);
            var extension = extensions.First(x => x.GetType().GetField("arrowManager", BindingFlags.NonPublic | BindingFlags.Instance) != null);
            var arrowManager =
                extension.GetType()
                    .GetField("arrowManager", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(extension);

            button.eventActiveStateIndexChanged += (component, value) =>
            {
                if (value == 0)
                {
                    arrowManager.GetType().GetMethod("DestroyArrows").Invoke(arrowManager, new object[] {});
                }
                else
                {
                    arrowManager.GetType().GetMethod("CreateArrows").Invoke(arrowManager, new object[] {});
                }
            };
            
            Destroy(this);
        }
    }
}