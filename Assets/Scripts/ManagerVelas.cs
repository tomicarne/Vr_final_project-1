using UnityEngine;
using System.Collections.Generic;

public class VelasManager : MonoBehaviour
{
    public List<ControlVela> todasLasVelas = new List<ControlVela>();

    private int siguienteVelaEsperada = 0;
    private bool puzzleResuelto = false;

    private void OnEnable()  => ControlVela.OnVelaPrendida += OnVelaPrendida;
    private void OnDisable() => ControlVela.OnVelaPrendida -= OnVelaPrendida;

    private void OnVelaPrendida(ControlVela velaQueSeEncendio)
    {
        if (puzzleResuelto) return;

        if (velaQueSeEncendio.numeroVela == siguienteVelaEsperada)
        {
            siguienteVelaEsperada++;
            Debug.Log($"[Velas] Vela {velaQueSeEncendio.numeroVela} correcta ({siguienteVelaEsperada}/{todasLasVelas.Count})");

            if (siguienteVelaEsperada >= todasLasVelas.Count)
                PuzzleCompletado();
        }
        else
        {
            ApagarTodas();
        }
    }

    private void PuzzleCompletado()
    {
        puzzleResuelto = true;
        Debug.Log("[Velas] Secuencia correcta — puzzle completado");

        // La vela marcada como "del miedo" se derrite y revela nota + llave
        foreach (ControlVela vela in todasLasVelas)
        {
            if (vela.esVelaDelMiedo)
            {
                vela.Derretir();
                break;
            }
        }

        // Notificar al sistema de progreso
        ProgressTracker.Instance?.SetPuzzleSolved(PuzzleID.Velas);
    }

    private void ApagarTodas()
    {
        foreach (ControlVela vela in todasLasVelas)
            vela.Apagar();
        siguienteVelaEsperada = 0;
        Debug.Log("[Velas] Secuencia incorrecta — apagando todo");
    }
    #if UNITY_EDITOR
[ContextMenu("DEBUG — Simular puzzle completado")]
private void Debug_Completar() => PuzzleCompletado();
#endif
}