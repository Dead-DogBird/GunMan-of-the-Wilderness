using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField] private Transform Weapon;

    private MonsterDefault _monsterDefault;
    // Start is called before the first frame update
    void Start()
    {
        _monsterDefault = gameObject.GetComponent<MonsterDefault>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TargetedPlayer()
    {
        
        
    }
}
