using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryStateLopperMono : MonoBehaviour
{

    public bool m_isLooperRequested;
    public LotteryFundingStateMachineChecker m_lotteryFundingChecker;
    public float m_checkStateInSecond = 10;
    IEnumerator Start()
    {
        while (true) {
            yield return new WaitForEndOfFrame();
            if (m_isLooperRequested) { 
                yield return new WaitForSeconds(m_checkStateInSecond);
                if (!m_lotteryFundingChecker.IsProcessing()) {

                    m_lotteryFundingChecker.ComputeTheWinnerAndHistoryOfLottery();
                }
            }
        
        }
    }

    public void SetAsLooping(bool isOn) {
        m_isLooperRequested = isOn;
    }

    public void SetTimeOfLoop(string timeInSecond)
    {

        float.TryParse(timeInSecond, out float s);
        SetTimeOfLoop(s);
    }
    public void SetTimeOfLoop(float timeInSecond)
    {
        if (timeInSecond <= 0.1f)
            timeInSecond = 0.1f;
        m_checkStateInSecond = timeInSecond;

    }


}
