# Bim4EveryoneSetup

[![JetBrains Rider](https://img.shields.io/badge/JetBrains-Rider-blue.svg)](https://www.jetbrains.com/rider)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)
[![Revit 2020-2024](https://img.shields.io/badge/Revit-2020--2024-blue.svg)](https://www.autodesk.com/products/revit/overview)

Платформа созданная для упрощения разработки проектной документации в Autodesk Revit.

## Сборка проекта

```
dotnet build -c Release
```

### Cвойства настроек платформы:

* `AUTOUPDATE` - `enable/disable` включение автообновления (`enable`)
* `ROCKETMODE` - `enable/disable` включение рокетмода (`enable`)
* `CHECKUPDATES` - `enable/disable` проверку обновлений (`enable`)
* `USERCANEXTEND` - `yes/no` разрешение добавление расширений пользователю (`yes`)
* `USERCANCONFIG` -  `yes/no` разрешение редактировать конфигурацию пользователю (`yes`)
* `COREUSERLOCALE` - `en/en_us/ru/fr/...etc` локализация pyRevit при запуске (`ru`)

### Пример использовании свойств
```
dotnet build -c Release -p AUTOUPDATE=enable -p ROCKETMODE=disable -p COREUSERLOCALE=fr
```

## Установка платформы

```
msiexec /i Bim4Everyone_24.1.1.msi
```

### Настройка развертывания платформы

Для настройки развертывания платформы `Bim4Everyone` в сети
требуется запустить `.msi` файл с передачей параметров `.msi` через командную строку.
Список **свойств** перечислен в главе "**Cвойства настроек платформы**".

### Пример

```
msiexec /i Bim4Everyone_24.1.1.msi AUTOUPDATE=enable ROCKETMODE=disable COREUSERLOCALE=fr
```

## Лицензия

Данный репозиторий под [лицензией MIT](https://en.wikipedia.org/wiki/MIT_License).

---

Понравился репозиторий? Пожалуйста, [оцените проект на GitHub](../../stargazers)!

---

Copyright © 2024 Bim4Everyone, Biseuv Olzhas (dosymep)