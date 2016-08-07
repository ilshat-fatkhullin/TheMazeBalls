using UnityEngine;
using System.Collections;

public class ObjectDestroyer : MonoBehaviour {

    public float DestroyingTime;
    float startTime;

	void Start () {
        startTime = Time.time;
	}
	
	void Update () {
        if (Time.time - startTime > DestroyingTime)
        {
            Destroy(gameObject);
        }
	}
}
