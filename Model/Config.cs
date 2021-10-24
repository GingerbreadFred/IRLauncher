
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IRLauncher
{
    public class Config
    {
        public struct IniKey
        {
            public readonly string section;
            public readonly string value;

            public IniKey(string section, string value)
            {
                this.section = section;
                this.value = value;
            }
        }

        public struct IniValue
        {
            public readonly IniKey key;
            public readonly string value;

            public IniValue(IniKey key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }

        public class Car
        {
            public string CarName;
            public List<IniValue> Keys = new List<IniValue>();

            public Car(string carName)
            {
                CarName = carName;
            }
        }

        public void DeleteCar(string selectedCar)
        {
            Cars.RemoveAll((c) => c.CarName.ToLower() == selectedCar.ToLower());
        }

        public List<IniKey> KeysToSave { get; set; } = new List<IniKey>();
        public List<Car> Cars { get; set; } = new List<Car>();
        public string UIPath { get; set; }
    }

    public static class ConfigIO
    {
        private static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"IRLauncher\config.json");

        public static Config ReadFromDisk()
        {
            if (File.Exists(ConfigPath))
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
            }
            else
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("IRLauncher.DefaultConfig.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    var config = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                    WriteToDisk(config);
                    return config;
                }
            }
        }
        public static void WriteToDisk(Config config)
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
    }
}
