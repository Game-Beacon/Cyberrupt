using System.IO;
using UnityEngine;

public class SaveLoadManager : GameBehaviour
{
    private static SaveLoadManager _instance = null;
    public static SaveLoadManager instance { get { return _instance; } }

    private string pathString;

    public override void GameAwake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontKillSelfOnLoad();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
            

        //除非確定之後會對存/讀檔有重大更改，不然建議別動這行
        pathString = Application.persistentDataPath + "/SaveFile.save";
        LoadFile();
    }

    public void SaveFile()
    {
        string fileString = JsonUtility.ToJson(SaveData.current);
        File.WriteAllText(pathString, fileString);
    }

    public void LoadFile()
    {
        if (!File.Exists(pathString))
            return;
        string fileString = File.ReadAllText(pathString);
        SaveData.OverwriteSaveData(JsonUtility.FromJson<SaveData>(fileString));
    }

    public void ClearFile()
    {
        if (File.Exists(pathString))
            File.Delete(pathString);
        SaveData.OverwriteSaveData(new SaveData());
    }
}
