using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


// TODO(SJG) Store sprites instead of names for destintion and items?
public class CarePackageDelivery
{
    public float deliveryTime;
    public string boardName;
    public string boxName;

    public string destinationName;
    public List<string> itemNames;
}

public class CarePackageDepot : MonoBehaviour
{
    public const int MaxBoxes = 12;
    public const float MinTime = .5f;

    [SerializeField] Transform moverContainer = default;
    [SerializeField] GameObject boxPrefab = default;
    [SerializeField] GameObject sideMoverPrefab = default;
    [SerializeField] GameObject topMoverPrefab = default;
    [SerializeField] SpriteRenderer destinationSpriteRenderer = default;
    [SerializeField] BoxQueue boxQueue = default;

    [Header("Enivronment")]
    public DepotDoor depotDoor = default;
    public DepotLift depotLift = default;
    public DepotGrabber depotGrabber = default;

    // Points to CarePackage.Instance.
    private CarePackage carePackage;

    private List<CarePackageDelivery> deliveryHistory = new List<CarePackageDelivery>();
    float lastDelivery = 0;

    private int boxCount = 0;

    private int moverIndex = 0;

    // Object to be deleted when the door closes.
    GameObject doorCaller;

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
        // Check for new deliveries.
        if (Time.time - lastDelivery > MinTime)
        {
            if (deliveryHistory.Count > 0 && boxCount < MaxBoxes)
            {

                foreach (CarePackageDelivery carePackageDelivery in deliveryHistory)
                {
                    if (carePackageDelivery.deliveryTime > lastDelivery)
                    {
                        CreateBox(carePackageDelivery);

                        lastDelivery = Time.time;
                        break;
                    }
                }
            }
        }

        // TODO: Periodically clear new deliveries.

    }

    private void OnCarePackageData(CarePackageData carePackageData)
    {
        // Box found.
        if (carePackageData.boardName == "DepotBoxA" || carePackageData.boardName == "DepotBoxB")
        {
            if (carePackageData.type == "tag.found" && carePackageData.boxName != null)
            {

                // Look for duplicate deliveries.
                foreach (CarePackageDelivery delivery in deliveryHistory)
                {
                    if (delivery.boxName == carePackageData.boxName && delivery.deliveryTime > Time.time - 2)
                    {
                        Debug.Log("DUPLICATE DELIVERY");
                        return;
                    }
                }

                // Prepare new delivery.

                List<string> itemNames = CarePackage.Instance.GetItemsInBox(carePackageData.boxName);
                string destinationName = CarePackage.Instance.GetDestinationForBox(carePackageData.boxName);


                // Set items and destination of not provided.

                if (itemNames == null)
                {
                    itemNames = CarePackage.Instance.GetRandomItemNameList(8);
                }

                if (destinationName == null)
                {
                    destinationName = CarePackage.Instance.GetRandomDestinationName();
                }

                // Clear the item and destination maps for this box.
                CarePackage.Instance.ResetBox(carePackageData.boxName);

                CarePackageDelivery carePackageDelivery = new CarePackageDelivery
                {
                    deliveryTime = Time.time,
                    boardName = carePackageData.boardName,
                    boxName = carePackageData.boxName,
                    itemNames = itemNames,
                    destinationName = destinationName
                };

                if (carePackageDelivery.itemNames != null && carePackageDelivery.itemNames.Count > 0)
                {
                    string itemString = System.String.Join(", ", carePackageDelivery.itemNames.ToArray());
                    Debug.LogFormat("OnCarePackageData: Box: {0} Destination: {1} Items: {2}", carePackageDelivery.boxName, carePackageDelivery.destinationName, itemString);
                }

                deliveryHistory.Add(carePackageDelivery);
            }
        }

        // Set destination for box.
        if (carePackageData.type == "loader.address" && carePackageData.destinationName != null && carePackageData.boxName != null)
        {
            carePackage.SetBoxDestination(carePackageData.boxName, carePackageData.destinationName);
        }

        // Item inserted into box.
        if (carePackageData.type == "loader.insert" && carePackageData.itemName != null && carePackageData.boxName != null)
        {
            carePackage.StoreItemInBox(carePackageData.boxName, carePackageData.itemName);
        }
    }

    public void CreateBox(CarePackageDelivery carePackageDelivery)
    {
        // Create the box.
        GameObject boxInstance = Instantiate(boxPrefab);
        boxInstance.name = carePackageDelivery.boxName;

        // Set box data.
        Box box = boxInstance.GetComponent<Box>();
        box.carePackageDelivery = carePackageDelivery;

        // Choose side or top mover.
        GameObject movderPrefab = (carePackageDelivery.boardName == "DepotBoxA") ? sideMoverPrefab : topMoverPrefab;

        // Create the box mover.
        GameObject moverInstance = Instantiate(movderPrefab, moverContainer);
        moverInstance.name = "Mover_" + moverIndex;

        // Initialize the box mover.
        Mover mover = moverInstance.GetComponent<Mover>();
        mover.index = moverIndex;
        mover.SetPackage(box);

        // Trigger grab if needed.
        if (carePackageDelivery.boardName == "DepotBoxB")
        {
            CarePackageDepot.Instance.GrabberGrab();
        }

        // Logging
        if (carePackageDelivery.itemNames != null && carePackageDelivery.itemNames.Count > 0)
        {
            string itemString = System.String.Join(", ", carePackageDelivery.itemNames.ToArray());
            Debug.LogFormat("CreateBox: Box: {0} Destination: {1} Items: {2}", carePackageDelivery.boxName, carePackageDelivery.destinationName, itemString);
        }

        // Increment counters.
        boxCount++;
        moverIndex++;
    }

    public void SetDestinationSprite(string destinationName)
    {
        Debug.Log("SetDestinationSprite: " + destinationName);

        if (destinationName != null)
        {
            destinationSpriteRenderer.sprite = CarePackage.Instance.GetSpriteForDestinationName(destinationName);
        }
        else
        {
            destinationSpriteRenderer.sprite = CarePackage.Instance.GetRandomDestinationSprite();
        }
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

    public void OpenDoor(GameObject doorCaller)
    {
        this.doorCaller = doorCaller;
        depotDoor.Animator.SetBool("Open", true);
    }

    public void CloseDoor()
    {
        depotDoor.Animator.SetBool("Open", false);
    }

    // Animation events.
    public void DoorOpening()
    {

    }

    public void DoorClosed()
    {
        Debug.Log("DoorClosed");

        // Delete the mover that asked for door open.
        Destroy(doorCaller);

        // Clear lift queue for next box.
        boxQueue.BoxClear();

        boxCount--;
    }

}
