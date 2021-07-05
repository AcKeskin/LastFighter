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
    public Transform target;
    public float maxSpeed = 100, minSpeed = 15;

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
                obj.transform.localScale *= Random.Range(5,15);
                obj.SetActive(false);
                objPool.Enqueue(obj);
                obj.tag = "Rock";
                obj.AddComponent<Rigidbody>().useGravity = false;
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
        spawning.GetComponent<Rigidbody>().velocity = (target.position - pos).normalized * Random.Range(minSpeed,maxSpeed) ;

        poolDict[tag].Enqueue(spawning);
        return spawning;
    }
}
