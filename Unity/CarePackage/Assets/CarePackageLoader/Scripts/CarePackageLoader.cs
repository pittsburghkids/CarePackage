﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarePackageLoader : MonoBehaviour
{
    private const float destinationDisplayTime = 1.5f;
    private const float maxItemsPerBox = 12;

    private enum InstructionState
    {
        InsertBox,
        ChooseAddress,
        InsertItem,
        BoxFull
    }

    [SerializeField] Camera loaderCamera = default;
    [SerializeField] Transform rootTransform = default;

    [SerializeField] Transform itemContainer;

    [SerializeField] GameObject itemPrefab = default;
    [SerializeField] GameObject labelPrefab = default;
    [SerializeField] Animator loaderAnimator = default;

    [Header("Instructions")]
    [SerializeField] GameObject insertBox = default;
    [SerializeField] GameObject insertItem = default;
    [SerializeField] GameObject chooseAddressA = default;
    [SerializeField] GameObject chooseAddressB = default;
    [SerializeField] GameObject deliverPackage = default;

    private CarePackage carePackage;
    GameObject chooseAddress;

    private string loaderItemBoardName = "LoaderItemA";
    private string loaderBoxBoardName = "LoaderBoxA";
    private string loaderAddressBoardName = "LoaderAddressA";

    // Track the address label so we can delete it when a new one comes.
    Coroutine labelRoutine;
    private GameObject labelInstance;

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
            rootTransform.position = Vector3.left;

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
            loaderAddressBoardName = "LoaderAddressA";

            chooseAddress = chooseAddressA;

            carePackage.loaderALoaded = true;
        }
        else if (carePackage.loaderBLoaded == false)
        {
            rootTransform.position = Vector3.right;

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
            loaderAddressBoardName = "LoaderAddressB";

            chooseAddress = chooseAddressB;

            loaderCamera.GetComponent<AudioListener>().enabled = false;

            carePackage.loaderBLoaded = true;
        }
    }

    private void OnCarePackageData(CarePackageData carePackageData)
    {
        if (carePackageData.boardName == loaderAddressBoardName)
        {
            if (carePackageData.type == "encoder.change" && carePackageData.destinationName != null)
            {
                SetDestination(carePackageData.destinationName);
            }
        }

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

    // Called from LoaderTriggerHandler on item collision.
    public void StoreItemFromTrigger(Collider collider)
    {
        if (currentBox != null)
        {
            carePackage.StoreItemInBox(currentBox, collider.gameObject.name);

            // Broadcast data.

            CarePackageData carePackageData = new CarePackageData
            {
                type = "loader.insert",
                itemName = collider.gameObject.name,
                boxName = currentBox
            };

            carePackage.WebSocketSend(carePackageData);

            // Destroy object.

            Destroy(collider.gameObject);
        }
    }

    // New box inserted into slot.
    public void InsertBox(string boxName)
    {
        if (boxName != null)
        {
            if (currentBox != boxName)
            {
                // TODO(SJG) Make sure this is actually a box.

                Debug.Log("InsertBox: " + boxName);
                currentBox = boxName;

                bool boxPresent = true;
                loaderAnimator.SetBool("Opened", boxPresent);

                carePackage.ResetBox(currentBox);
                ShowInstructions(InstructionState.ChooseAddress);
            }
        }
    }

    // Box removed from slot.
    public void RemoveBox(string boxName)
    {
        Debug.Log("Box removed");
        currentBox = null;

        ShowInstructions(InstructionState.InsertBox);

        // Clear destination.
        if (labelRoutine != null)
        {
            StopCoroutine(labelRoutine);
            labelRoutine = null;

            if (labelInstance != null)
            {
                Destroy(labelInstance);
            }
        }

        //

        bool boxPresent = false;
        loaderAnimator.SetBool("Opened", boxPresent);
    }

    // New item inserted into slot.
    public void InsertItem(string itemName)
    {

        if (currentBox != null)
        {
            // Handle instructions.
            ShowInstructions(InstructionState.InsertItem);
        }

        if (itemName != null)
        {
            GameObject itemInstance = Instantiate(itemPrefab, itemContainer);

            Sprite itemSprite = carePackage.GetSpriteForItemName(itemName);
            SpriteRenderer spriteRenderer = itemInstance.GetComponent<SpriteRenderer>();

            if (itemSprite != null)
            {
                spriteRenderer.sprite = itemSprite;
            }

            itemInstance.name = itemName;
        }

    }

    public void SetDestination(string destinationName)
    {
        if (currentBox != null && destinationName != null)
        {
            // Handle instructions.

            ShowInstructions(InstructionState.ChooseAddress);

            // Clear destination.
            if (labelRoutine != null)
            {
                StopCoroutine(labelRoutine);
                labelRoutine = null;

                if (labelInstance != null)
                {
                    Destroy(labelInstance);
                }
            }

            // Show image.
            labelRoutine = StartCoroutine(ShowDestinationRoutine(destinationName));

            // Send data.

            CarePackageData carePackageData = new CarePackageData
            {
                type = "loader.address",
                destinationName = destinationName,
                boxName = currentBox
            };

            carePackage.WebSocketSend(carePackageData);

        }
    }

    IEnumerator ShowDestinationRoutine(string destinationName)
    {

        Debug.Log("ShowDestinationRoutine: " + destinationName);

        labelInstance = Instantiate(labelPrefab, rootTransform);

        Sprite labelSprite = carePackage.GetSpriteForLabelName(destinationName);
        SpriteRenderer spriteRenderer = labelInstance.GetComponent<SpriteRenderer>();

        if (labelSprite != null)
        {
            spriteRenderer.sprite = labelSprite;
        }

        yield return new WaitForSeconds(destinationDisplayTime);

        if (labelInstance != null)
        {
            Destroy(labelInstance);
        }


        ShowInstructions(InstructionState.InsertItem);
    }

    private void ShowInstructions(InstructionState state)
    {
        insertBox.SetActive(false);
        chooseAddress.SetActive(false);
        insertItem.SetActive(false);
        deliverPackage.SetActive(false);

        if (currentBox != null)
        {
            List<string> boxItems = carePackage.GetItemsInBox(currentBox);
            if (boxItems != null && boxItems.Count > maxItemsPerBox)
            {
                deliverPackage.SetActive(true);
                return;
            }
        }

        switch (state)
        {
            case InstructionState.InsertBox:
                {
                    insertBox.SetActive(true);
                    break;
                }
            case InstructionState.ChooseAddress:
                {
                    chooseAddress.SetActive(true);
                    break;
                }
            case InstructionState.InsertItem:
                {
                    insertItem.SetActive(true);
                    break;
                }
        }


    }
}