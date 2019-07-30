using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class CarePackageBoxConfig
{
    public string name;
    public int[] id;
}

[System.Serializable]
public class CarePackageConfig
{
    public string meterSlot;
    public string meterBox;
    public string depotSlotA;
    public string depotSlotB;

    public CarePackageBoxConfig boxes;
}

public class CarePackage : MonoBehaviour
{
    public CarePackageConfig carePackageConfig;

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

}