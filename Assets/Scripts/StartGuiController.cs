using UnityEngine;
using System.Collections;

public class StartGuiController : MonoBehaviour {

	float screenCenter;
	float btnWidth;
	float btnHeight;

	// Use this for initialization
	void Start () {
		btnWidth = 150;
		btnHeight = 50;
		screenCenter = Screen.width / 2 - btnWidth  / 2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {
		if (GUI.Button (new Rect (screenCenter, 10, btnWidth, btnHeight), "Start Game")) {
			Application.LoadLevel("Scene1");
		}

		if (GUI.Button (new Rect (screenCenter, 2 * (10 + btnHeight), btnWidth, btnHeight), "Settings")) {
			Application.LoadLevel("Scene1");
		}

		if (GUI.Button (new Rect (screenCenter, 3 * (10 + btnHeight), btnWidth, btnHeight), "Quit Game")) {
			Application.LoadLevel("Scene1");
		}
	}
}
