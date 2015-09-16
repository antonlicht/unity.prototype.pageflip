using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PageFlipController : MonoBehaviour
{
    [Range(-1f,-1000)]
    public float A = -1f;
    [Range(Mathf.PI/2f, Mathf.PI)]
    public float Theta = 2.69f;
    [Range(0, -Mathf.PI)]
    public float Rho = 0f;

    public List<PageCurlModifier> PageCurlModifiers = new List<PageCurlModifier>();

    void Update()
    {
        foreach (var modifier in PageCurlModifiers)
        {
            modifier.A = A;
            modifier.Theta = Theta;
            modifier.Rho = Rho;
        }
    }
}
