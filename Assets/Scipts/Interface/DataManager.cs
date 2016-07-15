using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager {

    public static void SaveVars(float in_musicLevel, float in_effectsLevel, int in_qualityLevel)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(Application.persistentDataPath + "/settings.dat", FileMode.OpenOrCreate);

        Settings settingsStruct = new Settings();
        settingsStruct.qualityLevel = in_qualityLevel;
        settingsStruct.musicLevel = in_musicLevel;
        settingsStruct.effectsLevel = in_effectsLevel;

        bf.Serialize(file, settingsStruct);
        file.Close();
    }

    public static Settings LoadVars()
    {
        if (File.Exists(Application.persistentDataPath + "/settings.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/settings.dat", FileMode.Open);
            Settings settingsStruct = (Settings)bf.Deserialize(file);
            file.Close();
            return settingsStruct;
        }
        else
        {
            Settings settingsStruct = new Settings();
            settingsStruct.effectsLevel = 1;
            settingsStruct.musicLevel = 1;
            settingsStruct.qualityLevel = 1;
            return settingsStruct;
        }
    }
}

[Serializable]
public class Settings
{
    public int qualityLevel;
    public float musicLevel;
    public float effectsLevel;
}
