using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CarePackageDelivery
{
    public string boardName;
    public string boxName;
    public string locationName;
}

public class CarePackageDepot : MonoBehaviour
{

    [SerializeField] Transform moverContainer = default;
    [SerializeField] GameObject boxPrefab = default;
    [SerializeField] GameObject moverPrefab = default;
    [SerializeField] Animator depotAnimator = default;
    [SerializeField] SpriteRenderer locationSpriteRenderer = default;
    [SerializeField] BoxQueue boxQueue = default;

    private CarePackage carePackage;

    private Queue<CarePackageDelivery> deliveryQueue = new Queue<CarePackageDelivery>();
    float lastDelivery = 0;

    private int moverIndex = 0;

    // Singleton creation.
    private static CarePackageDepot instance;
    public static CarePackageDepot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CarePackageDepot>();
            }

            return instance;
        }
    }

    void Start()
    {
        carePackage = CarePackage.Instance;
        carePackage.OnData += OnCarePackageData;
    }

    void Update()
    {
        if (Time.time - lastDelivery > 1)
        {
            if (deliveryQueue.Count > 0)
            {
                CarePackageDelivery carePackageDelivery = deliveryQueue.Dequeue();

                if (carePackageDelivery.boxName != null)
                {
                    CreateBox(carePackageDelivery);
                }

                lastDelivery = Time.time;
            }
        }
    }

    private void OnCarePackageData(CarePackageData carePackageData)
    {
        // Box found.
        if (carePackageData.boardName == "DepotBoxA" || carePackageData.boardName == "DepotBoxB")
        {
            if (carePackageData.type == "tag.found" && carePackageData.boxName != null)
            {

                CarePackageDelivery carePackageDelivery = new CarePackageDelivery
                {
                    boardName = carePackageData.boardName,
                    boxName = carePackageData.boxName
                };

                deliveryQueue.Enqueue(carePackageDelivery);
            }
        }

        // Item inserted into box.
        if (carePackageData.type == "loader.insert" && carePackageData.itemName != null && carePackageData.boxName != null)
        {
            carePackage.Store(carePackageData.boxName, carePackageData.itemName);
        }
    }

    public void CreateBox(CarePackageDelivery carePackageDelivery)
    {
        // Create the box.
        GameObject boxInstance = Instantiate(boxPrefab);
        boxInstance.name = carePackageDelivery.boxName;

        // Populate the box with items.
        Box box = boxInstance.GetComponent<Box>();
        box.itemNames = CarePackage.Instance.GetItemsInBox(carePackageDelivery.boxName);

        // Create the box mover.
        GameObject moverInstance = Instantiate(moverPrefab, moverContainer);
        moverInstance.name = "Mover_" + moverIndex;

        // Initialize the box mover.
        Mover mover = moverInstance.GetComponent<Mover>();
        mover.index = moverIndex;
        mover.SetPackage(boxInstance);

        moverIndex++;
    }

    // Animation triggers.

    public void OpenDoor()
    {
        depotAnimator.SetBool("Open", true);
    }

    public void CloseDoor()
    {
        depotAnimator.SetBool("Open", false);
    }

    // Animation events.

    public void DoorOpening()
    {
        Debug.Log("DoorOpening");

        // For now get a random location sprite until we have a real plan for setting location.
        locationSpriteRenderer.sprite = carePackage.GetRandomLocationSprite();
    }

    public void DoorClosed()
    {
        Debug.Log("DoorClosed");
        boxQueue.BoxClear();
    }

}
