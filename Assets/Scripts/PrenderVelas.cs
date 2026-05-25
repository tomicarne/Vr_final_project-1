using UnityEngine;

public class PrenderVelas : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private ControlVela VelaActual;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Vela")
        {
           VelaActual = other.gameObject.GetComponent<ControlVela>();

           VelaActual.Prender();
        }
    }
}
