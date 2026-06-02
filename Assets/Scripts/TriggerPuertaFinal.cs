using UnityEngine;

public class TriggerPuertaFinal : MonoBehaviour
{
    public FinalManager finalManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            finalManager.JugadorEntroPuerta();
    }
}
