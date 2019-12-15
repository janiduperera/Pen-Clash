using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Singleton
    public static UIController Instance;
    private UIController() { }

    private void Awake()
    {
        Instance = this;
    }
    #endregion Singleton

    void Start()
    {
        m_StartScreen = StartPanel.transform.Find("StartScreen").gameObject;
        m_TutorialObj = StartPanel.transform.Find("Tutorial").gameObject;
        m_GameStartCounterObj = StartPanel.transform.Find("CounterTxt").gameObject;

        m_RaceProgressBar = GamePanel.transform.Find("RaceProgress").gameObject.GetComponent<Image>();
        m_PlayerPlaceTxt = GamePanel.transform.Find("PlayerPlaceTxt").gameObject.GetComponent<Text>();
        m_PlayerPlaceTxtAnimator = m_PlayerPlaceTxt.gameObject.GetComponent<Animator>();
        m_FoundWordTxt = GamePanel.transform.Find("FoundWordTxt").gameObject.GetComponent<Text>();
    }

    //Delegate to run when Count down timer is finished. 
    public delegate void OnCountDownFinishDelegate();
    public event OnCountDownFinishDelegate OnCountDownFinish;

    #region Start Panel
    public GameObject StartPanel;
    private GameObject m_StartScreen;
    private GameObject m_TutorialObj;
    private GameObject m_GameStartCounterObj;

    private bool m_IsPlayClicked = false;
    public void OnPlayButtonClicked()
    {
        if (!m_IsPlayClicked)
        {
            m_IsPlayClicked = true;
            StartCoroutine(AfterPlayButtonClick());
        }
    }

    IEnumerator AfterPlayButtonClick()
    {
        yield return new WaitForSeconds(0.2f);
        m_StartScreen.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        m_TutorialObj.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        float m_Val = 0;
        for (int i = 3; i > 0; i--)
        {
            m_Val = 0;
            m_GameStartCounterObj.transform.localScale = new Vector3(0, 0, 1);
            m_GameStartCounterObj.GetComponent<Text>().text = i + "";

            while (m_GameStartCounterObj.transform.localScale.x < 1f)
            {
                m_Val += 0.2f;
                m_GameStartCounterObj.transform.localScale = new Vector3(m_Val, m_Val, 1);
                yield return new WaitForEndOfFrame();
            }

            AudioController.Instance.PlayEffectAudio("CountDown");
            yield return new WaitForSeconds(0.15f);

            while (m_GameStartCounterObj.transform.localScale.x > 0)
            {
                m_Val -= 0.01f;
                m_GameStartCounterObj.transform.localScale = new Vector3(m_Val, m_Val, 1);
                yield return new WaitForEndOfFrame();
            }
        }
        m_GameStartCounterObj.transform.localScale = new Vector3(0, 0, 1);
        yield return new WaitForSeconds(1);

        m_GameStartCounterObj.GetComponent<Text>().text = "GO!";
        m_Val = 0;
        while (m_GameStartCounterObj.transform.localScale.x < 1f)
        {
            m_Val += 0.1f;
            m_GameStartCounterObj.transform.localScale = new Vector3(m_Val, m_Val, 1);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);
        m_GameStartCounterObj.GetComponent<Text>().text = "";
        StartPanel.SetActive(false);
        m_IsPlayClicked = false;

        StartCoroutine(StartPlayingBackgroundAudio());

        OnCountDownFinish();
    }

    IEnumerator StartPlayingBackgroundAudio()
    {
        AudioController.Instance.PlayEffectAudio("FireCracker");
        while (AudioController.Instance.IsEffectAudioPlaying)
        {
            yield return null;
        }
        AudioController.Instance.PlayBackgroundAudio();
    }
    #endregion Start Panel


    #region Settings Panel
    public GameObject SettingsPanel;
    private Color m_ToggleOnColor = new Color(255f / 255f, 186f / 255f, 49f / 255f);
    private Color m_ToggleOffColor = new Color(240f / 255f, 76f / 255f, 102f / 255f);

    public void OnSettingsBtnClicked()
    {
        SettingsPanel.SetActive(true);
        SettingsPanel.transform.Find("TotalCoinsTxt").gameObject.GetComponent<Text>().text = GameController.Instance.TotalCoinsCount + "";

        GameObject m_MusicToggle = SettingsPanel.transform.Find("MusicToggle").gameObject;
        GameObject m_SoundToggle = SettingsPanel.transform.Find("SoundToggle").gameObject;

        if (AudioController.Instance.WhatIsSoundStatus() == 1)
        {
            m_SoundToggle.GetComponent<Animator>().SetBool("On", true);
            m_SoundToggle.GetComponent<Image>().color = m_ToggleOnColor;
            m_SoundToggle.transform.GetChild(0).gameObject.GetComponent<Text>().text = "ON";
        }
        else
        {
            m_SoundToggle.GetComponent<Animator>().SetBool("On", false);
            m_SoundToggle.GetComponent<Image>().color = m_ToggleOffColor;
            m_SoundToggle.transform.GetChild(0).gameObject.GetComponent<Text>().text = "OFF";
        }

        if (AudioController.Instance.WhatIsMusicStatus() == 1)
        {
            m_MusicToggle.GetComponent<Animator>().SetBool("On", true);
            m_MusicToggle.GetComponent<Image>().color = m_ToggleOnColor;
            m_MusicToggle.transform.GetChild(0).gameObject.GetComponent<Text>().text = "ON";
        }
        else
        {
            m_MusicToggle.GetComponent<Animator>().SetBool("On", false);
            m_MusicToggle.GetComponent<Image>().color = m_ToggleOffColor;
            m_MusicToggle.transform.GetChild(0).gameObject.GetComponent<Text>().text = "OFF";
        }
    }

    public void OnSettingsBackBtnClicked()
    {
        SettingsPanel.SetActive(false);
    }

    private bool m_IsToggleClicked = false;
    public void OnMusicToggleClicked(Animator _ToggleAnimator)
    {
        if (!m_IsToggleClicked)
        {
            m_IsToggleClicked = true;
            StartCoroutine(SwichToggle(true, _ToggleAnimator));
        }
    }

    public void OnSoundToggleClicked(Animator _ToggleAnimator)
    {
        if (!m_IsToggleClicked)
        {
            m_IsToggleClicked = true;
            StartCoroutine(SwichToggle(false, _ToggleAnimator));
        }
    }

    private IEnumerator SwichToggle(bool _isMusic, Animator _ToggleAnimator)
    {
        if (_ToggleAnimator.GetBool("On"))
        {
            _ToggleAnimator.SetBool("On", false);
            _ToggleAnimator.gameObject.GetComponent<Image>().color = m_ToggleOffColor;
            _ToggleAnimator.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "OFF";

            if (_isMusic)
            {
                AudioController.Instance.SwitchMusicOnOff(0);
            }
            else
            {
                AudioController.Instance.SwitchSoundOnOff(0);
            }
        }
        else
        {
            _ToggleAnimator.SetBool("On", true);
            _ToggleAnimator.gameObject.GetComponent<Image>().color = m_ToggleOnColor;
            _ToggleAnimator.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "ON";

            if (_isMusic)
            {
                AudioController.Instance.SwitchMusicOnOff(1);
            }
            else
            {
                AudioController.Instance.SwitchSoundOnOff(1);
            }
        }

        yield return new WaitForSeconds(0.2f); // Wait till toggle animaiton finishes
        m_IsToggleClicked = false;
    }

    #endregion Settings Panel

    #region Game Panel
    public GameObject GamePanel;
    public GameObject CoinsTxtObj;
    private Image m_RaceProgressBar;
    private Text m_PlayerPlaceTxt;
    private Animator m_PlayerPlaceTxtAnimator;
    private bool m_IsTxtAnimatorPlaying = false;
    private int m_PreviousPlace = 0;
    public void UpdatePlayerPlace(int _place)
    {
        if (_place == 1)
        {
            m_PlayerPlaceTxt.text = _place + " st";
        }
        else if (_place == 2)
        {
            m_PlayerPlaceTxt.text = _place + " nd";
        }
        else if (_place == 3)
        {
            m_PlayerPlaceTxt.text = _place + " rd";
        }
        else
        {
            m_PlayerPlaceTxt.text = _place + " th";
        }

        if (m_PreviousPlace != _place && !m_IsTxtAnimatorPlaying)
        {
            m_IsTxtAnimatorPlaying = true;
            m_PreviousPlace = _place;
            m_PlayerPlaceTxtAnimator.SetBool("setPlace", true);
            StartCoroutine(AfterPlaceTxtAnimationEnd());
        }
    }

    IEnumerator AfterPlaceTxtAnimationEnd()
    {
        yield return new WaitForSeconds(0.3f);
        m_PlayerPlaceTxtAnimator.SetBool("setPlace", false);
        m_IsTxtAnimatorPlaying = false;
    }

    public void UpdateRaceProgress(float _amount)
    {
        m_RaceProgressBar.fillAmount = _amount;
    }

    private Text m_FoundWordTxt;
    public string SetFoundWordTxt
    {
        set { m_FoundWordTxt.text = value; }
    }
    #endregion Game Panel

    #region Game Over and Pause
    public GameObject GameOverAnimation;
    public IEnumerator BringInGameOverPenAnimation(Vector3 _pos)
    {
        GameOverAnimation.SetActive(true);
        GameOverAnimation.transform.position = new Vector3(_pos.x, GameOverAnimation.transform.position.y, _pos.z);
        GameOverAnimation.transform.Find("GameOverPen").gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        GameOverAnimation.transform.Find("GameOverPen").GetChild(0).gameObject.SetActive(true);
    }

    public GameObject PausePanel;
    public void BringInGamePausePanel()
    {
        PausePanel.SetActive(true);
        PausePanel.transform.Find("PositionTxt").gameObject.GetComponent<Text>().text = m_PlayerPlaceTxt.text;

        if (m_PlayerPlaceTxt.text == "1 st" && GameController.Instance.FoundLetterCount == GameController.Instance.RandomWordLetterCount)
        {
            PausePanel.transform.Find("WinOrLoose").gameObject.GetComponent<Text>().text = "YOU WIN";

            //Give Reward
            GameObject m_WordFoundObj = PausePanel.transform.Find("WordFound").gameObject;
            m_WordFoundObj.SetActive(true);
            m_WordFoundObj.transform.GetChild(0).gameObject.GetComponent<Text>().text = GameController.Instance.RandomWord;
            StartCoroutine(RewordCoinsAnimater(m_WordFoundObj.transform.GetChild(2).gameObject.GetComponent<Text>()));

        }
        else if (m_PlayerPlaceTxt.text == "1 st")
        {
            GameController.Instance.SetTotalCount(GameController.Instance.CurrentGameCoinCount);
            PausePanel.transform.Find("WinOrLoose").gameObject.GetComponent<Text>().text = "YOU WIN";
        }
        else
        {
            GameController.Instance.SetTotalCount(GameController.Instance.CurrentGameCoinCount);
            PausePanel.transform.Find("WinOrLoose").gameObject.GetComponent<Text>().text = "YOU LOOSE";
        }
    }

    IEnumerator RewordCoinsAnimater(Text _txt)
    {
        _txt.text = GameController.Instance.CurrentGameCoinCount + "";
        yield return new WaitForSeconds(2.3f);

        int m_RewardAmount = 200;
        int m_IncrementTime = 0;
        int m_Count = GameController.Instance.CurrentGameCoinCount;

        int m_LoopLimit = 10;
        int m_IncrementalLevel = m_RewardAmount / 10;

        if (m_IncrementalLevel < 10)
        {
            m_LoopLimit = m_RewardAmount / 10;
            m_IncrementalLevel = 1;
        }

        while (m_IncrementTime < m_LoopLimit)
        {
            m_IncrementTime++;
            m_Count += m_IncrementalLevel;

            _txt.text = "" + m_Count;

            AudioController.Instance.PlayEffectAudio("Reward");

            yield return new WaitForSeconds(0.08f);

        }

        GameController.Instance.SetTotalCount(GameController.Instance.CurrentGameCoinCount + m_RewardAmount);
        _txt.text = (GameController.Instance.CurrentGameCoinCount + m_RewardAmount) + "";
    }

    public void OnReplayBtnClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
    }

    public void OnExitBtnClicked()
    {
        Application.Quit();
    }
    #endregion Game Over and Pause
}
