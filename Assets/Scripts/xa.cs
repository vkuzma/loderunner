using UnityEngine;
using System.Collections;


public class xa : MonoBehaviour {

	public static float moveDir;

	public static bool isBeforeLadder;
	public static bool isOnLadder;

	public static bool bottomBlocked;
	public static bool rightBlocked;
	public static bool leftBlocked;
	public static bool facingRight;

	public static bool isUp;
	public static bool isDown;
	public static bool isLeft;
	public static bool isRight;

	public static bool isFalling;

	public static float lastMoveX;
	public static float lastMoveY;


	// Use this for initialization
	void Start () {
		facingRight = true;
	}
	
	// Update is called once per frame
	void Update () {

		isLeft = false;
		isRight = false;
		isUp = false;
		isDown = false;

		if(Input.GetKey(KeyCode.LeftArrow)) {
			isLeft = true;	
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			isRight = true;	
		}
		if(Input.GetKey(KeyCode.UpArrow)) {
			isUp = true;	
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			isDown = true;	
		}
	}
}
