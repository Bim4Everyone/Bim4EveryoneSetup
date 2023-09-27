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
            File.WriteAllText(Constants.MsiVersionFile, Constants.CurrentVersion.ToString());
            
            // Выкачиваем установщик pyRevit
            Console.WriteLine("Выкачиваем установщик pyRevit");
            Extensions.DownloadFile(Constants.pyRevitInstallUrl, Constants.pyRevitInstallFile);
            
            // Выкачиваем файл расширений платформы
            Console.WriteLine("Выкачиваем файл расширений платформы");
            Extensions.DownloadFile(Constants.ExtensionsFileUrl, Constants.ExtensionsAssetFile);
            
            // Выкачиваем все расширения
            foreach (FeatureExtension featureExtension in FeatureExtension.GetFeatures()) {
                Console.WriteLine($"Выкачиваем расширение платформы: {featureExtension.Name}");
                featureExtension.GitClone();
            }
            
            Console.WriteLine("Начинаем сборку установочного файла");
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
            
            project.OutDir = Constants.BinPath;
            project.OutFileName = "Bim4Everyone_" + Constants.CurrentVersion;

            project.SetBinaries();
            project.SetProductUI();
            project.SetProductInfo();
            project.SetProductActions();
            project.SetProductSettings();
            
            // Устанавливаем стратегию обновлений
            // разрешаем устанавливать более младшие версии
            project.MajorUpgrade = new MajorUpgrade() {
                Disallow = false,
                AllowDowngrades = false,
                DowngradeErrorMessage = "Установлена более поздняя версия продукта!"
            };
            
            // Добавляем свойства
            project.AddProperties(
                new Property(Constants.pyRevitVersionProp, "0.0.0"),
                new Property(Constants.pyRevitInstalledProp, "False")
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