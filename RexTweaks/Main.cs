using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using R2API;
using R2API.ContentManagement;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HIFURexTweaks
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "HIFURexTweaks";
        public const string PluginVersion = "1.3.0";

        public static ConfigFile HRTConfig;
        public static ConfigFile HRTBackupConfig;

        public static ConfigEntry<bool> enableAutoConfig { get; set; }
        public static ConfigEntry<string> latestVersion { get; set; }

        public static ManualLogSource HRTLogger;

        public static bool _preVersioning = false;

        public static BodyIndex rexBodyIndex;

        public void Awake()
        {
            HRTLogger = Logger;
            HRTConfig = Config;

            HRTBackupConfig = new(Paths.ConfigPath + "\\" + PluginAuthor + "." + PluginName + ".Backup.cfg", true);
            HRTBackupConfig.Bind(": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :");

            enableAutoConfig = HRTConfig.Bind("Config", "Enable Auto Config Sync", true, "Disabling this would stop HIFURexTweaks from syncing config whenever a new version is found.");
            _preVersioning = !((Dictionary<ConfigDefinition, string>)AccessTools.DeclaredPropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(HRTConfig, null)).Keys.Any(x => x.Key == "Latest Version");
            latestVersion = HRTConfig.Bind("Config", "Latest Version", PluginVersion, "DO NOT CHANGE THIS");
            if (enableAutoConfig.Value && (_preVersioning || (latestVersion.Value != PluginVersion)))
            {
                latestVersion.Value = PluginVersion;
                ConfigManager.VersionChanged = true;
                HRTLogger.LogInfo("Config Autosync Enabled.");
            }

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(TweakBase))
                                           select type;

            HRTLogger.LogInfo("==+----------------==TWEAKS==----------------+==");

            foreach (Type type in enumerable)
            {
                TweakBase based = (TweakBase)Activator.CreateInstance(type);
                if (ValidateTweak(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(MiscBase))
                                            select type;

            HRTLogger.LogInfo("==+----------------==MISC==----------------+==");

            foreach (Type type in enumerable2)
            {
                MiscBase based = (MiscBase)Activator.CreateInstance(type);
                if (ValidateMisc(based))
                {
                    based.Init();
                }
            }
        }

        private System.Collections.IEnumerator BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig)
        {
            yield return orig();
            rexBodyIndex = BodyCatalog.FindBodyIndex("TreebotBody(Clone)");
        }

        public bool ValidateTweak(TweakBase tb)
        {
            if (tb.isEnabled)
            {
                bool enabledfr = Config.Bind(tb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateMisc(MiscBase mb)
        {
            if (mb.isEnabled)
            {
                bool enabledfr = Config.Bind(mb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        private void PeripheryMyBeloved()
        {
        }
    }
}