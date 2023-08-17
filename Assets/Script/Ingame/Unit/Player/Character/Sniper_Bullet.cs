using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper_Bullet : Bullet
{
    private LineRenderer _lineRenderer;
    private float width = 0.3f;

    [SerializeField] private Transform linepos;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        //_lineRenderer.enabled = false;
    }

    new void OnEnable()
    {
        base.OnEnable();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        UpdateLine();
    }

    protected void OnDisable()
    {
        _lineRenderer.enabled = false;
    }

    public override GameObject Init(GetFireInstance getinfo)
    {
        width = 0.5f;
        transform.position = getinfo.firepos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(getinfo.playerpos+new Vector3(0,0.5f,0),
            getinfo.mousepos)+Random.Range(getinfo.spread,-getinfo.spread));
        damage = getinfo.damage;
        speed = getinfo.speed;
        _sprite.color = getinfo.bulletColor;
        orbitColor = getinfo.orbitcolors;
        transform.rotation = Quaternion.Euler(0,0,
            CustomAngle.PointDirection(getinfo.playerpos+new Vector3(0,0.5f,0),getinfo.mousepos));
        StartLine(getinfo.firepos);
        return gameObject;
        
    }
    void StartLine(Vector3 pos)
    {
        _lineRenderer.enabled = true;
        width = 0.5f;
        _lineRenderer.SetPosition(0,linepos.position);
        _lineRenderer.SetPosition(1,pos);
    }
    void UpdateLine()
    {
        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
        _lineRenderer.SetPosition(0,linepos.position);
        width += -width * Time.deltaTime * 10;
    }
}
