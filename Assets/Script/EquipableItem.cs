using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{
    public Animator animator;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetMouseButtonDown(0) && 
            InventorySystem.instance.isOpen == false && 
            CraftingSystem.instance.isOpen == false &&
            SelectionManager.instance.handIsVisible == false &&
            
            !ConstructionManager.instance.inConstructionMode)    // left mouse button

        {
            

            StartCoroutine(SwingSoundDelay());

            animator.SetTrigger("hit");
            
           



        } 
    }
    public void GetHit()
    {
        GameObject selectedTree = SelectionManager.instance.selectedTree;

        if (selectedTree != null)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.chopSound);
            selectedTree.GetComponent<ChoppableTree>().GetHit();

        }

    }

    IEnumerator SwingSoundDelay()
    {
        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.PlaySound(SoundManager.instance.toolSwingSound);
    }
}
