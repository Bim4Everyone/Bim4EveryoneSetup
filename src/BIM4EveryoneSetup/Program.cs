﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using WixSharp;
using WixSharp.CommonTasks;

using File = System.IO.File;

namespace BIM4EveryoneSetup {
    internal class Program {
        public static void Main() {
            // Создаем папку, куда сохраняем билд
            Directory.CreateDirectory(Constants.BinPath);
            
            // Создаем файл с версией
            Console.WriteLine("Creating msi version file");
            File.WriteAllText(Constants.MsiVersionFile, Constants.CurrentVersion.ToString());
            
            // Выкачиваем установщик pyRevit
            Console.WriteLine("Downloading pyRevit installer");
            Extensions.DownloadFile(Constants.pyRevitInstallUrl, Constants.pyRevitInstallFile);
            
            // Выкачиваем файл расширений платформы
            Console.WriteLine("Downloading platform extensions.json");
            Extensions.DownloadFile(Constants.ExtensionsFileUrl, Constants.ExtensionsAssetFile);
            
            // Выкачиваем все расширения
            foreach (FeatureExtension featureExtension in FeatureExtension.GetFeatures()) {
                Console.WriteLine($"Downloading platform extension: {featureExtension.Name}");
                featureExtension.GitClone();
            }
            
            Console.WriteLine("Building platform settings msi");
            BuildMsi();
        }

        private static string BuildMsi() {
            // Создаем проект установщика
            ManagedProject project = new ManagedProject(Constants.ProductName);

            // Добавляем расширения
            project.AddDir(new Dir(Constants.pyRevitExtensionsDirPath, FeatureExtension.GetFeatures()
                .Select(item => item.CreateDir())
                .OfType<WixEntity>()
                .ToArray()));

            // XXX: WIX1026: id is too long for an identifier.
            // Standard identifiers are 72 characters
            // project.CustomIdAlgorithm = entity => {
            //     return entity.Id.Length > 72
            //         ? Guid.NewGuid().ToString()
            //         : null;
            // };

            project.OutDir = Constants.BinPath;
            project.OutFileName = "Bim4Everyone_" + Constants.CurrentVersion;

            project.SetBinaries();
            project.SetProductUI();
            project.SetProductInfo();
            project.SetProductActions();
            project.SetProductSettings();
            project.SetProductConfiguration();
            
            // Устанавливаем стратегию обновлений
            // разрешаем устанавливать более младшие версии
            project.MajorUpgrade = new MajorUpgrade() {
                Disallow = false,
                AllowDowngrades = false,
                DowngradeErrorMessage = "Установлена более поздняя версия продукта!"
            };
            
            Console.WriteLine("Environment");
            Console.WriteLine($"\tAUTOUPDATE: {Environment.GetEnvironmentVariable("AUTOUPDATE")}");
            Console.WriteLine($"\tROCKETMODE: {Environment.GetEnvironmentVariable("ROCKETMODE")}");
            Console.WriteLine($"\tCHECKUPDATES: {Environment.GetEnvironmentVariable("CHECKUPDATES")}");
            Console.WriteLine($"\tUSERCANEXTEND: {Environment.GetEnvironmentVariable("USERCANEXTEND")}");
            Console.WriteLine($"\tUSERCANCONFIG: {Environment.GetEnvironmentVariable("USERCANCONFIG")}");
            Console.WriteLine($"\tCOREUSERLOCALE: {Environment.GetEnvironmentVariable("COREUSERLOCALE")}");

            // Добавляем свойства
            project.AddProperties(
                new Property(Constants.pyRevitVersionProp, "0.0.0"),
                new Property(Constants.pyRevitInstalledProp, "False"),
                new Property("AUTOUPDATE", Environment.GetEnvironmentVariable("AUTOUPDATE")),
                new Property("ROCKETMODE", Environment.GetEnvironmentVariable("ROCKETMODE")),
                new Property("CHECKUPDATES", Environment.GetEnvironmentVariable("CHECKUPDATES")),
                new Property("USERCANEXTEND", Environment.GetEnvironmentVariable("USERCANEXTEND")),
                new Property("USERCANCONFIG", Environment.GetEnvironmentVariable("USERCANCONFIG")),
                new Property("COREUSERLOCALE", Environment.GetEnvironmentVariable("COREUSERLOCALE"))
            );

            // Добавляем изображения платформы
            project.WixVariables = new Dictionary<string, string>() {
                {"WixUIBannerBmp", Constants.WixUIBannerBmp},
                {"WixUIDialogBmp", Constants.WixUIDialogBmp}
            };
            
            // Добавляем зависимости библиотек CustomActions
            project.DefaultRefAssemblies.Add(typeof(JsonConvert).Assembly.Location);

            return project.BuildMsi();
        }
    }
}