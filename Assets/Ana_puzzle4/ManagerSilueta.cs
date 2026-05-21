using UnityEngine;
using System.Collections;

/// <summary>
/// ManagerSilueta — Puzzle 4: La Silueta Fragmentada
///
/// Setup en Inspector:
///   - sombraAdulto / sombraNina: los dos GameObjects de sombra en la pared
///   - tapaPecho: la tapa del compartimento del corazon
///   - objetoLlave: la llave 4 (empieza desactivada)
///   - notaLore: la nota doblada del lore final (empieza desactivada)
///   - dedo: el Transform del dedo de la silueta
///   - audioSource + tonos: un AudioSource y 4 clips de tonos (grave a agudo)
/// </summary>
public class ManagerSilueta : MonoBehaviour
{
    [Header("Sombra proyectada")]
    public GameObject sombraAdulto;
    public GameObject sombraNina;

    [Header("Compartimento del pecho")]
    public Transform tapaPecho;
    public Vector3 rotacionAbierta = new Vector3(-85f, 0f, 0f);

    [Header("Contenido del pecho")]
    public GameObject objetoLlave;
    public GameObject notaLore;

    [Header("Dedo apuntando")]
    public Transform dedo;
    public Vector3 rotacionDedoApuntando = new Vector3(0f, 0f, -45f);

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
        if (sombraNina != null) sombraNina.SetActive(false);
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

        // Tono único según cuántas piezas van colocadas
        ReproducirTono(piezasColocadas);

        piezasColocadas++;

        Debug.Log($"[Silueta] Pieza colocada: {pieza.tipoPieza} ({piezasColocadas}/4)");

        // Si es el corazón, abrir el pecho
        if (pieza.tipoPieza == PiezaSilueta.TipoPieza.Corazon)
        {
            StartCoroutine(AbrirPecho());
        }

        // Si ya están las 4, completar el puzzle
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

        // Silencio — el GDD lo dice explícitamente
        yield return new WaitForSeconds(1.5f);

        // Cambio de sombra: adulto → niña (Alma)
        yield return StartCoroutine(CambiarSombra());

        // Dedo apunta a la puerta
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(AnimarDedo());
    }

    // ──────────────────────────────────────────────
    // CAMBIO DE SOMBRA
    // ──────────────────────────────────────────────

    private IEnumerator CambiarSombra()
    {
        float duracion = 2f;
        float elapsed = 0f;

        // Fade out sombra adulto
        Renderer rAdulto = sombraAdulto != null ? sombraAdulto.GetComponent<Renderer>() : null;
        Renderer rNina = sombraNina != null ? sombraNina.GetComponent<Renderer>() : null;

        if (sombraNina != null) sombraNina.SetActive(true);

        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duracion;

            if (rAdulto != null)
            {
                Color c = rAdulto.material.color;
                rAdulto.material.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t));
            }

            if (rNina != null)
            {
                Color c = rNina.material.color;
                rNina.material.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, 1f, t));
            }

            yield return null;
        }

        if (sombraAdulto != null) sombraAdulto.SetActive(false);
    }

    // ──────────────────────────────────────────────
    // ANIMACIÓN DEL DEDO
    // ──────────────────────────────────────────────

    private IEnumerator AnimarDedo()
    {
        if (dedo == null) yield break;

        Quaternion inicio = dedo.localRotation;
        Quaternion destino = Quaternion.Euler(rotacionDedoApuntando);
        float duracion = 1.5f;
        float elapsed = 0f;

        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duracion);
            dedo.localRotation = Quaternion.Slerp(inicio, destino, t);
            yield return null;
        }

        dedo.localRotation = destino;
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
