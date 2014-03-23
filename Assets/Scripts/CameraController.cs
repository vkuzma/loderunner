using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
		
	public Transform player;

	private float xDifference = 0f;
	private float yDifference = 0f;
	private Vector2 margin;

	// Use this for initialization
	void Start () {
		margin = new Vector2(0.6f, 0.6f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void LateUpdate() {
		int xDirection = 1;
		int yDirection = 1;
		if(player) {
			xDifference = player.transform.position.x - transform.position.x;
			yDifference = player.transform.position.y - transform.position.y;
			if(Mathf.Abs(xDifference) > margin.x) {
				if(xDifference < 0)
					xDirection = -1;
				transform.position = new Vector3(player.transform.position.x - margin.x * xDirection, transform.position.y, transform.position.z);
			}
			if(Mathf.Abs(yDifference) > margin.y) {
				if(yDifference < 0)
					yDirection = -1;
				transform.position = new Vector3(transform.position.x, player.transform.position.y - margin.y * yDirection, transform.position.z);
			}
		}
	}
}
