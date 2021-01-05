using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System.Linq;

// Data structure for boards in config.json.
[System.Serializable]
public class CarePackageBoardConfig
{
    public string name;
    public string serialNumber;
}

// Data structure for items in config.json.
[System.Serializable]
public class CarePackageItemConfig
{
    public string name;
    public int[] ids;
}

// Data structure for destination in config.json.
[System.Serializable]
public class CarePackageDestinationConfig
{
    public string name;
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
    public CarePackageBoardConfig[] boards;
    public CarePackageBoxConfig[] boxes;
    public CarePackageItemConfig[] items;
    public CarePackageDestinationConfig[] destinations;
}

public class CarePackageData
{
    public string type;

    public string boardName;
    public string boardSerialNumber;

    public string itemName;
    public string destinationName;
    public string boxName;
}

public class CarePackage : MonoBehaviour
{
    public bool loaderALoaded = false;
    public bool loaderBLoaded = false;

    // Local websocket connection.
    public WebSocketBridge webSocketBridge;

    // Data recieved events.
    public delegate void DataDelegate(CarePackageData carePackageData);
    public event DataDelegate OnData;

    // Config object loaded from config.json.
    public CarePackageConfig carePackageConfig;

    // Config events.
    public bool ConfigLoaded { get; private set; } = false;
    public delegate void ConfigDelegate();
    public event ConfigDelegate OnConfigLoaded;

    // Storage map for items placed in boxes and destinations.
    private Dictionary<string, List<string>> storageMap = new Dictionary<string, List<string>>();
    private Dictionary<string, string> destinationMap = new Dictionary<string, string>();

    // Sprite map for items and destinations.
    private Dictionary<string, Sprite> spriteMapItems = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> spriteMapDestinations = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> spriteMapLabels = new Dictionary<string, Sprite>();
    //private Dictionary<string, Sprite> spriteMapIcons = new Dictionary<string, Sprite>();


