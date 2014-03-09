using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float movementX;
	private float movementY;

	private int groundMask  = 1 << 8;
	private int ladderMask = 1 << 9;

	private int ladderEntered = 0;
	private float ladderHitArea = 0.08f;

	private Animator animator;

	private RaycastHit2D lastLadderHit;

	BoxCollider2D boxCollider2D;

	// Use this for initialization
	void Start () {
		boxCollider2D = (BoxCollider2D) collider2D;

		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		updateRaycast();

		xa.isFalling = true;

		if(xa.isRight) {
			xa.leftBlocked = false;
			animator.SetBool("walk", true);
			if(!xa.facingRight)
				Flip();
			xa.facingRight = true;
		}
		else if(xa.isLeft) {
			xa.rightBlocked = false;
			animator.SetBool("walk", true);

			if(xa.facingRight)
				Flip();

			xa.facingRight = false;
		}
		else {
			animator.SetBool("walk", false);
		}

		if(xa.isBeforeLadder &&	xa.isUp) {
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
	}

	void SnapToLadder() {

	}

	void Flip() {
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
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
		if(xa.isUp && xa.isBeforeLadder) {
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
		// is on top of a ladder
		if(xa.isDown && !xa.isOnLadder) {

			RaycastHit2D hitLadder = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.3f), -Vector2.up, boxCollider2D.size.y, ladderMask);
			// top ladder hit
			if (hitLadder) {
				Debug.Log ("hit");
				xa.bottomBlocked = false;
				xa.isOnLadder = true;
			}
		}
		if(xa.isRight && !xa.rightBlocked && !xa.isOnLadder) {
			RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, rayLengthY, groundMask);
			if (hitRight) {
				xa.rightBlocked = true;
				transform.position = new Vector2(hitRight.collider.transform.position.x - ((BoxCollider2D)hitRight.collider).size.x, transform.position.y);
			}
		}
		if(xa.isLeft && !xa.leftBlocked && !xa.isOnLadder) {
			RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -Vector2.right, rayLengthY, groundMask);
			if (hitLeft) {
				xa.leftBlocked = true;

				transform.position = new Vector2(hitLeft.collider.transform.position.x + ((BoxCollider2D)hitLeft.collider).size.x, transform.position.y);
			}
		}
	}
	
	void OnTriggerStay2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			if(ladderEntered > 0) {
				float ladderCenter = other.transform.position.x + ((BoxCollider2D) other.transform.collider2D).size.x / 2f;
				float playerCenter = transform.position.x + boxCollider2D.size.x / 2f;
				if(Mathf.Abs(ladderCenter - playerCenter) < ladderHitArea) {
					xa.isBeforeLadder = true;
				}
				else {
					xa.isBeforeLadder = false;
				}
			}
		}
	}
	
	void OnTriggerEnter2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			ladderEntered += 1;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			ladderEntered -= 1;
			if(ladderEntered == 0) {
				xa.isBeforeLadder = false;
			}
		}
	}
}
