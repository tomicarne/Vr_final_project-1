using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Pool de fuentes de audio espacial")]
    [Tooltip("Mínimo 2 — uno para hints, uno libre para efectos simultáneos.")]
    [SerializeField] private int spatialPoolSize = 4;

    private AudioSource[] _spatialPool;
    private int           _poolIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
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

    /// <summary>
    /// Reproduce un AudioClip en una posición del mundo con audio 3D.
    /// HintSystem lo llama con la posición del puzzle activo.
    /// </summary>
    public void PlaySpatialHint(Vector3 worldPosition, AudioClip clip, float volume = 0.8f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlaySpatialHint: clip es null.");
            return;
        }

        AudioSource src        = GetNextPooledSource();
        src.transform.position = worldPosition;
        src.clip               = clip;
        src.volume             = volume;
        src.Play();
    }

    private AudioSource GetNextPooledSource()
    {
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
}
