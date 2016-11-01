using MetroOverhaul.NEXT;

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

        public static void CommonCustomizationAlt(NetInfo prefab, NetInfoVersion version)
        {
            if (version != NetInfoVersion.Ground)
            {
                return;
            }
            prefab.m_halfWidth = 5;
        }
    }
}