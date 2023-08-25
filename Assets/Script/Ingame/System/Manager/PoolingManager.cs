using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PoolingManager : MonoBehaviour
{
    private GameManager _gameManager;
    private Dictionary<Type, object> poolingLists = new();

    private void OnEnable()
    {
        GameManager.Instance._poolingManager = this;
    }
    private void Start()
    {
        
    }
    
    // 새로운 제네릭 타입의 풀을 추가 (기존에 있으면 기존 리스트를 반환)
    public PoolingList AddPoolingList<T>(int initialSize,GameObject obj) where T : PoolableObj
    {
        Type type = typeof(T);
        if (poolingLists.TryGetValue(type, out var list))
        {
            // 이미 존재하는 타입의 리스트가 있을 경우 기존 리스트 반환
            return list as PoolingList;
        }
        // 새로운 타입의 리스트 생성
        PoolingList poolingList = new();
        poolingList.Initialize($"{type}",initialSize,obj);
        poolingLists.Add(type, poolingList);
        return poolingList;
    }

    // 오브젝트를 활성화하여 사용
    public T Spawn<T>() where T : PoolableObj
    {
        if (poolingLists.TryGetValue(typeof(T), out object poolingList) 
            && poolingList is PoolingList)
        {
             return ((PoolingList)poolingList).Spawn<T>();
        }
        return null;
    }
    // 사용한 오브젝트를 비활성화하여 풀에 반환
    public void Despawn<T>(T obj) where T : PoolableObj
    {
        if (poolingLists.TryGetValue(typeof(T), out object poolingList) && poolingList is PoolingList)
        {
            (poolingList as PoolingList).Despawn(obj.gameObject);
        }
    }

    // 모든 오브젝트를 비활성화하여 풀을 비움
    public void Clear<T>() where T : PoolableObj
    {
        if (poolingLists.TryGetValue(typeof(T), out object poolingList) && poolingList is PoolingList)
        {
            (poolingList as PoolingList).Clear();
        }
    }

    public void AllDespawn<T>() where T : PoolableObj
    {
        if (poolingLists.TryGetValue(typeof(T), out object poolingList) && poolingList is PoolingList)
        {
            (poolingList as PoolingList).AllDespawn();
        }
    }
}


// PoolableObj를 상속받는 제네릭 타입을 사용한 풀링 리스트
public class PoolingList : MonoBehaviour
{
    public List<GameObject> Allpool;
    private LinkedList<GameObject> PoolList;
    private LinkedList<GameObject> ActiveList;
    private GameObject originalObject;
    private GameObject ojectPar;
    public void Initialize(string type,int initialSize,GameObject pObj)
    {
        ojectPar = new GameObject(type+"s");
        Allpool = new List<GameObject>();
        PoolList = new LinkedList<GameObject>();
        ActiveList = new LinkedList<GameObject>();
        originalObject = pObj;
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateObject(pObj);
            PoolList.AddLast(obj);
            Allpool.Add(obj);
        }
    }

    private GameObject CreateObject(GameObject pObj)
    {
        var temp =  Instantiate(pObj, ojectPar.transform, true);
        temp.SetActive(false);
        return temp;
    }

    public T Spawn<T>()
    {
        GameObject obj;
        if (PoolList.Count > 0)
        {
            obj = PoolList.First.Value;
            PoolList.RemoveFirst();
        }
        else
        {
            obj = CreateObject(originalObject); // 풀에 남은 오브젝트가 없을 때 새로운 오브젝트 생성
            Allpool.Add(obj);
            
        }
        obj.SetActive(true);
        
        ActiveList.AddLast(obj);
        return obj.GetComponent<T>();
    }

    // 사용한 오브젝트를 비활성화하여 풀에 반환
    public void Despawn(GameObject obj)
    {
        if (!ActiveList.Contains(obj)) return;
        obj.SetActive(false);
        ActiveList.Remove(obj);
        PoolList.AddLast(obj);
    }

    // 모든 오브젝트를 비활성화하여 풀을 비움
    public void Clear()
    {
        PoolList.Clear();
        ActiveList.Clear();
    }
    public void AllDespawn()
    {
        if (ActiveList.Count <= 0) return;
        while(ActiveList.Count>0)
        {
            ActiveList.First.Value.SetActive(false);
            PoolList.AddLast(ActiveList.First.Value);
            ActiveList.RemoveFirst();
        }
    }

    
}
