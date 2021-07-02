using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject[] prefabs;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDict;

    #region Singletonish
    public static ObjectPooler Instance; // Some sort of singleton here

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool p in pools)
        {
            Queue<GameObject> objPool = new Queue<GameObject>();
            for (int i = 0; i < p.size; i++)
            {
                var obj = Instantiate(p.prefabs[Random.Range(0, p.prefabs.Length)]);
                obj.SetActive(false);
                objPool.Enqueue(obj);
            }
            poolDict.Add(p.tag, objPool);
        }

    }
    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning("No pool with tag " + tag);
            return null;
        }
        var spawning = poolDict[tag].Dequeue();
        spawning.SetActive(true);
        spawning.transform.position = pos;
        spawning.transform.rotation = rot;

        poolDict[tag].Enqueue(spawning);
        return spawning;
    }
}
