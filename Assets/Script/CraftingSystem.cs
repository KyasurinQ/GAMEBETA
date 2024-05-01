using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI, survivalSreenUI, refineSrceenUI, constructionSreenUI;

    public List<string> inventoryItenList = new List<string>();

    // category buttons
    Button toolsBTN, survivalBTN, refineBTN, constructionBTN;

    //craft buttons
    Button craftAxeBTN, craftPlankBTN, craftFoundationBTN, craftWallBTN;

    //Requirement text

    TextMeshProUGUI AxeReq1, AxeReq2, PlankReq1, FoundationReq1, WallReq1;

    public bool isOpen;

    public static Blueprint AxeBLP = new Blueprint("Axe", 1, 2,"Stone", 3,"Stick", 3);
    public static Blueprint PlankBLP = new Blueprint("Plank",2 , 1, "Log", 1, "", 0);
    public static Blueprint FoundationBLP = new Blueprint("Foundation", 2, 1, "Plank", 4, "", 0);
    public static Blueprint WallBLP = new Blueprint("Wall", 2, 1, "Plank", 2, "", 0);


    // all blueprint



    public static CraftingSystem instance { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this )
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

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

       
        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(delegate { OpenSurvivalCategory(); });

       
        refineBTN = craftingScreenUI.transform.Find("RefineButton").GetComponent<Button>();
        refineBTN.onClick.AddListener(delegate { OpenRefineCategory(); });


        constructionBTN = craftingScreenUI.transform.Find("ConstructionButton").GetComponent<Button>();
        constructionBTN.onClick.AddListener(delegate { OpenConstructionCategory(); });

        




        //Axe
        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<TextMeshProUGUI>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });

        //Plank
        PlankReq1 = refineSrceenUI.transform.Find("Plank").transform.Find("req1").GetComponent<TextMeshProUGUI>();
        

        craftPlankBTN = refineSrceenUI.transform.Find("Plank").transform.Find("Button").GetComponent<Button>();
        craftPlankBTN.onClick.AddListener(delegate { CraftAnyItem(PlankBLP); });

        //Foundation
        FoundationReq1 = constructionSreenUI.transform.Find("Foundation").transform.Find("req1").GetComponent<TextMeshProUGUI>();

        craftFoundationBTN = constructionSreenUI.transform.Find("Foundation").transform.Find("Button").GetComponent<Button>();
        craftFoundationBTN.onClick.AddListener(delegate { CraftAnyItem(FoundationBLP); });

        //Wall
        WallReq1 = constructionSreenUI.transform.Find("Wall").transform.Find("req1").GetComponent<TextMeshProUGUI>();

        craftWallBTN = constructionSreenUI.transform.Find("Wall").transform.Find("Button").GetComponent<Button>();
        craftWallBTN.onClick.AddListener(delegate { CraftAnyItem(WallBLP); });



    }

    void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        refineSrceenUI.SetActive(false);
        survivalSreenUI.SetActive(false);
        constructionSreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
        
    }

    void OpenSurvivalCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        refineSrceenUI.SetActive(false);
        constructionSreenUI.SetActive(false);
        survivalSreenUI.SetActive(true);
       
    }

    void OpenRefineCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        survivalSreenUI.SetActive(false);
        constructionSreenUI.SetActive(false);
        refineSrceenUI.SetActive(true);
        
    }

    void OpenConstructionCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        survivalSreenUI.SetActive(false);
        refineSrceenUI.SetActive(false);
        constructionSreenUI.SetActive(true);

    }







void CraftAnyItem(Blueprint blueprintToCraft) //1
    {


        SoundManager.instance.PlaySound(SoundManager.instance.craftingSound);
        //produce the amount of items according to the blueprint
       StartCoroutine(craftedDelayForSound(blueprintToCraft) );
       
     
        if (blueprintToCraft.numOfRequirements == 1)
        {
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        }
        else if (blueprintToCraft.numOfRequirements == 2)
        {
            
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
            InventorySystem.instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
        }



        StartCoroutine(calculate());

        
        //add item into inventory


        //remove resources from inventory



    }

    public IEnumerator calculate()
    {
        yield return 0;

        InventorySystem.instance.ReCalculeList();
        RefreshNeededItems();
    }

    IEnumerator craftedDelayForSound(Blueprint blueprintToCraft)
    {
        yield return new WaitForSeconds(1f);


        for (var i = 0; i < blueprintToCraft.numberOfItemToProduce; i++)
        {
            InventorySystem.instance.AddToInventory(blueprintToCraft.itemName);

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isOpen && !ConstructionManager.instance.inConstructionMode)
        {
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.instance.DisableSelection();
            SelectionManager.instance.GetComponent<SelectionManager>().enabled = false;
            isOpen = true;
        }

        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            survivalSreenUI.SetActive(false);
            refineSrceenUI.SetActive(false);
            constructionSreenUI.SetActive(false);

            // Disable InteractionInfo when closing the crafting screen
            SelectionManager.instance.DisableInteractionInfo();

            if (!InventorySystem.instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.instance.EnableSelection();
                SelectionManager.instance.GetComponent<SelectionManager>().enabled = true;
            }

            isOpen = false;
        }
    }


    public void RefreshNeededItems()
    {

        int stone_count = 0;
        int stick_count = 0;
        int log_count = 0;
        int plank_count = 0;

        inventoryItenList = InventorySystem.instance.itemList;

        foreach (string itemName in inventoryItenList)
        {

            switch (itemName)
            {
                case "Stone":
                    stone_count += 1;

                    break;
                case "Stick":
                    stick_count += 1;

                    break;
                case "Log":
                    log_count += 1;

                    break;
                case "Plank":
                    plank_count += 1;
                    break;
            }

        }

        //Axe____

        AxeReq1.text = "3 Stone[" + stone_count + "]";
        AxeReq2.text = "3 Stick[" + stick_count + "]";

        if (stone_count >= 3 && stick_count >=3 && InventorySystem.instance.CheckSlotsAvailble(1))
        {
            craftAxeBTN.gameObject.SetActive(true);

        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false);
        }

        //Plank x2


        PlankReq1.text = "1 Log[" + log_count + "]";
       

        if (log_count >= 1 && InventorySystem.instance.CheckSlotsAvailble(2))
        {
            craftPlankBTN.gameObject.SetActive(true);

        }
        else
        {
            craftPlankBTN.gameObject.SetActive(false);
        }

        FoundationReq1.text = "4 Plank[" + plank_count + "]"; //Thêm vào cuối phương thức RefreshNeededItems

        if (plank_count >= 4 && InventorySystem.instance.CheckSlotsAvailble(1))
        {
            craftFoundationBTN.gameObject.SetActive(true);
        }
        else
        {
            craftFoundationBTN.gameObject.SetActive(false);
        }

        //Wall
        WallReq1.text = "2 Plank[" + plank_count + "]"; //Thêm vào cuối phương thức RefreshNeededItems

        if (plank_count >= 2 && InventorySystem.instance.CheckSlotsAvailble(1))
        {
            craftWallBTN.gameObject.SetActive(true);
        }
        else
        {
            craftWallBTN.gameObject.SetActive(false);
        }



    }








}
