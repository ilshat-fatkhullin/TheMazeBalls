using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

    AudioSource[] sources;
    int lastIndex = -1;
    int index;

	void Start () {
        sources = gameObject.GetComponents<AudioSource>();
        UpdateIndex();
        sources[index].Play();
	}

    void UpdateIndex()
    {
        index = Random.Range(0, sources.GetLength(0));
        if (index == lastIndex)
        {
            index++;
            if (index >= sources.GetLength(0))
            {
                index = 0;
            }
        }

        lastIndex = index;
    }
	
	void Update () {
        if (!sources[index].isPlaying)
        {
            UpdateIndex();
            sources[index].Play();
        }
	}
}
