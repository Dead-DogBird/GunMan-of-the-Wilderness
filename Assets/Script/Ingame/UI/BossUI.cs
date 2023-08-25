using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private Text BossName;

    [SerializeField] private Text BossHpText;
    [SerializeField] private Image BossHpbar;
    
    
    private DefaultBossMonster _bossMonster;

    private float BossMaxHp;
    private float BossCurHp;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BossHpbar.fillAmount += ((_bossMonster.BossHp / BossMaxHp) - BossHpbar.fillAmount) * (Time.unscaledDeltaTime * 5);
        BossHpText.text = $"HP :{(_bossMonster.BossHp/BossMaxHp)*100:F1}%";
    }

    public void Init(DefaultBossMonster bossMonster)
    {
        _bossMonster = bossMonster;
        BossMaxHp = bossMonster.BossHp;
        BossCurHp = BossMaxHp;
        BossName.text = GameManager.Instance.BossNames[GameManager.Instance.wolrd];
    }
}
