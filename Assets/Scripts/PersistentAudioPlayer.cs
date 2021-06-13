using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentAudioPlayer : MonoBehaviour
{
    public bool isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        //only one allowed per scene
        PersistentAudioPlayer[] otherObjects = FindObjectsOfType<PersistentAudioPlayer>();

        for (int i = 0; i < otherObjects.Length; i++)
        {
            if (otherObjects[i].isPlaying)
            {
                Destroy(gameObject);
                return;
            }
        }

        DontDestroyOnLoad(gameObject);
        isPlaying = true;
    }
}
