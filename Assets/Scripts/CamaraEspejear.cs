using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Mirror : MonoBehaviour
{
    public Camera mirrorCamera;
    public RenderTexture renderTexture;

    void Start()
    {
        mirrorCamera.targetTexture = renderTexture;
        GetComponent<Renderer>().material.mainTexture = renderTexture;
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }
}
