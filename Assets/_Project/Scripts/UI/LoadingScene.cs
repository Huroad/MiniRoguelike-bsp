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
        // 0.1초~1초 정도 일부러 대기(로딩 연출, 애니메이션 위해)
        yield return new WaitForSeconds(2f);

        // 실제로 비동기로 씬 로드 시작
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);

        // (옵션) 자동 전환 방지하고, 수동으로 넘어가게 할 수도 있음
        // op.allowSceneActivation = false;

        // 로딩 진행상황 표시
        while (!op.isDone)
        {
            if (loadingBar != null)
                loadingBar.value = op.progress;

            // (옵션) op.progress가 0.9에서 멈추고, 조건 만족시 allowSceneActivation=true로 전환 가능
            yield return null;
        }
    }
}
