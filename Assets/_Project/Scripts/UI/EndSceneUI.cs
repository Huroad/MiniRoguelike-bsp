using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneUI : MonoBehaviour
{
    public GameObject endPanel;

    public void ShowEnding()
    {
        // endPanel.SetActive(true);
        SceneManager.LoadScene("EndScene");
        Cursor.visible = true;
    }

    public void OnRestartClicked()
    {
        SceneManager.LoadScene("StartScene");
        GameManager.Instance.isFight = false;
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
