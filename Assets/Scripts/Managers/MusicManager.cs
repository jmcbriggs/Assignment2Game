using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    [SerializeField] private EventReference _mainMusic;
    [SerializeField] private EventReference _forestMusic;
    [SerializeField] private EventReference _caveMusic;
    [SerializeField] private EventReference _finalMusic;
    [SerializeField] private EventReference _winScreenMusic;
    [SerializeField] private EventReference _startBattleSting;
    [SerializeField] private EventReference _winBattleSting;
    [SerializeField] private EventReference _loseBattleSting;
    FMOD.Studio.EventInstance _mainInstance;
    FMOD.Studio.EventInstance _forestInstance;
    FMOD.Studio.EventInstance _caveInstance;
    FMOD.Studio.EventInstance _finalInstance;
    FMOD.Studio.EventInstance _winScreenInstance;
    FMOD.Studio.EventInstance _startBattleInstance;
    FMOD.Studio.EventInstance _winBattleInstance;
    FMOD.Studio.EventInstance _loseBattleInstance;
    FMOD.Studio.EventInstance _currentInstance;
    FMOD.Studio.EventInstance _currentSting;
    StudioEventEmitter _fmodEmitter;

    int mute = 0;
    bool stingPlaying = false;
    public enum MusicState
    {
        Main,
        Forest,
        Cave,
        Final,
        Win
    }

    public enum Sting
    {
        Start,
        Win,
        Lose
    }
    // Start is called before the first frame update
    void Start()
    {
        _mainInstance = RuntimeManager.CreateInstance(_mainMusic);
        _forestInstance = RuntimeManager.CreateInstance(_forestMusic);
        _caveInstance = RuntimeManager.CreateInstance(_caveMusic);
        _finalInstance = RuntimeManager.CreateInstance(_finalMusic);
        _winScreenInstance = RuntimeManager.CreateInstance(_winScreenMusic);
        _startBattleInstance = RuntimeManager.CreateInstance(_startBattleSting);
        _winBattleInstance = RuntimeManager.CreateInstance(_winBattleSting);
        _loseBattleInstance = RuntimeManager.CreateInstance(_loseBattleSting);
        _fmodEmitter = GetComponent<StudioEventEmitter>();
        _mainInstance.start();
        _currentInstance = _mainInstance;
        ChangeMute();

    }

    public void ChangeMusic(MusicState state)
    {
        switch (state)
        {
            case MusicState.Main:
                _mainInstance.start();
                mute = 1;
                _forestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _caveInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _finalInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _winScreenInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentInstance = _mainInstance;
                break;
            case MusicState.Forest:
                _forestInstance.start();
                _mainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _caveInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _finalInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _winScreenInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentInstance = _forestInstance;
                break;
            case MusicState.Cave:
                _caveInstance.start();
                _mainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _forestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _finalInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _winScreenInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentInstance = _caveInstance;
                break;
            case MusicState.Final:
                _finalInstance.start();
                _mainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _forestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _caveInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _winScreenInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentInstance = _finalInstance;
                break;
            case MusicState.Win:
                _winScreenInstance.start();
                _mainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _forestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _caveInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _finalInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _currentInstance = _winScreenInstance;
                break;
        }
    }

    public void ChangeMute()
    {
        if (mute == 0)
        {
            mute = 1;
            _currentInstance.setParameterByName("Mute", mute);
        }
        else
        {
            mute = 0;
            _currentInstance.setParameterByName("Mute", mute);
        }
    }

    public void PlaySting(Sting sting)
    {
        stingPlaying = true;
        _startBattleInstance.setParameterByName("Volume", GameController.Instance.GetMusicVolume());
        _winBattleInstance.setParameterByName("Volume", GameController.Instance.GetMusicVolume());
        _loseBattleInstance.setParameterByName("Volume", GameController.Instance.GetMusicVolume());
        switch (sting)
        {
            case Sting.Start:
                _startBattleInstance.start();
                _currentSting = _startBattleInstance;
                break;
            case Sting.Win:
                _winBattleInstance.start();
                _currentSting = _winBattleInstance;
                break;
            case Sting.Lose:
                _loseBattleInstance.start();
                _currentSting = _loseBattleInstance;
                break;
        }
        _currentInstance.setParameterByName("Volume", 0);
        StartCoroutine(CheckSting());

    }

    IEnumerator CheckSting()
    {
        _currentSting.getTimelinePosition(out int position);
        FMOD.RESULT result = _currentSting.getDescription(out FMOD.Studio.EventDescription description);
        if (result != FMOD.RESULT.OK)
        {
            yield break;
        }
        description.getLength(out int length);
        while (position < (length * 0.7))
        {
            _currentSting.getTimelinePosition(out position);
            yield return null;
        }
        while (_currentInstance.getParameterByName("Volume", out float volume) == FMOD.RESULT.OK && volume <= GameController.Instance.GetMusicVolume())
        {
            volume += Time.deltaTime / 3;
            _currentInstance.setParameterByName("Volume", volume);
            if (_currentSting.getParameterByName("Volume", out float stingVolume) == FMOD.RESULT.OK)
            {
                if(stingVolume > 0)
                {
                    stingVolume -= Time.deltaTime / 3;
                    _currentSting.setParameterByName("Volume", stingVolume);
                }
            }
            yield return null;
        }
        stingPlaying = false;
    }


    private void Update()
    {
        if (GameController.Instance != null && !stingPlaying)
        {
            _currentInstance.setParameterByName("Volume", GameController.Instance.GetMusicVolume());
        }
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}
