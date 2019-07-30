using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class CarePackageItemConfig
{
    public string name;
    public int[] ids;
}

[System.Serializable]
public class CarePackageBoxConfig
{
    public string name;
    public int[] ids;
}

[System.Serializable]
public class CarePackageConfig
{
    public string meterSlot;
    public string meterBox;
    public string depotSlotA;
    public string depotSlotB;

    public CarePackageBoxConfig[] boxes;
    public CarePackageItemConfig[] items;
}

public class CarePackage : MonoBehaviour
{
    public CarePackageConfig carePackageConfig;

    private Dictionary<string, List<string>> storageMap = new Dictionary<string, List<string>>();

    private static CarePackage instance;
    public static CarePackage Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject instanceGameObject = new GameObject("CarePackage");
                instance = instanceGameObject.AddComponent<CarePackage>();
                DontDestroyOnLoad(instanceGameObject);
            }

            return instance;
        }
    }

    void Awake()
    {
        string path = Path.Combine(Application.dataPath, "config.json");

        if (File.Exists(path))
        {
            string fileText = File.ReadAllText(path);
            carePackageConfig = JsonUtility.FromJson<CarePackageConfig>(fileText);

            Debug.Log("CarePackageConfig loaded.");
        }
    }

    public void Store(string box, string item)
    {
        Debug.LogFormat("Storing {0} in {1}", item, box);

        if (!storageMap.ContainsKey(box))
        {
            storageMap.Add(box, new List<string>());
        }

        storageMap[box].Add(item);
    }

    public void EmptyBox(string boxName)
    {
        if (boxName != null && storageMap.ContainsKey(boxName))
        {
            storageMap[boxName].Clear();
        }
    }

    public List<string> GetItemsInBox(string boxName)
    {
        if (boxName != null && storageMap.ContainsKey(boxName))
        {
            return storageMap[boxName];
        }

        return null;
    }

    public string GetBoxByID(string id)
    {
        int sideID = -1;
        int.TryParse(id, out sideID);
        if (sideID == -1) return null;
        return GetBoxByID(sideID);
    }

    public string GetBoxByID(int id)
    {
        foreach (CarePackageBoxConfig carePackageBox in carePackageConfig.boxes)
        {
            foreach (int sideID in carePackageBox.ids)
            {
                if (id == sideID)
                {
                    return carePackageBox.name;
                }
            }
        }
        return null;
    }

    public string GetItemByID(string id)
    {
        int sideID = -1;
        int.TryParse(id, out sideID);
        if (sideID == -1) return null;
        return GetItemByID(sideID);
    }

    public string GetItemByID(int id)
    {
        foreach (CarePackageItemConfig carePackageItem in carePackageConfig.items)
        {
            foreach (int sideID in carePackageItem.ids)
            {
                if (id == sideID)
                {
                    return carePackageItem.name;
                }
            }
        }
        return null;
    }

}