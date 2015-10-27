using System;
using System.Collections;
using ColossalFramework;

namespace ElevatedTrainStationTrack
{
    public class Loading
    {
        public static IEnumerator ActionWrapper(Action a)
        {
            a.Invoke();
            yield break;
        }

        public static void QueueLoadingAction(Action action)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(ActionWrapper(action));
        }

        public static void QueueLoadingAction(IEnumerator action)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(action);
        }

    }
}