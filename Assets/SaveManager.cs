using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance { get; set; }


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

        DontDestroyOnLoad(gameObject);



    }


    //Json project save path
    string jsonPathProject;

    //json External/real save path

    string jsonPathPersistant;
    //binary save path


    string binaryPath;


    string fileName = "SaveGame";


    public bool isSavingToJson;

    public bool isLoading;

    public Canvas loadingScreen;




    private void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar;
        jsonPathPersistant = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        binaryPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        

    }




    #region || ------------General Section----------||

    #region || ------------Saving----------||

    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();


        data.enviromentData = GetEnviromentData();




        SavingTypeSwitch(data, slotNumber);
    }

    private EnviromentData GetEnviromentData()
    {
        List<string> itemsPickedup = InventorySystem.instance.itemsPickedup;

        return new EnviromentData(itemsPickedup);
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.instance.currentHealth;
        playerStats[1] = PlayerState.instance.currentCalories;
        playerStats[2] = PlayerState.instance.currentHydrationPercent;



        float[] playerPosAndRot = new float[6];
        playerPosAndRot[0] = PlayerState.instance.playerBody.transform.position.x;
        playerPosAndRot[1] = PlayerState.instance.playerBody.transform.position.y;
        playerPosAndRot[2] = PlayerState.instance.playerBody.transform.position.z;



        playerPosAndRot[3] = PlayerState.instance.playerBody.transform.rotation.x;
        playerPosAndRot[4] = PlayerState.instance.playerBody.transform.rotation.y;
        playerPosAndRot[5] = PlayerState.instance.playerBody.transform.rotation.z;

        string[] inventory = InventorySystem.instance.itemList.ToArray();

        string[] quickSlots = GetQuickSlotsContent();



        return new PlayerData(playerStats, playerPosAndRot, inventory, quickSlots);



    }

    private string[] GetQuickSlotsContent()
    {
        List<string> temp = new List<string>();

        foreach(GameObject slot in EquipSystem.instance.quickSlotsList)
        {
            if(slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string cleanName = name.Replace(str2, "");
                temp.Add(cleanName);
            }
        }
        return temp.ToArray();





    }

    public void SavingTypeSwitch(AllGameData gameData, int slotNumber)
    {
        if(isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData, slotNumber);

        }
        else
        {
            SaveGameDataToBinaryFile(gameData, slotNumber);

        }
    }


    #endregion




    #region || ------------Loading----------||

    public AllGameData LoadingTypeSwitch(int slotNumber)
    {

        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile(slotNumber);
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromBinaryFile(slotNumber);
            return gameData;
        }


    }


    public void LoadGame(int slotNumber)
    {

        //player data

        SetPlayerData(LoadingTypeSwitch(slotNumber).playerData);




        //Enviroment data
        SetEnviromentData(LoadingTypeSwitch(slotNumber).enviromentData);


        isLoading = false;

        DisableLoadingScreen();

    }

    private void SetEnviromentData(EnviromentData enviromentData)
    {


        foreach(Transform itemType in EnviromentManager.instance.allItems.transform)
        {

            foreach(Transform item in itemType.transform)
            {
                if (enviromentData.pickedupItems.Contains(item.name))
                {
                    Destroy(item.gameObject);
                }
            }
        }


        InventorySystem.instance.itemsPickedup = enviromentData.pickedupItems;












    }

    private void SetPlayerData(PlayerData playerData)
    {


        //Setting player stats


        PlayerState.instance.currentHealth = playerData.playerStats[0];
        PlayerState.instance.currentCalories = playerData.playerStats[1];
        PlayerState.instance.currentHydrationPercent = playerData.playerStats[2];

        //setting player position



        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];


        PlayerState.instance.playerBody.transform.position = loadedPosition;



        //setting player rotation


        Vector3 loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];

        PlayerState.instance.playerBody.transform.rotation = Quaternion.Euler(loadedRotation);


        //setting the inventory content

        foreach( string item in playerData.inventoryContent)
        {
            InventorySystem.instance.AddToInventory(item);
        }

        //Setting the quick slots content

        foreach( string item in playerData.quickSlotsContent)
        {
            //find next free quick slot

            GameObject availableSlot = EquipSystem.instance.FindNextEmptySlot();

            var itemToAdd = Instantiate(Resources.Load<GameObject>(item));

            itemToAdd.transform.SetParent(availableSlot.transform, false);
        }



        





    }

    public void StartLoadedGame(int slotNumber)
    {

        ActivateLoadingScreen();


        isLoading = true;


        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayedLoading(slotNumber));
    }





    private IEnumerator DelayedLoading(int slotNumber)
    {
        yield return new WaitForSeconds(1f);


        LoadGame(slotNumber);



    }




    #endregion







    #endregion




    #region || ------------To Binary Section----------||


    public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber  )
    {


        BinaryFormatter formatter = new BinaryFormatter();

        
        FileStream stream = new FileStream(binaryPath + fileName + slotNumber + ".bin", FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to" + binaryPath + fileName + slotNumber + ".bin");


    }


    public AllGameData LoadGameDataFromBinaryFile( int slotNumber)
    {


       

        if (File.Exists(binaryPath + fileName + slotNumber + ".bin"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binaryPath + fileName + slotNumber + ".bin", FileMode.Open);

            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();


            print("Data Loaed from" + binaryPath + fileName + slotNumber + ".bin");

            return data;

        }
        else
        {
            return null;
        }




    }


    #endregion





    #region || ------------To Json Section----------||


    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {

        string json = JsonUtility.ToJson(gameData);


        //string encrypted = EncryptionDecryption(json);



        using(StreamWriter writer = new StreamWriter(jsonPathProject + fileName + slotNumber + ".json"))
        {
            writer.Write(json);
            print("Saved Game to Json file at :" + jsonPathProject + fileName + slotNumber + ".json");                                                                                                                
        };
       


    }


    public AllGameData LoadGameDataFromJsonFile(int slotNumber)
    {
        using (StreamReader reader = new StreamReader(jsonPathProject + fileName + slotNumber + ".json"))
        {

            string json = reader.ReadToEnd();

            //string decrypted = EncryptionDecryption(json);

            AllGameData data = JsonUtility.FromJson<AllGameData>(json);
            return data;



        };




    }


    #endregion



    #region || ------------Settings Section----------||

    #region || ------------Volume Section--------||



    //---else

    [System.Serializable]

    public class VolumeSettings
    {
        public float music;
        public float effects;
        public float master;

    }

    public void SaveVolumeSettings( float _music, float _effects, float _master)
    {


        VolumeSettings volumeSettings = new VolumeSettings()
        {
            music = _music,
            effects = _effects,
            master = _master

        };
        PlayerPrefs.SetString("Volume", JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();

        print("Saved to Player Pref");

    }


    public VolumeSettings LoadVolumeSettings() 
    {

        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
  
    }
    
    public float LoadMusicVolume() 
    {

        var volumeSettings = JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
        return volumeSettings.music;
  
    }







    #endregion

    #endregion



    #region || ------------Encryption----------|

    public string EncryptionDecryption(string jsonString) 
    { ;
        string keyword = "1234567";

        string result = "";

        for(int i = 0; i < jsonString.Length; i++)
        {
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);
        }

        return result; // encrypted of dectypted string


        // XOR = "is there a diff"

        //--- Encrypt----
        //Mike - 01101101 01101001 01101011 01100101
        //M -  01101101
        //Key- 00000001
        //
        //Encrypted - 01101100


        //---Decrypt
        // Encrypted - 01101100
        //Key -        00000001   
        //
        //M - 01101101




    }




    #endregion

    #region || ------------Loading Section----------||
    public void ActivateLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        //music for loading screen


        //Animation


        //Show


    }



    public void DisableLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);

    }

    #endregion

    #region || ------------Utility----------||


    public bool DoesFileExists(int slotNumber)
    {
        if(isSavingToJson)
        {
            if(System.IO.File.Exists(jsonPathProject + fileName + slotNumber + ".json"))// SaveGame0.json //SaveGame1.json // SaveGame2.json
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (System.IO.File.Exists(binaryPath + fileName + slotNumber + ".bin"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    public bool IsSlotEmpty(int slotNumber)
    {
        if (DoesFileExists(slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

    #endregion


}
