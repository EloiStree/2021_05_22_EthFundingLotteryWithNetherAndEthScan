
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

    public void AddListener(object v)
    {
        throw new NotImplementedException();
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
    //{"jsonrpc":"2.0","id":1,"result":{
    //blockHash : "0x8aa1c512eedcb2cdbeb87e24d0b2d0570766ff93a5931f5649142d44de9c2a11"
//blockNumber : "0x9d8794"
//chainId : "0x3"
//condition : null
//creates : null
//from : "0x1881f2e4d56fa99369fdf7ad885774da1c912855"
//gas : "0x5208"
//gasPrice : "0xba43b7400"
//hash : "0xccc46a316ecb9623c30bc9921794a38d816a0f95c26d0d31cf944e13059f0142"
//input : "0x"
//nonce : "0x0"
//publicKey : "0x0505dea2a2587b41ef1d12d6460ef8ddd4933fa49993599e9928cd1547a118ef8367d9bc6b5a099d28c10331f3c1298d463ed817c4cba6b33030e1c248fd6c67"
//r : "0x26d49631a94634afaa4984a87f2793b197ab9068316109f644ddc8f0426523bd"
//raw : "0xf86480850ba43b74008252089460f6a0dc848ed1d0a27de73630efdf46a6a11039018029a026d49631a94634afaa4984a87f2793b197ab9068316109f644ddc8f0426523bda00c1e3c0ddafad32967cc3d700e93c1de1e4af7471fb207d2fb42e1196d99fa73"
//s : "0xc1e3c0ddafad32967cc3d700e93c1de1e4af7471fb207d2fb42e1196d99fa73"
//standardV : "0x0"
//to : "0x60f6a0dc848ed1d0a27de73630efdf46a6a11039"
//transactionIndex : "0x0"
//v : "0x29"
//value : "0x1"
//id : 1
//
    [System.Serializable]
    public class Json_TransactionInfo {
        //blockHash : "0xac1416f9044348d07b80638797e09a5ab3b24b049f246e628c531a75aa3869ec"
        public string blockHash;
        //blockNumber : "0xbed124"
        public string blockNumber;
        public uint GetBlockNumberAsInt() { return (uint) Convert.ToInt32(blockNumber, 16); }

        public string GetWeiAsDecimal()
        {
            return ( Convert.ToUInt64(value, 16)).ToString();
        }

        public ulong GetTimestamp()
        {
            throw new NotImplementedException();
        }

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







[System.Serializable]
public class EthScanRequest_GetJsonBlockInformation : PublicRestRequest
{
    //https://api.etherscan.io/api?module=account&action=txlistinternal&txhash=0x40eb908387324f2b575b4879cd9d7188f69c8fc9d87c901b9e2daaea4b442170&apikey=YourApiKeyToken

    public bool isConverted;
    public Json_Result result;
    public ulong m_blockId;
    public EthScanRequest_GetJsonBlockInformation(string apiToken, ulong blockId) : base(EthScanUrl.GetRawBlockInformation(apiToken, blockId))
    {
        m_blockId = blockId;
    }



    protected override void NotifyToChildrenAsChanged()
    {
        //{"status":"1","message":"OK","result":{"ethbtc":"0.06149","ethbtc_timestamp":"1620179532","ethusd":"3370.85","ethusd_timestamp":"1620179533"}}
        if (!HasError() && HasText())
        {
            result = JsonUtility.FromJson<Json_Result>(GetText());
            isConverted = true;
            // m_currentBlockNumber = result.GetBlockNumberNow();
        }
        else isConverted = false;
    }


    internal void SetBlock(string apiToken, ulong blockId)
    {
        m_blockId = blockId;
        SetUrl(EthScanUrl.GetRawBlockInformation(apiToken, blockId));
    }

    public void GetBlockTimestamp(out bool found, out ulong getTimeStampInSeconds)
    {
        getTimeStampInSeconds = 0;
        if (isConverted && result != null && result.result != null)
        {
            found = true;
            getTimeStampInSeconds = result.result.GetTimestamp();
        }
        else found = false;
    }


    [System.Serializable]
    public class Json_Result
    {
        public string jsonrpc;
        public string id;
        public Json_BlockInfo result;

    }
   
    [System.Serializable]
    public class Json_BlockInfo
    {

        //timestamp : "0x55c9ea07"
        public string timestamp;

//author : "0x81ebad31e218aa6580b14ad0259d8bc35b890e05"
//difficulty : "0x6abf2db"
//extraData : "0xd583010502846765746885676f312e37856c696e7578"
//gasLimit : "0x47e7c4"
//gasUsed : "0x0"
//hash : "0xbe909c494615f0264ad3adf527a77d002825357310aac2748470e9a7309415e8"
//logsBloom : "0x00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
//miner : "0x81ebad31e218aa6580b14ad0259d8bc35b890e05"
//mixHash : "0xe1d01d87a9d0e73f66efc8c43b6fefe45437eae10c819d8c4cefe0c924aa04bc"
//nonce : "0x0d88e3e82f9d39a2"
//number : "0x10d4f"
//parentHash : "0xc7913a55ef1a8cb23a522f0e5ae4433616b1470c51f2a8c86b5a2149e5d1c15d"
//receiptsRoot : "0x56e81f171bcc55a6ff8345e692c0f86e5b48e01b996cadc001622fb5e363b421"
//sealFields
//sha3Uncles : "0x1dcc4de8dec75d7aab85b567b6ccd41ad312451b948a7413f0a142fd40d49347"
//size : "0x219"
//stateRoot : "0xb40cb1edc7c6d7b4d277be659040d754f6f32276fcb33c25308e40d56d6ceb82"
//timestamp : "0x583ee945"
//totalDifficulty : "0x67beb9c3ea3"
//transactions
//transactionsRoot : "0x56e81f171bcc55a6ff8345e692c0f86e5b48e01b996cadc001622fb5e363b421"
//
        public ulong GetTimestamp()
        {
            return (Convert.ToUInt64(timestamp, 16));
        }
     




    }
}