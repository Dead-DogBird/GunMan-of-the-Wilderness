using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class IntroText : MonoBehaviour
{
    [SerializeField] private Text _text;

    [SerializeField] private string nextScene;
    [SerializeField] private string[] texts =
    {
        "모든 문명이 동시에 특이점을 뛰어넘고 폭발적인 역인과를 통해\n" +
        "한없이 과거부터 미래까지 문명의 기술이 치솟게 되고",
        "한 없이 풍요로운 기간을 지나고 지나",
        "온 우주가 마침내 열적, 양자적, 상대적으로 골골대며 죽어가며\n" +
        "아공간의 아공간을 열며 연명을 하고 살아갈 때",
        "반 쯤 멸망해버린 행성에서 우주를 향한\n" +
        "최후의 선발대가 되려는 총잡이들이 있었다."
    };

    private bool isDone=false;
    // Start is called before the first frame update
    void Start()
    {
        _text.color = Color.clear;
        TextTask().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDone&&Input.anyKeyDown)
            LoadingSceneManager.LoadScene(nextScene);
    }

    async UniTaskVoid TextTask()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            FadeText(false).Forget();
            await UniTask.WaitUntil(() => isFadeDone);
            _text.text = texts[i];
            FadeText().Forget();
            await UniTask.WaitUntil(() => isFadeDone);
            for (int j = 0; j < 40; j++)
            {
                if (!_text) return;
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }
        isDone = true;
    }

    private bool isFadeDone = false;
    async UniTaskVoid FadeText(bool var = true)
    {
        isFadeDone = false;
        if(!var)
        {
            while((_text.color.a-0.01f).Abs()>0.05f)
            {
                if (!_text) return;
                _text.color += (new Color(1f, 1f, 1f, 0f)-_text.color)*(Time.unscaledDeltaTime*5);
                await UniTask.Yield(PlayerLoopTiming.LastUpdate);
            }
        }
        else
        {
            while ((_text.color.a - 1).Abs() > 0.05f)
            {
                if (!_text) return;
                _text.color += (new Color(1f, 1f, 1f, 1f) - _text.color) * (Time.unscaledDeltaTime * 5);
                await UniTask.Yield(PlayerLoopTiming.LastUpdate);
            }
        }

        isFadeDone = true;
    }

}
