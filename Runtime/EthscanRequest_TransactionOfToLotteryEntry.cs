using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EthscanRequest_TransactionOfToLotteryEntry : MonoBehaviour
{

    public string m_transactionStart = "e0a4ea92c797b6423c3e3203645693801f40d92322c605814ab550e8fd935800";
    public string m_startBlock = "12489560";
    public string m_endBlock = "999999999";
    public string m_etherScanAPI= "DUH7HG8BV9A2C3M7K991ECDPD5IMCMIVWG"; // PUblic API Ether Scan that will be deleted later.
    public string m_etherScanRealAddress= "0x58dd0d88950e834741ef1f4a192fc054aba51c12"; // Random User of thereum with lot's of activity
    public string m_etherScanRapsten = "0x1881f2e4d56Fa99369fDf7ad885774DA1C912855"; // my tesst account;
    public Experiment_EtherRequestAntiSpamAPI m_antiSpamRequest;
    public EthScanRequest_GetWalletTransaction m_transactions;
    public ComputeEntry m_lotteryComputeEntry;

    public CSVTransaction m_fetched = new CSVTransaction();
    [System.Serializable]
    public class CSVTransaction
    {
        [TextArea(0, 5)]
        public string m_csvTransaction = "";
    }

    [ContextMenu("Fetch Info")]
    public void FetchInfo() {
       
        m_transactions = new EthScanRequest_GetWalletTransaction(m_etherScanAPI, EthScanUrl.IsRequestingRapsten() ? m_etherScanRapsten: m_etherScanRealAddress, m_startBlock, m_endBlock);
        m_transactions.AddListener(ConvertToCSVTransaciton);
        m_antiSpamRequest.AddRequest(m_transactions);
    }

    
    private void ConvertToCSVTransaciton(PublicRestRequest arg0)
    {
        EthScanRequest_GetWalletTransaction transactions = arg0 as EthScanRequest_GetWalletTransaction;
        if (transactions != null) {
            transactions.GetTransactionInformatoin(out List<EthScanRequest_GetWalletTransaction.Json_Transaction> trans);
            List<Transaction> t= new List<Transaction>();
            trans.Reverse();
            foreach (var item in trans)
            {
                if ( item.GetHash().IndexOf(m_transactionStart) >= 0)
                    break;
                t.Add(new Transaction(item.GetHash(), item.GetFromWallet(), item.GetToWallet(), string.Format("{0:0}",  item.GetValueInWei())));
               
            }
            m_lotteryComputeEntry.SetEntry(t, true);
        }

        
    }
}
