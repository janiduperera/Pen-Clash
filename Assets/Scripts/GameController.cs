using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region Singleton
    public static GameController Instance;
    private GameController() { }

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }
    #endregion Singleton


    // Start is called before the first frame update
    void Start()
    {
        // Set Platforms
        SetPlatforms();

        //Set Obstacles
        SetObstacles();

        //Set Props
        SpawnProps();

        //Set Trail Colors
        SetTrailColors();

        //Set words list
        SetUpWordList();

        //Set Coins
        InitiateCoins();

        m_AllRunnerList.Clear();

        InitiateNPCPlayers();

        m_AllRunnerList.Add(PlayerObj.GetComponent<BaseRunner>());

        SortTheRunners();

        //Fog colors
        RenderSettings.fogColor = m_ColorList[Random.Range(13, 18)];
        Camera.main.backgroundColor = RenderSettings.fogColor;
        m_FoundTextOriginalColorHex = ColorUtility.ToHtmlStringRGB(RenderSettings.fogColor);

        m_TotalCoinsCount = PlayerPrefs.GetInt("TotCoins", 0);

    }

    #region Persitant Data
    private int m_TotalCoinsCount;
    public int TotalCoinsCount
    {
        get { return m_TotalCoinsCount; }
    }
    public void SetTotalCount(int _count)
    {
        m_TotalCoinsCount += _count;
        PlayerPrefs.SetInt("TotCoins", m_TotalCoinsCount);
    }
    private int m_CurrentGameCoinCount;
    public int CurrentGameCoinCount
    {
        get { return m_CurrentGameCoinCount; }
    }
    public void AddCoins(int _coinAmount)
    {
        m_CurrentGameCoinCount += _coinAmount;

        UIController.Instance.CoinsTxtObj.GetComponent<Text>().text = m_CurrentGameCoinCount + "";
        if(!UIController.Instance.CoinsTxtObj.GetComponent<Animator>().GetBool("collect"))
        {
            UIController.Instance.CoinsTxtObj.GetComponent<Animator>().SetBool("collect", true);
            Invoke("CoinAddAnimationEnd",  0.2f);
        }
    }

    private void CoinAddAnimationEnd()
    {
        UIController.Instance.CoinsTxtObj.GetComponent<Animator>().SetBool("collect", false);
    }

    #endregion Persitant Data

    #region Other Functions
    public delegate void OnRaceFinishDelegate();
    public event OnRaceFinishDelegate OnRaceFinish;

    public GameObject PlayerObj;

    private List<BaseRunner> m_AllRunnerList = new List<BaseRunner>();
    public void SortTheRunners()
    {
        if(m_IsRaceOver)
        {
            return;
        }

        m_AllRunnerList.Sort((r1, r2) => r1.ZPosition.CompareTo(r2.ZPosition)); // Sort runner position from their Z position. 

        int m_Place = m_AllRunnerList.Count; 
        foreach(BaseRunner m_bRunner in m_AllRunnerList)
        {
            m_bRunner.PlaceInTheRace = m_Place;
            m_bRunner.AfterSortingRunners();
            m_Place--;
        }
    }

    private float m_RaceFinishLineZ;
    public float RaceFinishLineZ
    {
        get { return m_RaceFinishLineZ; }
    }
    private bool m_IsRaceOver = false;
    public void CallRaceFinish()
    {
        m_IsRaceOver = true;
        OnRaceFinish();
        StartCoroutine(CallGameOver(Vector3.zero, true)); // Call game Over screen On Finish Line. 
    }

    public IEnumerator CallGameOver(Vector3 _pos, bool _isRaceFinish = false)
    {
        if(!_isRaceFinish)
        {
            AudioController.Instance.StopBackgroundAudio();
            AudioController.Instance.PlayEffectAudio("GameOver");
        }
        yield return new WaitForSeconds(2);
        if (!_isRaceFinish) // Player crasshed, therefore, play the crash animaiton before game over screen. 
        {
            yield return StartCoroutine(UIController.Instance.BringInGameOverPenAnimation(_pos));
            yield return new WaitForSeconds(5);
            foreach (BaseRunner bRun in m_AllRunnerList)
            {
                bRun.StopRace();
            }
        }
        UIController.Instance.BringInGamePausePanel();
    }
    #endregion Other Functions

    #region Colors
    private List<Color> m_ColorList = new List<Color>();
    private List<Color> m_TempColorList = new List<Color>();
    //https://sashat.me/2017/01/11/list-of-20-simple-distinct-colors/
    private void SetTrailColors()
    {
        m_ColorList.Clear();
        m_ColorList.Add(new Color(128f/255f, 0, 0));
        m_ColorList.Add(new Color(170f/255f, 110f/255f, 40f/255f));
        m_ColorList.Add(new Color(128f / 255f, 128f / 255f, 0));
        m_ColorList.Add(new Color(0, 128f / 255f, 128f / 255f));
        m_ColorList.Add(new Color(0, 0, 128f / 255f));
        m_ColorList.Add(new Color(0, 0, 0));
        m_ColorList.Add(new Color(230f / 255f, 25f / 255f, 75f / 255f));
        m_ColorList.Add(new Color(210f / 255f, 245f / 255f, 60f / 255f));
        m_ColorList.Add(new Color(60f / 255f, 180f / 255f, 75f / 255f));
        m_ColorList.Add(new Color(0, 130f / 255f, 200f / 255f));
        m_ColorList.Add(new Color(145f / 255f, 30f / 255f, 180f / 255f));
        m_ColorList.Add(new Color(240f / 255f, 50f / 255f, 230f / 255f));
        m_ColorList.Add(new Color(230f / 255f, 190f / 255f, 255f / 255f));
        m_ColorList.Add(new Color(128f / 255f, 128f / 255f, 128f / 255f)); // Fog color
        m_ColorList.Add(new Color(70f / 255f, 240f / 255f, 240f / 255f)); // Fog color
        m_ColorList.Add(new Color(250f / 255f, 190f / 255f, 190f / 255f)); // Fog color 
        m_ColorList.Add(new Color(255f / 255f, 215f / 255f, 180f / 255f)); // Fog color
        m_ColorList.Add(new Color(255f / 255f, 250f / 255f, 200f / 255f)); // Fog color 
        m_ColorList.Add(new Color(170f / 255f, 255f / 255f, 195f / 255f));

        m_TempColorList.Clear();
        foreach(Color c in m_ColorList)
        {
            m_TempColorList.Add(c);
        }
    }

    public Color GetRandomColor()
    {
        int m_RandomColorIndex;
        if(m_TempColorList.Count == 0)
        {
            return new Color(1, 1, 25f / 255f);
        }
        else
        {
            m_RandomColorIndex = Random.Range(0, m_TempColorList.Count - 1);
            m_TempColorList.RemoveAt(m_RandomColorIndex);
            return m_ColorList[m_RandomColorIndex];
        }
    }
    #endregion Colors

    #region Word List
    private List<string> m_WordList = new List<string>();
    private void SetUpWordList()
    {
        m_WordList.Clear();
        m_WordList.Add("KETCHUP");
        m_WordList.Add("UNITY");
        m_WordList.Add("MOBILE");
        m_WordList.Add("UBISOFT");
        m_WordList.Add("GAME");
        m_WordList.Add("OWLIENT");
    }

    private string m_FoundTextOriginalColorHex; // Put the Fog color here to hide this. 
    private List<string> m_FoundLetterList = new List<string>();     
    private string m_RandomWord;
    public string RandomWord
    {
        get { return m_RandomWord; }
        set { m_RandomWord = value; }
    }
    private int m_RandomWordLetterCount;
    public int RandomWordLetterCount
    {
        get { return m_RandomWordLetterCount; }
        set { m_RandomWordLetterCount = value; }
    }
    private int m_FoundLetterCount;
    public int FoundLetterCount
    {
        get { return m_FoundLetterCount; }
        set { m_FoundLetterCount = value; }
    }

    // Set Random word for each game play
    private void SetRandomWord()
    {
        m_FoundLetterList.Clear();
        m_FoundLetterCount = 0;
        m_RandomWord = m_WordList[Random.Range(0, m_WordList.Count)];
        m_RandomWordLetterCount = m_RandomWord.Length;
        SetFoundTextEffect();
    }

    public bool IsTypeContainsInWord(string _type)
    {
        SetFoundTextEffect(_type);
        if (System.String.IsNullOrEmpty(_type))
        {
            return false;
        }

        return m_RandomWord.Contains(_type);
    }

    private string m_FinalStr = "";
    private void SetFoundTextEffect(string _character = "")
    {
        if (System.String.IsNullOrEmpty(_character))
        {
            return;
        }

        m_FoundLetterList.Add(_character);
        m_FoundLetterCount++;
        m_FinalStr = "";
        for (int i = 0; i < m_RandomWord.Length; i++)
        {
            bool _isPresent = false;
            foreach(string s in m_FoundLetterList)
            {
                if(s == m_RandomWord[i]+"")
                {
                    _isPresent = true;
                    break;
                }
            }

            if(_isPresent)
            {
                m_FinalStr += "<color=yellow>" + m_RandomWord[i] + "</color>";
            }
            else
            {
                m_FinalStr += "<color=#" + m_FoundTextOriginalColorHex + ">" + m_RandomWord[i] + "</color>";
            }
        }

        UIController.Instance.SetFoundWordTxt = m_FinalStr;
    }
    #endregion Word List

    #region NPC Players
    public GameObject NPCPlayerPrefab;
    private int m_NPCPlayerCount = 19; // Change the other players count here. This race has total of 20 runner including the main player. 
    private Dictionary<int, List<float>> m_FreePointsNearObstaclesDic = new Dictionary<int, List<float>>();
    private void InitiateNPCPlayers ()
    {
        //Select random word
        SetRandomWord();
        float m_Variance = Mathf.Round((float)m_NPCPlayerCount / m_RandomWord.Length);

        GameObject m_NPCPlayerObj;
        int m_StartZ = 30;
        for(int i = 0; i < m_NPCPlayerCount; i++)
        {
            m_NPCPlayerObj = Instantiate(NPCPlayerPrefab, new Vector3(GetXPoint(m_StartZ), -4.3f, m_StartZ), Quaternion.identity);
            m_NPCPlayerObj.name = "NPC_" + (i+1);
            m_AllRunnerList.Add(m_NPCPlayerObj.GetComponent<BaseRunner>());
            m_AllRunnerList[i].Type = "";
            m_StartZ += 10;
        }

        int m_StartIndex = 0;
        int m_EndIndex = 0;
        for(int i = 0; i < m_RandomWord.Length; i++)
        {
            if(m_StartIndex + (int)m_Variance > m_AllRunnerList.Count)
            {
                m_EndIndex = m_AllRunnerList.Count;
            }
            else
            {
                m_EndIndex = m_StartIndex + (int)m_Variance;
            }

            m_AllRunnerList[Random.Range(m_StartIndex, m_EndIndex)].Type = m_RandomWord[i]+"";
            m_StartIndex += (int)m_Variance;
        }
    }

    // Gives X cordinate that Obstacales are not present. 
    private float GetXPoint(int _startZ, bool _isCoin = false)
    {
        if (!_isCoin)
        {
            if (_startZ >= 20 && _startZ < 40)
            {
                _startZ = 40;
            }
            else if (_startZ >= 60 && _startZ < 80)
            {
                _startZ = 80;
            }
            else if (_startZ >= 100 && _startZ < 120)
            {
                _startZ = 120;
            }
            else if (_startZ >= 140 && _startZ < 160)
            {
                _startZ = 160;
            }
            else if (_startZ >= 180 && _startZ < 200)
            {
                _startZ = 200;
            }
            else if (_startZ >= 220 && _startZ < 240)
            {
                _startZ = 240;
            }
        }
        List<float> m_FreeXPointList;
        if(m_FreePointsNearObstaclesDic.TryGetValue(_startZ, out m_FreeXPointList))
        {
            return m_FreeXPointList[Random.Range(0, m_FreeXPointList.Count)];
        }

        return Random.Range(-6f, 6f);
    }
    #endregion NPC Players

    #region Coins
    public GameObject CoinPrefab;
    private int m_CoinZPos = 35;
    private int m_CoinMaxCount = 10;
    private void InitiateCoins()
    {
        GameObject m_CoinObj;
        for (int i = 0; i < m_CoinMaxCount; i++)
        {
            m_CoinObj = Instantiate(CoinPrefab, new Vector3(GetXPoint(m_CoinZPos, true), -3f, m_CoinZPos), Quaternion.identity);
            m_CoinObj.name = "Coin_" + (i + 1);
            m_CoinObj.transform.SetParent(transform);
            m_CoinZPos += 10;
        }
    }

    public void PlaceCoinAgain(GameObject _coinObj)
    {
        if(PlayerObj.transform.position.z >= m_CoinZPos)
        {
            m_CoinZPos = (int)PlayerObj.transform.position.z + 30;
        }

        if(m_CoinZPos >= RaceFinishLineZ)
        {
            return;
        }
        _coinObj.GetComponent<Collider>().enabled = true;
        _coinObj.transform.localRotation = Quaternion.identity;
        _coinObj.transform.localPosition = new Vector3(GetXPoint(m_CoinZPos, true), -3f, m_CoinZPos);
        m_CoinZPos += 10;
    }
    #endregion Coins

    #region Platform
    public GameObject StartPlatformPrefab;
    public GameObject EndPlatformPrefab;
    public GameObject[] OtherPlatformPrefabs; // 0th is a full platform, others are half platforms.
    public GameObject RaceFinishLinePrefab;
    private List<GameObject> m_PlatformList = new List<GameObject>();
    private void SetPlatforms()
    {
        m_PlatformList.Clear();
        m_ClampAreaDictionary.Clear();

        GameObject m_PlatformObj;
        float m_StartZ = 0;
        bool m_HalfPlatformSetPreviously = false;
        int m_RandomIndex;
        for(int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                m_PlatformObj = Instantiate(StartPlatformPrefab);

            }
            else if(i == 4)
            {
                m_PlatformObj = Instantiate(EndPlatformPrefab);
            }
            else
            {
                if(m_HalfPlatformSetPreviously)
                {
                    m_HalfPlatformSetPreviously = false;
                    m_RandomIndex = 0;
                }
                else
                {
                    m_RandomIndex = Random.Range(0, OtherPlatformPrefabs.Length);
                }

                if (m_RandomIndex != 0)
                {
                    m_HalfPlatformSetPreviously = true;

                }
                m_PlatformObj = Instantiate(OtherPlatformPrefabs[m_RandomIndex]);

            }

            m_PlatformObj.transform.position = new Vector3(0, -5, m_StartZ + m_PlatformObj.transform.localScale.z / 2f);
            m_StartZ += m_PlatformObj.transform.localScale.z;

            m_ClampAreaDictionary.Add(m_StartZ, new Vector2(-m_PlatformObj.transform.localScale.x * 0.5f + 1, m_PlatformObj.transform.localScale.x * 0.5f - 1));

            m_PlatformList.Add(m_PlatformObj);
        }

        //Race Finish Line
        m_RaceFinishLineZ = m_PlatformList[m_PlatformList.Count - 1].transform.position.z + m_PlatformList[m_PlatformList.Count - 1].transform.localScale.z * 0.5f - 10;
        Instantiate(RaceFinishLinePrefab, new Vector3(0, -4, m_RaceFinishLineZ), Quaternion.identity);
    }

    private Dictionary<float, Vector2> m_ClampAreaDictionary = new Dictionary<float, Vector2>();
    public Vector2 ClampAreaOnPlatForm(float _zPos)
    {
        foreach(KeyValuePair<float, Vector2> pair in m_ClampAreaDictionary)
        {
            if(_zPos <= pair.Key)
            {
                return pair.Value;
            }
        }

        return new Vector2(-9f, 9f);
    }
    #endregion Platform

    #region Obstacles
    public GameObject[] ObsticlesPrefabs;
    private void SetObstacles()
    {
        int m_Count = 0;
        int m_StartPosZ = 40;
        int m_EndPosZ;

        float m_PlatformCornerMinX, m_PlatformCornerMaxX;

        foreach (GameObject m_Pf in m_PlatformList)
        {
            m_PlatformCornerMinX = m_Pf.transform.position.x - m_Pf.transform.localScale.x * 0.5f;
            m_PlatformCornerMaxX = m_Pf.transform.position.x + m_Pf.transform.localScale.x * 0.5f;

            if (m_Count == 0) // Starting platform
            {
                m_Count++;

                m_StartPosZ = 40;
                m_EndPosZ = (int)(m_Pf.transform.position.z + m_Pf.transform.localScale.z * 0.5f);
                CreateObstacle(m_StartPosZ, m_EndPosZ, m_PlatformCornerMinX, m_PlatformCornerMaxX, true);
            }
            else
            {
                if(m_Pf.transform.localScale.x < 20) // Half platform
                {
                    continue;
                }
                else
                {
                    m_StartPosZ = (int)(m_Pf.transform.position.z - m_Pf.transform.localScale.z * 0.5f) + 40;
                    m_EndPosZ = (int)(m_Pf.transform.position.z + m_Pf.transform.localScale.z * 0.5f);
                    CreateObstacle(m_StartPosZ, m_EndPosZ, m_PlatformCornerMinX, m_PlatformCornerMaxX);
                }
            }
        }
    }

    private void CreateObstacle(int _StartPosZ, int _EndPosZ, float _PlatformCornerMinX, float _PlatformCornerMaxX, bool _startingPlatform = false)
    {
        GameObject m_RandomObsticleObj;
        int m_RandomObsticleIndex;
        float m_ObsticleHalfXWidth;
        List<float> m_AvailablePointsList;

        if(_startingPlatform)
        {
            m_FreePointsNearObstaclesDic.Clear();
        }


        for (int i = _StartPosZ; i < _EndPosZ; i += 40)
        {
            //Create obstacle
            m_RandomObsticleIndex = Random.Range(0, ObsticlesPrefabs.Length);
           // m_RandomObsticleIndex = 5;
            m_RandomObsticleObj = Instantiate(ObsticlesPrefabs[m_RandomObsticleIndex]);
            m_RandomObsticleObj.name = ObsticlesPrefabs[m_RandomObsticleIndex].name;
            m_ObsticleHalfXWidth = m_RandomObsticleObj.transform.localScale.x * 0.5f;


            if (m_RandomObsticleIndex < 3) // Type one obsticle
            {
                m_RandomObsticleObj.transform.position = new Vector3(Mathf.Round(Random.Range(_PlatformCornerMinX + m_ObsticleHalfXWidth, _PlatformCornerMaxX - m_ObsticleHalfXWidth)), -3.5f, i);
                m_AvailablePointsList = m_RandomObsticleObj.transform.Find("ObsticleTrigger").gameObject.GetComponent<ObsticleTrigger>().Initiate(1);
            }
            else if (m_RandomObsticleIndex < 4) // Type two obsticle
            {
                m_RandomObsticleObj.transform.position = new Vector3(0, -3.5f, i);
                m_AvailablePointsList = m_RandomObsticleObj.transform.Find("ObsticleTrigger").gameObject.GetComponent<ObsticleTrigger>().Initiate(2);
            }
            else // Type three obsticle
            {
                if (Random.Range(1, 100) < 50)
                {
                    m_RandomObsticleObj.transform.position = new Vector3(Mathf.Round(_PlatformCornerMinX - 5 + m_ObsticleHalfXWidth), -3.5f, i);
                }
                else
                {
                    m_RandomObsticleObj.transform.position = new Vector3(Mathf.Round(_PlatformCornerMaxX + 5 - m_ObsticleHalfXWidth), -3.5f, i);
                }
                m_AvailablePointsList = m_RandomObsticleObj.transform.Find("ObsticleTrigger").gameObject.GetComponent<ObsticleTrigger>().Initiate(3);
            }

            if(_startingPlatform) // First platform Obstacles.
            {
                m_FreePointsNearObstaclesDic.Add(i, m_AvailablePointsList);
            }
        }
    }
    #endregion Obstacles

    #region Props
    public GameObject[] Props;
    private List<int> m_PropsIndexs = new List<int>() { 0, 1, 2 };
    private List<GameObject> m_PropsSpawned = new List<GameObject>();
    private float m_SpawnZPos;
    public void SpawnProps(int _spawnCount = 2)
    {
        GameObject m_Prop;
        if (m_PropsSpawned.Count < 3) // We only intiate 3 Props, After that we can repeat them
        {
            for(int i = 0; i < _spawnCount; i++)
            {
                int m_RandomPropsIndex = Random.Range(0, m_PropsIndexs.Count);
                m_Prop = Instantiate(Props[m_PropsIndexs[m_RandomPropsIndex]], new Vector3(0, 0, m_SpawnZPos), Quaternion.identity);
                m_Prop.name = "Props_" + m_PropsIndexs[m_RandomPropsIndex];
                m_PropsSpawned.Add(m_Prop);
                m_SpawnZPos += 300;
                m_PropsIndexs.RemoveAt(m_RandomPropsIndex);
            }
        }
        else
        {
            m_Prop = m_PropsSpawned[0];
            m_Prop.transform.position = new Vector3(0, 0, m_SpawnZPos);
            m_SpawnZPos += 300;
            m_PropsSpawned.Remove(m_Prop);
            m_PropsSpawned.Add(m_Prop);
        }
    }
    #endregion Props
}
