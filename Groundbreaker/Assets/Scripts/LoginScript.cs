using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginScript : MonoBehaviour {

	public Text infoText;
	public InputField nameField;
	public InputField passwordField;

	void OnEnable(){
		UDPClient.OnLoginResult += playerLoggedIn;
	}

	void OnDisable(){
		UDPClient.OnLoginResult -= playerLoggedIn;
	}

	void playerLoggedIn(Helper.Player p){
		if(p == null){
			Debug.Log("<color=#f00>login failed</color>\nplease try again.");
			infoText.text = "<color=#f00>login failed</color> please try again.";
			passwordField.text = "";
		} else {
			Debug.Log("login successful.\nwelcome back " + p.name);
			infoText.text = "login successful.\nwelcome back " + p.name;
			GameController.Instance.player = p;
			StartCoroutine(loadMainGame());
		}
	}

	public void tryLogin(){
		infoText.text = "trying to login, please wait...";
		UDPClient.Instance.tryToLogin(nameField.text, passwordField.text);
	}

	IEnumerator loadMainGame(){
		yield return new WaitForSeconds(1);
		UDPClient.Instance.startPeriodicRequests();
		SceneManager.LoadScene("MainGame");
	}
}
