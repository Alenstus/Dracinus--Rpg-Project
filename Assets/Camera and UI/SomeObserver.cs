using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeObserver : MonoBehaviour {

    CameraRaycaster cameraRaycaster;

	// Use this for initialization
	void Start () {
        cameraRaycaster = GetComponent<CameraRaycaster>();
        cameraRaycaster.onLayerChange += SomeHandlingFunction;
	}
	
	void SomeHandlingFunction(Layer newLayer)
    {
        print("Yahoo, handled from elsewhere");
    }
}
