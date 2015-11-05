using System;

namespace OneWayTrainTrack
{
    public static class ActionExtensions
    {
        public static Action<T1> Apply<T1, T2>(this Action<T1, T2> action, T2 param1)
        {
            return arg => action(arg, param1);
        }

        public static Action<T1> Apply<T1, T2, T3>(this Action<T1, T2, T3> action, T2 param1, T3 param2)
        {
            return arg => action(arg, param1, param2);
        }

        public static Action<T1> Chain<T1>(this Action<T1> action1, Action<T1> action2)
        {
            return arg =>
            {
                action1.Invoke(arg);
                action2.Invoke(arg);
            };
        } 
    }
}