using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EtherLotteryCurrentLogToMarkdownFile : MonoBehaviour
{






    public string  GetMarkdownReport(EthereumLotteryDetails fullDetails , string m_lotteryOwnerContactInformation, string m_lotteryOwnerInformationToUsers ,string m_lotteryOwnerHumanityDefense)
    {
        
        WinnerHashStackTrace trace =
        fullDetails.m_currentStateLog.m_winnerExplainedStackTrace;

        DateTime now = DateTime.Now;
        StringBuilder log = new StringBuilder();
            log.Append("# Lottery result Log  \n\n");
            log.Append("Date: " + now.ToString() + "  \n");
            log.Append("## Initial params  \n\n");
            log.Append(string.Format(
            "- Title Hash:{0}  \n - Init Transaction Hash: {1}  \n - Participants Count: {2}  \n - Used Transactions Count: {3}  \n",
            trace.m_title, trace.m_transactionHash, trace.m_participantNumber, trace.m_transactionHash.Count));

        string logTmp = "";
        string previousHash = "";
            log.Append("## Compute the Winner Hash \n\n");
            log.Append(string.Format("\n> Convert Title To Hash  \n {0} > H256 > {1}  \n ", trace.m_title, trace.m_titleHash256));
            log.Append(string.Format("\n> 'Title Hash' + 'Transaction Start Hash' to an 'initial hash'  \n{0}  \n\t Append {1}  \n\t\t  > H256> {2}  \n ",
                trace.m_titleHash256, trace.m_transactionHash, trace.m_titleHash256AppendTransactionHash));
       
            log.Append(string.Format("\n\n Previous Hash | Append | Transaction ID | H256 | New Hash  "));
        
            log.Append(string.Format("\n --- | --- | --- | --- | ---  \n"));

        for (int i = 0; i < trace.m_transactionHash.Count; i++)
        {
                log.Append(string.Format("{0} | + | {1} | > | {2}  \n", 
                    trace.m_transactionHash[i].m_initHash,
                    trace.m_transactionHash[i].m_transactionHash,
                    trace.m_transactionHash[i].m_newHash));
        }


            log.Append("## Hash to Winner \n\n");
            log.Append(string.Format("\nIf the lottery would finish now, the winner is... ?  "));

            log.Append(string.Format("\nHash representing the winner: {0}  ", trace.m_finalHash));
      
            log.Append(string.Format("\n**ash as Char array**: {0}  ", trace.m_finalHashAsChars));
            log.Append(string.Format("\n**Hash as Int array**: {0}  ", trace.m_finalHashAsNumbers));
            log.Append(string.Format("\n**Hash as an number**: {0}  ", trace.m_finalHashAsNumber));
            log.Append(string.Format("\n**Number % Participants**:  {0}%{1}={2}  ", trace.m_finalHashAsNumbers, trace.m_participantNumber, trace.m_winnerIndex));
        log.Append(string.Format("\n> **Winner is the index {0}**:  {1}  ", trace.m_winnerIndex, fullDetails.m_currentStateLog.m_winnerAddress));
        log.Append(string.Format("\n> **See the compute participants array for more information**  ")); 
        log.Append(string.Format("\n  \n **Reminder**: The following array of participants is based on the transaction history:" +
                "\n - Ordered by latest transaction to new one that are over the minimum entry value.  " +
                "\n - From the transaction {0} to now {1}  " +
                "\nThe list is not random in aim to let anyone to verify/compute the result of this lottery based on their own code.  " +
                "\nThat basically the main purpose of this log.  \n\n" +
                "\nThis lottery is a 'fair' one. So it don't matter how much you put or how many transaction to inject. One address = one ticket. (See GitHub doc for more information).  ", trace.m_startTransaction, now));



            log.Append("\n\n\n\n");
            log.Append("\n## PARTICIPANTS (ordered):  ");

            for (int i = 0; i < fullDetails.m_valideParticipantsInJoinOrderArray.Length; i++)
            {

                log.Append(string.Format("\n{0}: {1}  ", i, fullDetails.m_valideParticipantsInJoinOrderArray[i]));

            }
            log.Append("\n\n");
            log.Append("\n## ETHER TRANSACTION ID USED (ordered):  ");

            string[] usedTransactionInOrder = fullDetails.m_currentStateLog.m_winnerExplainedStackTrace.GetTransactionsUsedInOrder();
            for (int i = 0; i < usedTransactionInOrder.Length; i++)
            {
                log.Append(string.Format("\n{0}: {1}  ", i, usedTransactionInOrder[i]));
            }
            
                log.Append("\n\n\n\n");
                log.Append("\n-----------------------------\n");
                log.Append("\n## Ether Funding Lottery Developer   ");
                log.Append(string.Format("\nThanks for participating to this lottery.  "));
                log.Append(string.Format("\nFind more on the source project here: https://github.com/EloiStree/2021_05_22_EtherFundingLottery  "));
                log.Append(string.Format("\nDisclaimer: I created this code for the fun and curiosity. I am not the one executing it.  "));
                log.Append(string.Format("\nDisclaimer: All responsability is on the user of my this code.  "));
                log.Append(string.Format("\nDisclaimer: I am not responsible of people modifying this code or scamming people with it. That is not design for that but can be use as such.  "));
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
                if (string.IsNullOrEmpty(m_lotteryOwnerContactInformation))
                {
                    log.Append("\nNot specified  ");
                }
                else
                {
                    log.Append(m_lotteryOwnerContactInformation);
                }
                log.Append("\n```  \n");

                log.Append("\n### Communication to participants\n  ");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(m_lotteryOwnerInformationToUsers))
                {
                    log.Append("\nNot specified  "); 
                }
                else
                {
                    log.Append(m_lotteryOwnerInformationToUsers);
                }
                log.Append("\n```  \n");
                log.Append("\n### Humanity defense\n  ");
                log.Append("\n*The aim of this project is to fund and help people/association by doing easy to set public lottery.*  ");
                log.Append("\nThe following is the reason why the organizer had set up this lottery...  ");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(m_lotteryOwnerHumanityDefense))
                {
                    log.Append("\nNot specified  ");
                }
                else
                {
                    log.Append(m_lotteryOwnerHumanityDefense);
                }
                log.Append("\n```  \n");
            
            return log.ToString();
        }
    }
