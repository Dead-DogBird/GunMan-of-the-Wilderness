using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Image[] panners;
    [SerializeField] private Image[] prizeImages;
    [SerializeField] private Text[] prizeTexts;
    [SerializeField] private Text[] prizeInfoTexts;
    [SerializeField] private Text[] priceTexts;
    [SerializeField] private Text restorepriceText;
    [SerializeField] private Sprite[] SourceImage;

    [SerializeField] private Text PlayerMoney;
    [SerializeField] private Text PlayerHpText;
    [SerializeField] private Image PlayerHpImage;
    [SerializeField] private Image Cursor;
    
    
    private Price[] _prices = new Price[7]
    {
        new Price("총기강화", "총기를 랜덤으로" +
                          "\n업그레이드 합니다." +
                          "\n(여러 번 구매 가능)" +
                          "\n(탄속, 재장전, 연사,공격력, 탄창 중 1)",
            1000, new Color(235 / 255f, 40 / 255f, 93 / 255f, 1)),
        new Price("체력회복", "체력을 회복합니다.",
            1000, new Color(255 / 255f, 54 / 255f, 1 / 255f, 1)),
        new Price("날렵한 몸놀림", "이동속도가 증가합니다.",
            800, new Color(92 / 255f, 204 / 255f, 29 / 255f, 1)),
        new Price("중력 거부", "점프력이 증가합니다.",
            900, new Color(78 / 255f, 191 / 255f, 180 / 255f, 1)),
        new Price("벌크업!", "최대 체력이 증가합니다.",
            800, new Color(235 / 255f, 40 / 255f, 93 / 255f, 1)),
        new Price("하우 투 킬", "스킬 게이지의 충전량이\n증가합니다.",
            700, new Color(74 / 255f, 66 / 255f, 255 / 255f, 1)),
        new Price("살아움직이는 망자", "1회 부활 합니다.",
            5000, new Color(235 / 255f, 40 / 255f, 93 / 255f, 1))
    };

    private int[] Id = new int[3] { -1, -1, -1 };
    private int reStok = 500;
    
    //private GameManager _gm;
    //private UIManager _uim;
    //private AudioManager _aum;

#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        isActive = false;
    }
#endif
    // Start is called before the first frame update
    void Start()
    {
    }
    private RectTransform[] rectTransform = new RectTransform[3];
    private void OnEnable()
    {
       // _gm = GameManager.Instance;
       // _uim = UIManager.Instance;
       // _aum = AudioManager.Instance;
        Calculate();
        for (int i = 0; i < 3; i++)
        {
            rectTransform[i] = panners[i].GetComponent<RectTransform>();
            rectTransform[i].position = new Vector3(rectTransform[i].position.x, -2000);
        }
        for (int i = 0; i < 3; i++)
        {
            Id[i] = GetRandomPrizeNonDuple();
        }
        reStok = 500;
        restorepriceText.text = $"{reStok}";
        for (int i = 0; i < 3; i++)
        {
            panners[i].color = _prices[Id[i]].color;
            prizeImages[i].sprite = SourceImage[Id[i]];
            prizeTexts[i].text = _prices[Id[i]].name;
            prizeInfoTexts[i].text = _prices[Id[i]].Info;
            priceTexts[i].text = _prices[Id[i]].price.ToString();
        }
        
        money = GameManager.Instance.player.Money;
        playerMaxHp = GameManager.Instance.player.playerMaxHp;
        playerHp = GameManager.Instance.player.PlayerHp;
        isActive = true;
        Debug.Log(money);
        PlayerMoney.text = $"{money}";
        UpdateMoneyTask().Forget();
    }

    void Update()
    {
        Cursor.transform.position  =  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        PlayerHpImage.fillAmount = playerHp / playerMaxHp;
        for (int i = 0; i < 3; i++)
        {
            rectTransform[i].position += 
                new Vector3(0,((Screen.height/2f)-rectTransform[i].position.y)*(Time.unscaledDeltaTime* 15));
        }
    }

    private float playerHp;
    private float playerMaxHp;
    private int money = 0;
    private int updateMoney =0;
    private bool isActive = false;
    async UniTaskVoid UpdateMoneyTask()
    {
        while (isActive)
        {
            money += (updateMoney - money)/3;
            if (MathF.Abs(money - updateMoney) <= 3)
            {
                money = updateMoney;
            }
            PlayerMoney.text =(money>0)? $"{money:#,###}" : "0";
            await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);
        }
    }
    
    public void GetBuy(int i)
    {
        if (_prices[i].price > GameManager.Instance.player.Money) return;
        
        updateMoney = GameManager.Instance.player.Money;
        GameManager.Instance.player.SetMoney(-_prices[i].price);
        rectTransform[i].position = new Vector3(rectTransform[i].position.x, 2000);
        Id[i] = GetRandomPrizeNonDuple();
        panners[i].color = _prices[Id[i]].color;
        prizeImages[i].sprite = SourceImage[Id[i]];
        prizeTexts[i].text = _prices[Id[i]].name;
        prizeInfoTexts[i].text = _prices[Id[i]].Info;
        priceTexts[i].text = _prices[Id[i]].price.ToString();

        updateMoney = GameManager.Instance.player.Money;
        playerMaxHp = GameManager.Instance.player.playerMaxHp;
        playerHp = GameManager.Instance.player.PlayerHp;
        PlayerHpImage.fillAmount = playerHp / playerMaxHp;
        
    }
    
    
    
    private float[] probabilities = new float[] { 2.0f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 0.5f };
    float[] cumulativeProbabilities;
    void Calculate()
    {
        cumulativeProbabilities = new float[probabilities.Length];
        float cumulativeProbability = 0;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            cumulativeProbabilities[i] = cumulativeProbability;
        }
    }
    int GetRandomPrize()
    {
        float randomValue = Random.Range(0.0f, cumulativeProbabilities[cumulativeProbabilities.Length - 1]);
        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (randomValue <= cumulativeProbabilities[i])
            {
                return i;
            }
        }
        return cumulativeProbabilities.Length - 1;
    }
    int GetRandomPrizeNonDuple()
    {
        int var = GetRandomPrize();
        for(int i =0;i<3;i++)
        {
            if (Id[i] == var)
            {
                return GetRandomPrizeNonDuple();
            }
        }
        return var;
    }
}

public class Price
{
    public string name;
    public string Info;
    public int price;
    public Color color;
    public Price(string _name, string _Info, int _price,Color _color)
    {
        name = _name;
        Info = _Info;
        price = _price;
        color = _color;
    }
}
