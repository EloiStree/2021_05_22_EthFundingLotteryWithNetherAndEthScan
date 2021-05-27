
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class EthScanRequest_GetJsonTransactionInformation : PublicRestRequest
{
    //https://api.etherscan.io/api?module=account&action=txlistinternal&txhash=0x40eb908387324f2b575b4879cd9d7188f69c8fc9d87c901b9e2daaea4b442170&apikey=YourApiKeyToken

    public bool isConverted;
    public Json_Result result;
    public string m_transactionHash;
    public EthScanRequest_GetJsonTransactionInformation(string apiToken, string transactionHash) : base(EthScanUrl.GetRawTransactionInformation(apiToken, transactionHash))
    {
        m_transactionHash = transactionHash;
    }



    protected override void NotifyToChildrenAsChanged()
    {
        //{"status":"1","message":"OK","result":{"ethbtc":"0.06149","ethbtc_timestamp":"1620179532","ethusd":"3370.85","ethusd_timestamp":"1620179533"}}
        if (!HasError() && HasText())
        {
            isConverted = true;
            result = JsonUtility.FromJson<Json_Result>(GetText());
           // m_currentBlockNumber = result.GetBlockNumberNow();
        }
        else isConverted = false;
    }

    internal void SetTranscation(string apiToken, string transactionHash)
    {
        m_transactionHash = transactionHash;
        SetUrl(EthScanUrl.GetRawTransactionInformation(apiToken, transactionHash));
    }

    public void GetBlockNumber(out bool found, out uint blockNumberOfFetchTransaction) {
        blockNumberOfFetchTransaction = 0;
        if (isConverted && result != null && result.result != null)
        {
            found = true;
            blockNumberOfFetchTransaction = result.result.GetBlockNumberAsInt();
        }
        else found = false;
    }


    [System.Serializable]
    public class Json_Result
    {
        public string jsonrpc;
        public string id;
        public Json_TransactionInfo result;
       
    }
    //{"jsonrpc":"2.0","id":1,"result":{"blockHash":"0xac1416f9044348d07b80638797e09a5ab3b24b049f246e628c531a75aa3869ec","blockNumber":"0xbed124","from":"0x5a0b54d5dc17e0aadc383d2db43b0a0d3e029c4c","gas":"0x186a0","gasPrice":"0x165a0bc00","hash":"0xab31bf0892f91842e2772097373c20023e1ed86aeee4180ddd10bf08c3eb5190","input":"0x","nonce":"0x5c6cc7","to":"0x8fd00f170fdf3772c5ebdcd90bf257316c69ba45","transactionIndex":"0x1","value":"0x4239abe5d575e9101","type":"0x0","v":"0x26","r":"0x3b4fc954f05d51194c892709783786af55a1b4e06dafb8e47f038a0ae500fb61","s":"0x12ec7f523833e0ece228daf107c00041d7370b722416a602aeb8d60452a79624"}}

    [System.Serializable]
    public class Json_TransactionInfo {
        //blockHash : "0xac1416f9044348d07b80638797e09a5ab3b24b049f246e628c531a75aa3869ec"
        public string blockHash;
        //blockNumber : "0xbed124"
        public string blockNumber;
        public uint GetBlockNumberAsInt() { return (uint) Convert.ToInt32(blockNumber, 16); }
        //from : "0x5a0b54d5dc17e0aadc383d2db43b0a0d3e029c4c"
        public string from;
        //gas : "0x186a0"
        public string gas;
        //gasPrice : "0x165a0bc00"
        public string gasPrice;
        //hash : "0xab31bf0892f91842e2772097373c20023e1ed86aeee4180ddd10bf08c3eb5190"
        public string hash;
        //input : "0x"
        public string input;
        //nonce : "0x5c6cc7"
        public string nonce;
        //to : "0x8fd00f170fdf3772c5ebdcd90bf257316c69ba45"
        public string to;
        //transactionIndex : "0x1"
        public string transactionIndex;
        //value : "0x4239abe5d575e9101"
        public string value;
        //type : "0x0"
        public string type;
        //v : "0x26"
        public string v;
        //r : "0x3b4fc954f05d51194c892709783786af55a1b4e06dafb8e47f038a0ae500fb61"
        public string r;
        //s : "0x12ec7f523833e0ece228daf107c00041d7370b722416a602aeb8d60452a79624"
        public string s;

    }
}