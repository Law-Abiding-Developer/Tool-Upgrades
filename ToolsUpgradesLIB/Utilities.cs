using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using Nautilus.Handlers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UpgradesLIB;

public static class Utilities
{
     public static IEnumerator CreateUpgradesContainer(TechType tech, string equipmentTypeName, string storageName, string storageClassID, string label, int totalSlots, BaseUnityPlugin owner, Action<GameObject> method = null)
     {
         return CreateUpgradesContainer<ModdedUpgradeConsoleInput>(tech, equipmentTypeName, storageName, storageClassID, label, totalSlots, owner, method);
     }
     
    public static IEnumerator CreateUpgradesContainer<T>(TechType tech, string equipmentTypeName, string storageName, string storageClassID, string label, int totalSlots, BaseUnityPlugin owner, Action<GameObject> method = null) 
        where T : ModdedUpgradeConsoleInput
    {
        EquipmentType equipmentType = EquipmentType.None;
        if (EnumHandler.ModdedEnumExists<EquipmentType>(equipmentTypeName)) ErrorMessage.AddError($"Equipment type of name {equipmentTypeName} already exists!");
        else
        {
            equipmentType = EnumHandler.AddEntry<EquipmentType>(equipmentTypeName).Value;
            if (!Types.ContainsKey(owner))
                Types.Add(owner, new List<EquipmentType>()
                {
                    equipmentType
                });
            else Types[owner].Add(equipmentType);
        }

        Plugin.Logger.LogInfo($"Fetching {tech}'s Prefab...");
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(tech);//fetch the prefab
        yield return task;//wait for prefab to finish
        Plugin.Logger.LogInfo("Prefab Fetched Successfully!");
        GameObject prefab = task.GetResult();//get the prefab
        Plugin.Logger.LogInfo($"The prefab for {tech} is {prefab}. Creating container for the prefab.");//log it because why no

        var child = new GameObject(storageName);
        child.transform.SetParent(prefab.transform, false);
        var cOI = child.AddComponent<ChildObjectIdentifier>();
        cOI.ClassId = storageClassID;
        
        var component = prefab.AddComponent<T>();
        var slots = new string[totalSlots];
        for (var i = 0; i < totalSlots; i++)
        {
            var str = equipmentType.ToString() + (i + 1);
            slots[i] = str;
            Equipment.slotMapping.Add(str, equipmentType);
        }
        DataTypes.Slots.Add(new DataTypes(slots,tech));
        DataTypes.Equipment.Add(tech, slots);
        DataTypes.Labels.Add(tech, label);
        DataTypes.ChildObjects.Add(tech, child);
        
        if (method != null) method.Invoke(prefab);
        Plugin.Logger.LogInfo("Upgrade Panel Added. If it opens, the task was successful");//log it
    }
    #nullable enable
    public static uGUI_EquipmentSlot? CloneSlots(uGUI_Equipment equipment, DataTypes moddedUpgradeConsoleInput,
        string copyTarget = "SeamothModule", string? imageTarget = "Seamoth", Vector3[]? slotPositions = null,
        float scale = 1)
    {
        var slots = moddedUpgradeConsoleInput.Strings;
        //TODO: Auto position X amount of slots
        Plugin.Logger.LogInfo("Cloning slots...");
        if (slots.Length == 0) return null;

        uGUI_EquipmentSlot slot = CloneSlot(equipment, $"{copyTarget}1", slots[0], scale);
        if (imageTarget != null)
        {
            var image = slot.transform.Find(imageTarget).GetComponent<Image>();
            image.sprite = SpriteManager.Get(moddedUpgradeConsoleInput.TechType);
            image.SetNativeSize();
            Plugin.Logger.LogDebug($"color data: {image.color.r},  {image.color.g}, {image.color.b}, {image.color.a}");
            image.color = new Color(0, image.color.g/1.4f, image.color.b, 0.25f);
            image.rectTransform.localScale = Vector3.one * 2f;
        }

        if (slotPositions != null)
        {
            slot.transform.localPosition = slotPositions[0];
        }

        for (int i = 1; i < slots.Length; i++)
        {
            var clonedSlot = CloneSlot(equipment, $"{copyTarget}{Mathf.Min(4, i + 1)}", slots[i], scale);
            if (slotPositions != null)
            {
                clonedSlot.transform.localPosition = slotPositions[i];
            }
        }
        return slot;
    }
    #nullable disable
    private static uGUI_EquipmentSlot CloneSlot(uGUI_Equipment equipmentMenu, string childName, string newSlotName, float scale)
    {
        Transform newSlot = Object.Instantiate(equipmentMenu.transform.Find(childName), equipmentMenu.transform);
        newSlot.transform.localScale = Vector3.one * scale;
        newSlot.name = newSlotName;
        uGUI_EquipmentSlot equipmentSlot = newSlot.GetComponent<uGUI_EquipmentSlot>();
        equipmentSlot.slot = newSlotName;
        return equipmentSlot;
    }

    private static Dictionary<BaseUnityPlugin, List<EquipmentType>> Types = new();

    public static List<EquipmentType> ClaimEquipmentTypes(BaseUnityPlugin owner)
    {
        if (!Types.ContainsKey(owner)) {Plugin.Logger.LogError("No Equipment Types to claim for plugin " + owner.name);
            return null;
        }
        return Types[owner];
    }

    /// <summary>
    /// Attempts to perform two linear searches on your player tool instance to find an upgrade panel given a name
    /// </summary>
    /// <param name="instance">The instance you need the upgrade panel from</param>
    /// <param name="storageName">The name of the panel you need</param>
    /// <param name="storageClassID">If the first search fails, give this an input to try to search for your panel</param>
    /// <returns>The upgrade panel you want to find</returns>
    public static ModdedUpgradeConsoleInput GetPanel(GameObject instance, string storageName, string storageClassID = null)
    {
        if (instance.TryGetComponent<ModdedUpgradeConsoleInput>(out var console)) return console;
        foreach (Transform item in instance.transform.Find(storageName))
        {
            if (item.TryGetComponent<ModdedUpgradeConsoleInput>(out var input)) return input;
        }

        foreach (var child in instance.GetComponentsInChildren<ChildObjectIdentifier>())
        {
            if (!child.ClassId.Equals(storageClassID)) continue;
            if (!child.TryGetComponent<ModdedUpgradeConsoleInput>(out var input)) continue;
            return input;
        }

        return null;
    }
}