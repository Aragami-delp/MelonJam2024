using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MusicSoundManagement;

public class MusicSoundManagement : MonoBehaviour
{

    #region SFX
    [Serializable]
    public enum AUDIOTYPE
    {
        NONE = 0,
        BUTTON_UP,
        BUTTON_DOWN,
        BUTTON_POLARITY,
        SWITCH_POLARITY,
        FOOT_STEP,
        MINE_SCRAP,
        PLACE_SCRAP,
        MAGNET_MOVE,
        MAGNET_SCRAP_ATTACH,
        SCRAP_HIT,
        LAND_MINE,
        CLICK
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

    /// <summary>
    /// Stops one instance of a specifc audiotype
    /// </summary>
    /// <param name="audioType"></param>
    /// <param name="stopAll">stops all instances of that audiotype</param>
    public void StopSFXLoop(AUDIOTYPE audioType, bool stopAll = false)
    {
        foreach (DataAudioType loopSource in _loops)
        {
            if (loopSource.m_audioType == audioType)
            {
                loopSource.m_audioSource.Stop();
                if (!stopAll)
                    return;
                continue;
            }
        }
    }

    /// <summary>
    /// Stops all loops
    /// </summary>
    public void StopSFXLoop()
    {
        foreach (DataAudioType loopSource in _loops)
        {
            loopSource.m_audioSource.Stop();
            continue;
        }
    }

    public void PlaySfx(AUDIOTYPE audioType, bool loop = false, bool allowMultipleLoop = false)
    {
        foreach (AudioReference reference in _sfxReferences.ToList())
        {
            if (reference.m_type == audioType)
            {
                DataAudioType loopSource = null;
                foreach (DataAudioType dataAudioType in _loops)
                {
                    if (dataAudioType.m_audioSource.isPlaying)
                    {
                        if (!allowMultipleLoop && audioType == dataAudioType.m_audioType)
                            return;
                        continue;
                    }
                    loopSource = dataAudioType;
                    break;
                }
                if (loopSource == null)
                {
                    loopSource = Instantiate(_loopDummy, parent: _loopsParent);
                    _loops.Add(loopSource);
                }

                loopSource.m_audioType = audioType;
                loopSource.m_audioSource.clip = reference.m_audioClip;
                loopSource.m_audioSource.volume = reference.m_volMultiplier * _volume;
                loopSource.m_audioSource.loop = loop;
                loopSource.m_audioSource.Play();
                return;
            }
        }
        Debug.LogWarning($"Can't find AudioClip for {audioType.ToString()}");
    }

    [SerializeField]
    private List<AudioReference> _sfxReferences = new();

    [SerializeField]
    private Transform _loopsParent;

    [SerializeField]
    private DataAudioType _loopDummy;
    private List<DataAudioType> _loops = new();

    #endregion

    [SerializeField]
    private AudioClip _inGameMusic;

    [SerializeField]
    private AudioSource _musicSource;

    [SerializeField, InspectorName("Overall Volume Multiplier")]
    private float _volume = 1f;
    [SerializeField, InspectorName("Music Volume")]
    private float _musicvolume = 1f;

    [SerializeField]
    private bool _updateVolume = false;

    public static MusicSoundManagement Instance { get; private set; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopSFXLoop();
        if (!_musicSource.isPlaying)
        {
            _musicSource.clip = _inGameMusic;
            _musicSource.volume = _musicvolume;
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
