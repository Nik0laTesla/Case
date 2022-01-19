using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;



public class GameController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;
    public bool isLevelFinishFail;

    [Header("Tags")]
    public string TagPlayer;
    public string TagGround;
    public string TagFlipCounter;
    public string TagFinishTrigger;

    public int level = 1;
    public int randomLevelIndex;

    [SerializeField] private List<Transform> AllDrivers = new List<Transform>();


    UIController UI;
    DriveController DC;
    CameraController Camera;
    FinishTrigger Finish;
    AIManager AIM;

    public static GameController instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMethods();
    }

    void StartMethods()
    {
        UI = UIController.instance;
        DC = DriveController.instance;
        Camera = CameraController.instance;
        Finish = FinishTrigger.instance;
        AIM = AIManager.instance;
    }

    public void TapToStartActions()
    {
        DC.isLevelStart = true;
        isLevelStart = true;
        UI.isLevelStart = true;
        Camera.isLevelStart = true;
        AllDrivers.Add(DC.gameObject.transform);

        for (int i = 0; i < AIM.AllAI.Count; i++)
        {
            AIM.AllAI[i].GetComponent<AIController>().isLevelStart = true;
            AllDrivers.Add(AIM.AllAI[i].transform);
        }
    }

    public void LevelFailActions()
    {
        isLevelFail = true;
        UI.ShowLosePanel();
        DC.isLevelStart = false;
        DC.isLevelFail = true;
        UI.isLevelFail = true;
    }

    public void LevelDoneActions()
    {
        isLevelDone = true;
        UI.ShowWinPanel();
        DC.isLevelDone = true;
        Camera.isLevelDone = true;
        UI.isLevelDone = true;
        DC.isLevelStart = false;
    }

    public void EndGameButtonAction()
    {
        if (isLevelDone)
        {      

            if (PlayerPrefs.GetInt("Level") < 5)
            {
                PlayerPrefs.SetInt("Level", (PlayerPrefs.GetInt("Level") + 1));
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            }

            else
            {
                randomLevelIndex = GetRandom();

                if (randomLevelIndex == SceneManager.GetActiveScene().buildIndex + 1 )
                {
                    randomLevelIndex = GetRandom();
                }
                
                SceneManager.LoadScene(randomLevelIndex);
            }           
        }

        else if (isLevelFail)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private int GetRandom()
    {
        int random = UnityEngine.Random.Range(0, SceneManager.sceneCountInBuildSettings);

        if (random == SceneManager.GetActiveScene().buildIndex)
        {
            GetRandom();
        }

        return random;
    }
}