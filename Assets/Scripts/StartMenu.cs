using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public string mainSceneName = "Main";
    public float continueDelay = 0.5f;

    private float delayCompleteTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        delayCompleteTime = Time.time + continueDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > delayCompleteTime)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(mainSceneName);
            }
        }
        
    }
}
