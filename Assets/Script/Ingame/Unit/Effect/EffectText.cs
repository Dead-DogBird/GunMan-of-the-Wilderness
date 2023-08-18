using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EffectText : PoolableObj
{ 
    string Text;
    private TextMesh _textMesh;
    private List<TextMesh> _outlines = new();

    
    private MeshRenderer _meshRenderer; 
    private List<MeshRenderer> _outlineRenderers = new();
    

    private Color secColor;

    public float ypos = 0.07f;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _textMesh = GetComponent<TextMesh>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sortingOrder = 22;
        GetOutlines();
    }

    new void OnEnable()
    {
        _textMesh = GetComponent<TextMesh>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sortingOrder = 22;
        GetOutlines();
        
    }

    void GetOutlines()
    {
        if (_outlines.Count != 0 && _outlineRenderers.Count != 0) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            _outlines.Add(transform.GetChild(i).GetComponent<TextMesh>());
            _outlineRenderers.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
            transform.GetChild(i).GetComponent<MeshRenderer>().sortingOrder = 21;
        }
    }
    new void OnDisable()
    {
        _textMesh.color = Color.white;
    }

    private Color tempColor;
    private float alpha;
    void Update()
    {
        if (Time.timeScale == 0) return;

        transform.position += new Vector3(0, ypos, 0);
        transform.localScale = new Vector3(alpha, alpha);
        if(alpha <= 0.4f) GameManager.Instance._poolingManager.Despawn(this);
        
    }
    private void FixedUpdate()
    {
        ypos += (-0.05f-ypos)/15;
        alpha += -alpha/30;
        _textMesh.color += (toColor-_textMesh.color)/10;
    }

    private Color toColor;
    public GameObject Init(Vector3 pos, Color color,string text,float randomRange = 1)
    {
        transform.position = pos+(Random.Range(randomRange,-randomRange)*Vector3.right)+(Random.Range(0,randomRange*0.5f)*Vector3.up);
        _textMesh.text = text;
        _textMesh.color = Color.white;
        toColor = color;
        foreach (var _outline in _outlines)
        {
            _outline.text = text;
            _outline.color = Color.black;
        }

        foreach (var _renderer in _outlineRenderers)
        {
            _renderer.sortingOrder = 21;
        }
        ypos = 0.08f;
        tempColor = _textMesh.color;
        alpha = 1.2f;
        return transform.gameObject;
    }
    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
        if(gameObject.activeSelf)
            GameManager.Instance._poolingManager.Despawn(this); 
    }
}
