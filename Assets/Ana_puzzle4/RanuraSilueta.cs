using UnityEngine;

/// <summary>
/// Poner este script en cada ranura del marco de la silueta.
/// La ranura detecta cuando la pieza correcta entra en su trigger.
/// </summary>
public class RanuraSilueta : MonoBehaviour
{
    public PiezaSilueta.TipoPieza tipoQueAcepta;
    public bool ocupada = false;

    void OnTriggerEnter(Collider other)
    {
        if (ocupada) return;

        PiezaSilueta pieza = other.GetComponent<PiezaSilueta>();
        if (pieza == null || pieza.estaColocada) return;

        if (pieza.tipoPieza == tipoQueAcepta)
        {
            ocupada = true;
            pieza.Colocar(this.transform);
        }
    }
}
