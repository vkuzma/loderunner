using UnityEngine;
using System.Collections;


public class Shoot : MonoBehaviour {

	private int groundMask  = 1 << 8;
	private RaycastHit2D hitBottom;
	private Brick brick;
	private bool shootFired;


	// Use this for initialization
	void Start () {
		Destroy(gameObject, 0.8f);
		shootFired = false;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		hitBottom = Physics2D.Raycast(transform.position, -Vector2.up, 0.3f, groundMask);
		if(hitBottom)
			brick = hitBottom.transform.gameObject.GetComponent<Brick>();
		
		// ground hit
		if (hitBottom && !shootFired) {
			shootFired = true;
			StartCoroutine("ProcessDestroy");
		}
	}

	IEnumerator ProcessDestroy() {

		yield return new WaitForSeconds(0.2f);
		brick.ProcessHit();
	}
}
