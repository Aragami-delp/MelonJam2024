using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicSoundManagement : MonoBehaviour
{

    #region SFX
    [Serializable]
    public enum AUDIOTYPE
    {
        NONE = 0,
    }

    [Serializable]
    public struct AudioReference
    {
        [SerializeField] public AUDIOTYPE m_type;
        [SerializeField] public AudioClip m_audioClip;
        [SerializeField] public float m_volMultiplier;
    }

    // Call MusicSoundManagement.Instance.PlayAudio(AUDIOTYPE) to play a sound

    /// <summary>
    /// Plays an AudioClip on a given Source
    /// </summary>
    /// <param name="audioType"></param>
    public void PlayAudio(AUDIOTYPE audioType)
    {
        foreach (AudioReference reference in _sfxReferences.ToList())
        {
            if (reference.m_type == audioType)
            {
                // Jaja spielt sfx auf music source - so what
                _musicSource.PlayOneShot(reference.m_audioClip, reference.m_volMultiplier * _volume);
                return;
            }
        }
        Debug.LogWarning($"Can't find AudioClip for {audioType.ToString()}");
    }

    public void PlaySfxSource(AUDIOTYPE audioType)
    {
        foreach (AudioReference reference in _sfxReferences.ToList())
        {
            if (reference.m_type == audioType)
            {
                _sfxSource.clip = reference.m_audioClip;
                _sfxSource.volume = reference.m_volMultiplier * _volume;
                _sfxSource.Play();
                return;
            }
        }
        Debug.LogWarning($"Can't find AudioClip for {audioType.ToString()}");
    }

    public void StopSFXSource()
    {
        _sfxSource.Stop();
    }

    [SerializeField]
    private List<AudioReference> _sfxReferences = new();

    [SerializeField]
    private AudioSource _sfxSource;

    #endregion

    [SerializeField]
    private AudioClip _upgradeMusic, _inGameMusic;

    [SerializeField]
    private AudioSource _musicSource;

    [SerializeField, InspectorName("Vol Multiplier")]
    private float _volume = 1f;

    [SerializeField]
    private bool _updateVolume = false;

    public static MusicSoundManagement Instance { get; private set; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            _musicSource.clip = _upgradeMusic;
            _musicSource.Play();
        }
        else
        {
            _musicSource.clip = _inGameMusic;
            _musicSource.Play();
        }
    }

    private void Awake()
    {
        GameObject[] musicObject = GameObject.FindGameObjectsWithTag("GameMusic");
        if (musicObject.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        AdjustVolume(_volume);
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (_updateVolume)
        {
            AudioListener.volume = _volume;
            _updateVolume = false;
        }
    }

    public void AdjustVolume(float newVolume)
    {
        _volume = newVolume;
        _updateVolume = true;
    }
}
