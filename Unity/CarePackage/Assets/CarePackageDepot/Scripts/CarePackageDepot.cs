using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CarePackageDepot : MonoBehaviour
{

    [SerializeField] Transform moverContainer = default;
    [SerializeField] GameObject packagePrefab = default;
    [SerializeField] GameObject moverPrefab = default;
    [SerializeField] Animator depotAnimator = default;

    private CarePackage carePackage;
    private SerialReader slotReader;

    private Queue<string> deliveryQueue = new Queue<string>();
    float lastDelivery = 0;

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

        slotReader = new SerialReader(carePackage.carePackageConfig.depotSlot);

        slotReader.OnSerialMessage += Delivery;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Delivery("1794330372");
        }

        //

        if (Time.time - lastDelivery > 1)
        {
            if (deliveryQueue.Count > 0)
            {
                string message = deliveryQueue.Dequeue();
                string boxName = CarePackage.Instance.GetBoxByID(message);

                if (boxName != null)
                {
                    GameObject packageInstance = Instantiate(packagePrefab);
                    packageInstance.name = boxName;

                    GameObject moverInstance = Instantiate(moverPrefab, moverContainer);
                    Mover mover = moverInstance.GetComponent<Mover>();
                    mover.GetPackage(packageInstance);
                }

                lastDelivery = Time.time;
            }
        }

    }

    void OnDestroy()
    {
        slotReader.OnSerialMessage -= Delivery;

        slotReader?.Destroy();
    }

    public void Delivery(string message)
    {
        deliveryQueue.Enqueue(message);
    }

    public void OpenDoor()
    {
        depotAnimator.SetBool("Open", true);
    }

    public void CloseDoor()
    {
        depotAnimator.SetBool("Open", false);
    }

}
