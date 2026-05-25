// PuzzleManager.cs
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Ranuras en orden: 0=M, 1=I, 2=E, 3=D, 4=O")]
    public Ranura[] ranuras = new Ranura[5];

    [Header("Estado")]
    public bool puzzleResuelto = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void VerificarPuzzle()
    {
        // Que todas las ranuras tengan un papel
        foreach (Ranura r in ranuras)
        {
            if (r.LetraActual == null)
            {
                // Falta algún papel, no verificar aún
                return;
            }
        }

        // Verificar que formen "MIEDO"
        bool correcto = true;
        for (int i = 0; i < ranuras.Length; i++)
        {
            if (!ranuras[i].TieneLetraCorrecta())
            {
                correcto = false;
                break;
            }
        }

        if (correcto && !puzzleResuelto)
        {
            puzzleResuelto = true;
            Debug.Log("¡Puzzle resuelto! Las letras forman MIEDO.");
            OnPuzzleResuelto();
        }
        else if (!correcto)
        {
            puzzleResuelto = false;
            Debug.Log("Orden incorrecto. Sigue intentando...");
        }
    }

    private void OnPuzzleResuelto()
    {
        // TODO: implementar lo que se activa al resolver el puzzle
    }
}
