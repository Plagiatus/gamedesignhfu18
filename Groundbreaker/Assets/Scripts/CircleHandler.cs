using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleHandler : MonoBehaviour {

    public Text x_value;
    public Text y_value;
    public Text z_value;
    public Text previousPeak;
    public float peakLevel = 1.7f;

    private Vector3 avrAcc;
    private float avrgTime = 0.5f;

    // Use this for initialization
    void Start () {
        Vector3 avrAcc = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 curAcc = Input.acceleration;
        
        Vector3.Lerp(avrAcc, curAcc, avrgTime * Time.deltaTime);
        curAcc -= avrAcc;
      

        int peak_X = 0;
        int peak_Y = 0;
        int peak_Z = 0;
        // or +/- 1 according to the peak polarity:
        //Debug.Log("Peaks: " + curAcc.y + " and" + peakLevel + "Bool: " + (curAcc.y > peakLevel));
        if (curAcc.x > peakLevel) peak_X = 1;
        if (curAcc.x < -peakLevel) peak_X = -1;
        if (curAcc.y > peakLevel) peak_Y = 1;
        if (curAcc.y < -peakLevel) peak_Y = -1;
        if (curAcc.z > peakLevel) peak_Z = 1;
        if (curAcc.z < -peakLevel) peak_Z = -1;
        x_value.text = peak_X.ToString();
        y_value.text = peak_Y.ToString();
        z_value.text = peak_Z.ToString();
        Debug.Log("I don't work");
        if(peak_X != 0 || peak_Y != 0 || peak_Z != 0)
        {
            previousPeak.text = "X Peak: " + peak_X + " " + "Y Peak: " + peak_Y + " " + "Z Peak: " + peak_Z;
        }
    }
}
