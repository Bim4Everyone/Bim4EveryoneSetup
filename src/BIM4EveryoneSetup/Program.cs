﻿using System.IO;
using System.Text;

using Newtonsoft.Json;

using WixSharp;
using WixSharp.CommonTasks;

using File = System.IO.File;

namespace BIM4EveryoneSetup;

internal class Program {
    public static void Main() {
        // Создаем папку, куда сохраняем билд
        Directory.CreateDirectory(Constants.BinPath);

        // Создаем файл с версией
        Console.WriteLine("Creating a msi version file");
        File.WriteAllText(Constants.MsiVersionFile, Constants.CurrentTag);

        // Выкачиваем установщик pyRevit
        Console.WriteLine("Downloading pyRevit installer");
        Extensions.DownloadFile(Constants.pyRevitInstallUrl, Constants.pyRevitInstallFile);

        // Выкачиваем файл расширений платформы
        Console.WriteLine("Downloading platform extensions.json");
        Extensions.DownloadFile(Constants.ExtensionsFileUrl, Constants.ExtensionsAssetFile);

        // Выкачиваем все расширения
        foreach(FeatureExtension featureExtension in FeatureExtension.GetFeatures()) {
            Console.WriteLine($"Downloading platform extension: {featureExtension.Name}");
            featureExtension.GitClone();
        }

        Console.WriteLine("Building platform settings msi");
        BuildMsi();

#if RELEASE
        string branchName = Environment.GetEnvironmentVariable("GITHUB_REF")
                            ?? Process2.StartProcess("git", "branch --show-current").First();
        Console.WriteLine($"Current branch name: {branchName}");

        // refs/heads/main
        if(branchName.EndsWith("main")) {
            Console.WriteLine("Building extensions changelog");
            BuildChangelog();
        } else {
            Console.WriteLine("Skipping building extensions changelog");
        }
#endif
    }

    private static void BuildChangelog() {
        var builder = new StringBuilder();
        builder.AppendLine($"**{Constants.CurrentTag}**  ");

        // Обновляем расширения (чтобы возможно было пушить)
        string? botAccessToken = MsiVariables.BotAccessToken;
        if(!string.IsNullOrEmpty(botAccessToken)) {
            foreach(FeatureExtension featureExtension in FeatureExtension.GetFeatures()) {
                Console.WriteLine($"{featureExtension.Name}:");

                Console.WriteLine("\tUpdate remote");
                featureExtension.UpdateRemote(botAccessToken!);

                Console.WriteLine("\tCreate tag");
                featureExtension.CreateTag(Constants.CurrentTag);

                Console.WriteLine("\tPush tag");
                featureExtension.PushTag(Constants.CurrentTag);

                Console.WriteLine("\tGet changes");
                featureExtension.GetChanges(Constants.CurrentTag, Constants.LastTag, builder);
            }
        }

        string? value = Extensions.GetChanges(Constants.ProductUrl, null);
        if(!string.IsNullOrEmpty(value)) {
            builder.AppendLine(
                $"[Bim4EveryoneSetup]({Constants.ProductUrl}/compare/{Constants.LastTag}...{Constants.CurrentTag})");
            builder.AppendLine(value);
            builder.AppendLine();
        }

        Extensions.InsertText(Constants.ChangelogFile, builder.AppendLine("____").ToString());
        File.WriteAllText(Constants.ReleaseChangelogFile, builder.ToString());
    }

    private static string BuildMsi() {
        // Создаем проект установщика
        var project = new ManagedProject(Constants.ProductName);

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
        project.OutFileName = "Bim4Everyone_" + Constants.CurrentTag;

        var msiVariables = new MsiVariables();
        if(File.Exists(MsiVariables.MsiVariablesFile)) {
            Console.WriteLine("Msi variables file exists");
            string jsonContent = File.ReadAllText(MsiVariables.MsiVariablesFile!);
            msiVariables = JsonConvert.DeserializeObject<MsiVariables>(jsonContent) ?? new MsiVariables();
        }

        project.SetBinaries();
        project.SetProductUI();
        project.SetProductInfo();
        project.SetProductActions();
        project.SetProductSettings();
        project.SetProductConfiguration();

        project.SetProductProperties();
        project.SetProductSettingsProperties(msiVariables);
        project.SetProductSocialsProperties(msiVariables);
        project.SetProductTelemetryProperties(msiVariables);
        project.SetProductAppTelemetryProperties(msiVariables);
        project.SetProductLogTraceProperties(msiVariables);

        // Устанавливаем стратегию обновлений
        // разрешаем устанавливать более младшие версии
        project.MajorUpgrade = new MajorUpgrade {
            Disallow = false,
            AllowDowngrades = false,
            DowngradeErrorMessage = "Установлена более поздняя версия продукта!"
        };

        // Добавляем изображения платформы
        project.WixVariables = new Dictionary<string, string> {
            {"WixUIBannerBmp", Constants.WixUIBannerBmp}, {"WixUIDialogBmp", Constants.WixUIDialogBmp}
        };

        // Добавляем зависимости библиотек CustomActions
        project.DefaultRefAssemblies.Add(typeof(JsonConvert).Assembly.Location);

        return project.BuildMsi();
    }
}
