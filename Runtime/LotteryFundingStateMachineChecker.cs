using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LotteryFundingStateMachineChecker : MonoBehaviour
{

    [Header("Init params")]
    public EthereumLotteryInitialParametersMono m_initParameters;

    [Header("Compute")]
    public EthScanRequest_TransactionFromToBlockOrTimestampMono m_transactionsCurrentInfo;
    public PublicLotteryTransactionTranslator m_transactionsToCurrentState;


    [Header("Out Value")]
    public EthereumLotteryCurrentStateLogMono m_outCurrentStateValue;
    public EtherLotteryCurrentLogToFileStorage m_fileStorage;
    public EtherLotteryCurrentLogToUIRecap m_uiRecap;

    [Header("Stuffs")]
    public bool m_autoTestAtStart=true;

    public bool m_automaticPayement;

    [Header("Bool Steps")]
    public bool m_startBlockFetch;
    public bool m_currentBlockFetch;
    public bool m_transactionsFetch;

    public string m_stackTrace;
    public InputField m_stateAsText;

    public UnityEvent m_startProcessing;
    public UnityEvent m_stopProcessing;

    public void ClearLog()
    {
        m_stackTrace = "";
        if (m_stateAsText != null)
            m_stateAsText.text = "";
    }
    public void Append(string text) {

        m_stackTrace = text + "\n" + m_stackTrace;
        if (m_stateAsText != null)
            m_stateAsText.text = m_stackTrace;
    }

    private void Awake()
    {
        m_transactionsCurrentInfo.m_getTransactionAt.AddListener((k) => { m_startBlockFetch = true; });
        m_transactionsCurrentInfo.m_getBlockAt.AddListener((k) => { m_currentBlockFetch = true; });
        m_transactionsCurrentInfo.m_getTransactionAt.AddListener((k) => { m_transactionsFetch = true; });
        if (m_autoTestAtStart)
            StartCoroutine(DoTheThing());
    }

    public void ComputeTheWinnerAndHistoryOfLottery() {
        StartCoroutine(DoTheThing());
    }
    private bool m_isProcessing;
    public IEnumerator DoTheThing() {
        m_isProcessing = true;
        m_startProcessing.Invoke();
        ClearLog();
        Append("Start");
        m_currentBlockFetch = false;
        m_transactionsFetch = false;
        //Recovert Init Parameters
        yield return new WaitForSeconds(1);
        m_transactionsCurrentInfo.m_targetAddess = m_initParameters.m_lotteryData.m_lotteryAddress;
        Append("Target address: " + m_transactionsCurrentInfo.m_targetAddess);

        EthScanUrl.SetAsUsingRopsten(m_initParameters.m_lotteryData.m_serverTarget == EtherServerTarget.Ropsten);
        Append("Target server: " + m_initParameters.m_lotteryData.m_serverTarget);

        m_transactionsCurrentInfo.m_startTransaction = m_initParameters.m_lotteryData.m_startTransactionId;
        Append("Start transaction: " + m_transactionsCurrentInfo.m_startTransaction);

        Append("Start fetching transactions... ");
        m_transactionsCurrentInfo.FetchFollowingTransactionTo(m_transactionsCurrentInfo.m_startTransaction);

        yield return new WaitUntil(() => { return m_currentBlockFetch == true; });
        Append("Block fetch... ");

        yield return new WaitUntil(() => { return m_transactionsFetch == true; });
        Append("Transactions fetch... ");

        Append("Setup the init condition to compute");
        string startingTransactionHash = m_initParameters.m_lotteryData.m_startTransactionId;
        string addressOfLotteryWallet = m_initParameters.m_lotteryData.m_lotteryAddress;
        WalletTransactionsHistoryRange transactionHistory = m_transactionsCurrentInfo.m_recoveredHistory;

        EthereumConverttion.TryParse(m_initParameters.m_lotteryData.m_minimumEntryETH, out string minWeiOfLottery, EtherType.Ether, EtherType.Wei);
        EthereumConverttion.TryParse(m_initParameters.m_lotteryData.m_endLotteryETH, out string weiEndOfLottery, EtherType.Ether, EtherType.Wei);
        
        
        ulong m_unixTimeStampUTCEndOfLottery =(ulong) m_initParameters.m_lotteryData.m_outOfTimeCondition.GetTimeStampInSecondsUTC();

        Append("Start computing the participants and ETH count");
        PublicLotteryTransactionTranslator.ComputeParticipants(
            addressOfLotteryWallet,
            startingTransactionHash,
            decimal.Parse(minWeiOfLottery),
            decimal.Parse(weiEndOfLottery),
            m_unixTimeStampUTCEndOfLottery, transactionHistory, out LF_TransactionsToParticipantAndEtherCount result,
            out LF_ParticipantsAndEtherCountExplainedLog participantsExplained);
        result.GetWeiCount(out decimal vWei, EtherType.Wei);
        result.GetWeiCount(out decimal vEther, EtherType.Ether);
        Append(string.Format("Stop computing participants({0}) and {1:0.0000} ETH ({2:0}) ", result.GetParticipantsCount() , vEther,vWei) );

        Append("Start setup init parms for winner computation");

        string[] participantsInJoinOrder = result.GetParticipantsListInOrder();
        string [] transactionsToWinner = result.
             GetTransactionHistoryUsed().
             GetTransactionHashRecentToPastCount(
               (uint)  m_initParameters.m_lotteryData.m_numberOfTransactionsUsedToComputeWinner)
             .Reverse().Select(k=>k.GetTransactionIdHash()).ToArray();


        if (participantsInJoinOrder.Length <= 0) { 
            Append("Not enough participants to compute. ");
            yield  break;
        }
        
        Append("Start computing winner");
        CryptoDontTrustVerifyWinnerAlogrithm.ComputeWinnerIndexOf(
            "", startingTransactionHash,
            participantsInJoinOrder.Length,
            transactionsToWinner, out uint winnerIndex, out LF_WinnerExplainedLog winnerLog);

        Append("End computing winner: "+ winnerIndex +"/"+ participantsInJoinOrder.Length);

        Append("--------------");
        Append("Start saving");
        m_outCurrentStateValue.m_result.SetInitialCondition(m_initParameters.m_lotteryData);
        m_outCurrentStateValue.m_result.SetComputedCondition(result , participantsExplained , winnerLog);

        //yield return new WaitForSeconds(1);
        //Computer Fetch Transaction of wallet affected


        //yield return new WaitForSeconds(1);
        //Computer Participants

        //yield return new WaitForSeconds(1);
        //Computer Winner Index

        {


            Append("--------------");
            Append("Start Displaying");
            m_uiRecap.Refresh();

            Append("--------------");
            Append("Start Saving as files details");
            m_fileStorage.SaveStateAtPersistanceFilePath();
            //m_fileStorage.OpenMarkdownFile();
            //LOG
            //m_outCurrentStateValue.m_participantsAndEtherComputationStackLog = m_transactionsToCurrentState.m_stackTrace;
            //m_outCurrentStateValue.m_transactionCompiledResult = m_transactionsToCurrentState.m_snapshotResult;

            //yield return new WaitForSeconds(1);
            //m_outCurrentStateValue.m_result.m_valideParticipantsInJoinOrderArray = m_transactionsToCurrentState.m_snapshotResult.m_participants;
            //m_outCurrentStateValue.m_result.m_currentStateLog.m_participantsCount = m_transactionsToCurrentState.m_snapshotResult.m_participants.Length;
            //Produce log of participants

            //yield return new WaitForSeconds(1);
            //Produce log of winner


            //yield return new WaitForSeconds(1);
            //Produce Manual action to do for the organizer
            // Winner, Funders with %

            //yield return new WaitForSeconds(1);
            //Produce Complementary log

            //yield return new WaitForSeconds(1);
            //Make a copy of the log folder to target folder

            //yield return new WaitForSeconds(1);
            //Produce a use readable log in markdown


            //yield return new WaitForSeconds(1);
            //Git Push the Log on the Git Server.
        }





        if (m_automaticPayement) { 
        
            //yield return new WaitForSeconds(1);
            //Send payement with infuria
        }

        m_stopProcessing.Invoke();

        m_isProcessing = false;
    }

    public bool IsProcessing() {
        return m_isProcessing;
    }


    public IEnumerator CreateStartTransactionWithInputMetaData() {
        yield return new WaitForSeconds(1);
        // Check that connection to the wallet with private key

        yield return new WaitForSeconds(1);
        // Check that connection to the wallet with private key

    }
}
