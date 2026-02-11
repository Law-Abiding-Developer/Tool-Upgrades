using System.Collections.Generic;
using HarmonyLib;
using UpgradesLIB.Items.Equipment;

namespace UpgradesLIB;

[HarmonyPatch(typeof(GhostCrafter))]
public class GhostCrafterPatches
{
    [HarmonyPatch(nameof(GhostCrafter.OnHandHover))]
    [HarmonyPrefix]
    public static bool OnHandHover_Patches(GhostCrafter __instance, GUIHand hand)
    {
        if (!__instance.gameObject.
                TryGetComponent<HandHeldFabricator>(out var pt))
            return true;
        pt.pickupable.OnHandHover(hand);
        return false;
    }

    [HarmonyPatch(nameof(GhostCrafter.OnHandClick))]
    [HarmonyPrefix]
    public static bool OnHandClick_Patches(GhostCrafter __instance, GUIHand hand)
    {
        if (!__instance.gameObject.TryGetComponent<HandHeldFabricator>(out var pt)) return true;
        pt.pickupable.OnHandClick(hand);
        return false;
    }
}

[HarmonyPatch(typeof(uGUI_Equipment))]
public class uGUI_EquipmentPatches
{
    
    [HarmonyPatch(nameof(uGUI_Equipment.Awake))]
    [HarmonyPrefix]
    public static void Awake_Patches(uGUI_Equipment __instance)
    {
        foreach (var slotArray in DataTypes.Slots)
        {
            Utilities.CloneSlots(__instance, slotArray);
        }
    }
}