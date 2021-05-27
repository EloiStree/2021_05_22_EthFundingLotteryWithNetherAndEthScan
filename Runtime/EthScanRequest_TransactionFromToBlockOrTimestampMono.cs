using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EthScanRequest_TransactionFromToBlockOrTimestampMono : MonoBehaviour
{


    public string m_startTransaction;
    public string m_targetAddess;
    public PublicRequestKeyAPIAbstract m_apiToken;
    public Experiment_EtherRequestAntiSpamAPI m_antiSpam;

    public UnityEvent m_walletHistoryFetchEvent;

    [Header("Fetched Temp")]
    public string m_startBlock;
    public string m_currentBlock;
    public WalletTransactionsHistoryRange m_recoveredHistory = new WalletTransactionsHistoryRange();

    [Header("Request")]
    public EthScanRequest_GetJsonTransactionInformation m_getTransactionInformation;
    public EthScanRequest_GetBlockNumberByTimestamp m_getBlockAt;
    public EthScanRequest_GetWalletTransaction m_getTransactionAt;


    private void Awake()
    {
        m_getTransactionInformation.AddListener(NewTransactionInformation);
        m_getBlockAt.AddListener(NewCurrentBlockInformation);
        m_getTransactionAt.AddListener(NewTransactionImported);
    }

    
    [ContextMenu("Fetch Transaction Start Block")]
    public void FetchStartTransactionBlock()
    {
        m_getTransactionInformation.SetTranscation(m_apiToken.GetKey(), m_startTransaction);
        m_antiSpam.AddRequest(m_getTransactionInformation);


    }
    [ContextMenu("Fetch Current Block")]
    public void FetchBlockNumberFromTimestamp()
    {
        m_getBlockAt.LookForBlockNumberBefore(m_apiToken.GetKey(), -20);
        m_antiSpam.AddRequest(m_getBlockAt);


    }

    [ContextMenu("Fetch Transactions")]
    public void FetchAllTransactionFromWithValue()
    {
        m_getTransactionAt.SetWith(m_apiToken.GetKey(), m_targetAddess, m_startBlock, m_currentBlock);
        m_antiSpam.AddRequest(m_getTransactionAt);



    }
    public void FetchAllTransactionFrom(string address, string startBlock, string endBlock)
    {
        m_getTransactionAt.SetWith(m_apiToken.GetKey(), m_targetAddess, startBlock, endBlock);
        m_antiSpam.AddRequest(m_getTransactionAt);



    }

    public void FetchFollowingTransactionTo(string startTransaction)
    {
        m_startTransaction = startTransaction;
        FetchStartTransactionBlock();
    }

    private void NewTransactionInformation(PublicRestRequest arg0)
    {
        EthScanRequest_GetJsonTransactionInformation transactions = arg0 as EthScanRequest_GetJsonTransactionInformation;
        if (transactions != null)
        {

            transactions.GetBlockNumber(out bool found, out uint blocknumber);
            m_startBlock = string.Format("{0:0}", blocknumber);
            if (found)
            {
                FetchBlockNumberFromTimestamp();
            }
            else m_errorHappened = "Did not converted info to block number";


        }
        else m_errorHappened = "Did not succed to fetch number block of the transaction";
    }
    private string m_errorHappened;
    private void NewCurrentBlockInformation(PublicRestRequest arg0)
    {
        EthScanRequest_GetBlockNumberByTimestamp blocks = arg0 as EthScanRequest_GetBlockNumberByTimestamp;
         if (blocks != null)
        {
            ulong block = blocks.GetBlockNumber();
            m_currentBlock = block.ToString();
            if (block > 0)
            {
                FetchAllTransactionFromWithValue();
            }
            else m_errorHappened = "Current block not fetch.";
        }
        else m_errorHappened = "Block timestamp not reached";

    }
    private void NewTransactionImported(PublicRestRequest arg0)
    {
        EthScanRequest_GetWalletTransaction transactions = arg0 as EthScanRequest_GetWalletTransaction;
        if (transactions != null)
        {
            if (transactions.HasError() || !transactions.HasText())
            {
                m_errorHappened = "Did not fetch transactions";
            }
            else { 

                transactions.GetTransactionInformation(out string usedStartBlock, out string usedEndBlock, out TransactionFullInfoBean[] transactionsInfo);
                m_recoveredHistory.SetWith(transactions.GetStartBlock(), transactions.GetEndBlock(), transactionsInfo);
                m_walletHistoryFetchEvent.Invoke();
            }
        }
        else m_errorHappened = "Did not succed to fetch transactions";


    }


}
