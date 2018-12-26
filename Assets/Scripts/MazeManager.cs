using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MazeManager : MonoBehaviour {
	
	//Level
	List<LevelManager> Levels = new List<LevelManager>();
	public GameObject ParentPrefab;
	private int LevelGenerate = 0;
	
	
	//Bounds
	public int CellsByLine;
	public int LinesByLevel;
	public int MaxLevel;
	
	//
	private Cell ExitCell;
	private Cell StartCell;
	List<Cell> PathFinding = new List<Cell>();
	
	//Instance
	public static MazeManager Instance;
	private GameManager GameMngr;
	
	private void Awake()
	{
		//get instance of him
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		
		GameMngr = GameManager.Instance;
		//Init();
		
	}

	public void Init()
	{
		//Instantiate Maze
		InstantiateMaze();
	}


	void InstantiateMaze()
	{
		//Init bounds of maze
		CellsByLine = Random.Range(5, 10);
		LinesByLevel = Random.Range(5, 10);
		MaxLevel = Random.Range(3, 8);

		//Create maze levels
		for (int i = 0; i < MaxLevel; i++)
		{
			LevelManager NewLevel = CreateLevel(i);
			Levels.Add(NewLevel);
			
			//Init each level
			NewLevel.Init(i);
		}
		
	}
	
	void GenerateMaze()
	{
		ExitCell = Levels[Levels.Count - 2].GetRandomCell(false);
		StartCell = Levels[0].GetRandomCell(false);
		
		List<Cell> CellsStack = new List<Cell>();
		
		Cell CurrentCell = StartCell;
		

		int MaxCells = LinesByLevel * CellsByLine * (MaxLevel-1);
		int VisitedCells = 1;

		while (VisitedCells < MaxCells)
		{
			int Level = CurrentCell.LevelNumber;

			Dictionary<string,Cell> Neighbours = Levels[Level].GetNeighbours(CurrentCell);

			if (Neighbours.Count > 0)
			{
				string[] Directions = Neighbours.Keys.ToArray();
				string Dir = Directions[Random.Range(0, Directions.Length)];
				Cell NextCell = Neighbours[Dir];
				
				//Break walls
				NextCell.BreakWall();
				CurrentCell.BreakWall(Dir);
				
				//Add to stack & Pathfinding
				if (!CellsStack.Contains(CurrentCell))
				{
					CellsStack.Add(CurrentCell);
					if (PathFinding.Count == 0 || PathFinding.Last() != ExitCell) PathFinding.Add(CurrentCell);
				}
			
				CurrentCell = NextCell;
				VisitedCells++;
			}
			else
			{
				//Remove from Stack and get the last cell
				if (CellsStack.Contains(CurrentCell))
				{
					CellsStack.Remove(CurrentCell);
					if (PathFinding.Count> 0 && PathFinding.Last() != ExitCell) PathFinding.Remove(CurrentCell);
				}

				CurrentCell = CellsStack.Last();
			}
		}
		
		//Make Exit a lift to access last level
		ExitCell.BreakWall("Top");
		
		//Display Right path
		DisplayPathFinding();
		
		//Ini Game
		GameMngr.InitGame(StartCell);

	}

	//Display Right path from start cell to exit cell
	public void DisplayPathFinding()
	{
		foreach (Cell cell in PathFinding)
		{
			cell.DisplayPath();
		}
	}
	
	//Create new level
	private LevelManager CreateLevel(int _index)
	{
		//Crete parent in scene
		GameObject LevelParent = Instantiate(ParentPrefab);
		LevelParent.transform.SetParent(transform);
		LevelParent.transform.position = new Vector3(0, 4*_index);
		LevelParent.name = "Level"+ _index;
		return LevelParent.GetComponent<LevelManager>();
	}

	public List<LevelManager> GetLevels()
	{
		return Levels;
	}
	
	//Check of many level are generated
	public void LevelGenerated()
	{
		LevelGenerate++;
		if (LevelGenerate == MaxLevel)
		{
			GenerateMaze();
		}
	}

	

	
}
