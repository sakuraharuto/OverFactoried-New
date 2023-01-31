using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPrinter : MonoBehaviour
{
    public void PrintFireLogA() => GameManager.Instance._infoBoard.text = $"Fires involve solid materials";
    public void PrintFireLogB() => GameManager.Instance._infoBoard.text = $"Fires involve liquids";
    public void PrintFireLogC() => GameManager.Instance._infoBoard.text = $"Fires involve flammable gases";
    public void PrintFireLogD() => GameManager.Instance._infoBoard.text = $"Fires involve metals";
    public void PrintFireLogE() => GameManager.Instance._infoBoard.text = $"Fires involve live electrical equipment";
    public void PrintFireLogF() => GameManager.Instance._infoBoard.text = $"Fires involve cooking oils and fats";
    
    public void FireAMatched() => GameManager.Instance._infoBoard.text = $"Fires A(solid materials) can be extinguished by water, foam, powder, wet chemical";
    public void FireBMatched() => GameManager.Instance._infoBoard.text = $"Fires B(liquids) can be extinguished by foam, powder, CO2";
    public void FireCMatched() => GameManager.Instance._infoBoard.text = $"Fires C(flammable gases) can be extinguished by powder";
    public void FireDMatched() => GameManager.Instance._infoBoard.text = $"Fires D(metals) can be extinguished by powder";
    public void FireEMatched() => GameManager.Instance._infoBoard.text = $"Fires E(live electrical equipment) can be extinguished by powder, CO2";
    public void FireFMatched() => GameManager.Instance._infoBoard.text = $"Fires F(cooking oils and fats) can be extinguished by wet chemical";
    
}
