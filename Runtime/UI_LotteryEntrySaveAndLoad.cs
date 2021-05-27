using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LotteryEntrySaveAndLoad : MonoBehaviour
{

    public EthereumLotteryInitialParametersMono m_initParams;

    public InputField m_minEther;
    public InputField m_goalEther;
    public Dropdown m_serverTarget;
    public InputField m_startTransaction;
    public InputField m_lotteryAddress;
    public InputField m_fundingAddress;
    public InputField m_pourcentToFunding;
    public InputField m_transactionWinnerCount;

    [Header("Timestamp UTC")]
    public InputField m_year;
    public InputField m_month;
    public InputField m_day;
    public InputField m_hour;
    public InputField m_minute;
    public InputField m_second;
    [Header("Timestamp UTC")]
    public InputField m_etherscanAPI;

    void Start()
    {
        Load();

    }

    public void Load()
    {
        try
        {
            LF_InitialLotteryParameters value = JsonUtility.FromJson<LF_InitialLotteryParameters>(PlayerPrefs.GetString("LotteryEntrySaveUI"));
            if (value != null)
                m_initParams.m_lotteryData = value;
        }
        catch (Exception e) { }

        PushDataToUI();

        m_etherscanAPI.text = PlayerPrefs.GetString("EtherScanAPI");
    }

    public void PushDataToUI()
    {
        m_initParams.m_lotteryData.GetAsMinEntryEther(out decimal minvalue);
        m_initParams.m_lotteryData.GetGoalAsEther(out decimal goalvalue);
        m_minEther.text = "" + minvalue;
        m_goalEther.text ="" + goalvalue;
        m_startTransaction.text = m_initParams.m_lotteryData.m_startTransactionId;
        m_lotteryAddress.text = m_initParams.m_lotteryData.m_lotteryAddress;
        m_fundingAddress.text = m_initParams.m_lotteryData.m_fundingAddress;
        m_pourcentToFunding.text =string.Format( "{0:0.0}", m_initParams.m_lotteryData.m_pourcentToFunding * 100.0); 
        m_transactionWinnerCount.text =""+ m_initParams.m_lotteryData.m_numberOfTransactionsUsedToComputeWinner;
        m_serverTarget.SetValueWithoutNotify( m_initParams.m_lotteryData.m_serverTarget == EtherServerTarget.Mainset ?0:1);

        m_year.text = m_initParams.m_lotteryData.m_outOfTimeCondition.year;
        m_month.text = m_initParams.m_lotteryData.m_outOfTimeCondition.month;
        m_day.text = m_initParams.m_lotteryData.m_outOfTimeCondition.day;
        m_hour.text = m_initParams.m_lotteryData.m_outOfTimeCondition.hour;
        m_minute.text = m_initParams.m_lotteryData.m_outOfTimeCondition.minute;
        m_second.text = m_initParams.m_lotteryData.m_outOfTimeCondition.second;
    }

    public void Save() {
        Debug.Log("Save UI");
        PushUIToData();
        PlayerPrefs.SetString("LotteryEntrySaveUI", JsonUtility.ToJson(m_initParams.m_lotteryData));
        PlayerPrefs.SetString("EtherScanAPI", m_etherscanAPI.text);
    
    }

    public void PushUIToData()
    {
        m_initParams.m_lotteryData.SetMinEntryEther(m_minEther.text);
        m_initParams.m_lotteryData.SetGoalEther(m_goalEther.text);

        m_initParams.m_lotteryData.m_startTransactionId= m_startTransaction.text;
        m_initParams.m_lotteryData.m_lotteryAddress= m_lotteryAddress.text;
        m_initParams.m_lotteryData.m_fundingAddress= m_fundingAddress.text;

        double.TryParse(m_pourcentToFunding.text, out double pourcent);
        m_initParams.m_lotteryData.m_pourcentToFunding = pourcent/100.0;
        uint.TryParse(m_transactionWinnerCount.text, out uint transactionCount);
         m_initParams.m_lotteryData.m_numberOfTransactionsUsedToComputeWinner = transactionCount;
        m_initParams.m_lotteryData.m_serverTarget = m_serverTarget.value == 0 ? EtherServerTarget.Mainset : EtherServerTarget.Ropsten;

        int value = 0; ;
        int.TryParse(m_year.text, out value);
        value = Mathf.Clamp(value, 2020, 2050);
        m_initParams.m_lotteryData.m_outOfTimeCondition.year= "" + value;
        int.TryParse(m_month.text, out value);
        value = Mathf.Clamp(value, 1, 13);
        m_initParams.m_lotteryData.m_outOfTimeCondition.month = "" + value;
        int.TryParse(m_day.text, out value);
        value = Mathf.Clamp(value, 1, 32);
        m_initParams.m_lotteryData.m_outOfTimeCondition.day = "" + value;
        int.TryParse(m_hour.text, out value);
        value = Mathf.Clamp(value, 0, 24);
        m_initParams.m_lotteryData.m_outOfTimeCondition.hour = "" + value;
        int.TryParse(m_minute.text, out value);
        value = Mathf.Clamp(value, 0, 60);
        m_initParams.m_lotteryData.m_outOfTimeCondition.minute = "" + value;
        int.TryParse(m_second.text, out value);
        value = Mathf.Clamp(value, 0, 60);
        m_initParams.m_lotteryData.m_outOfTimeCondition.second = "" + value;
        PushDataToUI();
    }

    private void OnDisable()
    {
    }
    private void OnApplicationQuit()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}
