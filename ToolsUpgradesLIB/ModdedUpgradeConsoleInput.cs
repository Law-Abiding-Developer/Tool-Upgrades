using System.Collections.Generic;
using UnityEngine;

namespace UpgradesLIB;

public class ModdedUpgradeConsoleInput : MonoBehaviour, IProtoEventListener
{
    private string[] _slots;
    public Equipment equipment;
    public void Awake()
    {
        equipment = new Equipment(gameObject, gameObject.transform);
        var techType = CraftData.GetTechType(gameObject);
        _slots = DataTypes.Equipment[techType];
        equipment._label = DataTypes.Labels[techType];
        equipment.AddSlots(_slots);
    }

    public void OpenPDA()
    {
        if (equipment == null) return;
        Inventory.main.SetUsedStorage(equipment);
        if (Player.main.pda.Open(PDATab.Inventory)) return;
        Player.main.pda.Close();
    }

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        Plugin.SaveData.instances.Add(GetPrefabIdentifier().Id, equipment.SaveEquipment());
        Plugin.SaveData.Save();
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        StorageHelper.TransferEquipment(gameObject, Plugin.SaveData.instances[GetPrefabIdentifier().Id], equipment);
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