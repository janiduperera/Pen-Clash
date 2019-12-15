using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    #region Singleton
    public static AudioController Instance;
    private AudioController() { }

    private void Awake()
    {
        Instance = this;
        InitiateSound();
        InitiateMusic();
    }
    #endregion Singleton

    #region Background Sound
    public AudioSource BackgroundAudio;

    private void InitiateSound()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            BackgroundAudio.mute = false;
        }
        else
        {
            BackgroundAudio.mute = true;
        }
    }

    public void SwitchSoundOnOff(int _status)
    {
        PlayerPrefs.SetInt("Sound", _status);
        if (_status == 1)
        {
            BackgroundAudio.mute = false;
        }
        else
        {
            BackgroundAudio.mute = true;
        }
    }

    public int WhatIsSoundStatus()
    {
        return PlayerPrefs.GetInt("Sound", 1);
    }

    public void PlayBackgroundAudio()
    {
        if(!BackgroundAudio.isPlaying)
        {
            BackgroundAudio.Play();
        }
    }

    public void StopBackgroundAudio()
    {
        BackgroundAudio.Stop();
    }
    #endregion Background Sound

    #region Effects Sound
    public AudioSource EffectsAudio;
    public AudioClip ButtonClickAudioClip;
    public AudioClip RewardAudioClip;
    public AudioClip CoinCollectAudioClip;
    public AudioClip CountDownAudioClip;
    public AudioClip GameStartFireCrackerClip;
    public AudioClip LetterFoundAudioClip;
    public AudioClip GameOverAudioClip;
    private AudioClip m_ChoosenAudioClip;

    private void InitiateMusic()
    {
        if (PlayerPrefs.GetInt("Music", 1) == 1)
        {
            EffectsAudio.mute = false;
        }
        else
        {
            EffectsAudio.mute = true;
        }
    }

    public void SwitchMusicOnOff(int _status)
    {
        PlayerPrefs.SetInt("Music", _status);
        if (_status == 1)
        {
            EffectsAudio.mute = false;
        }
        else
        {
            EffectsAudio.mute = true;
        }
    }

    public int WhatIsMusicStatus()
    {
        return PlayerPrefs.GetInt("Music", 1);
    }

    public bool IsEffectAudioPlaying
    {
        get { return EffectsAudio.isPlaying; }
    }

    public void PlayEffectAudio(string _audioClip)
    {
        if (EffectsAudio.isPlaying)
        {
            return;
        }
        switch (_audioClip)
        {
            case "ButtonClick": m_ChoosenAudioClip = ButtonClickAudioClip;
                break;
            case "CoinsCollect": m_ChoosenAudioClip = CoinCollectAudioClip;
                break;
            case "Reward":m_ChoosenAudioClip = RewardAudioClip;
                break;
            case "CountDown":m_ChoosenAudioClip = CountDownAudioClip;
                break;
            case "FireCracker": m_ChoosenAudioClip = GameStartFireCrackerClip;
                break;
            case "LetterFound": m_ChoosenAudioClip = LetterFoundAudioClip;
                break;
            case "GameOver":m_ChoosenAudioClip = GameOverAudioClip;
                break;
            default: Debug.Log("No such audio clip");
                break;
        }

        EffectsAudio.PlayOneShot(m_ChoosenAudioClip);
    }
    #endregion Effects Sound
}
