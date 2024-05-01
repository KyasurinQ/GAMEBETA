using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isTrashable;

    private GameObject itemInfoUI;

    private TextMeshProUGUI itemInfoUI_itemName;
    private TextMeshProUGUI itemInfoUI_itemDescription;
    private TextMeshProUGUI itemInfoUI_itemFunctionality;

    public string thisName, thisDescription, thisFunctionality;

    private GameObject itemPedingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;


    public bool isEquippable;
    private GameObject itemPendingEquipping;
    public bool isInsideQuickSlot;


    public bool isSelected;

    public bool isUseable;
    



    // Start is called before the first frame update
    private void Start()
    {
        itemInfoUI = InventorySystem.instance.ItemInfoUi;
        itemInfoUI_itemName = itemInfoUI.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemDescription = itemInfoUI.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemFunctionality = itemInfoUI.transform.Find("ItemFunctionality").GetComponent<TextMeshProUGUI>();


    }

    void Update()
    {
        if (isSelected)
        {
            gameObject.GetComponent<DragDrop>().enabled = false;    
        }
        else
        {
            gameObject.GetComponent<DragDrop>().enabled = true;
        }
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
        itemInfoUI_itemDescription.text = thisDescription;
        itemInfoUI_itemFunctionality.text = thisFunctionality;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable)
            {
                itemPedingConsumption = gameObject;
                consumingFunction(healthEffect, caloriesEffect, hydrationEffect);
            }

            if (isEquippable && isInsideQuickSlot == false && EquipSystem.instance.CheckIfFull() == false)
            {
                EquipSystem.instance.AddToQuickSlots(gameObject);
                isInsideQuickSlot = true;

            }

            if (isUseable)
            {
                ConstructionManager.instance.itemToBeDestroyed = gameObject;
                gameObject.SetActive(false);

                UseItem();
            }

        }


    }

   

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(isConsumable && itemPedingConsumption == gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.instance.ReCalculeList();
                CraftingSystem.instance.RefreshNeededItems();

            }

           
        }
    }


    private void UseItem()
    {

        itemInfoUI.SetActive(false);

        InventorySystem.instance.isOpen = false;
        InventorySystem.instance.InventoryScreenUI.SetActive(false);

        CraftingSystem.instance.isOpen = false;
        CraftingSystem.instance.craftingScreenUI.SetActive(false);
        CraftingSystem.instance.toolsScreenUI.SetActive(false);
        CraftingSystem.instance.survivalSreenUI.SetActive(false);
        CraftingSystem.instance.refineSrceenUI.SetActive(false);
        CraftingSystem.instance.constructionSreenUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SelectionManager.instance.EnableSelection();
        SelectionManager.instance.enabled = true;

        switch (gameObject.name)
        {
            case "Foundation(Clone)":
                ConstructionManager.instance.ActivateConstructionPlacement("FoundationModel");
                break;
            case "Foundation":
                ConstructionManager.instance.ActivateConstructionPlacement("FoundationModel"); // for testing
                break;case "Wall(Clone)":
                ConstructionManager.instance.ActivateConstructionPlacement("WallModel");
                break;
            case "Wall":
                ConstructionManager.instance.ActivateConstructionPlacement("WallModel"); // for testing
                break;

            default:
                break;
        }



    }



    private void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect)
    {
        itemInfoUI.SetActive(false);
        healthEffectCalculation(healthEffect);
        caloriesEffectCalculation(caloriesEffect);
        hydrationEffectCalculation(hydrationEffect);
    }

    private static void healthEffectCalculation(float healthEffect)
    {
        float healthBeforeConsumption = PlayerState.instance.currentHealth;
        float maxHealth = PlayerState.instance.maxHealth;

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealth)
            {
                PlayerState.instance.setHealth(maxHealth);
            }
            else
            {
                PlayerState.instance.setHealth(healthBeforeConsumption + healthEffect);
            }

        }
    }

    private static void caloriesEffectCalculation(float caloriesEffect)
    {
        float caloriesBeforeConsumption = PlayerState.instance.currentCalories;
        float maxCalories = PlayerState.instance.maxCalories;

        if (caloriesEffect != 0)
        {
            if((caloriesBeforeConsumption + caloriesEffect) > maxCalories)
            {
                PlayerState.instance.setCalories(maxCalories);

            }
            else
            {
                PlayerState.instance.setCalories(caloriesBeforeConsumption + caloriesEffect);
            }
        }
    }

    public static void hydrationEffectCalculation(float hydrationEffect)
    {
        float hydrationBeforeConsumption = PlayerState.instance.currentHydrationPercent;
        float maxHydration = PlayerState.instance.maxHydrationPercent;

        if(hydrationEffect != 0)
        {
            if((hydrationBeforeConsumption + hydrationEffect) > maxHydration)
            {
                PlayerState.instance.setHydration(maxHydration);
            }
            else
            {
                PlayerState.instance.setHydration(hydrationBeforeConsumption +hydrationEffect);
            }
        }
    }

    // Update is called once per frame
    
   
}
