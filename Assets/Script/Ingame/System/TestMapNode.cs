using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMapNode : MonoBehaviour
{
    int nodeId;

    [SerializeField]private Text idText;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitNode(int id)
    {
        nodeId = id;
        idText.text = nodeId.ToString();
    }
}
