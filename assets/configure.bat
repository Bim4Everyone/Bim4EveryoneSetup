@echo off
cd /d %appdata%\pyRevit-Master\bin

pyrevit detach --all
pyrevit attach master 277 2020
pyrevit attach master 277 2021
pyrevit attach master 277 2022
pyrevit attach master 277 2023
pyrevit attach master 277 2024

pyrevit extensions paths forget --all
pyrevit extensions paths add "%appdata%\pyRevit\Extensions"
pyrevit extensions update --all

pyrevit configs core:user_locale ru
pyrevit configs rocketmode enable
pyrevit configs autoupdate enable
pyrevit configs checkupdates enable
pyrevit configs usercanextend yes
pyrevit configs usercanconfig yes
pyrevit configs usercanconfig yes

pyrevit configs pyRevitBundlesCreatorExtension.extension:disabled true
pyrevit configs pyRevitBundlesCreatorExtension.extension:private_repo true
pyrevit configs pyRevitBundlesCreatorExtension.extension:username ""
pyrevit configs pyRevitBundlesCreatorExtension.extension:password ""

pyrevit configs pyRevitCore.extension:disabled true
pyrevit configs pyRevitCore.extension:private_repo true
pyrevit configs pyRevitCore.extension:username ""
pyrevit configs pyRevitCore.extension:password ""

pyrevit configs pyRevitDevHooks.extension:disabled true
pyrevit configs pyRevitDevHooks.extension:private_repo true
pyrevit configs pyRevitDevHooks.extension:username ""
pyrevit configs pyRevitDevHooks.extension:password ""

pyrevit configs pyRevitDevTools.extension:disabled true
pyrevit configs pyRevitDevTools.extension:private_repo true
pyrevit configs pyRevitDevTools.extension:username ""
pyrevit configs pyRevitDevTools.extension:password ""

pyrevit configs pyRevitTags.extension:disabled true
pyrevit configs pyRevitTags.extension:private_repo true
pyrevit configs pyRevitTags.extension:username ""
pyrevit configs pyRevitTags.extension:password ""

pyrevit configs pyRevitTemplates.extension:disabled true
pyrevit configs pyRevitTemplates.extension:private_repo true
pyrevit configs pyRevitTemplates.extension:username ""
pyrevit configs pyRevitTemplates.extension:password ""

pyrevit configs pyRevitTools.extension:disabled true
pyrevit configs pyRevitTools.extension:private_repo true
pyrevit configs pyRevitTools.extension:username ""
pyrevit configs pyRevitTools.extension:password ""

pyrevit configs pyRevitTutor.extension:disabled true
pyrevit configs pyRevitTutor.extension:private_repo true
pyrevit configs pyRevitTutor.extension:username ""
pyrevit configs pyRevitTutor.extension:password ""