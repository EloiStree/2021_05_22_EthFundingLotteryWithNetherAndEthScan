using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EtherLotteryInitParametersToFileStorage : MonoBehaviour
{

    public EthereumLotteryInitialParametersMono m_source;
    public string m_fileNameInitLog = "InitParamsEditorIn.json";
    public string m_filePath;

    [ContextMenu("Save at File Path")]
    public void SaveStateAsFilePath()
    {
        SaveStateAsFilePath(m_filePath);
    }
    [ContextMenu("Save at Unity Perma data File Path")]
    public void SaveStateAtPersistanceFilePath()
    {
        string path = Application.persistentDataPath;
        SaveStateAsFilePath(path);
    }

    [ContextMenu("Open perma file storage")]
    public void OpenPersistanceFilePath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    public void SaveStateAsFilePath(string path)
    {

        Directory.CreateDirectory(path);
        File.WriteAllText(path + "/" + m_fileNameInitLog, JsonUtility.ToJson(m_source.m_lotteryData, true));
    }

}
