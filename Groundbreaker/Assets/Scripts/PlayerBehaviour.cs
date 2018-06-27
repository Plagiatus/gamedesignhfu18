using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour {

	public Text playername;
	public void addName(string name){
		playername.text = name;
	}

	void Start(){
		if(this.name == "Player") playername.text = GameController.Instance.player.name;
	}
}
