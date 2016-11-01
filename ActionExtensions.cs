using System;

namespace MetroOverhaul
{
    public static class ActionExtensions
    {
        public static Action<T1, T2> BeginChain<T1, T2>()
        {
            return (arg1, arg2) => { };
        }

        public static Action<T1, T2> Chain<T1, T2, T3>(this Action<T1, T2> action1, Action<T1, T2, T3> action2, T3 param1)
        {
            return (arg1, arg2) =>
            {
                action1.Invoke(arg1, arg2);
                action2?.Invoke(arg1, arg2, param1);
            };
        }

        public static Action<T1, T2> Chain<T1, T2, T3, T4>(this Action<T1, T2> action1, Action<T1, T2, T3, T4> action2, T3 param1, T4 param2)
        {
            return (arg1, arg2) =>
            {
                action1.Invoke(arg1, arg2);
                action2.Invoke(arg1, arg2, param1, param2);
            };
        }

        public static Action<T1> Chain<T1>(this Action<T1> action1, Action<T1> action2)
        {
            return arg =>
            {
                action1.Invoke(arg);
                action2?.Invoke(arg);
            };
        }

        public static Action<T1, T2> Chain<T1, T2>(this Action<T1, T2> action1, Action<T1, T2> action2)
        {
            return (arg1, arg2) =>
            {
                action1.Invoke(arg1, arg2);
                action2?.Invoke(arg1, arg2);
            };
        }
    }
}