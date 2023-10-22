using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

struct SfxSourceStruct
{
    public AudioSource _audioSource;
    public int _index;

    public SfxSourceStruct(AudioSource audio, int _pindex)
    {
        _audioSource = audio;
        _index = _pindex;
    }
}
public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource BgmSource;
    [SerializeField] private AudioSource[] SfxSource;

    [SerializeField] private AudioClip[] BgmClip;
    [SerializeField] private AudioClip[] SfxClip;

    private bool isGamemanagerStop = false;

    public float sfxVolume;
    public float musicVolume;
    // Start is called before the first frame update
    void Start()
    {
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume")*PlayerPrefs.GetFloat("MasterVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume")*PlayerPrefs.GetFloat("MasterVolume");
        BgmSource.volume = musicVolume;
    }
    // Update is called once per frame
    void Update()
    {
        if (!BgmSource.isPlaying&&!isGamemanagerStop)
        {
            if (GameManager.Instance.wolrd < 2)
            {
                BgmSource.clip = BgmClip[Random.Range(0, 6)];
            }
            else
            {
                BgmSource.clip = BgmClip[Random.Range(6, 8)];
            }
            BgmSource.Play();
        }
        
    }
    SfxSourceStruct GetEmptySource()
    {
        int fastindex = 0;
        float lageProgress = 0;
        for (int i = 0; i < SfxSource.Length; i++)
        {
            if (!SfxSource[i].isPlaying)
            {
                return new SfxSourceStruct(SfxSource[i],i);
            }
            float progress = SfxSource[i].time / SfxSource[i].clip.length;
            if (progress > lageProgress && !SfxSource[i].loop)
            {
                fastindex = i;
                lageProgress = progress;
            }
        }
        return new SfxSourceStruct(SfxSource[fastindex],fastindex);
    }
    public int PlaySFX(int index, bool loop = false, float volume = 0.5f ,float pitch = 1)//효과음 재생
    {
        SfxSourceStruct _temp = GetEmptySource(); 
        AudioSource a = _temp._audioSource;
        a.loop = loop;
        a.pitch = pitch;
        a.volume = volume*sfxVolume;
        a.clip = SfxClip[index];
        a.Play();
        return _temp._index;
    }
    public void PlaySFXOnce(int index, bool loop = false, float volume = 0.5f, float pitch = 1) //효과음 재생
    {
        for (int i = 0; i < SfxSource.Length; i++)
        {
            if (SfxSource[i].isPlaying&&SfxSource[i].clip == SfxClip[index])
            {
                return;
            }
        }
        PlaySFX(index, loop, volume, pitch);
    }

    public void StopLoopedSfx(int index)
    {
        SfxSource[index].Stop();
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

    public void StopMusic()
    {
        isGamemanagerStop = true;
        BgmSource.Stop();
        
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

    public void SetMusicVolume()
    {
        BgmSource.volume = musicVolume;
    }
}
