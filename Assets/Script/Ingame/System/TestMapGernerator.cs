using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class TestMapGernerator : MonoBehaviour
{
    [SerializeField] private TestMapNode Node;

    private int[,] testMap = new int[4,4];
    //1, 전투맵 2, 상점맵 3,특수맵 
    // Start is called before the first frame update
    void Start()
    {
        int rand;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                rand = Random.Range(0, 100);
                testMap[i, j] = rand switch
                {
                    > 10 and < 60 => 1,
                    > 61 and < 85 => 2,
                    > 86 and < 91 => 3,
                    _ => 0
                };
            }
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                switch (testMap[i,j])
                {
                    case 1:
                    case 2:
                    case 3:
                        var temp = Instantiate(Node);
                        temp.transform.parent = transform;
                        temp.transform.localPosition = new Vector3(-10 + (i * 500), j * 300, 0);
                        temp.transform.localPosition += new Vector3(Random.Range(-30, 30), Random.Range(-20, 20));
                        temp.transform.localScale = Vector3.one;
                        temp.GetComponent<TestMapNode>().InitNode(testMap[i, j]);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
