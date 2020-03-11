using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class CarePackageDelivery
{
    public string boardName;
    public string boxName;
    public string locationName;
}

public class CarePackageDepot : MonoBehaviour
{
    public const int MaxBoxes = 12;

    [SerializeField] Transform moverContainer = default;
    [SerializeField] GameObject boxPrefab = default;
    [SerializeField] GameObject sideMoverPrefab = default;
    [SerializeField] GameObject topMoverPrefab = default;
    [SerializeField] SpriteRenderer locationSpriteRenderer = default;
    [SerializeField] BoxQueue boxQueue = default;

    [Header("BillBoard")]
    [SerializeField] TMP_Text deliveryCountText;

    [Header("Enivronment")]
    public DepotDoor depotDoor = default;
    public DepotLift depotLift = default;
    public DepotGrabber depotGrabber = default;

    private CarePackage carePackage;

    private Queue<CarePackageDelivery> deliveryQueue = new Queue<CarePackageDelivery>();
    float lastDelivery = 0;

    private int boxCount = 0;
    private int deliveryCount = 0;

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
            if (deliveryQueue.Count > 0 && boxCount < MaxBoxes)
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
        List<string> itemNames = CarePackage.Instance.GetItemsInBox(carePackageDelivery.boxName);
        box.itemNames = new List<string>(itemNames);

        // Choose side or top mover.
        GameObject movderPrefab = (carePackageDelivery.boardName == "DepotBoxA") ? sideMoverPrefab : topMoverPrefab;

        // Create the box mover.
        GameObject moverInstance = Instantiate(movderPrefab, moverContainer);
        moverInstance.name = "Mover_" + moverIndex;

        // Initialize the box mover.
        Mover mover = moverInstance.GetComponent<Mover>();
        mover.index = moverIndex;
        mover.SetPackage(boxInstance);

        // Trigger grab if needed.

        if (carePackageDelivery.boardName == "DepotBoxB")
        {
            CarePackageDepot.Instance.GrabberGrab();
        }

        boxCount++;
        moverIndex++;
    }

    // Animation triggers.

    public void GrabberGrab()
    {
        depotGrabber.Animator.SetTrigger("Grab");
    }

    public void LiftUp()
    {
        depotLift.Animator.SetTrigger("Up");
    }

    public void OpenDoor()
    {
        depotDoor.Animator.SetBool("Open", true);
    }

    public void CloseDoor()
    {
        depotDoor.Animator.SetBool("Open", false);
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

        deliveryCount++;
        deliveryCountText.text = deliveryCount.ToString();

        boxCount--;
    }

}
