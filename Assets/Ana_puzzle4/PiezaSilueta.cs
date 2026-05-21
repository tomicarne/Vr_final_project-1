using System;
using UnityEngine;

public class PiezaSilueta : MonoBehaviour
{
    public enum TipoPieza { Ojo, Corazon, Boca, Mano }

    public TipoPieza tipoPieza;
    public bool estaColocada = false;

    public static event Action<PiezaSilueta> OnPiezaColocada;

    public void Colocar(Transform ranura)
    {
        if (estaColocada) return;

        estaColocada = true;

        // Snap a la ranura
        transform.position = ranura.position;
        transform.rotation = ranura.rotation;
        transform.SetParent(ranura);

        // Desactivar físicas para que no se mueva
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Desactivar el collider de agarre
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        OnPiezaColocada?.Invoke(this);
    }
}
