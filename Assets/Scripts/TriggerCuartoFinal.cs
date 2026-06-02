using UnityEngine;

public class TriggerCuartoFinal : MonoBehaviour
{
    public FinalManager finalManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            finalManager.JugadorEntroCuarto();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            finalManager.JugadorSalioCuarto();
    }
}
