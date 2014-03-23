using UnityEngine;
using System.Collections;

public class InterfaceController : MonoBehaviour {

	private int pickups = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
	 	GUI.Label(new Rect(10,10,100,90), pickups.ToString());
	}

	void SetPickups(int pickups) {
		this.pickups = pickups;
	}
}


