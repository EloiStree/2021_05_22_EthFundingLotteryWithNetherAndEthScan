using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthereumLotteryCurrentStateLogMono : MonoBehaviour
{
    public EhterumLotteryMassiveDetails m_result;
    //  public Snapsho m_

}

[System.Serializable]
public class EhterumLotteryMassiveDetails 
{
  

    public void SetInitialCondition(LF_InitialLotteryParameters initParameters ) 
    {
        m_initialParametersOfLottery = initParameters;
    }
    public void SetComputedCondition(LF_TransactionsToParticipantAndEtherCount transactionsToParticipantAndEther,
        LF_ParticipantsAndEtherCountExplainedLog transactionsExplained,
       LF_WinnerExplainedLog winnerExplained)
    {
        m_winnerExplainedStackTrace = winnerExplained;
        m_lotteryStateExplainedBasedOnTransactionGiven = transactionsExplained;
        m_lotteryStateBasedOnTransactionsGiven = transactionsToParticipantAndEther;
    }

    public LF_InitialLotteryParameters m_initialParametersOfLottery;
    public LF_TransactionsToParticipantAndEtherCount m_lotteryStateBasedOnTransactionsGiven;

    public void GetContestTotaleAmount(out decimal wei)
    {

        m_lotteryStateBasedOnTransactionsGiven.GetWeiCount(out string weiAsString);
        decimal.TryParse(weiAsString, out wei);
    }
    public void GetContestTotaleAmount(out decimal wei, out double ethApproximation)
    {

        m_lotteryStateBasedOnTransactionsGiven.GetWeiCount(out string weiAsString);
        decimal.TryParse(weiAsString, out wei);
        EthereumConverttion.ApproximateConvert(wei, out decimal eth, EtherType.Wei, EtherType.Ether);
        ethApproximation = (double)eth;
    }

    public string GetFundAddress()
    {
       return  m_initialParametersOfLottery.m_fundingAddress;
    }

    public void IsLotteryFinished(out bool byAmount, out bool byTime, out bool isFinished)
    {
        byAmount = IsFinishByAmount();
        byTime = IsFinishByTimeoutCondition();
        isFinished = byTime || byAmount;
    }

    public void GetWinnerAmount(out decimal weiAmount, out double ethApprixmation)
    {
        GetContestTotaleAmount(out decimal amount, out double apprixmation);
        GetFundingAmount(out decimal fundAmount, out double fundApproximation);
        weiAmount = amount - fundAmount;
        ethApprixmation = apprixmation - fundApproximation;
    }

    public string GetDateAsString()
    {
        return m_initialParametersOfLottery.m_outOfTimeCondition.GetDate().ToString("yyyy-MM-dd hh:mm ss");
    }

    public long GetSecondsLeftBeforeEndOfLottery()
    {

      long lotteryDate=  m_initialParametersOfLottery.m_outOfTimeCondition.GetTimeStampInSecondsUTC();
        long now = EthScanUrl.GetTimestamp();
        return lotteryDate - now;
    }

    public void GetTotalAmount(out decimal weiAmount, out double ethApprixmation)
    {
        GetContestTotaleAmount(out weiAmount, out ethApprixmation);
    }

    public LF_ParticipantsAndEtherCountExplainedLog m_lotteryStateExplainedBasedOnTransactionGiven;

    public void GetFundingAmount(out decimal weiAmount, out double ethApprixmation)
    {
        GetContestTotaleAmount(out decimal amount, out double apprixmation);
        weiAmount = (amount * (decimal)(1.0-m_initialParametersOfLottery.m_pourcentToFunding) ); 
        EthereumConverttion.ApproximateConvert(weiAmount, out decimal eth, EtherType.Wei, EtherType.Ether);
        ethApprixmation = (double)eth;

    }

    public LF_WinnerExplainedLog m_winnerExplainedStackTrace;




    public LF_InitialLotteryParameters GetInitialLotteryParameters()
    {
        return m_initialParametersOfLottery;
    }


