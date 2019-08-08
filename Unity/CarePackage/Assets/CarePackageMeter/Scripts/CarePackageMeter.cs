using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackageMeter : MonoBehaviour
{
    [SerializeField] GameObject instructions = default;

    [SerializeField] Transform rootTransform = default;
    [SerializeField] GameObject itemPrefab = default;
    [SerializeField] Animator doorAnimator = default;

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
            instructions.SetActive(!boxPresent);

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

    // New box inserted into slot.
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
                instructions.SetActive(!boxPresent);

            }

            if (currentBox == boxName)
            {
                currentBoxTime = Time.time;
            }
        }
    }

    // New item inserted into slot.
    public void InsertItem(string message)
    {
        string itemName = CarePackage.Instance.GetItemByID(message);
        if (itemName != null)
        {
            GameObject itemInstance = Instantiate(itemPrefab, rootTransform);

            Sprite itemSprite = CarePackage.Instance.GetSpriteForItemName(itemName);
            SpriteRenderer spriteRenderer = itemInstance.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = itemSprite;

            itemInstance.name = itemName;
        }
    }
}