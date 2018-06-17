using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviour : MonoBehaviour {
	Helper.PointOfAction point;
	void Start () {
		
	}

	public void setPoint(Helper.PointOfAction _p){
		point = _p;
		this.name = point.name;
		loadModel();
	}

	void loadModel(){
		switch (point.attack){
			case Helper.Attacks.Wind:
			break;
		}
	}

	void Update () {
		
	}
}
