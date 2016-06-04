using ICities;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class LoadingExtension : LoadingExtensionBase
    {

        public static Initializer Container;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (Container == null)
            {
                Container = new GameObject("ExtraTrainStationTracks").AddComponent<Initializer>();
            }
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
        }
    }
}