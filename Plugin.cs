using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace Bloodcraft_Re
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static Harmony _harmony;
        internal static ManualLogSource Log { get; set; }

        public static class MyPluginInfo
        {
            public const string PLUGIN_GUID = "B_Re";
            public const string PLUGIN_NAME = "Bloodcraft_Re";
            public const string PLUGIN_VERSION = "1.0.0";
        }

        public override void Load()
        {
            Log = base.Log;
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            Log.LogInfo($"Bloodcraft_Re Loading...");
        }

        public override bool Unload()
        {

            Log.LogInfo($"Mod Unloaded: {MyPluginInfo.PLUGIN_NAME}");
            return base.Unload();
        }
    }
}
