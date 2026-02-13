using System;
using System.Collections.Generic;
using Nautilus.Json;

namespace UpgradesLIB;

[Serializable]
public class UpgradePanelSaveData : SaveDataCache
{
    public Dictionary<string, Dictionary<string, string>> instances = new();
}