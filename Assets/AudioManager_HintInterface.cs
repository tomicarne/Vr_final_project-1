using UnityEngine;

/// <summary>
/// AudioManager — Caránimas VR
/// FRAGMENTO: interfaz mínima que necesita HintSystem.
/// 
/// Este archivo es una guía para Angel. HintSystem.cs llama a:
///     audioManager.PlaySpatialHint(position, clip, volume)
/// 
/// Angel necesita implementar ese método en su AudioManager.cs existente.
/// Si prefiere, puede copiar solo el método y el AudioSource pool de abajo.
/// 
/// El resto del AudioManager (música, efectos de puzzles, candados) va aparte.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ─── Pool de AudioSources para audio espacial ─────────────────────────────
    // Usar un pool evita crear/destruir GameObjects en runtime (importante en Quest 3).

    [Header("Pool de fuentes de audio espacial")]
    [Tooltip("Mínimo 2 — uno para hints, uno libre para efectos simultáneos.")]
    [SerializeField] private int spatialPoolSize = 4;

    private AudioSource[] _spatialPool;
    private int           _poolIndex = 0;

    private void Awake()
    {
        BuildPool();
    }

    private void BuildPool()
    {
        _spatialPool = new AudioSource[spatialPoolSize];
        for (int i = 0; i < spatialPoolSize; i++)
        {
            GameObject go       = new GameObject($"SpatialAudio_{i}");
            go.transform.parent = transform;
            AudioSource src     = go.AddComponent<AudioSource>();
            src.playOnAwake     = false;
            src.spatialBlend    = 1f;
            src.rolloffMode     = AudioRolloffMode.Logarithmic;
            src.maxDistance     = 12f;
            _spatialPool[i]     = src;
        }
    }

    // ─── API que HintSystem llama ─────────────────────────────────────────────

    /// <summary>
    /// Reproduce un AudioClip en una posición del mundo (audio 3D).
    /// HintSystem lo llama con la posición del puzzle activo.
    /// 
    /// Uso desde HintSystem:
    ///     audioManager.PlaySpatialHint(puzzleAnchor.position, shadowClip, 0.8f);
    /// </summary>
    public void PlaySpatialHint(Vector3 worldPosition, AudioClip clip, float volume = 0.8f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlaySpatialHint: clip es null.");
            return;
        }

        AudioSource src          = GetNextPooledSource();
        src.transform.position   = worldPosition;
        src.clip                 = clip;
        src.volume               = volume;
        src.Play();
    }

    // ─── Helpers internos ─────────────────────────────────────────────────────

    private AudioSource GetNextPooledSource()
    {
        // Round-robin: si la fuente actual está reproduciendo, pasa a la siguiente
        for (int i = 0; i < spatialPoolSize; i++)
        {
            int idx = (_poolIndex + i) % spatialPoolSize;
            if (!_spatialPool[idx].isPlaying)
            {
                _poolIndex = (idx + 1) % spatialPoolSize;
                return _spatialPool[idx];
            }
        }

        // Si todas están ocupadas, interrumpir la más antigua
        AudioSource fallback = _spatialPool[_poolIndex];
        fallback.Stop();
        _poolIndex = (_poolIndex + 1) % spatialPoolSize;
        return fallback;
    }

    // ─── Otros métodos del AudioManager van aquí ─────────────────────────────
    // (música de ambiente, SFX de candados, audio de velas, etc.)
    // HintSystem solo necesita PlaySpatialHint — el resto es independiente.
}
