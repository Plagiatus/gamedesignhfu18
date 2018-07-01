using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Micro : MonoBehaviour
{
    public float sensitivity = 60;
    public float loudness = 0;
    public GameObject _cube;
    public new AudioSource audio;
    bool won = false;

    public GameObject popup;
    bool paused = false;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = Microphone.Start(null, true, 10, 44100);
        audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audio.Play();
    }

    void Update()
    {
        if(paused) return;
        loudness = GetAveragedVolume() * sensitivity;
        if (loudness > 10 && !won)
        {
            _cube.SetActive(true);
            won = true;
            StartCoroutine(timeoutToWin());
        }
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }

    IEnumerator timeoutToWin(){
        Helper.ServerRequest attack = new Helper.ServerRequest() {request = Helper.ServerRequestType.CreatePoint};
        Helper.NewPointAttack pa = new Helper.NewPointAttack() {attack = Helper.Attacks.Rain, playerID = GameController.Instance.player.id, circleID = Config.attackedPoint.circleID, pointID = Config.attackedPoint.id};
        Debug.Log(pa.circleID + " " + pa.pointID + " " + pa.attack);
        StartCoroutine(UDPClient.Instance.SendRequest<Helper.NewPointAttack, Helper.CircleOfAction>(attack, pa));
        Config.minigameSuccess = true;
        Helper.ServerRequest getCirc = new Helper.ServerRequest() { request = Helper.ServerRequestType.RecieveCircle };
        StartCoroutine(UDPClient.Instance.SendRequest<Vector2, Helper.CircleOfAction>(getCirc, new Vector3(GameController.Instance.CurrentGpsPosition.Latitude, GameController.Instance.CurrentGpsPosition.Longitude), true, (returnValue) =>{
            GameController.Instance.circle = returnValue;
        }));
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("MainGame");
    }

    public void pressX(){
        popup.SetActive(true);
        paused = true;
    }

    public void abortGame(){
        SceneManager.LoadScene("MainGame");
    }

    public void abortCancellation(){
        popup.SetActive(false);
        paused = false;
    }
}
