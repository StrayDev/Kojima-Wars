using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSpinScript : MonoBehaviour
{
    float speed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spinThatCam());
    }

    private IEnumerator spinThatCam() {
        while (true) {
            gameObject.transform.Rotate(new Vector3(0,speed,0));
            yield return null;
        }
    }
}
