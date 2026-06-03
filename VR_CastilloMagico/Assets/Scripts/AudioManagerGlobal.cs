using UnityEngine;

public class AudioManagerGlobal : MonoBehaviour
{
    public static AudioManagerGlobal Instance;

    [SerializeField] AudioSource ambientSource; 
    public AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            ambientSource.clip = backgroundMusic;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }
}