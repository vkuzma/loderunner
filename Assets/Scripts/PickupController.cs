using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {

	private GameController gameController;

	// Use this for initialization
	void Start () {
		gameController = GameObject.Find("Game").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Player")) {
			gameController.SendMessage("PickedUp");
			Destroy(gameObject);
		}
	}
}
