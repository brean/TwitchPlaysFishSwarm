using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Fish
{
    public GameObject gameObject;
    public string name = "";
};


public class SwarmLogic : MonoBehaviour {
    [Tooltip("prefab of a single fish")]
    public GameObject fishPrefab;

    [Tooltip("Twitch bot instance to get random names for fish")]
    public TwitchBot bot;

    List<Fish> allFish = new List<Fish>();

    public void addFish()
    {
        Fish fish = new Fish();
        fish.gameObject = Instantiate(fishPrefab);
        Vector3 pos = Random.insideUnitSphere;
        pos.x *= 3f;
        pos.z *= 3f;
        fish.gameObject.transform.SetParent(transform);
        fish.gameObject.transform.localPosition = pos;
        Animator ani = fish.gameObject.GetComponent<Animator>();
        ani.Play("", 0, Random.Range(0f, 1f));
        allFish.Add(fish);
    }

    public void removeFish()
    {
        Fish fish = allFish[0];
        Destroy(fish.gameObject);
        allFish.RemoveAt(0);
    }

	// Use this for initialization
	void Start () {
        // we will always have at least 10 fish
        for (int i = allFish.Count; i < 10; i++)
        {
            addFish();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (allFish.Count < 5)
        {
            //TODO: GameOver
        }
		/*if (Time.realtimeSinceStartup > nextTime)
        {
            addFish();
            Start();
        }*/
	}
}
