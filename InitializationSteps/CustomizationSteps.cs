using ColossalFramework.Globalization;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul.InitializationSteps
{
    public static class CustomizationSteps
    {
        public static void AddConcreteProps(NetInfo prefab, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Tunnel)
            {
                var lanes = prefab.m_lanes.ToList();
                var propLane = new NetInfo.Lane();
                propLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                var propsList = new List<NetLaneProps.Prop>();
                if (version == NetInfoVersion.Tunnel)
                {
                    var propName = "Tunnel Light Small Road";
                    propsList.AddBasicProp(propName, new Vector3(-3.5f, 6, 0), 270);
                }
                propLane.m_laneProps.m_props = propsList.ToArray();
                lanes.Add(propLane);
                prefab.m_lanes = lanes.ToArray();
            }
        }
        public static void AddConcreteStationProps(NetInfo prefab, NetInfoVersion version)
        {
            var propLanes = prefab.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            for (var i = 0; i < propLanes.Count; i++)
            {
                propLanes[i].m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                var propsList = new List<NetLaneProps.Prop>();
                if (version == NetInfoVersion.Tunnel)
                {
                    var propName = "Wall Light White";
                    propsList.AddBasicProp(propName, new Vector3(-1, 6.7f, 0), 90, 10);
                }
                propLanes[i].m_laneProps.m_props = propsList.ToArray();
            }
        }

        private static void AddBasicProp(this List<NetLaneProps.Prop> propList, string propName, Vector3 position, float angle, float repeatDistance = 0)
        {
            var theProp = new NetLaneProps.Prop();
            var lightPropInfo = PrefabCollection<PropInfo>.FindLoaded(propName);
            theProp.m_prop = lightPropInfo;
            theProp.m_position = position;
            theProp.m_repeatDistance = repeatDistance;
            theProp.m_angle = angle;
            propList.Add(theProp);
        }

        public static void CommonCustomizationNoBar(NetInfo prefab, NetInfoVersion version)  //TODO(earalov): do we need to customize slope version too?
        {
        }
        public static void SetStandardTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = 5;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.m_halfWidth = 4.9999f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Slope:
                    prefab.m_halfWidth = 6.5f;
                    prefab.m_pavementWidth = 3f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_pavementWidth = 2.5f;
                    prefab.m_halfWidth = 6;
                    break;
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 5;
                    prefab.m_pavementWidth = 2.5f;
                    break;
            }
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