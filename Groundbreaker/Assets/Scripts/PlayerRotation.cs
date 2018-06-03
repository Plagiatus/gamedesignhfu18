using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour {

	GameController GC;

	void Start () {
		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			Debug.Log(GC);
		} catch
		{
			throw;
		}
	}
	

	void Update () {
		transform.rotation = Quaternion.Euler(0, -GC.CurrentGpsPosition.HeadingDirection, 0);
	}
}
