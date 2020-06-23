﻿using System.Collections;
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

    public string rfidValue;
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
    public delegate void DataAction(CarePackageData carePackageData);
    public event DataAction OnData;

    // Config object loaded from config.json.
    public CarePackageConfig carePackageConfig;

    // Storage map for items placed in boxes and destinations.
    private Dictionary<string, List<string>> storageMap = new Dictionary<string, List<string>>();
    private Dictionary<string, string> destinationMap = new Dictionary<string, string>();

    // Sprite map for items and destinations.
    private Dictionary<string, Sprite> spriteMapItems = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> spriteMapDestinations = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> spriteMapLabels = new Dictionary<string, Sprite>();


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
        // Keyboard shortcuts for debugging.

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            OnWebSocketReceived("{\"type\":\"loader.address\",\"destinationName\":\"House\",\"boxName\":\"BoxA\"}");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            OnWebSocketReceived("{\"type\":\"tag.found\",\"boardName\":\"LoaderItemA\",\"boardSerialNumber\":\"\",\"rfidValue\":\"\",\"itemName\":\"Basketball\",\"boxName\":\"BoxA\"}");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            OnWebSocketReceived("{\"type\":\"loader.insert\",\"boardName\":\"\",\"boardSerialNumber\":\"\",\"rfidValue\":\"\",\"itemName\":\"Basketball\",\"boxName\":\"BoxA\"}");
            OnWebSocketReceived("{\"type\":\"tag.found\",\"rfidValue\":437973764,\"boardSerialNumber\":\"5583834373335190D031\",\"boardName\":\"DepotBoxB\",\"boxName\":\"BoxA\"}");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            OnWebSocketReceived("{\"type\":\"loader.insert\",\"boardName\":\"\",\"boardSerialNumber\":\"\",\"rfidValue\":\"\",\"itemName\":\"Basketball\",\"boxName\":\"BoxA\"}");
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
            string url = string.Format("http://localhost:8080/images/items/{0}.png", carePackageItem.name);

            Debug.Log("Downloading item image for " + carePackageItem.name);

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
                    Debug.Log("Creating item sprite for " + carePackageItem.name);

                    // Create and store sprite.
                    Sprite itemSprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), new Vector2(.5f, .5f), 8000);
                    spriteMapItems.Add(carePackageItem.name, itemSprite);
                }
            }
        }


        foreach (CarePackageDestinationConfig carePackageDestination in carePackageConfig.destinations)
        {
            string url = string.Format("http://localhost:8080/images/destinations/{0}.png", carePackageDestination.name);

            Debug.Log("Downloading destination image for " + carePackageDestination.name);

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
                    Debug.Log("Creating destination sprite for " + carePackageDestination.name);

                    // Create and store sprite.
                    Sprite destinationSprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), new Vector2(.5f, .5f), 100);
                    spriteMapDestinations.Add(carePackageDestination.name, destinationSprite);
                }
            }
        }

        foreach (CarePackageDestinationConfig carePackageDestination in carePackageConfig.destinations)
        {
            string url = string.Format("http://localhost:8080/images/labels/{0}.png", carePackageDestination.name);

            Debug.Log("Downloading label image for " + carePackageDestination.name);

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
                    Debug.Log("Creating label sprite for " + carePackageDestination.name);

                    // Create and store sprite.
                    Sprite labelSprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), new Vector2(.5f, .5f), 100);
                    spriteMapLabels.Add(carePackageDestination.name, labelSprite);
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

        if (!storageMap.ContainsKey(boxName))
        {
            storageMap.Add(boxName, new List<string>());
        }

        storageMap[boxName].Add(item);
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
        storageMap[boxName].Clear();
        destinationMap[boxName] = null;
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
    public Sprite GetRandomDestinationSprite()
    {
        // No destinations available.
        if (spriteMapDestinations.Count == 0) return null;

        int spriteIndex = Random.Range(0, spriteMapDestinations.Count);
        Sprite destinationSprite = spriteMapDestinations.ElementAt(spriteIndex).Value;
        return destinationSprite;
    }

    // Get sprite image for label name.
    public Sprite GetSpriteForLabelName(string name)
    {
        if (spriteMapLabels.ContainsKey(name))
        {
            return spriteMapLabels[name];
        }
        return null;
    }

}