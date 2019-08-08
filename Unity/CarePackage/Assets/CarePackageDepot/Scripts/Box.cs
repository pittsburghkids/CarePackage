using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab = default;
    [SerializeField] Transform itemSpawnPoint = default;
    [SerializeField] GameObject confetti = default;

    public void Start()
    {
        //CarePackage.Instance.Store(name, "star");
    }

    public void Emit()
    {
        confetti.SetActive(true);

        //

        List<string> itemNames = CarePackage.Instance.GetItemsInBox(gameObject.name);

        if (itemNames != null)
        {
            foreach (string itemName in itemNames)
            {
                if (itemName != null)
                {
                    GameObject itemInstance = Instantiate(itemPrefab);
                    itemInstance.transform.position = itemSpawnPoint.position;
                    itemInstance.name = itemName;

                    Sprite itemSprite = CarePackage.Instance.GetSpriteForItemName(itemName);
                    SpriteRenderer spriteRenderer = itemInstance.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = itemSprite;

                    Rigidbody rigidBody = itemInstance.GetComponent<Rigidbody>();
                    rigidBody.AddForce(Vector3.up + (Vector3.right * Random.Range(-1, 1)));

                    Destroy(itemInstance, 2f);
                }
            }

        }
        CarePackage.Instance.EmptyBox(gameObject.name);
    }
}
