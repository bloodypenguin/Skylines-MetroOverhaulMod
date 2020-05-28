using ColossalFramework;
using ColossalFramework.Globalization;
using System.Collections.Generic;
using System.Linq;
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

        public static void ModifyTitle(this PrefabInfo info, string newTitle)
        {
            var localizedStringsField = typeof(Locale).GetFieldByName("m_LocalizedStrings");
            var locale = SingletonLite<LocaleManager>.instance.GetLocale();
            var localizedStrings = (Dictionary<Locale.Key, string>)localizedStringsField.GetValue(locale);

            var kvp =
                localizedStrings
                .FirstOrDefault(kvpInternal =>
                    kvpInternal.Key.m_Identifier == "NET_TITLE" &&
                    kvpInternal.Key.m_Key == info.name);

            if (!Equals(kvp, default(KeyValuePair<Locale.Key, string>)))
            {
                localizedStrings[kvp.Key] = newTitle;
            }
        }
    }
}
