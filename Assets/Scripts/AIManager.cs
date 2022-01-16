using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    GameController GC;

    public List<GameObject> AllAI = new List<GameObject>(); 

    public static AIManager instance;

	private void Awake()
	{
		if (!instance)
		{
            instance = this;
		}
		GetAI();
	}

	// Start is called before the first frame update  
	void Start()
    {
        StartMethods();
    }

    private void StartMethods()
	{
        GC = GameController.instance;
	}

    private void GetAI()
	{
		for (int i=0; i<gameObject.transform.childCount;i++)
		{
			AllAI.Add(transform.GetChild(i).gameObject);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	
}
