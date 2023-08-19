using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject SniperHud;
    public GameObject SubSniperHud;
    [SerializeField] private Image Hpbar;
    [SerializeField] private Image Skillbar;
    [SerializeField] private Image HitEffect;
    [SerializeField] private Text PlayerHp;
    [SerializeField] private Text PlayerMoney;
    public bool isSniperSkill;
    // Start is called before the first frame update
    void Start()
    {
        SniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        SniperHud.transform.localEulerAngles = new Vector3(0, 0, -90);
        SubSniperHud.transform.localScale = new Vector3(5f, 5f, 1.2f);
        SubSniperHud.transform.localEulerAngles = new Vector3(0, 0, 180);
        HitEffect.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        HitEffect.color += (Color.clear-HitEffect.color)*(Time.unscaledDeltaTime);
        if (isSniperSkill)
        {
            SniperHud.transform.position =  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
            SubSniperHud.transform.position =  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        }
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

    public void PlayerHit()
    {
        HitEffect.color = Color.white;
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
}
