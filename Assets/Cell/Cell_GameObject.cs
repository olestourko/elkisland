using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell_GameObject : MonoBehaviour {
	public Cell cell;
	
	public Color originalColor = Color.white;
	
	//Models
	public List<GameObject> model_straight;
	public GameObject model_turn;
	public GameObject model_instance;
	
	public List<GameObject> models_random;
	public GameObject model_random;
	public Material random_model_material;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.GetChild(0).guiText.text = name;
	}
		
	private void InstantiateModel(GameObject _model)
	{
		model_instance = Instantiate(_model) as GameObject;
		model_instance.transform.position = this.transform.position;
		model_instance.transform.parent = this.transform;
	}
	
	private void DetectModelType()
	{
		GenerateRandomModels();
		
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
				GameObject model = model_straight[Random.Range(0, model_straight.Count)];
				InstantiateModel(model);
				return;
			}
		}
		
		if(cell.top != null && cell.bottom != null)
		{
			if(valid_cell_types.Contains(cell.top.cellType) && valid_cell_types.Contains(cell.bottom.cellType))
			{
				GameObject model = model_straight[Random.Range(0, model_straight.Count)];
				InstantiateModel(model);
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
	
	private void GenerateRandomModels()
	{		
		Destroy(model_random);
		if(cell.cellType != Cell.CellType.Woods) return;
		GameObject model_d = models_random[Random.Range(0, models_random.Count)];
		model_random = Instantiate(model_d) as GameObject;
		
		float random_offset_x = Random.Range(-0.5f, 0.5f);
		float random_offset_z = Random.Range(-0.5f, 0.5f);
		
		float random_rotation = Random.Range(0, Mathf.PI * 2.0f);
		model_random.transform.position = this.transform.position + new Vector3(random_offset_x, 0.0f, random_offset_z);
		model_random.transform.Rotate(new Vector3(0.0f, 0.0f, random_rotation * 57.3f));
		model_random.transform.parent = this.transform;
		model_random.renderer.material = random_model_material;
	}
	
	
	public void Redraw()
	{
		Color c = new Color(0.10f, 0.15f, 0.075f, 1.0f);	
		//Determine what the color is
		switch(cell.cellType)
		{
			case Cell.CellType.Path:
				c = new Color(0.05f, 0.01f, 0.0f, 1.0f);
				break;
			case Cell.CellType.Path_Start:
				c = new Color(0.05f, 0.01f, 0.0f, 1.0f);
				break;
			case Cell.CellType.Path_End:
				c = new Color(0.05f, 0.01f, 0.0f, 1.0f);
				break;
			case Cell.CellType.Woods:
				float w = (float)cell.cost/3;
				c.r *= w;
				c.g *= w;
				c.b *= w;
				break;
		}
		originalColor = c;
		//Tint color if selected
		if(cell.selected)
		{
			c.r *= 1.25f;
			c.g *= 1.25f;
			c.b *= 1.25f;
			c.g += 0.25f;
		}
		renderer.material.color = c;
		
		//Spawn models
		DetectModelType();
	}
	
}
