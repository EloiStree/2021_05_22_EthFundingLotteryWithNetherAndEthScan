using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthereumLotteryInitialParametersMono : MonoBehaviour
{
    public LF_InitialLotteryParameters m_lotteryData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



[System.Serializable]
public class LF_InitialLotteryParameters
{


    public EtherServerTarget GetServerTargetType()
    {
        return m_serverTarget;
    }

    public EtherServerTarget m_serverTarget;
    public string m_minimumEntryETH = "0.001";
    public string m_endLotteryETH = "32.0";
    public string m_startTransactionId = ""; //0x...

    [Tooltip("If the amount is not reach the lottery finish at this time")]
    public EndLotteryDateGMT m_outOfTimeCondition;

    public string m_lotteryAddress = "";//0x...
    public string m_fundingAddress = "";//0x...
    public double m_pourcentToFunding = 0.6;
    public uint m_numberOfTransactionsUsedToComputeWinner = 10;

    public void GetAsMinEntryEther(out decimal minvalue)
    {
        decimal.TryParse(m_minimumEntryETH, out minvalue);
    }

    public void GetGoalAsEther(out decimal goalvalue)
    {
        decimal.TryParse(m_endLotteryETH, out goalvalue);
    }

    public void SetMinEntryEther(string minEntryEther)
    {

        m_minimumEntryETH = minEntryEther;
    }
    public void SetGoalEther(string goalEther)
    {

        m_endLotteryETH = goalEther;
    }


    /// <summary>
    /// This date represent when the lottery end if the amount is not reach at GMT time
    /// </summary>
    /// 

    [System.Serializable]
    public class FundingRedirection
    {
        public string m_redirectionTitle;
        public string m_fundingWalletAddrese;
        public double m_pourcentFunding;
    }

    [System.Serializable]
    public class EndLotteryDateGMT
    {
        public string year = "", month = "", day = "", hour = "", minute = "", second = "";

        public long GetTimeStampInSecondsUTC()
        {
            return EthScanUrl.GetTimestampFrom(year, month, day, hour, minute, second);
        }

        public DateTime GetDate()
        {
            return new DateTime(
                int.Parse(year), 
                int.Parse(month), 
                int.Parse(day), 
                int.Parse(hour),
                int.Parse(minute),
                int.Parse(second));
        }
    }
}
