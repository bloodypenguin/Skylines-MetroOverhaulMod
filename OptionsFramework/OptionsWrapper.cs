using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ColossalFramework.IO;
using MetroOverhaul.OptionsFramework.Attibutes;
using UnityEngine;

namespace MetroOverhaul.OptionsFramework
{
    public class OptionsWrapper<T>
    {
        private static T _instance;

        public static T Options
        {
            get
            {
                try
                {
                    Ensure();
                }
                catch (XmlException e)
                {
                    UnityEngine.Debug.LogError("Error reading options XML file");
                    UnityEngine.Debug.LogException(e);
                }
                return _instance;
            }
        }

        public static void Ensure()
        {
            if (_instance != null)
            {
                return;
            }
            var type = typeof(T);
            var attrs = type.GetCustomAttributes(typeof(OptionsAttribute), false);
            if (attrs.Length != 1)
            {
                throw new Exception($"Type {type.FullName} is not an options type!");
            }
            _instance = (T)Activator.CreateInstance(typeof(T));
            LoadOptions();
        }

        private static void LoadOptions()
        {
            try
            {
                if (GetLegacyFileName() != string.Empty)
                {
                    try
                    {
                        ReadOptionsFile(GetLegacyFileName());
                        try
                        {
                            File.Delete(GetLegacyFileName());
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogException(e);
                        }
                        SaveOptions();
                    }
                    catch (FileNotFoundException)
                    {
                        ReadOptionsFile(GetFileName());
                    }
                }
                else
                {
                    ReadOptionsFile(GetFileName());
                }
            }
            catch (FileNotFoundException)
            {
                SaveOptions();// No options file yet
            }
        }

        private static void ReadOptionsFile(string fileName)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var streamReader = new StreamReader(fileName))
            {
                var options = (T) xmlSerializer.Deserialize(streamReader);
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

        internal static void SaveOptions()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var streamWriter = new StreamWriter(GetFileName()))
                {
                    xmlSerializer.Serialize(streamWriter, _instance);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static string GetFileName()
        {
            var type = _instance.GetType();
            var attrs = type.GetCustomAttributes(typeof(OptionsAttribute), false);
            var fileName = Path.Combine(DataLocation.localApplicationData, ((OptionsAttribute) attrs[0]).FileName);
            if (!fileName.EndsWith(".xml"))
            {
                fileName = fileName + ".xml";
            }
            return fileName;
        }

        private static string GetLegacyFileName()
        {
            var type = _instance.GetType();
            var attrs = type.GetCustomAttributes(typeof(OptionsAttribute), false);
            var fileName =  ((OptionsAttribute)attrs[0]).LegacyFileName;
            if (fileName == string.Empty)
            {
                return fileName;
            }
            if (!fileName.EndsWith(".xml"))
            {
                fileName = fileName + ".xml";
            }
            return fileName;
        }
    }
}