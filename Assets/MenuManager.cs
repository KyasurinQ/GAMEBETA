using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager instance { get; set; }

    public GameObject menuCanvas;

    public GameObject uiCanvas;

    public bool isMenuOpen;

    public GameObject saveMenu;

    public GameObject settingsMenu;

    public GameObject menu;


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


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M) && !isMenuOpen)
        {
            uiCanvas.SetActive(false);
            menuCanvas.SetActive(true);

            isMenuOpen = true;


            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;







        }
        else if (Input.GetKeyDown(KeyCode.M) && isMenuOpen)
        {

            saveMenu.SetActive(false);
            settingsMenu.SetActive(false);
            menu.SetActive(true);






            uiCanvas.SetActive(true);
            menuCanvas.SetActive(false);

            isMenuOpen = false;


            if(CraftingSystem.instance.isOpen == false && !InventorySystem.instance.isOpen == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }


            SelectionManager.instance.EnableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;





        }
    }



}
