using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
        _meshRenderer.sortingOrder = 2;
        GetOutlines();
    }

    new void OnEnable()
    {
        _textMesh = GetComponent<TextMesh>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sortingOrder = 2;
        GetOutlines();
        
    }

    void GetOutlines()
    {
        if (_outlines.Count != 0 && _outlineRenderers.Count != 0) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            _outlines.Add(transform.GetChild(i).GetComponent<TextMesh>());
            _outlineRenderers.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
            transform.GetChild(i).GetComponent<MeshRenderer>().sortingOrder = 1;
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
        
        alpha += -alpha*Time.deltaTime*2;
        
        transform.position += new Vector3(0, ypos, 0);
        _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, alpha);
        foreach (var outlines in _outlines)
        {
            outlines.color =new Color(0,0,0,alpha);
        }
        if(alpha <= 0.001f) GameManager.Instance._poolingManager.Despawn(this);
        
    }
    private void FixedUpdate()
    {
        ypos += -ypos/7;
    }

    public GameObject Init(Vector3 pos, Color color,string text)
    {
        transform.position = pos;
        _textMesh.text = text;
        _textMesh.color = color;
        foreach (var _outline in _outlines)
        {
            _outline.text = text;
            _outline.color = Color.black;
        }
        ypos = 0.1f;
        tempColor = _textMesh.color;
        alpha = _textMesh.color.a;
        return transform.gameObject;
    }
    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
        if(gameObject.activeSelf)
            GameManager.Instance._poolingManager.Despawn(this); 
    }
}
