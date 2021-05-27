using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicLotteryTransactionTranslator : MonoBehaviour
{
    public EthScanRequest_TransactionFromToBlockOrTimestampMono m_source;
    public WalletTransactionsHistoryRange m_walletRange;

    public string m_minWeiOfLottery="0";
    public string m_weiEndOfLottery = "0";
    public string m_addressOfLotteryWallet;
    public string m_startPointAddressHash;
    public ulong m_unixTimeStampUTCEndOfLottery;


    [Header("result")]
    public LF_TransactionsToParticipantAndEtherCount m_snapshotResult ;
    public LF_ParticipantsAndEtherCountExplainedLog m_stackTrace;


    public static void AddValue(decimal current, decimal toAdd, out decimal newValue, out decimal previousValue) {
        previousValue = current;
        newValue = current + toAdd;

    
    }
    public void Refresh() {
        m_walletRange = m_source.m_recoveredHistory;
        decimal.TryParse(m_minWeiOfLottery, out decimal minWeiValue);
        decimal.TryParse(m_weiEndOfLottery, out decimal endLotteryValue);
        ComputeParticipants(m_addressOfLotteryWallet, m_startPointAddressHash, minWeiValue, endLotteryValue, m_unixTimeStampUTCEndOfLottery, m_walletRange, out m_snapshotResult, out m_stackTrace);
    }
  
    public static void ComputeParticipants(
        string addressOfLotteryWallet,
        string startPointAddressHash,
        decimal minWeiValue,
        decimal endLotteryValue,
        ulong unixTimeStampUTCEndOfLottery,
        WalletTransactionsHistoryRange walletRangeUseToCompute,
        out LF_TransactionsToParticipantAndEtherCount snapshotResult,
        out LF_ParticipantsAndEtherCountExplainedLog stackResult)
    {
        stackResult = new LF_ParticipantsAndEtherCountExplainedLog();
        List<string> valideParticipantByOrder = new List<string>();

        decimal previousWeiCount = new decimal(0);
        decimal currentWeiCount = new decimal(0);

        bool endByAmountReach=false;
        bool endByOutOfTime=false ;

        bool startPointFound=false;
        string endedHash="";

        foreach (TransactionFullInfoBean t in walletRangeUseToCompute.GetTransactionsOrderedFromPastToRecent())
        {

          //  Debug.Log("T>>:" + t.GetOneLiner());
            if (!startPointFound) {

            //    Debug.Log(t.GetTransactionIdHash()+"<<??>>" +startPointAddressHash);
                if (((t.GetTransactionIdHash().Trim()).Equals(startPointAddressHash.Trim())) )
                {
                    startPointFound = true;
                    stackResult. EnqueueLog(t, currentWeiCount, "Transaction is the start point of the lottery: Start lottery after this transaction");
                    continue;
                }
                else {
                    stackResult.EnqueueLog(t, currentWeiCount, "No transaction start point found: Transaction ignore");
                    continue;

                }

            }
           
            if (t.HadError())
            {
                stackResult.EnqueueLog(t, currentWeiCount, "Transaction error: Ignore actions");
            }
            else if (!t.GetTransactionStatusSucced())
            {
                // How should I deal with that to make it unchangeable in time.
                stackResult.EnqueueLog(t, currentWeiCount, "Transaction not finished:  Ignore actions");
            }
            else if (unixTimeStampUTCEndOfLottery <= t.GetTimeStamp())
            {
                // How should I deal with that to make it unchangeable in time.
                stackResult.EnqueueLog(t, currentWeiCount, "Lottery is considered as finished: Not counted at all in this lottery");
            }
            else if (t.GetFromWallet().IndexOf(addressOfLotteryWallet) >=0)
            {
                // How should I deal with that to make it unchangeable in time.
                stackResult.EnqueueLog(t, currentWeiCount, "Transaction is the lottery: Ignore actions");
            }
            else if (valideParticipantByOrder.Contains(t.m_fromAddress))
            {
                AddValue(currentWeiCount, t.GetValueInWei(), out currentWeiCount, out previousWeiCount);
                stackResult.EnqueueLog(t, currentWeiCount, "Transaction is valide but participants already in: ETH counted but participants is not re-add.");
            }
            else if (!valideParticipantByOrder.Contains(t.m_fromAddress))
            {
                
                AddValue(currentWeiCount, t.GetValueInWei(), out currentWeiCount, out previousWeiCount);

                if (t.GetValueInWei() >= minWeiValue)
                {
                    valideParticipantByOrder.Add(t.GetFromWallet());
                    stackResult.EnqueueLog(t, currentWeiCount, "Transaction is valide: ETH counted and participant added to index: " + (valideParticipantByOrder.Count - 1)+" > "+t.GetFromWallet());
                }else
                {
                    stackResult.EnqueueLog(t, currentWeiCount, "Transaction minimum ETH is not reach: ETH counted and participant not added." );
                }
            }



            if (currentWeiCount >= endLotteryValue) {
                endByAmountReach = true;
                endedHash = t.GetTransactionIdHash();
                stackResult.EnqueueLog(t, currentWeiCount, "The goal of the lottery is reach. End is declared here." );

                break;
            }
            if (unixTimeStampUTCEndOfLottery <= t.GetTimeStamp()) {
                endByOutOfTime = true;
                endedHash = t.GetTransactionIdHash();
                stackResult.EnqueueLog(t, currentWeiCount, "The lottery was closed by time out of contest date: Transaction not count and lottery declared finished");
                break;
            }
        }


        snapshotResult = new LF_TransactionsToParticipantAndEtherCount(walletRangeUseToCompute, valideParticipantByOrder.ToArray(), currentWeiCount);
        
        snapshotResult.SetEndOfLotteryTransactionHash(endedHash);
        if (endByAmountReach)
            snapshotResult.SetAsEndedByAmount();
        if (endByOutOfTime)
            snapshotResult.SetAsEndedByTime();
    }

}


