using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class CarePackageDelivery
{
    public float deliveryTime;
    public string boardName;
    public string boxName;
    public string locationName;
}

public class CarePackageDepot : MonoBehaviour
{
    public const int MaxBoxes = 12;
    public const float MinTime = .5f;

    [SerializeField] Transform moverContainer = default;
    [SerializeField] GameObject boxPrefab = default;
    [SerializeField] GameObject sideMoverPrefab = default;
    [SerializeField] GameObject topMoverPrefab = default;
    [SerializeField] SpriteRenderer locationSpriteRenderer = default;
    [SerializeField] BoxQueue boxQueue = default;

    [Header("BillBoard")]
    [SerializeField] TMP_Text deliveryCountText = default;

    [Header("Enivronment")]
    public DepotDoor depotDoor = default;
    public DepotLift depotLift = default;
    public DepotGrabber depotGrabber = default;

    private CarePackage carePackage;

    private List<CarePackageDelivery> deliveryHistory = new List<CarePackageDelivery>();
    float lastDelivery = 0;

    private int boxCount = 0;
    private int deliveryCount = 0;

    private int moverIndex = 0;

    // For resetting the package count daily.
    private int lastDay = -1;

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
        // Reset delivery count if needed.
        if (System.DateTime.Now.Day != lastDay)
        {
            deliveryCountText.text = deliveryCount.ToString();
            lastDay = System.DateTime.Now.Day;
        }

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


                foreach (CarePackageDelivery delivery in deliveryHistory)
                {
                    if (delivery.boxName == carePackageData.boxName && delivery.deliveryTime > Time.time - 2)
                    {
                        Debug.Log("DUPLICATE DELIVERY");
                        return;
                    }
                }

                CarePackageDelivery carePackageDelivery = new CarePackageDelivery
                {
                    deliveryTime = Time.time,
                    boardName = carePackageData.boardName,
                    boxName = carePackageData.boxName
                };

                deliveryHistory.Add(carePackageDelivery);
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
        if (itemNames != null)
        {
            box.itemNames = new List<string>(itemNames);
        }

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
