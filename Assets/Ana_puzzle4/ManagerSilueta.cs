using UnityEngine;
using System.Collections;

/// <summary>
/// ManagerSilueta — Puzzle 4: La Silueta Fragmentada
///
/// Setup en Inspector:
///   - tapaPecho: la tapa del compartimento del corazon
///   - objetoLlave: la llave 4 (empieza desactivada)
///   - notaLore: la nota doblada del lore final (empieza desactivada)
///   - dedo: el Transform del dedo de la silueta
///   - audioSource + tonos: un AudioSource y 4 clips de tonos (grave a agudo)
/// </summary>
public class ManagerSilueta : MonoBehaviour
{
    [Header("Compartimento del pecho")]
    public Transform tapaPecho;
    public Vector3 rotacionAbierta = new Vector3(-85f, 0f, 0f);

    [Header("Contenido del pecho")]
    public GameObject objetoLlave;
    public GameObject notaLore;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] tonosPorPieza = new AudioClip[4]; // grave → agudo

    private int piezasColocadas = 0;
    private bool puzzleCompletado = false;

    // ──────────────────────────────────────────────
    // INICIALIZACIÓN
    // ──────────────────────────────────────────────

    void Start()
    {
        if (objetoLlave != null) objetoLlave.SetActive(false);
        if (notaLore != null) notaLore.SetActive(false);
    }

    void OnEnable()
    {
        PiezaSilueta.OnPiezaColocada += OnPiezaColocada;
    }

    void OnDisable()
    {
        PiezaSilueta.OnPiezaColocada -= OnPiezaColocada;
    }

    // ──────────────────────────────────────────────
    // CUANDO SE COLOCA UNA PIEZA
    // ──────────────────────────────────────────────

    private void OnPiezaColocada(PiezaSilueta pieza)
    {
        if (puzzleCompletado) return;

        ReproducirTono(piezasColocadas);
        piezasColocadas++;

        Debug.Log($"[Silueta] Pieza colocada: {pieza.tipoPieza} ({piezasColocadas}/4)");

        if (piezasColocadas >= 4)
        {
            StartCoroutine(SecuenciaCompletado());
        }
    }

    // ──────────────────────────────────────────────
    // ABRIR COMPARTIMENTO DEL PECHO
    // ──────────────────────────────────────────────

    private IEnumerator AbrirPecho()
    {
        if (tapaPecho == null) yield break;

        yield return new WaitForSeconds(0.5f);

        Quaternion inicio = tapaPecho.localRotation;
        Quaternion destino = Quaternion.Euler(rotacionAbierta);
        float duracion = 1.2f;
        float elapsed = 0f;

        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duracion);
            tapaPecho.localRotation = Quaternion.Slerp(inicio, destino, t);
            yield return null;
        }

        tapaPecho.localRotation = destino;

        // Revelar contenido del pecho
        if (objetoLlave != null) objetoLlave.SetActive(true);
        if (notaLore != null) notaLore.SetActive(true);
    }

    // ──────────────────────────────────────────────
    // SECUENCIA DE COMPLETADO
    // ──────────────────────────────────────────────

    private IEnumerator SecuenciaCompletado()
    {
        puzzleCompletado = true;
        ProgressTracker.Instance?.SetPuzzleSolved(PuzzleID.Silueta);
        yield return StartCoroutine(AbrirPecho());
    }

    // ──────────────────────────────────────────────
    // AUDIO
    // ──────────────────────────────────────────────

    private void ReproducirTono(int indice)
    {
        if (audioSource == null) return;
        if (tonosPorPieza == null || indice >= tonosPorPieza.Length) return;
        if (tonosPorPieza[indice] == null) return;

        audioSource.PlayOneShot(tonosPorPieza[indice]);
    }

    // ──────────────────────────────────────────────
    // DEBUG
    // ──────────────────────────────────────────────

    [ContextMenu("Simular completado")]
    private void DEBUG_Completar()
    {
        piezasColocadas = 4;
        StartCoroutine(SecuenciaCompletado());
    }
}
