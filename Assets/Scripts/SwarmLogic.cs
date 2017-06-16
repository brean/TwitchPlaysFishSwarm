using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish
{
    public GameObject gameObject;
    public string name = "";
};


public class SwarmLogic : MonoBehaviour {
    [Tooltip("prefab of a single fish")]
    public GameObject fishPrefab;

    [Tooltip("size of area where new fish can spawn")]
    public Vector3 spawnAreaSize = new Vector3(3.0f, 1.0f, 3.0f);

    [Tooltip("distance from spawn-area to center of game object")]
    public Vector3 spawnAreaOffset = Vector3.zero;

    [Tooltip("min. size of your swarm at the beginning")]
    public int startWith = 0;

    public List<Fish> allFish = new List<Fish>();

    public void addFish()
    {
        Fish fish = new Fish();
        fish.gameObject = Instantiate(fishPrefab);
        Vector3 pos = Random.insideUnitSphere;
        pos.x *= spawnAreaSize.x;
        pos.y *= spawnAreaSize.y;
        pos.z *= spawnAreaSize.z;
        pos += spawnAreaOffset;
        fish.gameObject.transform.SetParent(transform);
        fish.gameObject.transform.localPosition = pos;
        Animator ani = fish.gameObject.GetComponent<Animator>();
        ani.Play("", 0, Random.Range(0f, 1f));
        allFish.Add(fish);
    }

    public void removeFish()
    {
        if (allFish.Count > 0)
        {
            Fish fish = allFish[0];
            Destroy(fish.gameObject);
            allFish.RemoveAt(0);
        }
    }

    public void Start()
    {
        for (int i = allFish.Count; i < startWith; i++)
        {
            addFish();
        }
    }
}
