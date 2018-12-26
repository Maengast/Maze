using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public GameObject PlayerPrefab;
	public static GameManager Instance;

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
	}
	
	
	//Instantiate Player
	public void InitGame(Cell _StartCell)
	{
		BoxCollider Col = _StartCell.GetComponent<BoxCollider>();
		GameObject Player = Instantiate(PlayerPrefab, _StartCell.transform.position + Col.center, Quaternion.identity);
	}
}
