using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip BackGround;
    public AudioClip TakeDamage;
    public AudioClip CorrectAns;
    public AudioClip WrongAns;
    public AudioClip BtnClick;
    public AudioClip BtnBack;
    public AudioClip Gameover;
    public AudioClip Dead;
    public AudioClip Healpotion;
    public AudioClip ATKpotion;
    public AudioClip Emptypotion;
    public AudioClip Victory;
    public AudioClip Denied;

    private void Start()
    {
        MusicSource.clip = BackGround;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
