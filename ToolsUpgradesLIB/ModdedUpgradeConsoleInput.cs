using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesLIB;

[Serializable]
public class ModdedUpgradeConsoleInput : MonoBehaviour, IProtoEventListener
{
    [SerializeField]
    private string[] _slots;
    [SerializeField]
    public Equipment Equipment;
    public void Awake()
    {
        Equipment = new Equipment(gameObject, gameObject.transform);
        var techType = CraftData.GetTechType(gameObject);
        _slots = DataTypes.Equipment[techType];
        Equipment._label = DataTypes.Labels[techType];
        Equipment.AddSlots(_slots);
    }

    public void OpenPDA()
    {
        if (Equipment == null) return;
        Inventory.main.SetUsedStorage(Equipment);
        if (Player.main.pda.Open(PDATab.Inventory)) return;
        Player.main.pda.Close();
    }

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        Plugin.SaveData.instances.Add(GetPrefabIdentifier().Id, Equipment.SaveEquipment());
        Plugin.SaveData.Save();
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        StorageHelper.TransferEquipment(gameObject, Plugin.SaveData.instances[GetPrefabIdentifier().Id], Equipment);
    }

    private PrefabIdentifier GetPrefabIdentifier()
    {
        GameObject go = gameObject;
        while (go.GetComponent<PrefabIdentifier>() == null)
        {
            go = go.transform.parent.gameObject;
        }
        return go.GetComponent<PrefabIdentifier>();
    }
}

public class DataTypes
{
    public static readonly List<DataTypes> Slots = new();
    public static readonly Dictionary<TechType, string[]> Equipment = new();
    public static readonly Dictionary<TechType, string> Labels = new();
    public string[] Strings;
    public TechType TechType;
    public DataTypes(string[] strings, TechType techType)
    {
        Strings = strings;
        TechType = techType;
    }
}