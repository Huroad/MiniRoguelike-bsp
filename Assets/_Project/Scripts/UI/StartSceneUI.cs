using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class StartSceneUI : MonoBehaviour
{
    public GameObject playStartObj;
    public GameObject gameDifficultyObj;

    void Start()
    {
        // 시작 시 패널 상태 (원하면 숨기기/보이기)
        playStartObj.SetActive(true);
        gameDifficultyObj.SetActive(false);
    }

    // Play 버튼에 연결
    public void OnClickPlay()
    {
        playStartObj.SetActive(false);
        gameDifficultyObj.SetActive(true);
    }
 
    // 난이도 버튼에 연결
    public void OnClickEasy()
    {
        GameManager.Instance.GetDifficultyData(1);
        SceneManager.LoadScene("LoadingScene");
    }

    public void OnClickNormal()
    {
        GameManager.Instance.GetDifficultyData(2);
        SceneManager.LoadScene("LoadingScene");
    }

    public void OnClickHard()
    {
        GameManager.Instance.GetDifficultyData(3);
        SceneManager.LoadScene("LoadingScene");
    }

    // Exit 버튼
    public void OnClickExit()
    {
        Application.Quit();
    }
}

