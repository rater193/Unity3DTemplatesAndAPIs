using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR 
        SceneManager.LoadScene(2);
#else
        SceneManager.LoadScene(1);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
