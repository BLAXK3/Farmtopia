using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("BGM")]

    public AudioClip BackGroundMusic;

    [Header("UI Audio")]
    public AudioClip Click;
    public AudioClip BackClick;
    public AudioClip Money;
    public AudioClip Star;
    public AudioClip LevelUp;
    public AudioClip Bell;
    public AudioClip Buy;
    public AudioClip Pop;
    public AudioClip Pop2;
    public AudioClip Denied;
    public AudioClip Door;
    public AudioClip LevelUnlock;
    public AudioClip Unlock;

    [Header("Planting Audio")]
    public AudioClip Planting;

    [Header("Harvest")]
    public AudioClip PlantHarvest;
    public AudioClip FishHarvest;
    public AudioClip AnimalHarvest;

    [Header("Animal Audio")]
    public AudioClip Fish;
    public AudioClip Chicken;
    public AudioClip Pig;
    public AudioClip Buffalo;
    public AudioClip Cow;
    public AudioClip Horse;

    private void Start()
    {
        MusicSource.clip = BackGroundMusic;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
