using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;


public class UIManager : MonoSingleton<UIManager>
{
    public GameObject SniperHud;
    public GameObject SubSniperHud;
    [SerializeField] private Image Hpbar;
    [SerializeField] private Image Skillbar;
    [SerializeField] private Image HitEffect;
    [SerializeField] private Image Cursor_;

    [SerializeField] private Text PlayerHp;
    [SerializeField] private Text PlayerSkill;
    [SerializeField] private Text PlayerMoney;
    [SerializeField] private Text PlayerMag;

    [SerializeField] private Image SniperSkillGauge;
    [SerializeField] private Image ShotGunSkillImage;

    [SerializeField] private Image FadeImage;

    [SerializeField] private Text stageText;
    [SerializeField] private Text stageNameText;
    
    [SerializeField] private BossUI _bossUI;
    [SerializeField] private GameObject _GameOverUI;
    [SerializeField] private GameObject _PauseUI;
    private Color ShotGunSkillImageOriginalColor;
    public bool isSniperSkill;

    private float _playerMaxHp;
    private float _playerMaxSkill;
    private float _playerHp;
    private float _playerSkill;

    private int money = 0;
    private int updateMoney = 0;
    private bool isEditorEnd = false;

    private int mag;
    private int Allmag;

    private bool isReload = false;

    private bool fade = false;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        Cursor.visible = false;
        SniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        SniperHud.transform.localEulerAngles = new Vector3(0, 0, -90);
        
        SubSniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        SubSniperHud.transform.localEulerAngles = new Vector3(0, 0, 180);

        SniperSkillGauge.fillAmount = 0;
        
        HitEffect.color = Color.clear;
        ShotGunSkillImageOriginalColor = ShotGunSkillImage.color + new Color(0,0,0,1f);
    }

    private void OnEnable()
    {
        UpdateMoneyTask().Forget();
        _GameOverUI.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_GameOverUI.activeSelf)
            _GameOverUI.transform.localScale +=
                (Vector3.one - _GameOverUI.transform.localScale) * (Time.unscaledDeltaTime * 15);
        
        
        
        HitEffect.color += (Color.clear-HitEffect.color)*(Time.unscaledDeltaTime);
        Hpbar.fillAmount += (_playerHp - Hpbar.fillAmount)*(Time.unscaledDeltaTime*15); 
        Skillbar.fillAmount += (_playerSkill - Skillbar.fillAmount)*(Time.unscaledDeltaTime*7);
        
        Cursor_.fillAmount =(isReload)?(1-(doneTime-Time.time)/reloadDelay):1;
        
        PlayerMoney.transform.localScale +=
            (new Vector3(0.24f,0.24f) - PlayerMoney.transform.localScale) * (Time.unscaledDeltaTime * 7);
        PlayerMoney.color +=(Color.white-PlayerMoney.color)* (Time.unscaledDeltaTime * 15);

        ShotGunSkillImage.color += (Color.clear - ShotGunSkillImage.color) * (Time.unscaledDeltaTime*5);
        if(!isReload)
        {
            if (mag > 0) 
            {
                PlayerMag.color +=((mag<=Allmag/5?Color.red:Color.white)-PlayerMag.color)* (Time.unscaledDeltaTime * 10);
            }
            else
            {
                PlayerMag.color +=(new Color(130/255f,130/255f,150/255f,1f)-PlayerMag.color)*(Time.unscaledDeltaTime * 15);
            }
        }
        PlayerMag.transform.localScale +=
            (new Vector3(0.2f,0.2f) - PlayerMag.transform.localScale) * (Time.unscaledDeltaTime * 7);

        Cursor_.transform.localScale +=  (Vector3.one-Cursor_.transform.localScale)*(Time.unscaledDeltaTime*10);
        Cursor_.transform.position =  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        if (isSniperSkill)
        {
            SniperHud.transform.position =  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
            SubSniperHud.transform.position =  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        }

        FadeImage.color += ((!fade ? Color.clear : Color.black) - FadeImage.color) * (Time.unscaledDeltaTime * 20);

    }
#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        isEditorEnd = true;
    }