    public WalletTransactionsHistoryRange GetTransactionsHistory() {
        return m_lotteryStateBasedOnTransactionsGiven.GetTransactionHistoryUsed(); 
    }
    public uint GetParticipantsCount() {
        return m_lotteryStateBasedOnTransactionsGiven.GetParticipantsCount();
    }
    public uint GetTransactionsCount() { 
        return m_lotteryStateBasedOnTransactionsGiven.GetTransactionsCount();
    }
    public string [] GetParticipantsAddressesInOrder() {
        return m_lotteryStateBasedOnTransactionsGiven.GetParticipantsListInOrder();
    }
    public string [] GetTransactionsHashInOrder() {

        return m_lotteryStateBasedOnTransactionsGiven.GetTransactionsHashListInOrder();

    }



    public string GetStartTransactionHash()
    {
        string result;
        m_lotteryStateBasedOnTransactionsGiven.GetTransactionHistoryUsed().GetStart().GetTransactionId(out result);
        return result;
    }
    public string GetStartBlockID()
    {
        string result;
        m_lotteryStateBasedOnTransactionsGiven.GetTransactionHistoryUsed().GetStart().GetBlockId(out result);
        return result;
    }

    public void GetEndOfContestTransactionHash( out bool isLotteryEnded, out string transactionId) { throw new System.NotImplementedException(); }

    public void GetWinner(out bool isContestFinished, out string winnerAddrses) {
        isContestFinished = IsLotteryFinished();
        winnerAddrses = m_lotteryStateBasedOnTransactionsGiven.GetParticipantAddress(
            m_winnerExplainedStackTrace.GetIndexOfWinner());
    }
    public void GetLotteryEndingInformation(out decimal weiLeftToFinishTheContest, out long timeLeftBeforeEndOfContestInSecond) {

        m_initialParametersOfLottery.GetGoalAsEther(out decimal goalEther);
        EthereumConverttion.ApproximateConvert(goalEther, out decimal goalAsWei, EtherType.Ether, EtherType.Wei);
        m_lotteryStateBasedOnTransactionsGiven.GetWeiCount(out decimal currentEther, EtherType.Wei);
        weiLeftToFinishTheContest = goalAsWei - currentEther ;
        timeLeftBeforeEndOfContestInSecond = GetSecondsLeftBeforeEndOfLottery();
    }

    public void GetWinnerHashInformation(out string winnerHash, out int winnerIndex, out string winnerAddress)
    {
        winnerIndex = m_winnerExplainedStackTrace.GetIndexOfWinner();
        winnerHash = m_winnerExplainedStackTrace.GetWinnerHash();
        winnerAddress = m_lotteryStateBasedOnTransactionsGiven.GetParticipantAddress(winnerIndex);
    }

    public void GetWeiRepartition(out string weiForWinner, out string weiForFunding) { throw new System.NotImplementedException(); }
    public void GetWeiRepartition(out decimal weiForWinner, out decimal weiForFunding) { throw new System.NotImplementedException(); }
    public void GetWeiWonByLottery(out string weiWon) {
        m_lotteryStateBasedOnTransactionsGiven.GetWeiCount(out weiWon);
    }
    public void GetWeiWonByLottery(out decimal weiWon)
    {
        m_lotteryStateBasedOnTransactionsGiven.GetWeiCount(out string weiWonAsString);
        decimal.TryParse(weiWonAsString, out weiWon);
    }

    public bool IsFinishByTimeoutCondition()
    {
        if (IsFinishByAmount())
            return false;
        return
        m_lotteryStateBasedOnTransactionsGiven.IsEndByTimeOut();

    }
    public bool IsFinishByAmount()
    {
        return m_lotteryStateBasedOnTransactionsGiven.IsEndByAmount();
    }
    public bool IsLotteryFinished() {

        return m_lotteryStateBasedOnTransactionsGiven.IsEndByAmount() && m_lotteryStateBasedOnTransactionsGiven.IsEndByTimeOut(); 
    }
}



public class SHA256UrlChecker
{
    public static string GetUrl(string input)
    {
        return "https://codebeautify.org/sha256-hash-generator?input=" + input;

    }
    public static void OpenUrl(string input)
    {
        Application.OpenURL(GetUrl(input));
    }
}