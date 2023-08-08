using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class Orbit : PoolableObj
{
    private float sizeReduction =2f, targetFigure;
    private bool colored;
    private float destroyTime = 2;
    private SpriteRenderer _sprite;
    private Color myColor = Color.black;
    Color priColor, secColor;
    private Tween _tween;
    
    new void Start()
    {
        base.Start();
    }

    new void OnEnable()
    {
        ReleseReserv(destroyTime).Forget();
        _sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        transform.localScale -= new Vector3(Time.deltaTime*sizeReduction, Time.deltaTime*sizeReduction);
        //transform.localScale *= Time.deltaTime*sizeReduction;
        if (transform.localScale.x <= targetFigure)
        {
            _tween.Complete();
            GameManager.Instance._poolingManager.Despawn(this);
        }
        
    }
    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
        GameManager.Instance._poolingManager.Despawn(this); 
    }
    public Orbit Init(OrbitInfo info)
    {
        colored = info.isColored;
        targetFigure = info.targetFugure;
        transform.position = info.position;
        transform.localScale = new Vector3(info.scale,info.scale);
        if (colored)
        {
            priColor = info.colors?.priColor ?? myColor;
            secColor = info.colors?.secColor ?? myColor;
            _sprite.color = priColor;
            _tween = _sprite.DOColor(secColor, 0.2f);
            _sprite.sortingOrder = 7;
        }
        else
        {
            priColor = myColor;
            secColor = myColor;
            _sprite.color = myColor;
            _sprite.sortingOrder = 6;
        }
        return this;
    }
    public Orbit Init(OrbitInfo info,float outline)
    {
        Init(info);
        GameManager.Instance._poolingManager.
            Spawn<Orbit>().Init(new OrbitInfo(false,transform.position+new Vector3(0,0,0.1f),
                transform.localScale.x+outline,targetFigure+outline));
        
        return this;
    }
}

public struct OrbitInfo
{
    public bool isColored;
    public Vector3 position;
    public float scale;
    public float targetFugure;
    public OrbitColors? colors;
    
    public OrbitInfo(bool _isColored, Vector3 _position, float _scale, float _targetFugure, OrbitColors _colors)
    {
        isColored = _isColored;
        position = _position;
        targetFugure = _targetFugure;
        scale = _scale;
        colors = _colors;
    }
    public OrbitInfo(bool _isColored, Vector3 _position, float _scale,float _targetFugure)
    {
        isColored = _isColored;
        position = _position;
        scale = _scale;
        targetFugure = _targetFugure;
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