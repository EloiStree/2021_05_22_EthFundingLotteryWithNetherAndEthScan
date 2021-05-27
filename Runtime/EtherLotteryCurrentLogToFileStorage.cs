using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EtherLotteryCurrentLogToFileStorage : MonoBehaviour
{

    public EthereumLotteryCurrentStateLogMono m_source;
    public string m_fileNameInitLog = "InitParams.json";
    public string m_fileNameShortStateLog = "ShortLog.json";
    public string m_fileNameFullStateLog = "FullLog.json";
    public string m_fileNameMarkdownLog = "MarkdownLog.json";

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
    public void OpenPersistanceFilePath() { 
        Application.OpenURL(Application.persistentDataPath);
    }

    public void SaveStateAsFilePath(string path) {

        Directory.CreateDirectory(path);
        //File.WriteAllText(path + "/" + m_fileNameInitLog,JsonUtility.ToJson( m_source.m_result.m_currentStateLog.m_givenInitialParams,true));
        //File.WriteAllText(path + "/" + m_fileNameShortStateLog, JsonUtility.ToJson(m_source.m_result.m_currentStateLog, true));
        File.WriteAllText(path + "/" + m_fileNameFullStateLog, JsonUtility.ToJson(m_source.m_result, true));
        File.WriteAllText(path + "/" + m_fileNameMarkdownLog, EtherLotteryCurrentLogToMarkdownFile.GetMarkdownReport(
            m_source.m_result, "","",""));
    }

    internal void OpenMarkdownFile()
    {
        Application.OpenURL(Application.persistentDataPath + "/" + m_fileNameMarkdownLog);
    }

    public void CopyLogInClibpard() {

        TextEditor te = new TextEditor();
        te.text =(EtherLotteryCurrentLogToMarkdownFile.GetMarkdownReport(
            m_source.m_result, "", "", ""));
        te.SelectAll();
        te.Copy();

    }
}
