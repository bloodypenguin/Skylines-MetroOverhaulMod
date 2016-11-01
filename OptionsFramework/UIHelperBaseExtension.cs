using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using ICities;

namespace MetroOverhaul.OptionsFramework
{
    public static class UIHelperBaseExtension
    {
        public static void AddOptionsGroup<T>(this UIHelperBase helper) where T : IModOptions
        {

            var properties = (from property in typeof(T).GetProperties() select property.Name).Where(name => name != "FileName");
            var groups = new Dictionary<string, UIHelperBase>();
            foreach (var propertyName in properties)
            {
                var description = OptionsWrapper<T>.Options.GetPropertyDescription(propertyName);
                var groupName = OptionsWrapper<T>.Options.GetPropertyGroup(propertyName);
                if (groupName == null)
                {
                    helper.ProcessProperty<T>(propertyName, description);
                }
                else
                {
                    if (!groups.ContainsKey(groupName))
                    {
                        groups[groupName] = helper.AddGroup(groupName);
                    }
                    groups[groupName].ProcessProperty<T>(propertyName, description);
                }
            }
        }

        private static void ProcessProperty<T>(this UIHelperBase group, string name, string description) where T : IModOptions
        {
            if (OptionsWrapper<T>.Options.IsCheckbox(name))
            {
                group.AddCheckbox<T>(description, name, OptionsWrapper<T>.Options.GetCheckboxAction(name));
            }
            else if (OptionsWrapper<T>.Options.IsTextField(name))
            {
                group.AddTextField<T>(description, name, OptionsWrapper<T>.Options.GetTextFieldAction(name));
            }
            else if (OptionsWrapper<T>.Options.IsDropdown(name))
            {
                group.AddDropdown<T>(description, name, OptionsWrapper<T>.Options.GetDropdownItems(name), OptionsWrapper<T>.Options.GetDropdownAction(name));
            }
            //TODO: more control types
        }

        private static UIDropDown AddDropdown<T>(this UIHelperBase group, string text, string propertyName, IList<KeyValuePair<string, int>> items, Action<int> action) where T : IModOptions
        {
            var property = typeof(T).GetProperty(propertyName);
            var defaultCode = (int)property.GetValue(OptionsWrapper<T>.Options, null);
            int defaultSelection;
            try
            {
                defaultSelection = items.First(kvp => kvp.Value == defaultCode).Value;
            } catch {
                defaultSelection = 0;
                property.SetValue(OptionsWrapper<T>.Options, items.First().Value, null);
            }
            return (UIDropDown) group.AddDropdown(text, items.Select(kvp => kvp.Key).ToArray(), defaultSelection, sel =>
            {
                var code = items[sel].Value;
                property.SetValue(OptionsWrapper<T>.Options, code, null);
                OptionsWrapper<T>.SaveOptions();
                action.Invoke(code);
            });
        }

        private static UICheckBox AddCheckbox<T>(this UIHelperBase group, string text, string propertyName, Action<bool> action) where T : IModOptions
        {
            var property = typeof(T).GetProperty(propertyName);
            return (UICheckBox)group.AddCheckbox(text, (bool)property.GetValue(OptionsWrapper<T>.Options, null),
                b =>
                {
                    property.SetValue(OptionsWrapper<T>.Options, b, null);
                    OptionsWrapper<T>.SaveOptions();
                    action.Invoke(b);
                });
        }

        private static UITextField AddTextField<T>(this UIHelperBase group, string text, string propertyName, Action<string> action) where T : IModOptions
        {
            var property = typeof(T).GetProperty(propertyName);
            var initialValue = Convert.ToString(property.GetValue(OptionsWrapper<T>.Options, null));
            return (UITextField)group.AddTextfield(text, initialValue, s => { },
                s =>
                {
                    object value;
                    if (property.PropertyType == typeof(int))
                    {
                        value = Convert.ToInt32(s);
                    }
                    else if (property.PropertyType == typeof(short))
                    {
                        value = Convert.ToInt16(s);
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        value = Convert.ToDouble(s);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        value = Convert.ToSingle(s);
                    }
                    else
                    {
                        value = s; //TODO: more types
                    }
                    property.SetValue(OptionsWrapper<T>.Options, value, null);
                    OptionsWrapper<T>.SaveOptions();
                    action.Invoke(s);
                });
        }
    }
}