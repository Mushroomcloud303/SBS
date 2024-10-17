using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESC : MonoBehaviour
{
    public void ReturnToGame()
    {
        GameManager.instance.EscPanel.SetActive(false);
        GameManager.instance.Resume();
    }

    public void Surrender()
    {
        gameObject.SetActive(false);
        GameManager.instance.Resume();
        GameManager.instance.GameOver();
    }

    public void ExitGame()
    {
        //게임 빌드를 종료
        Application.Quit();
    }
}
