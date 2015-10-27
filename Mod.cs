using ICities;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class Mod : LoadingExtensionBase, IUserMod
    {

        public static Initializer Container;

        public string Name
        {
            get { return "Extra Train Station Tracks"; }
        }

        public string Description
        {
            get { return "Provides more types of train station tracks"; }
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (Container == null) {
                Container = new GameObject("ElevatedTrainStationTrack").AddComponent<Initializer>();
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