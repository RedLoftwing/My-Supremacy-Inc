using UnityEngine;

public class Audio2DManager : MonoBehaviour
{
    public static Audio2DManager Instance { get; private set; }
    [SerializeField] private AudioSource buildSfxSource;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void PlayBuildSfx() { if (!buildSfxSource.isPlaying) { buildSfxSource.Play(); } }
}