using MetroOverhaul.OptionsFramework.Attibutes;

namespace MetroOverhaul.OptionsFramework.Extensions
{
    public static class CommonExtensions
    {
        public static string GetPropertyDescription<T>(this T value, string propertyName)
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes =
                (AbstractOptionsAttribute[]) fi.GetCustomAttributes(typeof(AbstractOptionsAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : null;
        }

        public static string GetPropertyGroup<T>(this T value, string propertyName)
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes =
                (AbstractOptionsAttribute[]) fi.GetCustomAttributes(typeof(AbstractOptionsAttribute), false);
            return attributes.Length > 0 ? attributes[0].Group : null;
        }

        public static TR GetAttribute<T, TR>(this T value, string propertyName)where TR : AbstractOptionsAttribute
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (TR[])fi.GetCustomAttributes(typeof(TR), false);
            return attributes.Length != 1 ? null : attributes[0];
        }

    }
}