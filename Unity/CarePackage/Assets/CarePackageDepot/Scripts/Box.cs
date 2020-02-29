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
        // CarePackage.Instance.Store(name, "Star");
        // CarePackage.Instance.Store(name, "RedHeart");
        // CarePackage.Instance.Store(name, "RedHeart");
        // CarePackage.Instance.Store(name, "RedHeart");
        // CarePackage.Instance.Store(name, "RedHeart");
        // CarePackage.Instance.Store(name, "RedHeart");

        // Invoke("Emit", 1);
    }

    public void Emit()
    {
        StartCoroutine(EmitRoutine());
    }

    IEnumerator EmitRoutine()
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
                    rigidBody.AddForce((Vector3.up * Random.Range(3, 5f)) + (Vector3.right * Random.Range(-3f, 3f)));
                    rigidBody.AddTorque(Vector3.forward * Random.Range(-180f, 180f));

                    Destroy(itemInstance, 2f);

                    yield return new WaitForSeconds(.2f);
                }
            }

        }
    }

    void OnDestroy()
    {
        CarePackage.Instance.EmptyBox(gameObject.name);
    }
}
