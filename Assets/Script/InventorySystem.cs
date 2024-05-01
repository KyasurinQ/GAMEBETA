using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{

    public static InventorySystem instance { get; set; }

    public GameObject InventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>();

    public List<string> itemList = new List<string>();

    private GameObject itemToAdd;

    private GameObject whatSlotToEquip;


    public bool isOpen;

    //public bool isFull;

    //pickup popup

    public GameObject pickupAlert;
    public TextMeshProUGUI pickupName;
    public Image pickupImage;
    public GameObject ItemInfoUi;

    public List<string> itemsPickedup;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;

        PopulateSlotList();

        Cursor.visible = false;

    }

    private void PopulateSlotList()
    {

        foreach (Transform child in InventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen && !ConstructionManager.instance.inConstructionMode)
        {
            Debug.Log("I is pressed");
            InventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;

            // Disable InteractionInfo when opening inventory
            SelectionManager.instance.DisableInteractionInfo();

            isOpen = true;
        }

        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            InventoryScreenUI.SetActive(false);

            if (!CraftingSystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.instance.EnableSelection();
                SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
            }

            isOpen = false;
        }
    }


    public void AddToInventory(string itemName)
    {

        if(SaveManager.instance.isLoading == false) 
        {
            SoundManager.instance.PlaySound(SoundManager.instance.pickupItemSound);

        }



       whatSlotToEquip = FindNextEmptySlot();

       itemToAdd = (GameObject)Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation );
       itemToAdd.transform.SetParent(whatSlotToEquip.transform);

       itemList.Add(itemName);

     
       TriggerPickupPopup(itemName, itemToAdd.GetComponent<Image>().sprite);

       ReCalculeList();
       CraftingSystem.instance.RefreshNeededItems();


    }


    void TriggerPickupPopup(string itemName, Sprite itemSprite )
    {
        pickupAlert.SetActive(true);

        pickupName.text = itemName;

        pickupImage.sprite = itemSprite;

        StartCoroutine(HidePickupAlertAfterDelay(2f));
    }

    private IEnumerator HidePickupAlertAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);


        pickupAlert.SetActive(false);
    }


    public void PopupDelay()
    {
        pickupAlert.SetActive(false );
    }

    private GameObject FindNextEmptySlot()
    {
        foreach(GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }

        }
        return new GameObject();
    }

    public bool CheckSlotsAvailble(int emptyMeeded)//1
    {
        int emptySlot = 0;

        foreach(GameObject slot in slotList)
        {
            if (slot.transform.childCount <= 0)
            {
                emptySlot += 1;
            }

        
        }

        if(emptySlot >= emptyMeeded)//21
        {
          return true;

        }
        else
        {
          return false;
        }
    }

    public void RemoveItem(string nameToRemove, int amountToRemove)
    {

        int counter = amountToRemove;

        for(var i = slotList.Count - 1; i >= 0; i--)
        {

            if (slotList[i].transform.childCount >0)
            {
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
                {

                    Destroy(slotList[i].transform.GetChild(0).gameObject);

                    counter -= 1;

                }

            }

        }

        ReCalculeList();
        CraftingSystem.instance.RefreshNeededItems();

    }
    public void ReCalculeList()
    {
        itemList.Clear();

        foreach (GameObject slot in slotList)
        {

            if(slot.transform.childCount > 0)
            {

                string name = slot.transform.GetChild(0).name; // stone clone

                string srt1 = name;

                string srt2 = "(Clone)";

                string result = name.Replace(srt2, "");


                itemList.Add(result);
            }
        }
    }
}
