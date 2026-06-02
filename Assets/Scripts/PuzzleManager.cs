using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Ranuras en orden: 0=M, 1=I, 2=E, 3=D, 4=O")]
    public Ranura[] ranuras = new Ranura[5];

    [Header("Al resolver el puzzle")]
    public GameObject llave3;
    public GameObject notaLore;
    public GameObject piezaMano;

    [Header("Estado")]
    public bool puzzleResuelto = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void VerificarPuzzle()
    {
        foreach (Ranura r in ranuras)
        {
            if (r.LetraActual == null) return;
        }

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
            Debug.Log("[Diario] ¡MIEDO correcto! Puzzle resuelto.");
            OnPuzzleResuelto();
        }
    }

    private void OnPuzzleResuelto()
    {
        if (llave3 != null)    llave3.SetActive(true);
        if (notaLore != null)  notaLore.SetActive(true);
        if (piezaMano != null) piezaMano.SetActive(true);

        ProgressTracker.Instance?.SetPuzzleSolved(PuzzleID.Diario);
    }

    #if UNITY_EDITOR
[ContextMenu("DEBUG — Simular puzzle resuelto")]
private void Debug_Resolver() => OnPuzzleResuelto();
#endif
}
