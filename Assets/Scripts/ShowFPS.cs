using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        GetComponent<Text>().text = (1f/Time.deltaTime).ToString();
    }
}
