﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackageLoader : MonoBehaviour
{
    [SerializeField] GameObject insertBox = default;
    [SerializeField] GameObject insertItem = default;

    [SerializeField] Transform rootTransform = default;
    [SerializeField] GameObject itemPrefab = default;
    [SerializeField] Animator doorAnimator = default;

    private CarePackage carePackage;

    string currentBox = null;

    void Start()
    {
        carePackage = CarePackage.Instance;
        carePackage.OnData += OnCarePackageData;
    }

    private void OnCarePackageData(CarePackageData carePackageData)
    {
        Debug.Log(carePackageData.itemName);

        if (carePackageData.boardName == "LoaderItemA" || carePackageData.boardName == "LoaderItemB")
        {
            // Item found.
            if (carePackageData.type == "tag.found" && carePackageData.itemName != null)
            {
                Debug.Log(carePackageData.itemName);
                InsertItem(carePackageData.itemName);
            }
        }

        if (carePackageData.boardName == "LoaderBoxA" || carePackageData.boardName == "LoaderBoxB")
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