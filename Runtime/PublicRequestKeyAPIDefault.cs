using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public abstract class PublicRequestKeyAPIAbstract : MonoBehaviour, IPublicRequestKeyAPI
{
    public abstract string GetKey();
}


public class PublicRequestKeyAPIDefault : PublicRequestKeyAPIAbstract
{
    public string m_fromAbsolutePath;
    public InputField m_keyFromInputField;
    public EditorKeyUnsafe m_demoDefaultKey;

    public override string GetKey()
    {
        if (m_keyFromInputField != null && m_keyFromInputField.text.Length > 0)
        {
            return  m_keyFromInputField.text;
        }

        if (!string.IsNullOrEmpty(m_privateTmpHolder))
            return m_privateTmpHolder;

       
        if (File.Exists(m_fromAbsolutePath)) {
            return m_privateTmpHolder = File.ReadAllText(m_fromAbsolutePath).Trim();
        }
        return  m_demoDefaultKey.m_key;
    }
    // TO AVOID TO READ THE FILE EACH TIME THE PROGRAMME ASK THE PASSWORD
     string m_privateTmpHolder;

}

public interface IPublicRequestKeyAPI
{
    string GetKey();
}

[System.Serializable]
public class EditorKeyUnsafe {
    public string m_key;
    [TextArea(0,5)]
    public string m_reminder = "Don't store your key here if you are a streamer or a git user !!! Except if it is to give a demo api key. But check if there are not consequence for you in case of abuse of this key that is now public.";

}
