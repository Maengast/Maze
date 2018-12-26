using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class Cell : MonoBehaviour
{

	//Index
	public int IndexCell;
	public int LineNumber;
	public int LevelNumber;
	
	//Object
	public GameObject[] Anchors;
	public Transform WallsParent;
	public GameObject PathObject;


	private bool IsVisited = false;
	private Dictionary<string, GameObject> Walls;
	private GameObject WallToBreak;

	private Lift LiftScript;
	//Manager
	private MazeManager MazeMngr;
	private LevelManager LevelMngr;


	// Use this for initialization
	public void Init (LevelManager _Mngr) {
		MazeMngr = MazeManager.Instance;
		LevelMngr = _Mngr;
		LevelNumber = LevelMngr.GetLevelNumber();
		LevelMngr.AddNewCell(this);
		SetWalls();
		InstantiateNextCell();
	}

	private void SetWalls()
	{
		Walls = new Dictionary<string, GameObject>();
		foreach (Transform Child in WallsParent)
		{
			//instantiate walls
			Walls.Add(Child.name,Child.gameObject);
		}
	}

	#region Instantiate
	
	#region Instantiate NextCell

	private void InstantiateNextCell()
	{
		//Instantiate next cell in line
		if (IndexCell < MazeMngr.CellsByLine - 1)
		{
			Cell NewCell = InstantiateNextCell(LevelMngr.transform,Anchors[0].transform.position);
			SetNewCell(NewCell,LineNumber, IndexCell+1);
			NewCell.Init(LevelMngr);
		}
		
		//Instantiate first cell of next line
		if (LineNumber < MazeMngr.LinesByLevel-1 && IndexCell == 0)
		{
			Cell NewCell = InstantiateNextCell(LevelMngr.transform,Anchors[1].transform.position);
			SetNewCell(NewCell,LineNumber+1,0);
			NewCell.Init(LevelMngr);
		}
	}
	
	
	private Cell InstantiateNextCell(Transform _Parent, Vector3 _Position)
	{
		GameObject NewCell = Instantiate(LevelMngr.GetNewCell());
		NewCell.transform.position = _Position;
		NewCell.transform.SetParent(_Parent);
		

		return NewCell.GetComponent<Cell>();
	}

	private void SetNewCell(Cell _NewCell, int _LineNumber, int _IndexCell)
	{
		_NewCell.LineNumber = _LineNumber;
		_NewCell.IndexCell = _IndexCell;
	}
	
	#endregion
	
	#endregion
	

	#region Manage Walls

	#region BreakWalls

	//Prepare wall to break if this cell is choose
	public void PrepareBreakWall(string _Dir)
	{
		WallToBreak = Walls[_Dir];
	}
	
	//Break wall
	public void BreakWall()
	{
		Walls.Remove(WallToBreak.name);
		Destroy(WallToBreak);
		IsVisited = true;
		
		//Make this cell a lift
		int Factor = (WallToBreak.name == "Bottom") ? -1 : (WallToBreak.name == "Top") ? 1 : 0;

		if (Factor != 0)
		{
			if(!LiftScript)LiftScript = gameObject.AddComponent<Lift>();
			LiftScript.WallPos -= Factor;
		}
	}

	public void BreakWall(string _Dir)
	{
		WallToBreak = Walls[_Dir];
		BreakWall();
	}

	//Break All Walls of the level
	public void BreakAllWalls()
	{
		foreach (Transform Child in WallsParent)
		{
			Destroy(Child.gameObject);
		}
	}

	#endregion

	#endregion
	

	public bool GetIsVisited()
	{
		return IsVisited;
	}
	
	//Display the object to show the path
	public void DisplayPath()
	{
		PathObject.SetActive(true);
	}
	
	

	
}
