using JetBrains.Annotations;
using Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseCanvas : UICanvas
{
    public GamePlayCanvas gamePlayCanvas;
    public void RetryBtn()
    {
        Time.timeScale = 1;
        UIManager.Instance.CloseUI<LoseCanvas>(1.2f);
        StartCoroutine(Player.Instance.LoadFirtSence());
        StartCoroutine(ResetTime());
        
    }
    IEnumerator ResetTime()
    {
        yield return new WaitForSeconds(1f);
   
        gamePlayCanvas.turnTimer = gamePlayCanvas.turnDuration;
    }
    
}
