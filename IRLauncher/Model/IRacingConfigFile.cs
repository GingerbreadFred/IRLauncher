using System;
using System.IO;
using System.Linq;
using IniParser.Model;

namespace IRLauncher
{    
    internal static class IRacingConfigFile
    {
        public static void CopyToAppIni(Config config, string carName)
        {
            IniParser.FileIniDataParser srcParser = new IniParser.FileIniDataParser();
            srcParser.Parser.Configuration.SkipInvalidLines = true;
            IniData srcData = srcParser.ReadFile(GetIRacingConfigFile());

            Config.Car car = config.Cars.Find((c) => c.CarName == carName);

            if (car == null)
            {
                return;
            }

            foreach (var key in config.KeysToSave)
            {
                var keyEntry = car.Keys.FirstOrDefault((k) => k.key.Equals(key));
                if (keyEntry.Equals(default(Config.IniKey)))
                {
                    continue;
                }


                srcData.Sections[keyEntry.key.section][keyEntry.key.value] = keyEntry.value;
            }
            
            srcParser.WriteFile(GetIRacingConfigFile(), srcData);
        }

        public static void CopyFromAppIni(Config config, string carName)
        {
            IniParser.FileIniDataParser srcParser = new IniParser.FileIniDataParser();
            IniData srcData = srcParser.ReadFile(GetIRacingConfigFile());

            Config.Car car = config.Cars.Find((c) => c.CarName == carName);

            if (car == null)
            {
                car = new Config.Car(carName);
                config.Cars.Add(car);
            }

            car.Keys.Clear();

            foreach (var key in config.KeysToSave)
            {
                if (!srcData.Sections.ContainsSection(key.section) || !srcData.Sections[key.section].ContainsKey(key.value))
                {
                    throw new InvalidDataException(String.Format("iRacing app.ini does not contain data for {0}:{1}", key.section, key.value));
                }

                car.Keys.Add(new Config.IniValue(new Config.IniKey(key.section, key.value), srcData.Sections[key.section][key.value]));
            }

            ConfigIO.WriteToDisk(config);
        }

        public static string GetIRacingConfigFile()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string iRacingAppIni = Path.Combine(documentsFolder, @"iRacing\app.ini"); ;

            if (!File.Exists(iRacingAppIni))
            {
                throw new FileNotFoundException("Cannot find iracing ini");
            }

            return iRacingAppIni;
        }
    }
}
