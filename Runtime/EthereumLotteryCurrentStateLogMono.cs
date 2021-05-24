using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthereumLotteryCurrentStateLogMono : MonoBehaviour
{
    public EthereumLotteryDetails m_result;
    
}

[System.Serializable]
public class EthereumLotteryCurrentState
{
    public EthereumLotteryInitialParameters m_givenInitialParams;
    public WinnerHashStackTrace m_winnerExplainedStackTrace;
    public string m_startBlock;
    public string m_startTransactionId;

    public string m_currentBlock;
    public string m_currentTransactionId;

    public string m_endBlock;
    public string m_endTransactionId;

    public ulong m_participantsCount;
    public ulong m_transactionsCount;

    public string m_winnerHash;
    public string m_winnerAddress;

    public string m_wonInWeiByWinner;
    public string m_wonInWeiForFunding;

    public bool    m_finishedByTimeoutCondition;
    public bool   m_finishedByAmountReachCondition;
    internal string m_wonInWei;
}
[System.Serializable]
public class EthereumLotteryDetails
{
    public EthereumLotteryCurrentState m_currentStateLog;
    public string [] m_valideParticipantsInJoinOrderArray;
    
}