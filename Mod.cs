using ICities;
using UnityEngine;

namespace OneWayTrainTrack
{
    public class Mod : LoadingExtensionBase, IUserMod
    {

        public static Initializer Container;

        public string Name
        {
            get { return "One-Way Train Track"; }
        }

        public string Description
        {
            get { return "Provides a one-way train track"; }
        }

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (Container == null)
            {
                Container = new GameObject("OneWayTrainTrack").AddComponent<Initializer>();
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