using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviour : MonoBehaviour {
	public Helper.PointOfAction point;
	bool currentlyActive = false;
	public GameObject Rain;
	public GameObject Earthquake;
	public GameObject Hurricane;
	void Start () {
		
	}

	public void setPoint(Helper.PointOfAction _p){
		point = _p;
		this.name = point.name;
		loadModel();
	}

	void loadModel(){
		switch (point.attack){
			case Helper.Attacks.Rain:
				Destroy(this.transform.GetChild(0).gameObject);
				GameObject go = GameObject.Instantiate(Rain);
				go.transform.parent = this.transform;
				go.transform.localPosition = new Vector3(0, 0, 0);
			break;
			case Helper.Attacks.Earthquake:
				Destroy(this.transform.GetChild(0).gameObject);
				GameObject go2 = GameObject.Instantiate(Earthquake);
				go2.transform.parent = this.transform;
				go2.transform.localPosition = new Vector3(0, 0, 0);
			break;
			case Helper.Attacks.Wind:
				Destroy(this.transform.GetChild(0).gameObject);
				GameObject go3 = GameObject.Instantiate(Hurricane);
				go3.transform.parent = this.transform;
				go3.transform.localPosition = new Vector3(0, 1.1f, 0);
			break;
		}
	}

	void Update () {

	}
}
