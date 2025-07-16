using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 로딩바 쓸 경우

public class LoadingScene : MonoBehaviour
{
    public string nextSceneName = "PlayScene";
    public Slider loadingBar;

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        while (!op.isDone)
        {
            if (loadingBar != null)
                loadingBar.value = op.progress;
            yield return null;
        }
    }
}
