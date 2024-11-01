using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    public void pauseGame()
    {
        Time.timeScale = 0;
    }
}
