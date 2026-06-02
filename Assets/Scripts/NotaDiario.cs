using UnityEngine;

public class NotaDiario : MonoBehaviour
{
    [Header("Tipo de nota")]
    public bool esClavePuzzle = false;
    public bool esGuia = false;

    [Header("Letra clave (solo si esClavePuzzle = true)")]
    [Tooltip("La letra inicial que el jugador debe notar: J, N, O, T")]
    public char letraClave;

    [Header("Referencia")]
    public ManagerPaginas managerPaginas;

    private bool _contada = false;

    // Llama esto desde el evento OnSelectEntered del XR Grab Interactable
    public void AlAgarrar()
    {
        if (_contada) return;
        if (!esClavePuzzle && !esGuia) return;

        _contada = true;

        if (managerPaginas != null)
            managerPaginas.paginaCount++;
    }
}
