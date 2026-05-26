using UnityEngine;
using System.Collections;

// GameObject único en la escena. Coordina los 4 candados y ejecuta la secuencia final.
public class PadlockSystem : MonoBehaviour
{
    public static PadlockSystem Instance { get; private set; }

    [Header("Puerta norte")]
    [Tooltip("Transform de la puerta que se entreabre al final.")]
    public Transform puerta;
    [Tooltip("Rotación local final de la puerta (entreabierta, no 90°).")]
    public Vector3 rotacionPuertaFinal = new Vector3(0f, 25f, 0f);
    public float duracionAperturaPuerta = 3f;

    [Header("Luz fría del umbral")]
    [Tooltip("Light que aparece detrás de la puerta al final. Empieza desactivada.")]
    public Light luzUmbral;
    public float intensidadLuzFinal = 3f;

    [Header("Cadena")]
    [Tooltip("El GameObject de la cadena en la puerta. Desaparece al abrir el último candado.")]
    public GameObject cadena;

    [Header("Audio final")]
    [Tooltip("La voz que se escucha al abrirse la puerta.")]
    public AudioClip vozFinal;
    public AudioSource audioFinal;

    private int _candadosAbiertos = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (luzUmbral != null)
        {
            luzUmbral.intensity = 0f;
            luzUmbral.enabled   = false;
        }
    }

    void OnEnable()  => Padlock.OnCandadoAbierto += OnCandadoAbierto;
    void OnDisable() => Padlock.OnCandadoAbierto -= OnCandadoAbierto;

    private void OnCandadoAbierto(Padlock candado)
    {
        _candadosAbiertos++;

        // Marcar progreso para que HintSystem sepa que este puzzle está resuelto
        ProgressTracker.Instance?.SetPuzzleSolved(candado.puzzleID);

        Debug.Log($"[PadlockSystem] Candado {candado.puzzleID} abierto ({_candadosAbiertos}/4)");

        if (_candadosAbiertos >= 4)
            StartCoroutine(SecuenciaFinal());
    }

    private IEnumerator SecuenciaFinal()
    {
        yield return new WaitForSeconds(1f);

        // La cadena desaparece al caer el último candado
        if (cadena != null) cadena.SetActive(false);

        // Fade in de la luz fría
        if (luzUmbral != null)
        {
            luzUmbral.enabled = true;
            float elapsed = 0f, duracion = 2f;
            while (elapsed < duracion)
            {
                elapsed += Time.deltaTime;
                luzUmbral.intensity = Mathf.Lerp(0f, intensidadLuzFinal, elapsed / duracion);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Puerta se entreabre
        if (puerta != null)
            StartCoroutine(AbrirPuerta());

        // Voz final (con delay para que coincida con la apertura)
        if (vozFinal != null && audioFinal != null)
        {
            yield return new WaitForSeconds(1.5f);
            audioFinal.PlayOneShot(vozFinal);
        }
    }

    private IEnumerator AbrirPuerta()
    {
        Quaternion inicio  = puerta.localRotation;
        Quaternion destino = Quaternion.Euler(rotacionPuertaFinal);
        float elapsed = 0f;

        while (elapsed < duracionAperturaPuerta)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duracionAperturaPuerta);
            puerta.localRotation = Quaternion.Slerp(inicio, destino, t);
            yield return null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("DEBUG — Simular los 4 candados abiertos")]
    private void Debug_SimularFinal()
    {
        _candadosAbiertos = 4;
        StartCoroutine(SecuenciaFinal());
    }
#endif
}
