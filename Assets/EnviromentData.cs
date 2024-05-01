

using System.Collections.Generic;

[System.Serializable]
public class EnviromentData
{
    public List<string> pickedupItems;
    

    public EnviromentData(List<string> pickedup)
    {
        pickedupItems = pickedup;
        
    }
}
