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

        // Detener físicas antes de mover para evitar que el SDK de VR interfiera
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Deshabilitar el componente de agarre para soltar el objeto del SDK
        // (compatible con XR Interaction Toolkit y Meta/Oculus SDK)
        foreach (var mb in GetComponents<MonoBehaviour>())
        {
            if (mb == this) continue;
            var type = mb.GetType().FullName;
            if (type.Contains("Grab") || type.Contains("Interactable") || type.Contains("Grabbable"))
                mb.enabled = false;
        }

        // Snap a la ranura usando espacio local para evitar problemas de escala
        transform.SetParent(ranura, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Convertir collider a trigger para que no bloquee físicamente pero tampoco desaparezca
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
            col.enabled = true;
        }

        OnPiezaColocada?.Invoke(this);
    }
}
