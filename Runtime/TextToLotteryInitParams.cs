using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TextToLotteryInitParams : MonoBehaviour
{

    public UnityEvent m_importedEvent;
    public string m_urlIntegerToStandard = "https://raw.githubusercontent.com/EloiStree/EtherFundingLotteryStandard/main/Standard/IntegerToStandard.txt";
    public string[] m_intToXmlUrl;

    public EtherServerTarget m_targetServer;

    public Experiment_EtherRequestAntiSpamAPI m_antiSpam;
    public PrivateRequestKeyAPIAbstract m_etherScanKey;
    public EthScanRequest_GetJsonTransactionInformation m_transaction;
    public EthScanRequest_GetJsonBlockInformation m_blockFetch;

    public TransactionParams m_transactionOrigine;
    public StandardInitParams m_lastImported;


    public bool m_transactionFetch;
    public bool m_blockTimeFetch;
    public bool m_standardListFetch;
    public bool m_standardXmlFetch;
    public bool m_finished;
    public string m_error="";
    public Coroutine m_currentCoroutine;

    public void ResetToZero() {
        m_transactionFetch =
        m_blockTimeFetch =
        m_standardListFetch =
        m_standardXmlFetch =
        m_finished = false;
        m_error = "";
    }


    public void FetchAllInformationOfTransaction(string hash)
    {
        FetchAllInformationOfTransaction(hash, m_targetServer);
    }

    public void FetchAllInformationOfTransaction(string hash, EtherServerTarget server) {
        if (m_currentCoroutine != null)
            StopCoroutine(m_currentCoroutine);
        m_currentCoroutine = 
        StartCoroutine(CorotineFetchAllInformationOfTransaction(hash,server));
    }

    public EthereumLotteryInitialParametersMono m_toAffect;
    public UI_LotteryEntrySaveAndLoad m_uiToAffect;

    public IEnumerator CorotineFetchAllInformationOfTransaction(string hash, EtherServerTarget server) {
        ResetToZero();

        ImportFromTransactionHash(hash, server);
        yield return new WaitUntil(() => { return m_transactionFetch; });

        ImportFromBlockTimestap(m_transactionOrigine.m_blockId, server);
        yield return new WaitUntil(() => { return m_blockTimeFetch; });

        CheckThatStandardIndexDownloaded();

        yield return new WaitUntil(() => { return m_standardListFetch; });

        ulong.TryParse(m_transactionOrigine.m_weiIdAmount, out ulong wei);

        m_toAffect.m_lotteryData.m_serverTarget = server;
        m_toAffect.m_lotteryData.m_startTransactionId = hash;
        m_toAffect.m_lotteryData.m_lotteryAddress = m_transactionOrigine.m_origine;
        m_toAffect.m_lotteryData.m_fundingAddress = m_transactionOrigine.m_destination;

        if (wei < int.MaxValue) {

            GetXmlStandard((uint)wei, out string xmlTextStand);
            if (string.IsNullOrEmpty(xmlTextStand))
                yield return null;
            ImportFromText(xmlTextStand, out m_lastImported);

            EthereumConverttion.TryParse(m_lastImported.m_minWeiAmount, out string inieth, EtherType.Wei, EtherType.Ether);
            EthereumConverttion.TryParse(m_lastImported.m_goalWeiAmount, out string goalEth, EtherType.Wei, EtherType.Ether);

            m_toAffect.m_lotteryData.SetMinEntryEther(inieth);
            m_toAffect.m_lotteryData.SetGoalEther(goalEth);
            m_toAffect.m_lotteryData.m_numberOfTransactionsUsedToComputeWinner = m_lastImported.m_winnerHashTransactionCount;
            m_toAffect.m_lotteryData.m_pourcentToFunding = m_lastImported.m_pourcentFunding/100.0;
            m_toAffect.m_lotteryData.m_outOfTimeCondition.SetTimestampInSecondsUtc( m_transactionOrigine.m_timestampInSeconds +  m_lastImported.m_endbyTimeOutInsecondsDuration);
            m_uiToAffect.PushDataToUI();

        }




    }


    public void ImportFromBlockTimestap(string blockId, EtherServerTarget server)
    {
        ulong.TryParse(blockId, out ulong id);
        EthScanUrl.SetAsUsingRopsten(server == EtherServerTarget.Ropsten);
        m_blockFetch = new EthScanRequest_GetJsonBlockInformation(
        m_etherScanKey.GetKey(), id);
        m_blockFetch.AddListener(BlockDownloaded);
        m_antiSpam.AddRequest(m_blockFetch);

    }

    private void BlockDownloaded(PublicRestRequest arg0)
    {
        EthScanRequest_GetJsonBlockInformation block = arg0 as EthScanRequest_GetJsonBlockInformation;

        if (block != null && !block.HasError() && block.HasText())
        {
            m_transactionOrigine.m_timestampInSeconds = block.result.result.GetTimestamp();
            m_blockTimeFetch = true;
        }
    }

    public void ImportFromTransactionHash(string hash, EtherServerTarget server)
    {
        EthScanUrl.SetAsUsingRopsten(server == EtherServerTarget.Ropsten);
        m_transaction = new EthScanRequest_GetJsonTransactionInformation(
        m_etherScanKey.GetKey(), hash);
        m_transaction.AddListener(TransactionDownloaded);
        m_antiSpam.AddRequest(m_transaction);

    }

    private void TransactionDownloaded(PublicRestRequest arg0)
    {
        EthScanRequest_GetJsonTransactionInformation tran = arg0 as EthScanRequest_GetJsonTransactionInformation;

        if (tran != null && !tran.HasError() && tran.HasText()) {
            m_transactionOrigine = new TransactionParams();
            m_transactionOrigine.m_origine = tran.result.result.from;
            m_transactionOrigine.m_destination = tran.result.result.to;
            m_transactionOrigine.m_weiIdAmount = tran.result.result.GetWeiAsDecimal();
            m_transactionOrigine.m_blockId = tran.result.result.GetBlockNumberAsInt().ToString();
            m_transactionFetch = true;
        }

    }


    public void ImportFromInteger(int index, out StandardInitParams initParams) {
        initParams = null;
        if (index < 0) {
            return;
        }

        CheckThatStandardIndexDownloaded();
     
        if (m_intToXmlUrl != null && m_intToXmlUrl.Length >= index)
        {
            ImportFromUrl(m_intToXmlUrl[index], out initParams);

        }

    }
    public void GetXmlStandard(uint index, out string textXml)
    {
        GetUrlOfXmlStandard(index, out string url);

        if (url.Length > 0)
        {
            using (var client = new WebClient())
            {
                textXml = client.DownloadString(url);
            }
            m_standardXmlFetch = true;
        }
        else { 
            textXml = "";
            m_error = "XML not fetch";
        }

    }
    public void GetUrlOfXmlStandard(uint index, out string urlOfXml)
    {

        for (int i = 0; i < m_xmlStandardUrl.Count; i++)
        {
            if (m_xmlStandardUrl[i].m_integerId == index) {
                urlOfXml = m_xmlStandardUrl[i].m_xmlUrl;
                return;
            }
        }
        m_error = "Url of the xml standard not found";
        urlOfXml = "";
    }


    public List<IntToStandardUrl> m_xmlStandardUrl = new List<IntToStandardUrl>();
    [System.Serializable]
    public class IntToStandardUrl {
        public uint m_integerId=0;
        public string m_xmlUrl="";

        public IntToStandardUrl(uint integerId, string xmlUrl)
        {
            m_integerId = integerId;
            m_xmlUrl = xmlUrl;
        }
    }
    private void CheckThatStandardIndexDownloaded()
    {
        if (m_xmlStandardUrl.Count <= 0) { 
            using (var client = new WebClient())
            {
                m_xmlStandardUrl.Clear();
                var textFromFile = client.DownloadString(m_urlIntegerToStandard);
                string [] lines = textFromFile.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] tokens = lines[i].Split('|');
                    if (tokens.Length == 2) {
                        m_xmlStandardUrl.Add(new IntToStandardUrl(uint.Parse(tokens[0]), tokens[1]));
                    }
                }
            }
        }
        m_standardListFetch = true;

    }

    public string m_usedUrl;
    public void ImportFromUrl(string url , out StandardInitParams initParams)
    {
        using (var client = new WebClient())
        {
            var textFromFile =  client.DownloadString(url);
            ImportFromText(textFromFile, out initParams);
        }

    }
 
    public void ImportFromText(string text , out StandardInitParams initParams)
    {
        //Example https://raw.githubusercontent.com/EloiStree/EtherFundingLotteryStandard/main/Standard/2_BasicDonationLottery.xml
        TryToImportFromStandardXml( text, out bool wasParsed, out initParams);
        if (wasParsed)
            return;

    }

    private void TryToImportFromStandardXml( string text, out bool wasParsed, out StandardInitParams initParams)
    {
        initParams = new StandardInitParams();
        try
        {

            XDocument doc = XDocument.Parse(text.ToLower(), LoadOptions.PreserveWhitespace);
            XAttribute attribut;
            foreach (XElement element in doc.Descendants())
            {
                 if (element.Name.ToString().ToLower() == "id")
                {
                    attribut = element.Attribute("integerValue");
                    if (attribut != null)
                    {
                        uint.TryParse(attribut.Value, out uint id);
                        initParams.m_integerId = id;
                    }
                   
                }
                else if (element.Name.ToString().ToLower() == "minentry") {
                    attribut = element.Attribute("weiminentry");
                    initParams.m_minWeiAmount = attribut.Value;
                }
                else if (element.Name.ToString().ToLower() == "entrytype")
                {
                    attribut = element.Attribute("type");
                    if (attribut != null)
                    {
                        if (attribut.Value == "oneperaddress")
                            initParams.m_lotteryType = StandardInitParams.LotteryType.PerAddress;
                        if (attribut.Value == "oneperentry")
                            initParams.m_lotteryType = StandardInitParams.LotteryType.PerEntry;
                        if (attribut.Value == "oneperentryamount")
                            initParams.m_lotteryType = StandardInitParams.LotteryType.PerEntryAmount;
                    }
                }
                else if (element.Name.ToString().ToLower() == "endbyamount")
                {
                    attribut = element.Attribute("weiamount");
                    initParams.m_goalWeiAmount = attribut.Value;
                }
                else if (element.Name.ToString().ToLower() == "endbytimeout")
                {
                    attribut = element.Attribute("maxtimeinseconds");
                    if (attribut != null)
                    {
                        double.TryParse(attribut.Value, out double seconds);
                        initParams.m_endbyTimeOutInsecondsDuration = (ulong)seconds;
                    }
                    else { 
                        attribut = element.Attribute("maxtimeindays");
                        if (attribut != null) {

                            double.TryParse(attribut.Value, out double days);
                            initParams.m_endbyTimeOutInsecondsDuration = (ulong)(days*3600.0*24.0);
                        }
                    }
                }
                else if (element.Name.ToString().ToLower() == "winnerhash")
                {
                    attribut = element.Attribute("transactioncount");
                    if (attribut != null)
                    {
                        uint.TryParse(attribut.Value, out uint  count);
                        initParams.m_winnerHashTransactionCount = count;
                    }
                }
                else if (element.Name.ToString() == "funding")
                {
                    attribut = element.Attribute("pourcent");
                    if (attribut != null)
                    {
                        double.TryParse(attribut.Value, out double pourcent);
                        initParams.m_pourcentFunding = pourcent;
                    }
                }




            }
            wasParsed = true;
        }
        catch (Exception) { wasParsed = false; }
    }

    //[TextArea(0,5)]
    //public  string t_testXml;
    //private void OnValidate()
    //{
    //    ImportFromText(t_testXml, out m_lastImported);
    //}
}


[System.Serializable]
public class StandardInitParams
{
    public uint m_integerId;
    public string m_minWeiAmount;
    public string m_goalWeiAmount;
    public enum LotteryType { PerAddress, PerEntry, PerEntryAmount }
    public LotteryType m_lotteryType;
    public ulong m_endbyTimeOutInsecondsDuration;
    public uint m_winnerHashTransactionCount;
    public double m_pourcentFunding;

}
[System.Serializable]
public class TransactionParams
{
    public string m_blockId;
    public string m_weiIdAmount;
    public string m_origine;
    public string m_destination;
    public ulong m_timestampInSeconds;

}