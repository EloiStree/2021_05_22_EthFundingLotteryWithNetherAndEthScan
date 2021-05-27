using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherFundingLog_Recover : MonoBehaviour
{
    public EthereumLotteryCurrentStateLogMono m_reportLog;
    public EthereumLotteryInitialParametersMono m_initParams;
    public EtherFundingPublicLotteryMono m_lotteryComputer;
    public ComputeEntry m_entryValue;

    void Start()
    {
        
    }

    [ContextMenu("Recover")]
    void Recover()
    {
        //m_reportLog.m_result.m_currentStateLog.m_winnerAddress = m_lotteryComputer.m_currentWinnerAddress;
        //m_reportLog.m_result.m_currentStateLog.m_winnerHash = m_lotteryComputer.m_currentComputedHashOfVictory;
        //m_reportLog.m_result.m_currentStateLog.m_winnerExplainedStackTrace = m_lotteryComputer.m_computeStack;
        //if(m_initParams!=null)
        //    m_reportLog.m_result.m_currentStateLog.m_givenInitialParams = m_initParams.m_lotteryData;
        //m_reportLog.m_result.m_currentStateLog.m_startBlock ="";//= m_initParams.m_lotteryData.m_startBlockId ;
        //m_reportLog.m_result.m_currentStateLog.m_startTransactionId = m_initParams.m_lotteryData.m_startTransactionId;

        //m_reportLog.m_result.m_currentStateLog.m_currentBlock="";
        //m_reportLog.m_result.m_currentStateLog.m_currentTransactionId="";

        ////m_reportLog.m_result.m_currentStateLog.m_endBlock="";
        ////m_reportLog.m_result.m_currentStateLog.m_endTransactionId="";

        ////SHOULD BE FETCH FROM A SPECIFIC CLASS PUBLIC TO SPLITE PARTICIPANT OF AN ABSTRACT LIST
        //m_reportLog.m_result.m_currentStateLog.m_participantsCount= (ulong) m_lotteryComputer.m_participantsInJoinOrder.Length;
        //m_reportLog.m_result.m_currentStateLog.m_transactionsCount= (ulong) m_lotteryComputer.m_allTransactionInReceivedOrder.Length; 

        //m_reportLog.m_result.m_currentStateLog.m_winnerHash= m_lotteryComputer.m_computeStack.m_finalHash;
        //m_reportLog.m_result.m_currentStateLog.m_winnerAddress = m_lotteryComputer.m_currentWinnerAddress;

        //m_reportLog.m_result.m_currentStateLog.m_wonInWei = "0";
        //m_reportLog.m_result.m_currentStateLog.m_wonInWeiByWinner = "0";
        //m_reportLog.m_result.m_currentStateLog.m_wonInWeiForFunding="0";

        //// Is Won in wei < requested ETH
        //m_reportLog.m_result.m_currentStateLog.m_finishedByTimeoutCondition=false;
        //// Is Won in wei < requested ETH && last block timestamp > max time allocated to the lottery
        //m_reportLog.m_result.m_currentStateLog.m_finishedByAmountReachCondition=false;



        ////m_reportLog.m_result.m_transactionsUsedForWinnerHash = m_lotteryComputer.m_usedTransactionForHash;
        //m_reportLog.m_result.m_valideParticipantsAddessInJoinOrderArray = m_lotteryComputer.m_participantsInJoinOrder;
    }
}

