using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Image progressBar;

    private void Start()
    {
        LoadScene().Forget();
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("4.LoadSecene");
        Debug.Log("로딩씬 호출 됨.");
    }

    public static string NowSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    async UniTaskVoid LoadScene()
    {
        progressBar.fillAmount = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount += (op.progress - progressBar.fillAmount) * (Time.unscaledDeltaTime * 10);
            }
            else
            {
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.LastUpdate);
        }
        progressBar.fillAmount = 0.9f;
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        op.allowSceneActivation = true;
    }
}
