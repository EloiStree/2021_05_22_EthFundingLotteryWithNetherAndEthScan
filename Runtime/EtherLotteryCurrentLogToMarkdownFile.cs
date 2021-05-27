using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EtherLotteryCurrentLogToMarkdownFile : MonoBehaviour
{

  

    public static string GetMarkdownReport(EhterumLotteryMassiveDetails fullDetails , string lotteryOwnerContactInformation, string lotteryOwnerInformationToUsers ,string lotteryOwnerHumanityDefense)
    {

       EtherServerTarget serverType= fullDetails.GetInitialLotteryParameters().GetServerTargetType();

        LF_WinnerExplainedLog trace =
        fullDetails.m_winnerExplainedStackTrace;

        DateTime now = DateTime.UtcNow;
        StringBuilder log = new StringBuilder();
            log.Append("# Lottery result Log  \n\n");
        log.Append("## Current Result  \n\n");
        fullDetails.GetWinner(out bool contestFinished, out string winAddress);
        string fundAddress = fullDetails.GetFundAddress();
         //  log.Append(string.Format("\n {0} Winner:{1} ", contestFinished ? "Final" : "Current", address));
        decimal weiAmount;
            double pourcentEthEstimation;
        fullDetails.GetTotalAmount(out weiAmount, out pourcentEthEstimation);
        log.Append(string.Format("\n Total: {0:0.0000} ETH ({1})  ",  pourcentEthEstimation, weiAmount));
        fullDetails.GetWinnerAmount(out weiAmount, out pourcentEthEstimation);
        log.Append(string.Format("\n- Winner: {0:0.0000} ETH ({1})  -to-> [{2}]({3})", pourcentEthEstimation, weiAmount, winAddress, EthScanUrl.GetWalletUrl(serverType, winAddress)));
        fullDetails.GetFundingAmount(out weiAmount, out pourcentEthEstimation);
        log.Append(string.Format("\n- Funding: {0:0.0000} ETH ({1})  -to-> [{2}]({3})", pourcentEthEstimation, weiAmount, fundAddress, EthScanUrl.GetWalletUrl(serverType, fundAddress)));
        fullDetails.IsLotteryFinished(out bool byAmount, out bool byTime, out bool isFinished);
        log.Append("\n  \n**Is finish ?**  ");
        if (!isFinished) {
            long timeInSecondsLeft= fullDetails.GetSecondsLeftBeforeEndOfLottery();
            string measurement = "day";
            double t = timeInSecondsLeft / (3600.0 * 24.0);
            if (t < 1.0)
            {
                measurement = "hour";
                t = timeInSecondsLeft / (3600.0);
            }
            if (t < 1.0)
            {
                measurement = "minute";
                t = timeInSecondsLeft / (60.0);
            }
            if (t < 1.0)
            {
                measurement = "s";
                t = timeInSecondsLeft ;
            }

            log.Append(string.Format("\n Still in process: {0:0.00} ({2}) {1}  ",t, fullDetails.GetDateAsString(), measurement) );
        }
        else if (byAmount)
        { 
        log.Append("\n Is finish by amount reach  ");
        }
        else if (byTime)
        { 
        log.Append("\n Is finish by timeout reach  ");
        }


        log.Append("\n  ");


        log.Append("Date: " + now.ToString() + "  \n");
            log.Append("## Initial params  \n");
            log.Append(string.Format(
            "\n- Title Hash:{0}  \n- Init Transaction Hash: {1}  \n- Used Transactions Count: {2}  \n",
            trace.m_title, trace.m_startTransaction, trace.m_transactionHash.Count));

        string logTmp = "";
        string previousHash = "";
            log.Append("## Compute the Winner Hash \n\n");
            log.Append(string.Format("\n- Convert Title To Hash  {0} > H256 > {1}  ", trace.m_title, trace.m_titleHash256));
            log.Append(string.Format("\n- 'Title Hash' + 'Transaction Start Hash' to an 'initial hash'  " +
                "\n- {0}  Append {1}  > H256> {2}  ",
                trace.m_titleHash256, trace.m_startTransaction, trace.m_titleHash256AppendTransactionHash));
       
            log.Append(string.Format("\n\n Previous Hash | Append | Transaction ID | H256 | New Hash  "));
        
            log.Append(string.Format("\n --- | --- | --- | --- | ---  \n"));

        for (int i = 0; i < trace.m_transactionHash.Count; i++)
        {
            log.Append(string.Format("{0} | + | {1} | > | [{2}]({3})  \n",
                trace.m_transactionHash[i].m_initHash,
                trace.m_transactionHash[i].m_transactionHash,
                trace.m_transactionHash[i].m_newHash,
                SHA256UrlChecker.GetUrl(trace.m_transactionHash[i].m_initHash+trace.m_transactionHash[i].m_transactionHash)
                )) ;
        }


            log.Append("### Hash to Winner \n\n");
            log.Append(string.Format("\n**If the lottery would finish now, the winner is... ?**  "));

            log.Append(string.Format("\nHash representing the winner: {0}  ", trace.m_finalHash));
            log.Append(string.Format("\n**ash as Char array**: {0}  ", trace.m_finalHashAsChars));
            log.Append(string.Format("\n**Hash as Int array**: {0}  ", trace.m_finalHashAsNumbers));
            log.Append(string.Format("\n**Hash as an number**: {0}  ", trace.m_finalHashAsNumber));
            log.Append(string.Format("\n**Number % Participants**:  {0} % {1}={2}  ", trace.m_finalHashAsNumber, trace.m_participantNumber, trace.m_winnerIndex));
        
        fullDetails.GetWinnerHashInformation(out string winHash, out int winnerIndex, out string winnerAddress);
        log.Append(string.Format("\n> **Winner is the index {0}**:  {1}  ", winnerIndex, winnerAddress));
        log.Append(string.Format("\n> **See the compute participants array for more information**  "));
        log.Append(string.Format("\n  "));
        log.Append(string.Format("\n  \n **Reminder**: The following array of participants is based on the transaction history:" +
                "\n - Ordered by latest transaction to new one that are over the minimum entry value.  " +
                "\n - From the transaction {0} to now {1}  " +
                "\nThe list is not random in aim to let anyone to verify/compute the result of this lottery based on their own code.  " +
                "\nThat basically the main purpose of this log.  \n\n" +
                "\nThis lottery is a 'fair' one. So it don't matter how much you put or how many transaction to inject. One address = one ticket. (See GitHub doc for more information).  ", trace.m_startTransaction, now));



        log.Append("\n\n\n\n");
        log.Append("\n## PARTICIPANTS (ordered):  ");

        string[] participants = fullDetails.GetParticipantsAddressesInOrder();
        for (int i = 0; i < participants.Length; i++)
        {

            log.Append(string.Format("\n{0}: [{1}]({2})  ", i, participants[i],
                EthScanUrl.GetWalletUrl(serverType, participants[i])));

        }
        log.Append("\n\n\n\n");
        log.Append("\n## PARTICIPANTS EXPLAINED:  ");

        log.Append(string.Format("\n\n Index | Total | Explaination | Transaction Info  "));
        log.Append(string.Format("\n --- | --- | --- | ---  "));
        LotteryFundingStackTrace[] explainLog = fullDetails.m_lotteryStateExplainedBasedOnTransactionGiven.m_stackTrace.ToArray() ;
        for (int i = 0; i < explainLog.Length; i++)
        {

            log.Append(string.Format("\n{0} | {1:0.00000} ETH | {2} | [{3}]({4})  ",
                i,
                explainLog[i].GetValueAsETH(),
                explainLog[i].GetExplaination(),
                explainLog[i].GetOneLiner(':'),
                EthScanUrl.GetTransactionUrl(serverType, explainLog[i].GetTransactionHash()))) ;

        }
        log.Append("\n\n");
            log.Append("\n## ETHER TRANSACTION ID START TO NOW (ordered):  ");

            string[] usedTransactionInOrder = fullDetails.GetTransactionsHashInOrder();
            for (int i = 0; i < usedTransactionInOrder.Length; i++)
            {
                log.Append(string.Format("\n{0}: [{1}]({2})  ", i, usedTransactionInOrder[i],
                    EthScanUrl.GetTransactionUrl(serverType, usedTransactionInOrder[i])));
            }
            
                log.Append("\n\n\n\n");
                log.Append("\n-----------------------------\n");
                log.Append("\n## Ether Funding Developer   ");
                log.Append(string.Format("\nThanks for participating to this lottery.  "));
                log.Append(string.Format("\nFind more on the source project here: https://github.com/EloiStree/2021_05_22_EtherFundingLottery  "));
                log.Append(string.Format("\nDisclaimer: I created this code for the fun and curiosity. "));
                log.Append(string.Format("\nDisclaimer: All responsability is on the user of my this code.  "));
                log.Append(string.Format("\nDisclaimer: People can scam you by not sending the money at the end of the lottery. Make sure to background check the organiser of the lottery before participating."));
                log.Append(string.Format("\nDisclaimer: People can fork this project. I am not responsible of code that is from a fork."));
                log.Append(string.Format("\n  \nA developer that created this code for the good of humanity and , mainly, curiosity.    "));
                log.Append(string.Format("\n  \nKind regards,  \nEloi S.  "));
           
                log.Append("\n\n\n\n");
                log.Append("\n-----------------------------\n");
                log.Append("\n## Organizer information  \n");
                log.Append("\nGovernment HATE lottery, I don't expect organizer to let's contact information or comment...   ");
                log.Append("\nBut if they do, here is what they are providing...  ");
                log.Append("\n### Contact information\n");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(lotteryOwnerContactInformation))
                {
                    log.Append("\nNot specified  ");
                }
                else
                {
                    log.Append(lotteryOwnerContactInformation);
                }
                log.Append("\n```  \n");

                log.Append("\n### Communication to participants\n  ");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(lotteryOwnerInformationToUsers))
                {
                    log.Append("\nNot specified  "); 
                }
                else
                {
                    log.Append(lotteryOwnerInformationToUsers);
                }
                log.Append("\n```  \n");
                log.Append("\n### Humanity defense\n  ");
                log.Append("\n*The aim of this project is to fund and help people/association by doing easy to set public lottery.*  ");
                log.Append("\nThe following is the reason why the organizer had set up this lottery...  ");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(lotteryOwnerHumanityDefense))
                {
                    log.Append("\nNot specified  ");
                }
                else
                {
                    log.Append(lotteryOwnerHumanityDefense);
                }
                log.Append("\n```  \n");
            
            return log.ToString();
        }
    }
