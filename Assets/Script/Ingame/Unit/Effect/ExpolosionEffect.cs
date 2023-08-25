using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExpolosionEffect : MonoBehaviour
{
    [SerializeField] private bool isEffect;
    [SerializeField] private Color priColor,secColor;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,4);
        AudioManager.Instance.PlaySfXDelayed(24,0.2f,false,1,1);
        AudioManager.Instance.PlaySfXDelayed(24,0.3f,false,1,1);
        AudioManager.Instance.PlaySfXDelayed(24,0.4f,false,1,1);
    }

    // Update is called once per frame
    void Update()
    {
        if(isEffect)
            BoomEffect().Forget();
    }

    private async UniTaskVoid BoomEffect()
    {
        isEffect = false;
        IngameCamera.Instance.Shake(0.1f,0.1f,0,1.5f,15);
        AudioManager.Instance.PlaySFXOnce(23,false,1,1);
        int random = Random.Range(5, 7);
        for (int i = 0; i < random; i++)
        {
            GameManager.Instance.Effect(transform.position+new Vector3(Random.Range(-1.7f,1.7f),Random.Range(-1.7f,1.7f)),
                2,Random.Range(6.0f,8.2f),new OrbitColors(priColor,secColor),false,0.7f,20);
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f),false);
        }
    
    }
}
