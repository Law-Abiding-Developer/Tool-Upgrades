using System.Collections.Generic;
using Nautilus.Json;

namespace UpgradesLIB;

public class UpgradePanelSaveData : SaveDataCache
{
    public Dictionary<PrefabIdentifier, Dictionary<string, string>> instances = new();
}