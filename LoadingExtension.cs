using ICities;
using MetroOverhaul.Detours;
using MetroOverhaul.Redirection;
using UnityEngine;

namespace MetroOverhaul
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public static Initializer Container;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (Container == null)
            {
                Container = new GameObject("MetroOverhaul").AddComponent<Initializer>();
            }
            Redirector<DepotAIDetour>.Deploy();
            //Redirector<MetroTrainAIDetour>.Deploy(); //don't deploy this! For some reason that causes citizens not boarding trains
        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (Container == null)
            {
                return;
            }
            Object.Destroy(Container.gameObject);
            Container = null;
            Redirector<DepotAIDetour>.Revert();
            Redirector<MetroTrainAI>.Revert();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            foreach (var info in Resources.FindObjectsOfTypeAll<BuildingInfo>())
            {
                if (!(info.m_buildingAI is TransportStationAI) || info.m_class.m_subService != ItemClass.SubService.PublicTransportMetro)
                {
                    continue;
                }
                UpdateMetroStation(info);
            }
        }

        private static void UpdateMetroStation(BuildingInfo info)
        {
            var transportStationAi = ((TransportStationAI) info.m_buildingAI);
            transportStationAi.m_maxVehicleCount = 0;
        }
    }
}