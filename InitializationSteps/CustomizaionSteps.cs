using ColossalFramework.Globalization;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.InitializationSteps
{
    public static class CustomizaionSteps
    {
        public static void CommonConcreteCustomization(NetInfo prefab, NetInfoVersion version)
        {
            if (version != NetInfoVersion.Slope)
            {
                return;
            }
            prefab.m_halfWidth = 7.5f;
            prefab.m_pavementWidth = 4.8f;
        }

        public static void CommonCustomizationNoBar(NetInfo prefab, NetInfoVersion version)  //TODO(earalov): do we need to customize slope version too?
        {
            if (version != NetInfoVersion.Ground)
            {
                return;
            }
            prefab.m_halfWidth = 5;
        }

        public static void ReplaceTrackIcon(NetInfo prefab, NetInfoVersion version)
        {
            if (version != NetInfoVersion.Ground)
            {
                return;
            }
            var metroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
            prefab.m_Atlas = metroTrack.m_Atlas;
            prefab.m_Thumbnail = metroTrack.m_Thumbnail;
            prefab.m_InfoTooltipAtlas = metroTrack.m_InfoTooltipAtlas;
            prefab.m_InfoTooltipThumbnail = metroTrack.m_InfoTooltipThumbnail;
            prefab.m_isCustomContent = false;
            var locale = LocaleManager.instance.GetLocale();
            var key = new Locale.Key { m_Identifier = "NET_TITLE", m_Key = prefab.name };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, locale.Get(new Locale.Key { m_Identifier = "NET_TITLE", m_Key = "Metro Track" }));
            }
            key = new Locale.Key { m_Identifier = "NET_DESC", m_Key = prefab.name };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, locale.Get(new Locale.Key { m_Identifier = "NET_DESC", m_Key = "Metro Track" }));
            }
        }
    }
}