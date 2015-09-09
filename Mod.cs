using ICities;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class Mod : LoadingExtensionBase, IUserMod
    {

        public static Container Container;

        public string Name
        {
            get { return "ElevatedTrainStationTrack"; }
        }

        public string Description
        {
            get { return "ElevatedTrainStationTrack"; }
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (Container == null) {
                Container = new GameObject("ElevatedTrainStationTrack").AddComponent<Container>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            if (Container != null)
            {
                Object.Destroy(Container.gameObject);
            }
        }
    }
}