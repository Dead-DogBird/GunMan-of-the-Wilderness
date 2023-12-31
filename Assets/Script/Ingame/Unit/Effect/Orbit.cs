using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class Orbit : PoolableObj
{
    protected float sizeReduction =2f;
    public float targetFigure;
    protected bool colored;
    protected float destroyTime = 2;
    protected SpriteRenderer _sprite;
    protected Color myColor = Color.black;
    protected Tween _tween;
    
    protected new void Start()
    {
        base.Start();
    }

    protected new void OnEnable()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = myColor;
        _tween.Complete();
    }
    protected void Update()
    {
        transform.localScale -= new Vector3(Time.deltaTime*sizeReduction, Time.deltaTime*sizeReduction);
        //transform.localScale *= Time.deltaTime*sizeReduction;
        if (transform.localScale.x <= targetFigure)
        {
            if(colored)
                _tween.Complete();
            GameManager.Instance._poolingManager.Despawn(this);
        }
        
    }

    protected new void OnDisable()
    {
        _sprite.color = myColor;
        if(colored)
            _tween.Complete();
    }
#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        _tween.Complete();
    }
#endif
    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        for (float i = 0; i <= delay; i += 0.1f)
        {
            if (!GameManager.Instance) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: false);
        }
        GameManager.Instance._poolingManager.Despawn(this); 
    }
    public virtual Orbit Init(OrbitInfo info, bool isSize =false, float reduction =2)
    {
        colored = info.isColored;
        targetFigure = info.targetFigure;
        transform.position = info.position;
        transform.localScale = new Vector3(info.scale,info.scale);

        if (isSize)
            sizeReduction = reduction;
        else
            sizeReduction = 2;
        
        if (colored)
        {
            _sprite.color = info.colors.priColor;
            _tween = _sprite.DOColor(info.colors.secColor, 0.2f);
            _sprite.sortingOrder = 13;
        }
        else
        {
            _sprite.color = myColor;
            _sprite.sortingOrder = 12;
        }
        return this;
    }
    public virtual Orbit Init(OrbitInfo info,float outline,bool isSize = false, float reduction = 2)
    {
        var orbit = Init(info,isSize,reduction);
        GameManager.Instance._poolingManager.
            Spawn<Orbit>().Init(new OrbitInfo(false,orbit.transform.position+new Vector3(0,0,0.1f),
                orbit.transform.localScale.x+outline,orbit.targetFigure+outline),isSize,reduction);
        
        return this;
    }
    
}

public struct OrbitInfo
{
    public bool isColored;
    public Vector3 position;
    public float scale;
    public float targetFigure;
    public OrbitColors colors;
    
    public OrbitInfo(bool _isColored, Vector3 _position, float _scale, float targetFigure, OrbitColors _colors)
    {
        isColored = _isColored;
        position = _position;
        this.targetFigure = targetFigure;
        scale = _scale;
        colors = _colors;
    }
    public OrbitInfo(bool _isColored, Vector3 _position, float _scale,float targetFigure)
    {
        isColored = _isColored;
        position = _position;
        scale = _scale;
        this.targetFigure = targetFigure;
        colors = new OrbitColors();
    }


}

public struct OrbitColors
{
    public Color priColor;
    public Color secColor;

    public OrbitColors(Color ppriColor, Color psceColor)
    {
        priColor = ppriColor;
        secColor = psceColor;
    }

}