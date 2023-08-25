using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToScene : MonoBehaviour
{
    [SerializeField] private string NextScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
            LoadingSceneManager.LoadScene(NextScene);
    }
}
