using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MetroOverhaul.Redirection.Attributes;

namespace MetroOverhaul.Redirection
{
    public static class RedirectionUtil
    {

        public static Dictionary<MethodInfo, RedirectCallsState> RedirectAssembly()
        {
            var redirects = new Dictionary<MethodInfo, RedirectCallsState>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                redirects.AddRange(RedirectType(type));
            }
            return redirects;
        }

        public static void RevertRedirects(Dictionary<MethodInfo, RedirectCallsState> redirects)
        {
            if (redirects == null)
            {
                return;
            }
            foreach (var kvp in redirects)
            {
                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
            {
                return;
            }
            foreach (var element in source)
                target.Add(element);
        }

        public static Dictionary<MethodInfo, RedirectCallsState> RedirectType(Type type, bool onCreated = false)
        {
            var redirects = new Dictionary<MethodInfo, RedirectCallsState>();

            var customAttributes = type.GetCustomAttributes(typeof(TargetTypeAttribute), false);
            if (customAttributes.Length != 1)
            {
                throw new Exception($"No target type specified for {type.FullName}!");
            }
            if (!GetRedirectedMethods<RedirectMethodAttribute>(type).Any() && !GetRedirectedMethods<RedirectReverseAttribute>(type).Any())
            {
                throw new Exception($"No redirects specified for {type.FullName}!");
            }
            var targetType = ((TargetTypeAttribute)customAttributes[0]).Type;
            RedirectMethods(type, targetType, redirects, onCreated);
            RedirectReverse(type, targetType, redirects, onCreated);
            return redirects;
        }

        private static void RedirectMethods(Type type, Type targetType, Dictionary<MethodInfo, RedirectCallsState> redirects, bool onCreated)
        {
            foreach (var method in GetRedirectedMethods<RedirectMethodAttribute>(type, onCreated))
            {
                //                UnityEngine.Debug.Log($"Redirecting {targetType.Name}#{method.Name}...");
                RedirectMethod(targetType, method, redirects);
            }
        }

        private static IEnumerable<MethodInfo> GetRedirectedMethods<T>(Type type, bool onCreated) where T : RedirectAttribute
        {
            return GetRedirectedMethods<T>(type).Where(method =>
                {
                    var redirectAttributes = method.GetCustomAttributes(typeof(T), false);
                    return ((T)redirectAttributes[0]).OnCreated == onCreated;
                });
        }

        private static IEnumerable<MethodInfo> GetRedirectedMethods<T>(Type type) where T : RedirectAttribute
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(method =>
                {
                    var redirectAttributes = method.GetCustomAttributes(typeof(T), false);
                    return redirectAttributes.Length == 1;
                });
        }

        private static void RedirectReverse(Type type, Type targetType, Dictionary<MethodInfo, RedirectCallsState> redirects, bool onCreated)
        {
            foreach (var method in GetRedirectedMethods<RedirectReverseAttribute>(type, onCreated))
            {
                //                UnityEngine.Debug.Log($"Redirecting reverse {targetType.Name}#{method.Name}...");
                RedirectMethod(targetType, method, redirects, true);
            }
        }

        private static void RedirectMethod(Type targetType, MethodInfo method, Dictionary<MethodInfo, RedirectCallsState> redirects, bool reverse = false)
        {
            var tuple = RedirectMethod(targetType, method, reverse);
            redirects.Add(tuple.First, tuple.Second);
        }


        private static Tuple<MethodInfo, RedirectCallsState> RedirectMethod(Type targetType, MethodInfo detour, bool reverse)
        {
            var parameters = detour.GetParameters();
            Type[] types;
            if (parameters.Length > 0 && (
                (!targetType.IsValueType && parameters[0].ParameterType == targetType) ||
                (targetType.IsValueType && parameters[0].ParameterType == targetType.MakeByRefType())))
            {
                types = parameters.Skip(1).Select(p => p.ParameterType).ToArray();
            }
            else
            {
                types = parameters.Select(p => p.ParameterType).ToArray();
            }
            var originalMethod = targetType.GetMethod(detour.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, types,
                null);
            var redirectCallsState =
                reverse ? RedirectionHelper.RedirectCalls(detour, originalMethod) : RedirectionHelper.RedirectCalls(originalMethod, detour);
            return Tuple.New(reverse ? detour : originalMethod, redirectCallsState);
        }
    }
}