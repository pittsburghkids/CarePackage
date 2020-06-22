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
    public string unicode;
    public int[] ids;
}

// Data structure for locations in config.json.
[System.Serializable]
public class CarePackageLocationConfig
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
    public CarePackageLocationConfig[] locations;
}

public class CarePackageData
{
    public string type;

    public string boardName;
    public string boardSerialNumber;

    public string rfidValue;

    public string itemName;
    public string itemUnicode;

    public string boxName;
}

public class CarePackage : MonoBehaviour
{
    public bool loaderALoaded = false;
    public bool loaderBLoaded = false;

    // Local websocket connection.
    public WebSocketBridge webSocketBridge;

    // Data recieved events.
    public delegate void DataAction(CarePackageData carePackageData);
    public event DataAction OnData;

    // Config object loaded from config.json.
    public CarePackageConfig carePackageConfig;

    // Storage map for items placed in boxes.
    private Dictionary<string, List<string>> storageMap = new Dictionary<string, List<string>>();

    // Sprite map for items and locations.
    private Dictionary<string, Sprite> spriteMapItems = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> spriteMapLocations = new Dictionary<string, Sprite>();

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
                DontDestroyOnLoad(instanceGameObject);
            }

            return instance;
        }
    }

    void Awake()
    {
        StartCoroutine(GetConfig());

        // Connect to websocket.
        webSocketBridge = new WebSocketBridge("ws://localhost:8080");
        webSocketBridge.OnReceived += OnWebSocketReceived;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            OnWebSocketReceived("{\"type\":\"tag.found\",\"boardName\":\"LoaderItemA\",\"boardSerialNumber\":\"\",\"rfidValue\":\"\",\"itemName\":\"Basketball\",\"itemUnicode\":\"\",\"boxName\":\"BoxA\"}");
        }


        if (Input.GetKeyDown(KeyCode.T))
        {
            OnWebSocketReceived("{\"type\":\"loader.insert\",\"boardName\":\"\",\"boardSerialNumber\":\"\",\"rfidValue\":\"\",\"itemName\":\"Basketball\",\"itemUnicode\":\"\",\"boxName\":\"BoxA\"}");
            OnWebSocketReceived("{\"type\":\"tag.found\",\"rfidValue\":437973764,\"boardSerialNumber\":\"5583834373335190D031\",\"boardName\":\"DepotBoxB\",\"boxName\":\"BoxA\"}");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            OnWebSocketReceived("{\"type\":\"loader.insert\",\"boardName\":\"\",\"boardSerialNumber\":\"\",\"rfidValue\":\"\",\"itemName\":\"Basketball\",\"itemUnicode\":\"\",\"boxName\":\"BoxA\"}");
            OnWebSocketReceived("{\"type\":\"tag.found\",\"rfidValue\":437973764,\"boardSerialNumber\":\"5583834373335190D031\",\"boardName\":\"DepotBoxA\",\"boxName\":\"BoxA\"}");
        }
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

            if (webRequest.isNetworkError)
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
        carePackageConfig = JsonUtility.FromJson<CarePackageConfig>(configString);

        StartCoroutine(CreateSprites(carePackageConfig));
    }

    IEnumerator CreateSprites(CarePackageConfig carePackageConfig)
    {
        foreach (CarePackageItemConfig carePackageItem in carePackageConfig.items)
        {
            string url = string.Format("http://localhost:8080/images/items/{0}.png", carePackageItem.unicode);

            Debug.Log("Downloading image for " + carePackageItem.name);

            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    Debug.Log(unityWebRequest.error);
                }
                else
                {
                    Texture2D itemTexture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    Debug.Log("Creating sprite for " + carePackageItem.name);

                    // Create and store sprite.
                    Sprite itemSprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), new Vector2(.5f, .5f), 8000);
                    spriteMapItems.Add(carePackageItem.name, itemSprite);
                }
            }
        }


        foreach (CarePackageLocationConfig carePackageLocation in carePackageConfig.locations)
        {
            string url = string.Format("http://localhost:8080/images/locations/{0}.png", carePackageLocation.name);

            Debug.Log("Downloading image for " + carePackageLocation.name);

            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    Debug.Log(unityWebRequest.error);
                }
                else
                {
                    Texture2D itemTexture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    Debug.Log("Creating sprite for " + carePackageLocation.name);

                    // Create and store sprite.
                    Sprite itemSprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), new Vector2(.5f, .5f), 100);
                    spriteMapLocations.Add(carePackageLocation.name, itemSprite);
                }
            }
        }

    }

    public void WebSocketSend(CarePackageData carePackageData)
    {
        string carePackageJSON = JsonUtility.ToJson(carePackageData);
        webSocketBridge.Send(carePackageJSON);
    }

    private void OnWebSocketReceived(string message)
    {
        Debug.Log(message);

        try
        {
            CarePackageData carePackageData = JsonUtility.FromJson<CarePackageData>(message);
            if (OnData != null) OnData(carePackageData);
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Couldn't parse JSON message.");
            return;
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

    // Get sprite image for location name.
    public Sprite GetSpriteForLocationName(string name)
    {
        if (spriteMapLocations.ContainsKey(name))
        {
            return spriteMapLocations[name];
        }
        return null;
    }

    // Get random location sprite.
    public Sprite GetRandomLocationSprite()
    {
        // No locations available.
        if (spriteMapLocations.Count == 0) return null;

        int spriteIndex = Random.Range(0, spriteMapLocations.Count);
        Sprite locationSprite = spriteMapLocations.ElementAt(spriteIndex).Value;
        return locationSprite;
    }

}