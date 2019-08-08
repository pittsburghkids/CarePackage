using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Data structure for items in config.json.
[System.Serializable]
public class CarePackageItemConfig
{
    public string name;
    public int[] ids;
}

// Data structure for boxes in config.json.
[System.Serializable]
public class CarePackageBoxConfig
{
    public string name;
    public int[] ids;
}

// Data structure for config.json.
[System.Serializable]
public class CarePackageConfig
{
    public string meterSlot;
    public string meterBox;
    public string depotSlot;

    public CarePackageBoxConfig[] boxes;
    public CarePackageItemConfig[] items;
}

public class CarePackage : MonoBehaviour
{
    // Config object loaded from config.json.
    public CarePackageConfig carePackageConfig;

    // Storage map for items placed in boxes.
    private Dictionary<string, List<string>> storageMap = new Dictionary<string, List<string>>();

    // Sprite map for items.

    private Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();

    // Singleton creation.
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
        // Load configuration from config.json.
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");

        if (File.Exists(path))
        {
            string fileText = File.ReadAllText(path);
            carePackageConfig = JsonUtility.FromJson<CarePackageConfig>(fileText);

            Debug.Log("CarePackageConfig loaded.");
        }

        // Load item sprites.
        foreach (CarePackageItemConfig carePackageItem in carePackageConfig.items)
        {
            string itemFileName = carePackageItem.name + ".png";
            string itemFilePath = Path.Combine(Application.streamingAssetsPath, "items");
            itemFilePath = Path.Combine(itemFilePath, itemFileName);

            if (File.Exists(itemFilePath))
            {
                // Load texture from file.
                byte[] itemBytes = File.ReadAllBytes(itemFilePath);
                Texture2D itemTexture = new Texture2D(2, 2);
                itemTexture.LoadImage(itemBytes);

                // Create and store sprite.
                Sprite itemSprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), new Vector2(.5f, .5f), 4000);
                spriteMap.Add(carePackageItem.name, itemSprite);
            }
            else
            {
                Debug.LogWarningFormat("File not found: {0}", itemFileName);
            }
        }
    }

    // "Store" item in given box.
    public void Store(string box, string item)
    {
        Debug.LogFormat("Storing {0} in {1}", item, box);

        if (!storageMap.ContainsKey(box))
        {
            storageMap.Add(box, new List<string>());
        }

        storageMap[box].Add(item);
    }

    // Remove all items from box.
    public void EmptyBox(string boxName)
    {
        if (boxName != null && storageMap.ContainsKey(boxName))
        {
            storageMap[boxName].Clear();
        }
    }

    // Return a list of items stored in a box.
    public List<string> GetItemsInBox(string boxName)
    {
        if (boxName != null && storageMap.ContainsKey(boxName))
        {
            return storageMap[boxName];
        }

        return null;
    }

    // Find box by side ID from config.json.
    public string GetBoxByID(string id)
    {
        int sideID = -1;
        int.TryParse(id, out sideID);
        if (sideID == -1) return null;
        return GetBoxByID(sideID);
    }

    // Find box by side ID from config.json.
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

    // Find item by side ID from config.json.
    public string GetItemByID(string id)
    {
        int sideID = -1;
        int.TryParse(id, out sideID);
        if (sideID == -1) return null;
        return GetItemByID(sideID);
    }

    // Find item by side ID from config.json.
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

    // Get sprite image for item name.
    public Sprite GetSpriteForItemName(string itemName)
    {
        if (spriteMap.ContainsKey(itemName))
        {
            return spriteMap[itemName];
        }
        return null;
    }

}