#endif
    
    public void SetSkillGaougeColor(Color color)
    {
        Skillbar.color = color;
    }
    public void SetGauge(float hp,float skill)
    {
        _playerMaxHp = hp;
        _playerMaxSkill = skill;
    }
   
    public void UpdateGauge(float hp =0,float skill=0)
    {
        if (hp != 0)
        {
            _playerHp = (hp/_playerMaxHp);
            PlayerHp.text = $"Hp: {hp:F1}";
        }
        if (skill != 0)
        {
            _playerSkill = (skill / _playerMaxSkill);
            PlayerSkill.text = $"Skill: {(_playerSkill*100f):F1}%";
        }
    }
    public void ResetGauge(bool hp = false,bool skill = false)
    {
        if (hp)
        {
            _playerHp = 0;
            PlayerHp.text = $"Hp: {0}";
        }
        if (skill)
        {
            _playerSkill = 0;
            PlayerSkill.text = $"Skill: {0}%";
        }
    }
    
    public void UpdateMoney(int money)
    {
        updateMoney = money;
        PlayerMoney.transform.localScale += new Vector3(0, 0.1f);
        PlayerMoney.color = new Color(40/255f, 100/255f, 255/255f, 255f);
    }
    async UniTaskVoid UpdateMoneyTask()
    {
        while (!isEditorEnd)
        {
            if (MathF.Abs(money - updateMoney) > 3)
            {
                money += (updateMoney - money) / 3;
                if (MathF.Abs(money - updateMoney) <= 3)
                {
                    money = updateMoney;
                }
            }
            PlayerMoney.text =(money>0)? $"{money:#,###}" : "0";
            await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);
        }
    }

    public void UpdateMag(int _mag, bool isInit = false)
    {
        if (isInit)Allmag = _mag;
        mag = _mag;
        PlayerMag.text = $"{mag}/{Allmag}";
        PlayerMag.transform.localScale += new Vector3(-0.02f, 0.01f);
        PlayerMag.color = GameManager.Instance.player.colors.priColor;

    }

    private float reloadDelay,doneTime;
    public void UpdateMagReload(bool _reload = true,float _reloadDelay = 0)
    {
        isReload = _reload;

        if (isReload)
        {
            doneTime = _reloadDelay + Time.time;
            reloadDelay = _reloadDelay;
            PlayerMag.text = $"Reload!";
            PlayerMag.color  = new Color(130 / 255f, 130 / 255f, 150 / 255f, 1f);
        }

    }
    public void PlayerHit()
    {
        HitEffect.color = Color.white;
    }
    public void SetCursorEffect(float size = 1.2f)
    {
        Cursor_.transform.localScale = new Vector3(size, size, 1);
    }
    async UniTaskVoid SniperHudTask()
    {
        Image image = SniperHud.GetComponent<Image>();
        Image subImage = SubSniperHud.GetComponent<Image>();
        Outline outline = SubSniperHud.GetComponent<Outline>();
        SniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        SubSniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        while(isSniperSkill)
        {
            SniperHud.transform.localScale += (Vector3.one - SniperHud.transform.localScale) * (Time.unscaledDeltaTime * 30);
            SniperHud.transform.localEulerAngles += (new Vector3(0,0,0)-SniperHud.transform.localEulerAngles) * (Time.unscaledDeltaTime*20);
            image.color += (Color.white-image.color)*Time.unscaledDeltaTime*10;
            SubSniperHud.transform.localScale += (Vector3.one - SubSniperHud.transform.localScale) * (Time.unscaledDeltaTime * 30);
            SubSniperHud.transform.localEulerAngles += (new Vector3(0,0,90)-SubSniperHud.transform.localEulerAngles) * (Time.unscaledDeltaTime*20);
            outline.effectDistance += -outline.effectDistance * (Time.unscaledDeltaTime * 5);
            subImage.color += (new Color(1,1,1,0.5f)-subImage.color)*Time.unscaledDeltaTime*10;

            
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }
        SniperHud.transform.localEulerAngles = new Vector3(0, 0, -90);
        SniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        SubSniperHud.transform.localEulerAngles = new Vector3(0, 0, 180);
        SubSniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        image.color = Color.clear;
        subImage.color = Color.clear;
        outline.effectDistance = new Vector2(100, 100);
    }

    public void SniperSkillFire()
    {
        SubSniperHud.GetComponent<Outline>().effectDistance = new Vector2(60, 60);
    }
    public void SniperSkill(bool _isSkill)
    {
        isSniperSkill = _isSkill;
        SniperHud.SetActive(isSniperSkill);
        SubSniperHud.SetActive(isSniperSkill);
        if (isSniperSkill)
        {
            SniperHudTask().Forget();
        }
    }

    public void OnPlayerDie()
    {
        SniperHud.SetActive(false);
        SubSniperHud.SetActive(false);
    }
    public void UpdateSniperSkillGauge(float amount)
    {
        SniperSkillGauge.fillAmount = amount;
    }

    public void ShotGunSkill()
    {
        ShotGunSkillImage.color = ShotGunSkillImageOriginalColor;
    }
    public async UniTaskVoid SetFade(bool value = false,float Delay = 3)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Delay), true);
        fade = value;
    }

    public void SetStageName(string name)
    {
        stageNameText.text = name;
    }

    public void SetStage(string stage)
    {
        stageText.text = stage;
    }
    public void SetBossUI(DefaultBossMonster _boss)
    {
        _bossUI.gameObject.SetActive(true);
        _bossUI.Init(_boss);
    }
    public void SetBossUI()
    {
        _bossUI.gameObject.SetActive(false);
    }

    public void SetActiveGameoverUI()
    {
        _GameOverUI.SetActive(true);
    }
    public void SetActivePauseUI(bool var = true)
    {
        _PauseUI.SetActive(var);
    }

}
