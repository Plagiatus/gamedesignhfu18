﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public float avrgTime = 0.5f;
    public float peakLevel = 1.7f;
    public float endCountTime = 0.6f;
    public int shakeDir;
    public int shakeCount;

    Vector3 avrgAcc = Vector3.zero;
    int countPos;
    int countNeg;
    int lastPeak;
    int firstPeak;
    bool counting;
    float timer;

    bool ShakeDetector()
    {
        // read acceleration:
        Vector3 curAcc = Input.acceleration;
        // update average value:
        avrgAcc = Vector3.Lerp(avrgAcc, curAcc, avrgTime * Time.deltaTime);
        // calculate peak size:
        curAcc -= avrgAcc;
        // variable peak is zero when no peak detected...
        int peak = 0;
        // or +/- 1 according to the peak polarity:
        //Debug.Log("Peaks: " + curAcc.y + " and" + peakLevel + "Bool: " + (curAcc.y > peakLevel));
        if (curAcc.y > peakLevel) peak = 1;
        if (curAcc.y < -peakLevel) peak = -1;
        // do nothing if peak is the same of previous frame:
        if (peak == lastPeak)
            peak = 0;
        // peak changed state: process it
        lastPeak = peak; // update lastPeak
        if (peak != 0)
        { // if a peak was detected...
            timer = 0; // clear end count timer...
            counting = true;
            if (peak > 0) // and increment corresponding count
                countPos++;
            else
                countNeg++;
            if (!counting)
            { // if it's the first peak...
                counting = true; // start shake counting
                firstPeak = peak; // save the first peak direction
                
            }
            
        }
        else if (counting) // but if no peak detected...
        { // and it was counting...
            timer += Time.deltaTime; // increment timer
            Debug.Log("timer: " + timer + "Threshhold: " + endCountTime + " Bool: " + (timer > endCountTime));
            if (timer > endCountTime)
            { // if endCountTime reached...
                Debug.Log("I'm stupid and do not work for some reason");
                counting = false; // finish counting...
                shakeDir = firstPeak; // inform direction of first shake...
                if (countPos > countNeg) // and return the higher count
                    shakeCount = countPos;
                else
                    shakeCount = countNeg;
                // zero counters and become ready for next shake count
                countPos = 0;
                countNeg = 0;
                return true; // count finished
            }
        }
        Debug.Log(peak);
        return false;
    }

    // Update is called once per frame
    void Update () {
    if (ShakeDetector())
    { // call ShakeDetector every Update!
      // the device was shaken up and the count is in shakeCount
      // the direction of the first shake is in shakeDir (1 or -1)
            
    }
    // the variable counting tells when the device is being shaken:
    if (counting)
    {
            this.transform.position += Vector3.up * 0.01f;
            
    }
    else
    {
            Debug.Log("Not Shaking");
            this.transform.position += Vector3.up * 0.0f;
    }
}
}