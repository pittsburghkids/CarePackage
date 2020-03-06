using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CarePackageLoader : MonoBehaviour
{
    [SerializeField] Camera loaderCamera = default;
    [SerializeField] GameObject loaderSceneRoot = default;

    [SerializeField] GameObject insertBox = default;
    [SerializeField] GameObject insertItem = default;

    [SerializeField] Transform rootTransform = default;
    [SerializeField] GameObject itemPrefab = default;
    [SerializeField] Animator doorAnimator = default;

    private CarePackage carePackage;

    private string loaderItemBoardName;
    private string loaderBoxBoardName;

    string currentBox = null;

    void Start()
    {
        carePackage = CarePackage.Instance;
        carePackage.OnData += OnCarePackageData;

        bool multiDisplay = Display.displays.Length > 1;

        // // Configure external displays.
        if (multiDisplay)
        {
            Display.displays[1].Activate();
        }

        // See if this is the first or second instance of CarePackageLoader.
        if (carePackage.loaderALoaded == false)
        {
            loaderSceneRoot.transform.position = Vector3.left;

            if (!multiDisplay)
            {
                loaderCamera.rect = new Rect(0, 0, .5f, 1);
            }
            else
            {
                loaderCamera.targetDisplay = 0;
            }

            loaderItemBoardName = "LoaderItemA";
            loaderBoxBoardName = "LoaderBoxA";

            carePackage.loaderALoaded = true;
        }
        else if (carePackage.loaderBLoaded == false)
        {
            loaderSceneRoot.transform.position = Vector3.right;

            if (!multiDisplay)
            {
                loaderCamera.rect = new Rect(.5f, 0, .5f, 1);
            }
            else
            {
                loaderCamera.targetDisplay = 1;
            }

            loaderItemBoardName = "LoaderItemB";
            loaderBoxBoardName = "LoaderBoxB";

            loaderCamera.GetComponent<AudioListener>().enabled = false;

            carePackage.loaderBLoaded = true;
        }
    }

    private void OnCarePackageData(CarePackageData carePackageData)
    {
        if (carePackageData.boardName == loaderItemBoardName)
        {
            // Item found.
            if (carePackageData.type == "tag.found" && carePackageData.itemName != null)
            {
                Debug.Log(carePackageData.itemName);
                InsertItem(carePackageData.itemName);
            }
        }

        if (carePackageData.boardName == loaderBoxBoardName)
        {
            // Box found.
            if (carePackageData.type == "tag.found" && carePackageData.boxName != null)
            {
                Debug.Log(carePackageData.boxName);
                InsertBox(carePackageData.boxName);
            }

            // Box removed.
            if (carePackageData.type == "tag.removed" && carePackageData.boxName != null)
            {
                Debug.Log(carePackageData.boxName);
                RemoveBox(carePackageData.boxName);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        carePackage.Store(currentBox, collider.gameObject.name);

        // Broadcast data.

        CarePackageData carePackageData = new CarePackageData
        {
            type = "loader.insert",
            itemName = collider.gameObject.name,
            boxName = currentBox
        };

        carePackage.WebSocketSend(carePackageData);

    }

    // New box inserted into slot.
    public void InsertBox(string boxName)
    {
        if (boxName != null)
        {
            if (currentBox != boxName)
            {
                Debug.Log("InsertBox: " + boxName);
                currentBox = boxName;

                bool boxPresent = true;
                doorAnimator.SetBool("Open", boxPresent);
                insertBox.SetActive(!boxPresent);
                insertItem.SetActive(boxPresent);
            }
        }
    }

    // Box removed from slot.
    public void RemoveBox(string boxName)
    {
        bool boxPresent = false;
        doorAnimator.SetBool("Open", boxPresent);
        insertBox.SetActive(!boxPresent);
        insertItem.SetActive(boxPresent);

        Debug.Log("Box removed");
        currentBox = null;
    }


    // New item inserted into slot.
    public void InsertItem(string itemName)
    {
        insertItem.SetActive(false);

        if (itemName != null)
        {
            GameObject itemInstance = Instantiate(itemPrefab, rootTransform);

            Sprite itemSprite = carePackage.GetSpriteForItemName(itemName);
            SpriteRenderer spriteRenderer = itemInstance.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = itemSprite;

            itemInstance.name = itemName;
        }
    }
}