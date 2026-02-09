using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using Nautilus.Utility.ModMessages;
using UpgradesLIB.Items.Equipment;
using UnityEngine;
using UnityEngine.UI;
using PluginInfo = ToolsUpgradesLIB.PluginInfo;

namespace UpgradesLIB; // remember link: https://github.com/tinyhoot/Nautilus/blob/debugging-tutorial/Nautilus/Documentation/guides/debugging.md for debugging
/*
 * Remember:
 * CHECK THE GOD DAMN COMPONENT REQUIREMENTS
 * requirements contain anything that CANNOT BE NULL. NRE otherwise
 * Keywords:
 * Fields with AssertNotNull attribute
 * Fields not public (that don't have the NonSerialized attribute)
 * Public properties
 * "'I don't really get why it exists, it just decreases the chance of a collision from like 9.399613e-55% to like 8.835272e-111%, both are very small numbers' - Lee23"
 * Todo list:
 */
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }
    
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    public static TechGroup toolupgrademodules;
    public static TechGroup equipmentupgrademodules;
    public static TechCategory upgradelib;
    public static bool registered = false;
    public void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        Logger.LogInfo($"Awake method is running. Dependencies exist. Loading {PluginInfo.PLUGIN_NAME}");
        
        // Initialize custom prefabs
        InitializePrefabs();
        
        // Register Tech
        toolupgrademodules = EnumHandler.AddEntry<TechGroup>("Equipment Upgrades")
            .WithPdaInfo("Equipment Upgrade Modules");
        upgradelib = EnumHandler.AddEntry<TechCategory>("UpgradesLIB").WithPdaInfo("UpgradesLIB")
            .RegisterToTechGroup(equipmentupgrademodules);
        equipmentupgrademodules= EnumHandler.AddEntry<TechGroup>("EquipmentUpgrades")
            .WithPdaInfo("Equipment Upgrade Modules");

        ModMessageSystem.SendGlobal("FindMyUpdates","https://raw.githubusercontent.com/Law-Abiding-Troller/Tool-Upgrades/refs/heads/main/ToolsUpgradesLIB/Version.json");
        
        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_NAME}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded! Loading other plugins." );
    }

    private void InitializePrefabs()
    {
        Logger.LogInfo($"Initializing prefabs: " );
        Logger.LogInfo("Loading HandHeldFabricator..." );
        Handheldprefab.Register();
    }
}