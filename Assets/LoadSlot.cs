using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadSlot : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    public int slotNumber;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (SaveManager.instance.IsSlotEmpty(slotNumber))
        {
            buttonText.text = "";
        }
        else
        {
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }


    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveManager.instance.IsSlotEmpty(slotNumber) == false)
            {

                SaveManager.instance.StartLoadedGame(slotNumber);
                SaveManager.instance.DeselectButton();
            }
            else
            {
                //If empty do nothing
            }
        });
    }
}
