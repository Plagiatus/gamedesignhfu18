using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasBehaviour : MonoBehaviour {

	public GameObject blueOverlay;
	public GameObject bluePopup;
	public GameObject redOverlay;
	public GameObject redPopup;
	Helper.Attacks attack = Helper.Attacks.None;

	public static CanvasBehaviour Instance;

	void Start(){
		Instance = this;
	}
	
	public void blueOverlayVisibility(bool x){
		blueOverlay.SetActive(x);
	}

	public void redOverlayVisibility(bool x){
		redOverlay.SetActive(x);
	}

	public void redPopupVisibility(bool x){
		redPopup.SetActive(x);
	}

	public void bluePopupVisibility(bool x){
		bluePopup.SetActive(x);
	}

	public void startGame(){
		attack = Helper.Attacks.Rain;
		if(blueOverlay.activeSelf){
			bluePopupVisibility(true);
		} else {
			redPopupVisibility(true);
		}
	}

	public void confirmed(){
		Config.newAttack = attack;
		SceneManager.LoadScene("Minigame_Micro");
	}

	public void closeAllPopups(){
		blueOverlay.SetActive(false);
		redOverlay.SetActive(false);
		bluePopup.SetActive(false);
		redPopup.SetActive(false);

		attack = Helper.Attacks.None;
	}
}
