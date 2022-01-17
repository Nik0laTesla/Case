using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;

    [Header("Tags")]
    public string TagPlayer;
    public string TagGround;
    public string TagFlipCounter;
    public string TagFinishTrigger;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TapToStartActions()
    {
        DC.isLevelStart = true;
        isLevelStart = true;
        UI.isLevelStart = true;
        Camera.isLevelStart = true;
        AllDrivers.Add(DC.gameObject.transform);

		for (int i=0;i<AIM.AllAI.Count;i++)
		{
            AIM.AllAI[i].GetComponent<AIController>().isLevelStart = true;
            AllDrivers.Add(AIM.AllAI[i].transform);
		}
        // Player.StartRunAnim();
    }

    public void LevelFailActions()
    {
        //Player.isLevelFail = true;
        UI.ShowLosePanel();
        isLevelFail = true;
        DC.isLevelStart = false;
        DC.isLevelFail = true;
        UI.isLevelFail = true;
        Camera.isLevelFail = true;

        //Player.PlayerSkin.SetActive(false);
        //Player.PlayerCanvas.SetActive(false);
        //Player.StartDieAnim();
        //Burda playersprite Bas ve partikül çalýþtýr.
        // Player.MainSplashEffect.SetActive(true);
        //Player.MainStickmanParticle.GetComponent<ParticleSystem>().Play();
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

  
   

  
}
