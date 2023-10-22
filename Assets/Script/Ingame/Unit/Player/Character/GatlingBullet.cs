using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingBullet : Bullet
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
        GameManager.Instance.MoveOrbitEffect(transform.position,Random.Range(3,5),0.4f,
            orbitColor,
            false,0,2, transform.rotation.eulerAngles.z,Random.Range(12,20),0);
        _lineRenderer.enabled = false;
    }

    public override GameObject Init(GetFireInstance getinfo,float delay)
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
        _lineRenderer.startColor = getinfo.orbitcolors.priColor;
        _lineRenderer.endColor =new Color(getinfo.orbitcolors.secColor.r,getinfo.orbitcolors.secColor.g,getinfo.orbitcolors.secColor.b,getinfo.orbitcolors.secColor.a/3f);
        ReleseReserv(delay).Forget();
        return gameObject;
        
    }
    void StartLine(Vector3 pos)
    {
        _lineRenderer.enabled = true;
        width = 0.3f;
        _lineRenderer.SetPosition(0,linepos.position);
        _lineRenderer.SetPosition(1,pos);
    }
    void UpdateLine()
    {
        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
        width -= width * (Time.deltaTime);
        _lineRenderer.startColor += (new Color(_lineRenderer.startColor.r,_lineRenderer.startColor.g,_lineRenderer.startColor.b,0)-_lineRenderer.startColor)* (Time.deltaTime);
        _lineRenderer.endColor += (new Color(_lineRenderer.endColor.r,_lineRenderer.endColor.g,_lineRenderer.endColor.b,0)-_lineRenderer.endColor)* (Time.deltaTime);
        _lineRenderer.SetPosition(0,linepos.position);
    }
}
