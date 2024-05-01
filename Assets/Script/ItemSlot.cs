using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if(transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;

            }

            return null;
        }
    }


    public void OnDrop(PointerEventData evenData)
    {
        Debug.Log("OnDrop");

        if (!Item)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.dropItemSound);





            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0 ,0);

            if (transform.CompareTag("QuickSlot") == false)
            {
                DragDrop.itemBeingDragged.GetComponent<InventoryItem>().isInsideQuickSlot = false;
                InventorySystem.instance.ReCalculeList();
            }
            if (transform.CompareTag("QuickSlot"))
            {
                DragDrop.itemBeingDragged.GetComponent<InventoryItem>().isInsideQuickSlot = true; 
                InventorySystem.instance.ReCalculeList();
            }



        }
    }
   
}
