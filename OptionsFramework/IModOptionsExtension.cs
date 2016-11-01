using System;
using System.Collections.Generic;

namespace MetroOverhaul.OptionsFramework
{
    public static class IModOptionsExtension
    {
        public static string GetPropertyDescription<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes =
                (AbstractOptionsAttribute[]) fi.GetCustomAttributes(typeof(AbstractOptionsAttribute), false);
            if (attributes.Length > 0)
                return attributes[0].Description;
            throw new Exception($"No description specified for property {propertyName}!");
        }

        public static string GetPropertyGroup<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes =
                (AbstractOptionsAttribute[]) fi.GetCustomAttributes(typeof(AbstractOptionsAttribute), false);
            return attributes.Length > 0 ? attributes[0].Group : null;
        }

        public static bool IsCheckbox<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (CheckboxAttribute[]) fi.GetCustomAttributes(typeof(CheckboxAttribute), false);
            return attributes.Length > 0;
        }

        public static Action<bool> GetCheckboxAction<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (CheckboxAttribute[]) fi.GetCustomAttributes(typeof(CheckboxAttribute), false);
            if (attributes.Length != 1 || attributes[0].Action == null)
                return b => { };
            return attributes[0].Action;
        }

        public static bool IsTextField<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (TextFieldAttribute[]) fi.GetCustomAttributes(typeof(TextFieldAttribute), false);
            return attributes.Length > 0;
        }

        public static Action<string> GetTextFieldAction<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (TextFieldAttribute[]) fi.GetCustomAttributes(typeof(TextFieldAttribute), false);
            if (attributes.Length != 1 || attributes[0].Action == null)
                return b => { };
            return attributes[0].Action;
        }

        public static bool IsDropdown<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (DropDownAttribute[]) fi.GetCustomAttributes(typeof(DropDownAttribute), false);
            return attributes.Length > 0;
        }

        public static Action<int> GetDropdownAction<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (DropDownAttribute[]) fi.GetCustomAttributes(typeof(DropDownAttribute), false);
            if (attributes.Length != 1 || attributes[0].Action == null)
                return b => { };
            return attributes[0].Action;
        }

        public static IList<KeyValuePair<string, int>> GetDropdownItems<T>(this T value, string propertyName) where T : IModOptions
        {
            var fi = value.GetType().GetProperty(propertyName);
            var attributes = (DropDownAttribute[])fi.GetCustomAttributes(typeof(DropDownAttribute), false);
            if (attributes.Length != 1 || attributes[0].Items == null)
                return new List<KeyValuePair<string, int>>();
            return attributes[0].Items;
        }

    }
}