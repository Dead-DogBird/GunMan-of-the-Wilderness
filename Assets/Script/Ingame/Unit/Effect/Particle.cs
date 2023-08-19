using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Particle : MonoBehaviour
{
    [SerializeField] Color priColor,secColor;
    [SerializeField] private float Angle;
    [SerializeField] private float random;
    [SerializeField] private int count = 5;
    [SerializeField] private float size = 1;
    [SerializeField] private bool isDefault = false;
    [SerializeField] private Vector2 Randompos;
    private bool isActive = true;

    [SerializeField] private float Outline = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(Random.Range(-Randompos.x,Randompos.x),Random.Range(-Randompos.y,Randompos.y)),Random.Range(0,count),size,
            new OrbitColors(priColor,secColor),
            (Outline!=0),Outline,2, Angle + Random.Range(-random,random),Random.Range(7,12));
         if(isDefault)
             ParticleTask().Forget();
    }

    private void OnDestroy()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async UniTaskVoid ParticleTask()
    {
        while (isActive && gameObject.activeSelf)
        {
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(Random.Range(-Randompos.x,Randompos.x),Random.Range(-Randompos.y,Randompos.y)),Random.Range(0,count),size,
                new OrbitColors(priColor,secColor),
                (Outline!=0),Outline,2, Angle + Random.Range(-random,random),Random.Range(7,12));
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        }
    }
}
