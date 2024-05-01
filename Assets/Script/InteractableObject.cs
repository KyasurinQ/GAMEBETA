using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    public bool playerInRange;

    public string ItemName;
    public string GetItemName()
    {
        return ItemName;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.selectedObject == gameObject)
        {

            if (InventorySystem.instance.CheckSlotsAvailble(1))
            {

                InventorySystem.instance.AddToInventory(ItemName);


                InventorySystem.instance.itemsPickedup.Add(gameObject.name);







                Destroy(gameObject);

            }
            else
            {
                Debug.Log("inventory is full");
            }

            
        }   
    }






    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {

            playerInRange = true;


        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            playerInRange = false;


        }

    }
}
