using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell_GameObject : MonoBehaviour {
	public Cell cell;
	
	public Color originalColor = Color.white;
	

	//Models
	public GameObject model_straight;
	public GameObject model_turn;
	public GameObject model_instance;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.GetChild(0).guiText.text = name;
	}
	
	public void DestroyModel()
	{
		Destroy(model_instance);
	}
	
	private void InstantiateModel(GameObject _model)
	{
		model_instance = Instantiate(_model) as GameObject;
		model_instance.transform.position = this.transform.position;
		model_instance.transform.parent = this.transform;
	}
	
	public void DetectModelType()
	{
		Destroy(model_instance);
		List<Cell.CellType> valid_cell_types = new List<Cell.CellType>();
		valid_cell_types.Add(Cell.CellType.Path);
		valid_cell_types.Add(Cell.CellType.Path_Start);
		valid_cell_types.Add(Cell.CellType.Path_End);	
		if(!valid_cell_types.Contains(cell.cellType)) return;
		
		int num_adjacent_paths = 0;
		if(cell.left != null) { if(valid_cell_types.Contains(cell.left.cellType)) num_adjacent_paths++; }
		if(cell.right != null) { if(valid_cell_types.Contains(cell.right.cellType)) num_adjacent_paths++; }
		if(cell.top != null) { if(valid_cell_types.Contains(cell.top.cellType)) num_adjacent_paths++; }
		if(cell.bottom != null) { if(valid_cell_types.Contains(cell.bottom.cellType)) num_adjacent_paths++; }
		if(num_adjacent_paths > 2) return;
		
		//Straights
		if(cell.left != null && cell.right != null)
		{
			if(valid_cell_types.Contains(cell.left.cellType) && valid_cell_types.Contains(cell.right.cellType))
			{
				InstantiateModel(model_straight);
				return;
			}
		}
		
		if(cell.top != null && cell.bottom != null)
		{
			if(valid_cell_types.Contains(cell.top.cellType) && valid_cell_types.Contains(cell.bottom.cellType))
			{
				InstantiateModel(model_straight);
				model_instance.transform.RotateAround(Vector3.up, Mathf.PI / 2.0f);
				return;
			}
		}
		//Turns
		if(cell.top != null && cell.left != null)
		{
			if(valid_cell_types.Contains(cell.top.cellType) && valid_cell_types.Contains(cell.left.cellType))
			{
				InstantiateModel(model_turn);
				//model_instance.transform.RotateAround(Vector3.up, Mathf.PI / 2.0f);
				return;
			}
		}
		if(cell.bottom != null && cell.right != null)
		{
			if(valid_cell_types.Contains(cell.bottom.cellType) && valid_cell_types.Contains(cell.right.cellType))
			{
				InstantiateModel(model_turn);
				model_instance.transform.RotateAround(Vector3.up, Mathf.PI);
				return;
			}
		}
		
		if(cell.top != null && cell.right != null)
		{
			if(valid_cell_types.Contains(cell.top.cellType) && valid_cell_types.Contains(cell.right.cellType))
			{
				InstantiateModel(model_turn);
				model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				return;
			}
		}
		if(cell.bottom != null && cell.left != null)
		{
			if(valid_cell_types.Contains(cell.bottom.cellType) && valid_cell_types.Contains(cell.left.cellType))
			{
				InstantiateModel(model_turn);
				model_instance.transform.RotateAround(Vector3.up, Mathf.PI);
				model_instance.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				return;
			}
		}
	}
	
}
