using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource chillSource;
    [SerializeField] AudioSource dangerSource;

    private void Start()
    {
        chillSource.volume = 1;
        dangerSource.volume = 0;
    }

    public void PlayerDetected(bool detected)
    {
        if (detected)
        {
            chillSource.volume = 1;
            dangerSource.volume = 1;
        }
        else
        {
            chillSource.volume = 1;
            dangerSource.volume = 0;
        }
    }
}