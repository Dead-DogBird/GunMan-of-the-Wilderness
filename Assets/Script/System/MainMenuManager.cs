using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenuManager : MonoSingleton<MainMenuManager>
{

    private AudioSource _audioSource;

    [SerializeField] private Canvas[] canvass;
    public int focusCanvas = 0;

    [SerializeField] private int focusImage = 0;
    [SerializeField] private Image[] _images;

    [SerializeField] private Text CharNameText;
    [SerializeField] private Text CharInfoText;
    [SerializeField] private Text CharUltText;
    [SerializeField] private Text CharUltInfoText;
    
    [SerializeField] private Text CharPassiveText;
    [SerializeField] private Text CharPassiveInfoText;

    [SerializeField] private GameObject SelectButton;
    [SerializeField] private Image BlackImage;

    [SerializeField] private Slider musicSlider, sfxSlider, masterSlider;
    
    string[]charname = { "라스트 건맨","디†에고","골드테이커","별:바람","Sn1-p3r","헤비레인"};
    string[]charinfo = { "밸런스 잡힌 리볼버 입니다.","근거리에서 파괴적인 샷건입니다.",
        "빠른 연사를 자랑하는 돌격소총입니다.","높은 정확도와 준수한 공격력을 가진 반자동 소총입니다.","강력한 저격총입니다. 신중한 발사를 요합니다","근거리에서 막강한 파괴력을 자랑하는 개틀링건 입니다." };

    private string[] ultname = { "추적 미사일","알타 데 라 무에르테","Subspace Raser ","천둥새:바람","시퀀스-강습저격","요격터렛"};
    string[]ultinfo = { "적을 추적하는 미사일을 발사합니다.","캐릭터의 양 옆으로 제단을 내리 꽂아 공격합니다.","커서의 위치에 강력한 피해를 입히는 레이저를 아공간에서 발사합니다.",
        "적을 자동으로 공격하는 토템을 설치합니다.","총이 강화 됩니다. 우클릭을 하여 정조준을 할 수 있습니다.","적 혹은 적의 총알을 요격하는 터렛을 소환합니다." };
    
    private string[] passive = { "One In the Chamber","단자 데 라 무에르테 ","Gold Rush","산들:바람","시퀀스-손상복구","고철재활용" };
    string[]passiveInfo = { "첫번째와 마지막 총알이 강화됩니다.","1회 부활 합니다.","일정 확률로 더 많은 재화를 획득합니다.",
        "이동속도가 좀 더 빠르고 3단 점프를 합니다.","일정시간마다 체력을 조금씩 회복합니다.","적을 처치시 탄창이 일정량 장전 됩니다." };

    
    
    private void Awake()
    {
       
    }

    void Start()
    {
        Cursor.visible = true;
        focusImage = PlayerPrefs.GetInt("Player");
        CharNameText.text = charname[focusImage];
        CharInfoText.text = charinfo[focusImage];
        CharUltText.text = ultname[focusImage];
        CharUltInfoText.text = ultinfo[focusImage];
        CharPassiveText.text = passive[focusImage];
        CharPassiveInfoText.text = passiveInfo[focusImage];
        SelectButton.SetActive(false);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");

    }

    public void MoveCanvasButton(int i)
    {
        focusCanvas = i;
    }

    public void MoveImage(bool isUp)
    {
        if (isUp)
        {
            if (focusImage < 5)
                focusImage++;
        }
        else
        {
            if (focusImage > 0)
                focusImage--;
        }

        if (focusImage > 5)
            focusImage = 5;
        if (focusImage < 0)
            focusImage = 0;
        
        CharNameText.text = charname[focusImage];
        CharInfoText.text = charinfo[focusImage];
        CharUltText.text = ultname[focusImage];
        CharUltInfoText.text = ultinfo[focusImage];
        CharPassiveText.text = passive[focusImage];
        CharPassiveInfoText.text = passiveInfo[focusImage];
        if(focusImage != PlayerPrefs.GetInt("Player"))
            SelectButton.SetActive(true);
        else
        {
            SelectButton.SetActive(false);
        }
    }

    public void Select()
    {
        PlayerPrefs.SetInt("Player",focusImage);
        SelectButton.SetActive(false);
    }

    void Update()
    {
        foreach (var canvas in canvass)
        {
            if (canvas == canvass[focusCanvas])
            {
                canvas.transform.localPosition += new Vector3(0,
                    (0.5f - canvas.transform.localPosition.y) *
                    (Time.unscaledDeltaTime*10));
            }
            else
                canvas.transform.localPosition += ((new Vector3(0, -1800) - canvas.transform.localPosition))*(Time.unscaledDeltaTime*5);
        }

        for(int i=0;i<5;i++)
        {
            if (i == focusImage)
            {
                _images[i].transform.localPosition += ((new Vector3(0.5f, 0) - _images[i].transform.localPosition))*(Time.unscaledDeltaTime*5);
            }
            if(i<focusImage)
                _images[i].transform.localPosition += ((new Vector3(-4000, 0) - _images[i].transform.localPosition))*(Time.unscaledDeltaTime*5);
            if(i>focusImage)
                _images[i].transform.localPosition += ((new Vector3(4000, 0) - _images[i].transform.localPosition))*(Time.unscaledDeltaTime*5);
        }

        if (focusCanvas != 0)
            BlackImage.color += (new Color(0, 0, 0, 150 / 255f) - BlackImage.color) * (Time.unscaledDeltaTime * 5);
        else
            BlackImage.color += (new Color(0, 0, 0, 0) - BlackImage.color) * (Time.unscaledDeltaTime * 5);
    }

    public void Sfx()
    {
        AudioManager.Instance.PlaySFX(0,false,0.6f);
    }
    
    
    public void GameStart()
    {
       LoadingSceneManager.LoadScene("2.Ingame");
    }

    public void SetMusicVolume(Slider vol)
    {
        PlayerPrefs.SetFloat("MusicVolume",vol.value);
        AudioManager.Instance.musicVolume = vol.value*PlayerPrefs.GetFloat("MasterVolume");
        AudioManager.Instance.SetMusicVolume();
    }
    
    public void SetSfxVolume(Slider vol)
    {
        PlayerPrefs.SetFloat("SfxVolume",vol.value);
        AudioManager.Instance.sfxVolume = vol.value*PlayerPrefs.GetFloat("MasterVolume");
    }
    public void SetMasterVolume(Slider vol)
    {
        PlayerPrefs.SetFloat("MasterVolume",vol.value);
        AudioManager.Instance.sfxVolume = vol.value*PlayerPrefs.GetFloat("SfxVolume");
        AudioManager.Instance.musicVolume = vol.value*PlayerPrefs.GetFloat("MusicVolume");
        AudioManager.Instance.SetMusicVolume();
    }
    
}
