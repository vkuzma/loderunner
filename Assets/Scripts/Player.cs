using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float movementX;
	private float movementY;
	private float maxSpeed = 1.5f;
	private float moveForce = 50f;

	private int groundMask  = 1 << 8;
	private int ladderMask = 1 << 9;

	private int ladderEntered = 0;

	private RaycastHit2D lastLadderHit;

	BoxCollider2D boxCollider2D;

	// Use this for initialization
	void Start () {
		boxCollider2D = (BoxCollider2D) collider2D;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		updateRaycast();

		xa.isFalling = true;

		if(xa.isRight)
			xa.leftBlocked = false;
		if(xa.isLeft)
			xa.rightBlocked = false;

		if(xa.isBeforeLadder &&	xa.isUp) {
			xa.isOnLadder = true;
		}
		else if(xa.isOnTopOfLadder && xa.isDown) {
			xa.isOnLadder = true;
		}
		// is not before ladder
		else if(!xa.isBeforeLadder){
			xa.isOnLadder = false;
		}

		// is on ladder
		if(xa.isOnLadder) {
			xa.isFalling = false;
			if(xa.isUp) {
				xa.bottomBlocked = false;
			}
		}

		// hit ground stop falling
		if(xa.bottomBlocked) {
			xa.isFalling = false;
		}

		UpdateMovement();
	}

	void UpdateMovement () {
		movementX = 0;
		movementY = 0;

		if(xa.isOnLadder) {
			if(xa.isUp) {
				movementY = 1f;
			}
			if(xa.isDown && !xa.bottomBlocked) {
				movementY = -1f;
			}

			// no x movement on ladder
			movementX = 0;
		}
		else {
			if(xa.isLeft && !xa.leftBlocked) movementX = -1f;
			if(xa.isRight && !xa.rightBlocked) movementX = 1f;
		}

		if(xa.isFalling) {
			movementY = -1f;
		}

		xa.lastMoveX = movementX * Time.deltaTime;
		xa.lastMoveY = movementY * Time.deltaTime;
		transform.position = new Vector2(transform.position.x + xa.lastMoveX, transform.position.y + xa.lastMoveY);

		if(movementY == 0f && xa.isOnLadder) {
//			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
		}

		// stop player immediatly
		if(movementX == 0f) {
//			rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);
		}
	}

	void updateRaycast() {
		float rayLengthY = Mathf.Abs(xa.lastMoveY * 2) + boxCollider2D.size.y / 2;
		float rayLengthX = Mathf.Abs(xa.lastMoveX * 2) + boxCollider2D.size.x / 2;
		if(xa.isFalling || xa.isDown) {
			RaycastHit2D hitBottom = Physics2D.Raycast(transform.position, -Vector2.up, rayLengthY, groundMask);

			// ground hit
			if (hitBottom) {
				xa.bottomBlocked = true;
				xa.isOnLadder = false;
				transform.position = new Vector2(transform.position.x, hitBottom.collider.transform.position.y + boxCollider2D.size.y);
			}
		}
		if(xa.isUp) {
			RaycastHit2D hitLadder = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - boxCollider2D.size.y / 2), Vector2.up, rayLengthY, ladderMask);
			
			// ground hit
			if (!hitLadder && lastLadderHit) {
				xa.bottomBlocked = true;
				xa.isOnLadder = false;
				xa.isBeforeLadder = false;
				transform.position = new Vector2(transform.position.x, lastLadderHit.collider.transform.position.y + boxCollider2D.size.y);
			}
			else {
				lastLadderHit = hitLadder;
			}
		}
		if(xa.isDown && !xa.isOnLadder) {
			RaycastHit2D hitLadder = Physics2D.Raycast(transform.position, -Vector2.up, boxCollider2D.size.y, ladderMask);
			Debug.Log("down");
			// ground hit
			if (hitLadder) {
				xa.bottomBlocked = false;
				xa.isOnLadder = true;
				Debug.Log ("OK");
			}
		}
		if(xa.isRight && !xa.rightBlocked) {
			RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, rayLengthY, groundMask);
			if (hitRight) {
				xa.rightBlocked = true;
				transform.position = new Vector2(hitRight.collider.transform.position.x - ((BoxCollider2D)hitRight.collider).size.x, transform.position.y);
			}
		}
		if(xa.isLeft && !xa.leftBlocked) {
			RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -Vector2.right, rayLengthY, groundMask);
			if (hitLeft) {
				xa.leftBlocked = true;

				transform.position = new Vector2(hitLeft.collider.transform.position.x + ((BoxCollider2D)hitLeft.collider).size.x, transform.position.y);
			}
		}
	}
	
	void OnTriggerStay2D (Collider2D other) {

	}
	
	void OnTriggerEnter2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			ladderEntered += 1;
			if(ladderEntered > 0)
				xa.isBeforeLadder = true;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			ladderEntered -= 1;
			if(ladderEntered == 0)
				xa.isBeforeLadder = false;
		}
	}
}
