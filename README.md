# Bim4EveryoneSetup

[![JetBrains Rider](https://img.shields.io/badge/JetBrains-Rider-blue.svg)](https://www.jetbrains.com/rider)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)
[![Revit 2022-2024](https://img.shields.io/badge/Revit-2022--2024-blue.svg)](https://www.autodesk.com/products/revit/overview)

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
* `CORP_NAME` - `any value` имя компании установки (`Bim4Everyone`)
* `CORP_SETTINGS_PATH` - `path` путь до корпоративных настроек компании (`""`)

### Cвойства настроек [телеметрии](https://github.com/Bim4Everyone/Bim4EveryoneTelemetry):

* `TELEMETRY_ACTIVE` - `enable/disable` включение телеметрии (`disable`)
* `TELEMETRY_USE_UTC` - `yes/no` включение отправки времени как utc (`yes`)
* `TELEMETRY_SERVER_URL` - `link` ссылка до точки доступа телеметрии (`localhost`)
* `APP_TELEMETRY_ACTIVE` - `enable/disable` включение телеметрии приложения (`disable`)
* `APP_TELEMETRY_EVENT_FLAGS` - `HEX` флаги телеметрии приложения (`0x4000400004003`)
* `APP_TELEMETRY_SERVER_URL` - `link` ссылка до точки доступа телеметрии (`localhost`)
* `LOG_TRACE_ACTIVE` - `enable/disable` включение трасировки логов (`disable`)
* `LOG_TRACE_LEVEL` - `debug/fatal/information` уровень логгирования (`information`)
* `LOG_TRACE_SERVER_URL` - `link` ссылка до точки доступа трассировки логов (`localhost`)

### Cвойства социальных сетей:

* `SOCIALS_MAIN` - `link` ссылка на основной чат (`https://t.me/bim4everyone_group`)
* `SOCIALS_NEWS` - `link` ссылка на новосной канал (`https://t.me/bim4everyone_news`)
* `SOCIALS_DISCUSS` - `link` ссылка на обсуждение новостей (`https://t.me/bim4everyone_discuss`)
* `SOCIALS_2D` - `link` ссылка на чат вкладки 2D (`https://t.me/bim4everyone_group/12`)
* `SOCIALS_BIM` - `link` ссылка на чат вкладки BIM (`https://t.me/bim4everyone_group/11`)
* `SOCIALS_AR` - `link` ссылка на чат вкладки АР (`https://t.me/bim4everyone_group/8`)
* `SOCIALS_KR` - `link` ссылка на чат вкладки КР (`https://t.me/bim4everyone_group/7`)
* `SOCIALS_OVVK` - `link` ссылка на чат вкладки ОВиВК (`https://t.me/bim4everyone_group/6`)
* `SOCIALS_PAGE` - `link` ссылка на страницу (`https://bim4everyone.com/`)
* `SOCIALS_DOWNLOADS` - `link` ссылка на закачку (`https://github.com/Bim4Everyone/Bim4EveryoneSetup/releases/latest`)

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