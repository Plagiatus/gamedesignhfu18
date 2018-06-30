using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviour : MonoBehaviour {
	public Helper.PointOfAction point;
	bool currentlyActive = false;
	public GameObject Earthquake;
	void Start () {
		
	}

	public void setPoint(Helper.PointOfAction _p){
		point = _p;
		this.name = point.name;
		loadModel();
	}

	void loadModel(){
		switch (point.attack){
			case Helper.Attacks.Earthquake:
				Destroy(this.transform.GetChild(0).gameObject);
				GameObject go = GameObject.Instantiate(Earthquake);
				go.transform.parent = this.transform;
				go.transform.position = new Vector3(0, 0, 0);
			break;
		}
	}

	void Update () {

	}
}
