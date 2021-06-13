using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameOnEscape : MonoBehaviour
{
    public bool isInitialized = false;
    // Start is called before the first frame update
    void Start()
    {
        //only one allowed per scene
        QuitGameOnEscape[] otherObjects = FindObjectsOfType<QuitGameOnEscape>();

        for (int i = 0; i < otherObjects.Length; i++)
        {
            if (otherObjects[i].isInitialized)
            {
                Destroy(gameObject);
                return;
            }
        }

        DontDestroyOnLoad(gameObject);
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
