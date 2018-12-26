using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	
	List<List<Cell>> Lines = new List<List<Cell>>();
	
	//Cell
	public GameObject[] CellsPrefab;
	public float PerLift;
	
	//Index
	private int LevelNumber = -1;
	
	//Manager
	private MazeManager MazeMngr;
	
	private void Awake()
	{
		MazeMngr = MazeManager.Instance;
	}

	#region Generate Level

	public void Init(int _LevelNumber)
	{
		LevelNumber = _LevelNumber;
		
		//Instantiate First cell
		GameObject Cell = Instantiate(GetNewCell());
		Cell CellScript = Cell.GetComponent<Cell>();
		Cell.transform.position = new Vector3(0, LevelNumber * 4, 0);
		Cell.transform.SetParent(transform);
		CellScript.Init(this);
	}

	
	//Add a cell instantiate in scene to lines
	public void AddNewCell(Cell _Cell)
	{
		//Add new line if doesn't exist
		if (Lines.Count <= _Cell.LineNumber)
		{
			List<Cell> NewLine = new List<Cell>();
			Lines.Add(NewLine);
		}
		
		//Add cell to line
		List<Cell> Line = Lines[_Cell.LineNumber];
		if (!Line.Contains(_Cell))
		{
			Line.Add(_Cell);
		}
		
		CheckIsGenerate();
		
	}

	//Check if all cell are generated in this level
	private void CheckIsGenerate()
	{
		int NbCells = Lines[Lines.Count - 1].Count;
		if (Lines.Count() == MazeMngr.LinesByLevel && NbCells == MazeMngr.CellsByLine)
		{
			if (LevelNumber == MazeMngr.MaxLevel - 1)
			{
				foreach (List<Cell> line in Lines)
				{
					foreach (Cell cell in line)
					{
						cell.BreakAllWalls();
					}
				}
			}
			MazeMngr.LevelGenerated();
		}
	}

	#endregion
	
	
	#region GetCell

	//return cell prefabs
	public GameObject GetNewCell()
	{
		return CellsPrefab[0];
			
	}

	//Get specific cell with index
	public Cell GetCell(int _LineIndex, int _CellIndex)
	{
		return Lines[_LineIndex][_CellIndex];
	}
	
	//Get a random cell and choose if lift or not
	public Cell GetRandomCell(bool _CanIsLift)
	{
		Cell Cell ;
		while (true)
		{
			int IndexLine = Random.Range(0, Lines.Count);
			int IndexCell = Random.Range(0, Lines[IndexLine].Count);
			Cell = GetCell(IndexLine, IndexCell);
			if(_CanIsLift == CheckIsLift(Cell)) break;
		}
		
		return Cell;

	}
	
	#endregion
	
	
	public int GetLevelNumber()
	{
		return LevelNumber;
	}

	
	//Check if cell is a lift
	private bool CheckIsLift(Cell _Cell)
	{

		if (_Cell.GetComponent<Lift>())
		{
			return true;
		}

		return false;
	}


	#region Find Neighbours

	//Find all Neighbours of current cell
	public Dictionary<string,Cell> GetNeighbours(Cell _Cell)
	{
		Dictionary<string,Cell> Neighbours = new Dictionary<string,Cell>();

		int Line = _Cell.LineNumber;
		int Index = _Cell.IndexCell;

		int MaxLines = MazeMngr.LinesByLevel;
		int MaxCells = MazeMngr.CellsByLine;

		Cell NextCell;

		//get cells on side
		if (Line == 0 || (Line > 0 && Line < MaxLines-1))
		{
			NextCell = GetCell(Line + 1, Index);
			if (CanTakeCell(NextCell,"East"))Neighbours.Add("West",NextCell);
		}
		
		if (Line == MaxLines-1 || (Line > 0 && Line < MaxLines-1))
		{
			NextCell = GetCell(Line - 1, Index);
			if (CanTakeCell(NextCell,"West"))Neighbours.Add("East",NextCell);
		}

		if (Index == 0 || (Index > 0 && Index < MaxCells-1))
		{
			NextCell = GetCell(Line, Index + 1);
			if (CanTakeCell(NextCell,"South"))Neighbours.Add("North",NextCell);
		}
		
		if (Index == MaxCells-1 || (Index > 0 && Index < MaxCells-1))
		{
			NextCell = GetCell(Line, Index - 1);
			if (CanTakeCell(NextCell,"North"))Neighbours.Add("South",NextCell);
		}

		
			//Lift -> get the cell on next/last floor 
		
			if (LevelNumber == 0 || (LevelNumber > 0 & LevelNumber < MazeMngr.MaxLevel - 2))
			{
				NextCell = MazeMngr.GetLevels()[LevelNumber + 1].GetCell(Line, Index);
				if (CanTakeCell(NextCell,"Bottom"))Neighbours.Add("Top",NextCell);
			}

			if (LevelNumber == MazeMngr.MaxLevel - 2 || (LevelNumber > 0 & LevelNumber < MazeMngr.MaxLevel - 2))
			{
				NextCell = MazeMngr.GetLevels()[LevelNumber - 1].GetCell(Line, Index);
				if (CanTakeCell(NextCell,"Top"))Neighbours.Add("Bottom",NextCell);
			}
		
		
		
		return Neighbours;
	}
	
	//Check if visited and prepare cell
	private bool CanTakeCell(Cell _Next, string _WallNext)
	{
		if (!_Next.GetIsVisited())
		{
			_Next.PrepareBreakWall(_WallNext);
			return true;
		}

		return false;
	}

	#endregion
	

}
