using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace DoubleJump
{
    // Token: 0x02000004 RID: 4
    [BepInPlugin("com.thepro.doublejump", "Double Jump", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        // Token: 0x06000003 RID: 3 RVA: 0x0000206C File Offset: 0x0000026C
        private void Awake()
        {
            harmony = new Harmony(Info.Metadata.GUID);
            logger = Logger;
            config = Config;
            maxNJumps = config.Bind<int>("Settings", "Added air jumps", 1);
            harmony.PatchAll();
        }

        // Token: 0x04000002 RID: 2
        internal static Harmony harmony;

        // Token: 0x04000003 RID: 3
        internal static ManualLogSource logger;

        // Token: 0x04000004 RID: 4
        internal static ConfigFile config;

        // Token: 0x04000005 RID: 5
        internal static ConfigEntry<int> maxNJumps;

        // Token: 0x04000006 RID: 6
        internal static Sprite atomSprite;
    }
}