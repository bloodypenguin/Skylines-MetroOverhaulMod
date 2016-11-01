using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace MetroOverhaul.OptionsFramework
{
    public class OptionsWrapper<T> where T : IModOptions
    {
        private static T _instance;

        public static T Options
        {
            get
            {
                if (_instance == null)
                {
                    LoadOptions();
                }
                return _instance;
            }
        }

        private static void LoadOptions()
        {
            try
            {
                _instance = (T)Activator.CreateInstance(typeof(T));
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    var fileName = _instance.FileName;
                    if (!fileName.EndsWith(".xml"))
                    {
                        fileName = fileName + ".xml";
                    }
                    using (var streamReader = new StreamReader(fileName))
                    {
                        var options = (T)xmlSerializer.Deserialize(streamReader);
                        foreach (var propertyInfo in typeof(T).GetProperties())
                        {
                            if (!propertyInfo.CanWrite)
                            {
                                continue;
                            }
                            var value = propertyInfo.GetValue(options, null);
                            propertyInfo.SetValue(_instance, value, null);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    SaveOptions();// No options file yet
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static void SaveOptions()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var fileName = _instance.FileName;
                if (!fileName.EndsWith(".xml"))
                {
                    fileName = fileName + ".xml";
                }
                using (var streamWriter = new StreamWriter(fileName))
                {
                    xmlSerializer.Serialize(streamWriter, _instance);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}