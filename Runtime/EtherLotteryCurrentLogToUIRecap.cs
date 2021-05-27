using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EtherLotteryCurrentLogToUIRecap : MonoBehaviour
{
    public EthereumLotteryCurrentStateLogMono m_currentState;

    public InputField m_winnerIndex;
    public InputField m_winnerParticipantCount;
    public InputField m_winnerHash;
    public InputField m_winnerAddress;


    public Toggle m_lotteryFinished;
    public Toggle m_winByAmount;
    public Toggle m_winByTimeout;
    public InputField m_timeLeft;
    public InputField m_amountLeft;

    public InputField m_totalEther;
    public InputField m_winnerEther;
    public InputField m_fundingEther;

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        m_currentState.m_result.GetWinner(out bool isFinish, out string winnerAddress);
        m_currentState.m_result.IsLotteryFinished(out bool byAmountValue, out bool byTimeValue, out bool isFinishedValue);
            m_currentState.m_result.GetWinnerHashInformation(out string wHash, out int wIndex, out string wAddress);
        m_winnerHash.text = wHash;
        m_winnerAddress.text = wAddress;
        m_winnerIndex.text = ""+wIndex;
        m_winnerParticipantCount.text = "" + m_currentState.m_result.GetParticipantsCount();

        m_lotteryFinished.isOn = isFinishedValue;
        m_winByAmount.isOn = byAmountValue;
        m_winByTimeout.isOn = byTimeValue;
        //m_currentState.m_result.GetEndOfContestTransactionHash(out bool isEnded, out string transactionEnd);
        m_currentState.m_result.GetLotteryEndingInformation(out decimal weiLeft, out long timeLeft);

        if (!isFinishedValue)
        {
            long timeInSecondsLeft = timeLeft;
            string measurement = "day";
            double t = timeInSecondsLeft / (3600.0 * 24.0);
            if (t < 1.0)
            {
                measurement = "hour";
                t = timeInSecondsLeft / (3600.0);
            }
            if (t < 1.0)
            {
                measurement = "minute";
                t = timeInSecondsLeft / (60.0);
            }
            if (t < 1.0)
            {
                measurement = "s";
                t = timeInSecondsLeft;
            }
            m_timeLeft.text = string.Format("\n Still in process: {0:0.00} ({2}) {1}  ", t, m_currentState.m_result.GetDateAsString(), measurement);
        }
        else m_timeLeft.text = "Finished";

        EthereumConverttion.ApproximateConvert(weiLeft, out decimal weiLeftAsEth, EtherType.Wei, EtherType.Ether);
        m_amountLeft.text =string.Format("Left Amount: {0:0.00000} ETH", weiLeftAsEth);

        decimal wei;
        double eth;
        m_currentState.m_result.GetTotalAmount(out wei, out eth);
        m_totalEther.text = string.Format("Total: {0:0.00000} ETH  ({1:0}) ", eth, wei);
        m_currentState.m_result.GetWinnerAmount(out wei, out eth);
        m_winnerEther.text = string.Format("Winner: {0:0.00000} ETH  ({1:0}) ", eth, wei);
        m_currentState.m_result.GetFundingAmount(out wei, out eth);
        m_fundingEther.text =  string.Format("Funding: {0:0.00000} ETH  ({1:0}) ", eth, wei); 

    }

    public void SaveMarkdownInClipboard()
    {

        SaveInClipboard(EtherLotteryCurrentLogToMarkdownFile.GetMarkdownReport(m_currentState.m_result,"","",""));
    }


    public void SaveFullInformationRawInClipboard() {

        SaveInClipboard(JsonUtility.ToJson(m_currentState.m_result, true) );
    }

    public void SaveInClipboard(string text) {
        GUIUtility.systemCopyBuffer = text;
    }
}