[System.Serializable]
public class LF_TransactionsToParticipantAndEtherCount {

    //Not filtered
    [SerializeField] WalletTransactionsHistoryRange m_etherTransactionsObserved = new WalletTransactionsHistoryRange();
    [SerializeField] string[] m_participants = new string[0];
    [SerializeField] string m_currentWeiCount = "0";
    [SerializeField] bool m_endedByAmount;
    [SerializeField] bool m_endedByTimeout;
    [SerializeField] string m_endTransactionHash= "";

    public void GetWeiCount(out string weiCount) { weiCount = m_currentWeiCount; }
    public void GetWeiCount(out decimal value, EtherType etherType)
    {
        decimal.TryParse(m_currentWeiCount, out decimal currentWei);
        EthereumConverttion.ApproximateConvert(currentWei, out value, EtherType.Wei, etherType);
    }
    public void GetWeiCount(out decimal currentWei)
    {
        decimal.TryParse(m_currentWeiCount, out  currentWei);
    }


    public void SetAsEndedByTime() { m_endedByTimeout = true; }
    public void SetAsEndedByAmount() { m_endedByAmount = true; }

    public string GetParticipantAddress(int winnerIndex)
    {
       return m_participants[winnerIndex];
    }

    public string[] GetParticipantsListInOrder() {
        return m_participants;
    }
    public WalletTransactionsHistoryRange GetTransactionHistoryUsed() { return m_etherTransactionsObserved; }

    public bool IsEndByAmount() { return m_endedByAmount; }
    public bool IsEndByTimeOut() { return m_endedByTimeout; }
    public bool IsFinished() { return IsEndByAmount() || IsEndByTimeOut(); }

    public void SetEndOfLotteryTransactionHash(string endedHash)
    {
        m_endTransactionHash = endedHash;
    }
    public bool HasEndHash() {
        return !string.IsNullOrEmpty(m_endTransactionHash);
    }

    public string GetEndTransactionHash() { 
        return m_endTransactionHash;
    }

    public uint GetParticipantsCount()
    {
       return (uint) m_participants.Length;
    }

    public uint GetTransactionsCount()
    {
        return (uint)m_etherTransactionsObserved.GetTransactionsCount();
    }

    public string[] GetTransactionsHashListInOrder()
    {
        return m_etherTransactionsObserved.GetTransactionsHashOrderedFromPastToRecent();
    }

    public LF_TransactionsToParticipantAndEtherCount()
    {
    }
    public LF_TransactionsToParticipantAndEtherCount(WalletTransactionsHistoryRange transactionsObserveInEther, string[] participants, decimal currentWeiCount)
    {
        m_etherTransactionsObserved = transactionsObserveInEther;
        m_participants = participants;
        m_currentWeiCount = string.Format("{0:0}",currentWeiCount);
    }
}
[System.Serializable]
public class LF_ParticipantsAndEtherCountExplainedLog
{
    public List<LotteryFundingStackTrace> m_stackTrace = new List<LotteryFundingStackTrace>();

    public void EnqueueLog(TransactionFullInfoBean transaction, decimal weiTotal, string commentary)
    {
        m_stackTrace.Add(new LotteryFundingStackTrace(transaction, weiTotal, commentary));
    }
  

}

[System.Serializable]
public class LotteryFundingStackTrace
{
    public TransactionFullInfoBean m_transaction= new TransactionFullInfoBean();
    public string m_lotteryStateInWei="";
    public string m_stackcommentary="";

    public LotteryFundingStackTrace()
    {
    }

    public LotteryFundingStackTrace(TransactionFullInfoBean transaction, decimal lotteryStateInWei, string stackcommentary)
    {
        m_transaction = transaction;
        m_lotteryStateInWei = string.Format("{0:0}",lotteryStateInWei);
        m_stackcommentary = stackcommentary;
    }
    public LotteryFundingStackTrace(TransactionFullInfoBean transaction, string lotteryStateInWei, string stackcommentary)
    {
        m_transaction = transaction;
        m_lotteryStateInWei = lotteryStateInWei;
        m_stackcommentary = stackcommentary;
    }

    public object GetExplaination()
    {
        return m_stackcommentary;
    }

    public object GetOneLiner(char spliter)
    {
        return m_transaction.GetOneLiner(spliter);
    }

    public string GetTransactionHash()
    {
        return m_transaction.GetTransactionIdHash();
    }

    public double GetValueAsETH()
    {
        decimal.TryParse(m_lotteryStateInWei, out decimal result);
        EthereumConverttion.ApproximateConvert(result, out decimal newValue, EtherType.Wei, EtherType.Ether);
        return (double)newValue;
    }
}
