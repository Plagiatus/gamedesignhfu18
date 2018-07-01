using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginScript : MonoBehaviour {

	public Text infoText;
	public Text teamText;
	public InputField nameField;
	public InputField passwordField;

	public GameObject rSelected;
	public GameObject bSelected;

	public GameObject Register;
	public GameObject TeamSelect;

	void OnEnable(){
		UDPClient.OnLoginResult += playerLoggedIn;
		UDPClient.OnRegisterResult += playerRegistered;
	}

	void OnDisable(){
		UDPClient.OnLoginResult -= playerLoggedIn;
		UDPClient.OnRegisterResult -= playerRegistered;
	}

	void playerLoggedIn(Helper.Player p){
		if(p == null){
			Debug.Log("<color=#f00>login failed</color>\nplease try again.");
			infoText.text = "login failed. please try again.";
			passwordField.text = "";
		} else {
			Debug.Log("login successful.\nwelcome back " + p.name);
			infoText.text = "login successful.\nwelcome back " + p.name;
			GameController.Instance.player = p;
			StartCoroutine(loadMainGame());
		}
	}

	void playerRegistered(Helper.Player p){
		if(p == null){
			Debug.Log("Registration failed. someone else already has that name");
			infoText.text =  "registration failed. someone else probably already has that name";
			passwordField.text = "";
		} else {
			Debug.Log("register successful.\nwelcome " + p.name);
			infoText.text = "register successful.\nwelcome " + p.name;
			GameController.Instance.player = p;
			StartCoroutine(loadTeamSelection());
		}
	}

	public void tryLogin(){
		infoText.text = "trying to login, please wait...";
		UDPClient.Instance.tryToLogin(nameField.text, passwordField.text);
	}

	public void tryRegister(){
		infoText.text = "registering new account. please wait.";
		Debug.Log("regstering..");
		UDPClient.Instance.tryToRegister(nameField.text, passwordField.text);
	}

	public void teamChosen(){
		teamText.text = "welcome aboard, ";
		if(rSelected.activeSelf){
			teamText.text += "invader.";
		} else {
			teamText.text += "defender.";
		}
	}

	IEnumerator loadMainGame(){
		yield return new WaitForSeconds(1);
		UDPClient.Instance.startPeriodicRequests();
		SceneManager.LoadScene("MainGame");
	}

	IEnumerator loadTeamSelection(){
		yield return new WaitForSeconds(2);
		Register.SetActive(false);
		TeamSelect.SetActive(true);
	}

	public void chooseRed(){
		rSelected.SetActive(true);
		bSelected.SetActive(false);
	}

	public void chooseBlue(){
		rSelected.SetActive(false);
		bSelected.SetActive(true);
	}




}