    // Box tracking.
    string currentMeterBoxA;

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
                instanceGameObject.AddComponent<CarePackageHUD>();
                DontDestroyOnLoad(instanceGameObject);
            }

            return instance;
        }
    }

    void Awake()
    {
#if !UNITY_EDITOR
        Cursor.visible = false;
#endif

        // Grab the config file.
        StartCoroutine(GetConfig());

        // Connect to websocket.
        webSocketBridge = new WebSocketBridge("ws://localhost:8080");
        webSocketBridge.OnReceived += OnWebSocketReceived;
    }

    void OnDestroy()
    {
        if (webSocketBridge != null) webSocketBridge.Close();
    }

    private IEnumerator GetConfig()
    {
        string url = "http://localhost:8080/config.json";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("UnityWebRequest Error: " + webRequest.error);
            }
            else
            {
                ProcessConfig(webRequest.downloadHandler.text);
            }
        }

    }

    private void ProcessConfig(string configString)
    {
        ConfigLoaded = true;
        if (OnConfigLoaded != null) OnConfigLoaded();

        carePackageConfig = JsonUtility.FromJson<CarePackageConfig>(configString);

        StartCoroutine(CreateSprites(carePackageConfig));
    }

    IEnumerator CreateSprites(CarePackageConfig carePackageConfig)
    {
        // Items.

        foreach (CarePackageItemConfig carePackageItem in carePackageConfig.items)
        {
            string url = string.Format("http://localhost:8080/images/items/{0}.png", carePackageItem.name);

            Debug.Log("Downloading item image for " + carePackageItem.name);

            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(unityWebRequest.error);
                }
                else
                {
                    Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    Debug.Log("Creating item sprite for " + carePackageItem.name);

                    Texture2D texture = new Texture2D(downloadedTexture.width, downloadedTexture.height);
                    texture.SetPixels(downloadedTexture.GetPixels(0));
                    texture.Apply();
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Trilinear;
                    texture.anisoLevel = 5;

                    // Create and store sprite.
                    Sprite itemSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 4096);
                    spriteMapItems.Add(carePackageItem.name, itemSprite);
                }
            }
        }

        // Icons.

        // foreach (CarePackageItemConfig carePackageItem in carePackageConfig.items)
        // {
        //     string url = string.Format("http://localhost:8080/images/icons/{0}.png", carePackageItem.name);

        //     Debug.Log("Downloading icon image for " + carePackageItem.name);

        //     using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
        //     {
        //         yield return unityWebRequest.SendWebRequest();

        //         if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        //         {
        //             Debug.Log(unityWebRequest.error);
        //         }
        //         else
        //         {
        //             Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(unityWebRequest);
        //             Debug.Log("Creating icon sprite for " + carePackageItem.name);

        //             Texture2D texture = new Texture2D(downloadedTexture.width, downloadedTexture.height);
        //             texture.SetPixels(downloadedTexture.GetPixels(0));
        //             texture.Apply();
        //             texture.wrapMode = TextureWrapMode.Clamp;
        //             texture.filterMode = FilterMode.Trilinear;
        //             texture.anisoLevel = 5;

        //             // Create and store sprite.
        //             Sprite iconSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 8196);
        //             spriteMapIcons.Add(carePackageItem.name, iconSprite);
        //         }
        //     }
        // }

        // Destinations.

        foreach (CarePackageDestinationConfig carePackageDestination in carePackageConfig.destinations)
        {
            string url = string.Format("http://localhost:8080/images/destinations/{0}.png", carePackageDestination.name);

            Debug.Log("Downloading destination image for " + carePackageDestination.name);

            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(unityWebRequest.error);
                }
                else
                {
                    Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    Debug.Log("Creating destination sprite for " + carePackageDestination.name);

                    Texture2D texture = new Texture2D(downloadedTexture.width, downloadedTexture.height);
                    texture.SetPixels(downloadedTexture.GetPixels(0));
                    texture.Apply();
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Trilinear;
                    texture.anisoLevel = 5;

                    // Create and store sprite.
                    Sprite destinationSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
                    spriteMapDestinations.Add(carePackageDestination.name, destinationSprite);
                }
            }
        }

        // Labels.

        foreach (CarePackageDestinationConfig carePackageDestination in carePackageConfig.destinations)
        {
            string url = string.Format("http://localhost:8080/images/labels/{0}.png", carePackageDestination.name);

            Debug.Log("Downloading label image for " + carePackageDestination.name);

            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(unityWebRequest.error);
                }
                else
                {
                    Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    Debug.Log("Creating label sprite for " + carePackageDestination.name);

                    Texture2D texture = new Texture2D(downloadedTexture.width, downloadedTexture.height);
                    texture.SetPixels(downloadedTexture.GetPixels(0));
                    texture.Apply();
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Trilinear;
                    texture.anisoLevel = 5;

                    // Create and store sprite.
                    Sprite labelSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 8196);
                    spriteMapLabels.Add(carePackageDestination.name, labelSprite);
                }
            }
        }

    }

    public void WebSocketSend(CarePackageData carePackageData)
    {
        string carePackageJSON = JsonUtility.ToJson(carePackageData);
        Debug.Log(carePackageJSON);
        webSocketBridge.Send(carePackageJSON);
    }

    public void OnWebSocketReceived(string message)
    {
        Debug.Log(message);

        CarePackageData carePackageData = null;
        try
        {
            carePackageData = JsonUtility.FromJson<CarePackageData>(message);
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Couldn't parse JSON message.");
        }

        if (carePackageData != null && OnData != null) OnData(carePackageData);
    }

    // Set deistionaion for given box.

    public void SetBoxDestination(string boxName, string destination)
    {
        Debug.LogFormat("Addressing {0} to {1}", boxName, destination);
        destinationMap[boxName] = destination;
    }

    public string GetDestinationForBox(string boxName)
    {
        if (destinationMap.ContainsKey(boxName))
        {
            return destinationMap[boxName];
        }

        return null;
    }

    // "Store" item in given box.
    public void StoreItemInBox(string boxName, string item)
    {
        Debug.LogFormat("Storing {0} in {1}", item, boxName);

        if (boxName != null)
        {
            if (!storageMap.ContainsKey(boxName))
            {
                storageMap.Add(boxName, new List<string>());
            }

            storageMap[boxName].Add(item);
        }
    }

    // Return a list of items stored in a box.
    public List<string> GetItemsInBox(string boxName)
    {
        if (boxName != null && storageMap.ContainsKey(boxName))
        {
            return new List<string>(storageMap[boxName]);
        }

        return null;
    }

    // Remove saved values for destination and items.
    public void ResetBox(string boxName)
    {
        if (storageMap.ContainsKey(boxName)) { storageMap[boxName].Clear(); }
        if (destinationMap.ContainsKey(boxName)) { destinationMap[boxName] = null; }
    }

    // public string GetRandomDestinationName()
    // {
    //     string destinationName = null;

    //     if (spriteMapDestinations.Count > 0)
    //     {
    //         int spriteIndex = Random.Range(0, spriteMapDestinations.Count);
    //         destinationName = spriteMapDestinations.ElementAt(spriteIndex).Key;
    //     }

    //     return destinationName;
    // }

    // public List<string> GetRandomItemNameList(int count)
    // {
    //     List<string> itemNames = null;

    //     // Make an array and shuffle it.
    //     string[] itemNameArray = spriteMapItems.Keys.ToArray();

    //     if (itemNameArray.Length == 0)
    //     {
    //         return null;
    //     }

    //     for (int i = 0; i < itemNameArray.Length; i++)
    //     {
    //         int j = Random.Range(i, itemNameArray.Length);

    //         string temp = itemNameArray[i];
    //         itemNameArray[i] = itemNameArray[j];
    //         itemNameArray[j] = temp;
    //     }

    //     itemNames = new List<string>(itemNameArray);
    //     itemNames = itemNames.GetRange(0, count);

    //     return itemNames;
    // }


    // Find board by serial number.
    public CarePackageBoardConfig GetBoardBySerialNumber(string serialNumber)
    {
        foreach (CarePackageBoardConfig carePackageBoard in carePackageConfig.boards)
        {
            if (carePackageBoard.serialNumber == serialNumber)
            {
                return carePackageBoard;
            }
        }

        return null;
    }

    // Get sprite image for item name.
    public Sprite GetSpriteForItemName(string name)
    {
        if (spriteMapItems.ContainsKey(name))
        {
            return spriteMapItems[name];
        }
        return null;
    }

    // Get sprite image for destination name.
    public Sprite GetSpriteForDestinationName(string name)
    {
        if (spriteMapDestinations.ContainsKey(name))
        {
            return spriteMapDestinations[name];
        }
        return null;
    }

    // Get random destination sprite.
    // public Sprite GetRandomDestinationSprite()
    // {
    //     // No destinations available.
    //     if (spriteMapDestinations.Count == 0) return null;

    //     int spriteIndex = Random.Range(0, spriteMapDestinations.Count);
    //     Sprite destinationSprite = spriteMapDestinations.ElementAt(spriteIndex).Value;
    //     return destinationSprite;
    // }

    // Get sprite image for label name.
    public Sprite GetSpriteForLabelName(string name)
    {
        if (name != null && spriteMapLabels.ContainsKey(name))
        {
            return spriteMapLabels[name];
        }

        return null;
    }

}