using Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCanvas : UICanvas
{
    // Start is called before the first frame update
    public void StartBtn()
    {

        UIManager.Instance.CloseUIDirectly<StartCanvas>();
        Time.timeScale = 1.0f;
    }
}
