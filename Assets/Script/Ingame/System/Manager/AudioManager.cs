using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private AudioSource[] SfxSource;

    [SerializeField] private AudioClip[] BgmClip;
    [SerializeField] private AudioClip[] SfxClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    AudioSource GetEmptySource()
    {
        int fastindex = 0;
        float lageProgress = 0;
        for (int i = 0; i < SfxSource.Length; i++)
        {
            if (!SfxSource[i].isPlaying)
            {
                return SfxSource[i];
            }
            float progress = SfxSource[i].time / SfxSource[i].clip.length;
            if (progress > lageProgress && !SfxSource[i].loop)
            {
                fastindex = i;
                lageProgress = progress;
            }
        }
        return SfxSource[fastindex];
    }
    public void PlaySFX(int index, bool loop = false, float volume = 0.5f ,float pitch = 1)//효과음 재생
    {
        AudioSource a = GetEmptySource();
        a.loop = loop;
        a.pitch = pitch;
        a.volume = volume;
        a.clip = SfxClip[index];
        a.Play();
    }
    public void PlayBgm(int index)
    {
        BgmSource.Stop();
        BgmSource.clip = BgmClip[index];
    }

    public void PlaySfXDelayed(int index,float delaytime,bool loop = false, float volume = 0.5f, float pitch = 1)
    {
        PlaySfxTask(index, delaytime, loop, volume, pitch).Forget();
    }

    async UniTaskVoid PlaySfxTask(int index,float delaytime,bool loop = false, float volume = 0.5f, float pitch = 1)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delaytime), true);
        PlaySFX(index, loop, volume, pitch);
    }
    public void SetMusicPitch(float var,bool isSmooth = false)
    {
        if (!isSmooth)
        {
            BgmSource.pitch = var;
            return;
        }
        MusicPitchTask(var).Forget();
    }

    async UniTaskVoid MusicPitchTask(float var)
    {
        while ((var-BgmSource.pitch).Abs() > 0.01f)
        {
            BgmSource.pitch += (var-BgmSource.pitch)/10;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledTime), true);
        }
        BgmSource.pitch = var;
    }
}
