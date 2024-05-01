using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SaveManager;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance { get; set; }


    public Button backBTN;



    public Slider masterSlider;
    public GameObject masterValue;


    public Slider musicSlider;
    public GameObject musicValue;

    public Slider effectsSlider;
    public GameObject effectsValue;


    private void Start()
    {
        backBTN.onClick.AddListener(() =>
        {


            SaveManager.instance.SaveVolumeSettings(musicSlider.value, effectsSlider.value, masterSlider.value);

            


        });


        StartCoroutine(LoadAndApplySettings());


    }

    private IEnumerator LoadAndApplySettings()
    {
        LoadAndSetVolume();

        //Load Graphics Settings
        //Load Key Bindinds


        yield return new WaitForSeconds(0.1f);
    }

    private void LoadAndSetVolume()
    {
        VolumeSettings volumeSettings = SaveManager.instance.LoadVolumeSettings();

        masterSlider.value = volumeSettings.master;
        musicSlider.value = volumeSettings.music;
        effectsSlider.value = volumeSettings.effects;

        print("Volume Settings are loaded");


    }



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

        masterValue.GetComponent<TextMeshProUGUI>().text = "" + (masterSlider.value) + "";
        musicValue.GetComponent<TextMeshProUGUI>().text = "" + (musicSlider.value) + "";
        effectsValue.GetComponent<TextMeshProUGUI>().text = "" + (effectsSlider.value) + "";




    }



}
