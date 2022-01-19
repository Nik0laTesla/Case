using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;

    [Header("Tap To Start Panel")]
    public GameObject TapToStartPanel;

    [Header("In Game Panel")]
    public GameObject InGamePanel;
    public Slider slider;

    [Header("Win Panel")]
    public GameObject WinPanel;

    [Header("Lose Panel")]
    public GameObject LosePanel;

    [Header("Timer")]
    public Text TimerText;
    private float time;
 
    GameController GC;
    FinishTrigger Finish;
    DriveController Drive;
    
    public static UIController instance;

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
        GC = GameController.instance;
        Finish = FinishTrigger.instance;
        Drive = DriveController.instance;
       
        ShowTapToStartPanel();
        GetSliderMaxValue();
    }

	private void Update()
	{
        if (isLevelStart && !isLevelFail && !isLevelDone)
        {
            slider.value = Drive.transform.position.z;

            time += Time.deltaTime;
            TimerText.text = time.ToString("F2");
        }
	}

	void GetSliderMaxValue()
	{
       slider.maxValue = Finish.transform.position.z;
	}

    void ShowTapToStartPanel()
    {
        TapToStartPanel.SetActive(true);
    }

    void CloseTapToStart()
    {
        TapToStartPanel.SetActive(false);
    }

    public void ButtonActionTapToStart()
    {
        CloseTapToStart();
        GC.TapToStartActions();
        ShowInGamePanel();
    }

    void ShowInGamePanel()
    {
        InGamePanel.SetActive(true);
    }

    public void ShowWinPanel()
    {
        InGamePanel.SetActive(false);
        WinPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        StartCoroutine(WaitForLosePanel());
    }

    IEnumerator WaitForLosePanel()
    {
        TimerText.transform.SetParent(LosePanel.transform);
        yield return new WaitForSeconds(0.5f);
        InGamePanel.SetActive(false);
        LosePanel.SetActive(true);
    }
}
