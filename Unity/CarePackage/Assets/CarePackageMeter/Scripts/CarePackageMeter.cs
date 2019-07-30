using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackageMeter : MonoBehaviour
{
    [SerializeField] Transform rootTransform;

    [SerializeField] GameObject heartPrefab;
    [SerializeField] GameObject starPrefab;
    [SerializeField] GameObject shirtPrefab;
    [SerializeField] GameObject cookiePrefab;

    [SerializeField] Animator doorAnimator;

    string currentBox = null;
    float currentBoxTime = 0;

    public bool boxPresent = false;

    private SerialReader slotReader;
    private SerialReader boxReader;

    void Start()
    {
        CarePackage carePackage = CarePackage.Instance;

        slotReader = new SerialReader(carePackage.carePackageConfig.meterSlot);
        boxReader = new SerialReader(carePackage.carePackageConfig.meterBox);

        slotReader.OnSerialMessage += InsertItem;
        boxReader.OnSerialMessage += InsertBox;
    }

    void Update()
    {
        if (currentBox != null && (Time.time - currentBoxTime) > .5f)
        {
            boxPresent = false;
            doorAnimator.SetBool("Open", boxPresent);

            Debug.Log("Box removed");
            currentBox = null;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        CarePackage.Instance.Store(currentBox, collider.gameObject.name);
    }

    void OnDestroy()
    {
        slotReader.OnSerialMessage -= InsertItem;
        boxReader.OnSerialMessage -= InsertBox;

        slotReader?.Destroy();
        boxReader?.Destroy();
    }

    public void InsertBox(string message)
    {
        string boxName = CarePackage.Instance.GetBoxByID(message);
        if (boxName != null)
        {
            if (currentBox != boxName)
            {
                Debug.Log("InsertBox: " + boxName);
                currentBox = boxName;
                currentBoxTime = Time.time;

                boxPresent = true;
                doorAnimator.SetBool("Open", boxPresent);
            }

            if (currentBox == boxName)
            {
                currentBoxTime = Time.time;
            }
        }
    }

    public void InsertItem(string message)
    {
        string itemName = CarePackage.Instance.GetItemByID(message);
        if (itemName != null)
        {

            if (itemName == "heart")
            {
                GameObject itemInstance = Instantiate(heartPrefab, rootTransform);
                itemInstance.name = itemName;
            }

            if (itemName == "star")
            {
                GameObject itemInstance = Instantiate(starPrefab, rootTransform);
                itemInstance.name = itemName;
            }

            if (itemName == "shirt")
            {
                GameObject itemInstance = Instantiate(shirtPrefab, rootTransform);
                itemInstance.name = itemName;
            }

            if (itemName == "cookie")
            {
                GameObject itemInstance = Instantiate(cookiePrefab, rootTransform);
                itemInstance.name = itemName;
            }
        }
    }
}