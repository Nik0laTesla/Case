using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
	[Header("Tags")]
	string TagPlayer;

	GameController GC;

    public static FinishTrigger instance;

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

	private void StartMethods()
	{
		GC = GameController.instance;

		GetTags();
	}

	private void GetTags()
	{
		TagPlayer = GC.TagPlayer;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(TagPlayer))
		{
			GC.LevelDoneActions();
		}
	}
}
