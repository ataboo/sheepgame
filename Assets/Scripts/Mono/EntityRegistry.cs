using UnityEngine;
using System.Collections;

public class EntityRegistry : MonoBehaviour {
	int entityCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string CheckIn(EntityController entity)
	{
		entityCount++;
		return entity.UID_Class + "_" + entityCount;
	}

	public void CheckOut(EntityController iEntity)
	{

	}
}
