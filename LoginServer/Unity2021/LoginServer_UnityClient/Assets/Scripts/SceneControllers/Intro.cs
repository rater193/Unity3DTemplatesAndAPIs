using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        GameObject logoobj = GameObject.Find("LogoImage");
        Image logoimg = logoobj.GetComponent<Image>();
        logoimg.color = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(0);
        yield return new WaitForFixedUpdate();

        for(float place = 0; place < 1; place += 0.05f)
        {
            logoimg.color = new Color(1, 1, 1, place);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(3);

        for (float place = 1; place > 0; place -= 0.05f)
        {
            logoimg.color = new Color(1, 1, 1, place);
            yield return new WaitForFixedUpdate();
        }
        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
