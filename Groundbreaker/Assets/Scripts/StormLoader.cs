using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormLoader : MonoBehaviour {

    public GameObject stormPrefab;

	// Use this for initialization
	void Start () {

        Vector3 stormPos = new Vector3(0.0f, 1.0f, 0.0f);

        GameObject storm = Instantiate(stormPrefab, stormPos, Quaternion.identity);
		
	}
	

}
