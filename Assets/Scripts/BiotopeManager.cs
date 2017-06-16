using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiotopeManager : MonoBehaviour {
    public List<GameObject> biotopes = new List<GameObject>();

	// Use this for initialization
	void Start () {
		foreach(Transform child in transform)
        {
            if (!child.transform.name.StartsWith("biotope"))
            {
                continue;
            }
            biotopes.Add(child.gameObject);
            GameObject biotope = child.gameObject;
            foreach (Transform bioChild in child)
            {
                if (bioChild.gameObject.name == "biotope_area")
                {
                    Destroy(bioChild.gameObject);
                }
            }
        }
	}

    public void RotateBiotope(GameObject biotope, float offset)
    {
        foreach (Transform bioChild in biotope.transform)
        {
            // randomly rotate around swarm
            if (bioChild.gameObject.tag == "rotatable")
            {
                bioChild.rotation = new Quaternion();
                bioChild.Rotate(transform.up, Random.Range(offset+10, 40f+offset));
            }
        }
    }
	
    public GameObject getRandomBiotope(List<GameObject> biotopeList)
    {
        return biotopes[(int)Random.Range(0.0f, biotopeList.Count * 1.0f)];
    }

	// Update is called once per frame
	void Update () {
		
	}
}
