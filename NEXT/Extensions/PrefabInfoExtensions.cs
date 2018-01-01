using System.Reflection;
using UnityEngine;

namespace MetroOverhaul.NEXT.Extensions
{
    public static class PrefabInfoExtensions
    {
        public static T Clone<T>(this T originalPrefabInfo, string newName)
            where T : PrefabInfo
        {
            var gameObject = Object.Instantiate(originalPrefabInfo.gameObject);
            gameObject.transform.parent = originalPrefabInfo.gameObject.transform; // N.B. This line is evil and removing it is killoing the game's performances
            gameObject.name = newName;

            var info = gameObject.GetComponent<T>();
            info.m_prefabInitialized = false;

            return info;
        }

        public static void SetUICategory(this PrefabInfo info, string category)
        {
            typeof(PrefabInfo).GetField("m_UICategory", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(info, category);
        }
    }
}
