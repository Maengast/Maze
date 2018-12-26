using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
	private PlayerController Player;
	private Vector3 CellCenter = Vector3.zero;
	private BoxCollider Col;
	public int WallPos = 0;
	
	private void Start()
	{
		//get bounds of cell
		Col = gameObject.GetComponent<BoxCollider>();
		CellCenter = Col.center;
		Instantiate(Resources.Load("ForceShield"), transform.position + CellCenter, Quaternion.identity);

	}

	private void Update()
	{
		if (Player)
		{
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				Player.Float(-1);
				Player.SetIsFloating(true);
				PropulsePlayer();
			}
			
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				Player.Float(1);
				Player.SetIsFloating(true);
				PropulsePlayer();
			}
		}
			

	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Player = other.GetComponent<PlayerController>();
			Player.SetIsFloating(true);
			PropulsePlayer();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" && Player)
		{
			Player.SetIsFloating(false);
			Player = null;
		}
	}

	//float player in right dir
	private void PropulsePlayer()
	{
		int Factor = (Player.transform.position.y > CellCenter.y + transform.position.y) ? -1 : 1;
		Player.Float(Factor);
		if (WallPos == Factor)
		{
			Player.SetIsFloating(false);
		}

	}

	

}
