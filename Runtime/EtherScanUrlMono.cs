using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherScanUrlMono : MonoBehaviour
{
    public static void OpenUrl(string url) {
        Application.OpenURL(url);
    }

    public void OpenEtherScanOrg() {
        OpenUrl("https://etherscan.io/"); 
    }
    public void OpenRopstenEtherScanOrg()
    {
        OpenUrl("https://ropsten.etherscan.io/");
    }
    public void OpenRopstenFreeEther()
    {
        OpenUrl("https://faucet.ropsten.be/");
    }

    


}
