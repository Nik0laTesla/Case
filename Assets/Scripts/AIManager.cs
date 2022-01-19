using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{   
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

    private void GetAI()
	{
		for (int i=0; i<gameObject.transform.childCount;i++)
		{
			AllAI.Add(transform.GetChild(i).gameObject);
		}
	}
}
