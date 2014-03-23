using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float movementX;
	private float movementY;

	private int groundMask  = 1 << 8;
	private int ladderMask = 1 << 9;
	private int ropeMask = 1 << 11;
	private int groundLadderMask = (1 << 8) | (1 << 9);

	private int ladderEntered = 0;
	private float ladderHitArea = 0.08f;

	private int ropeEntered = 0;

	private Animator animator;

	public Transform ShootPrefab;

	private RaycastHit2D lastLadderHit;

	private BoxCollider2D boxCollider2D;
	private Collider2D currentLadder;

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

		// is on ladder
		if(xa.isOnLadder) {
			xa.isFalling = false;
			if(xa.isUp) {
				xa.bottomBlocked = false;
			}
		}

		// hit ground stop falling
		if(xa.bottomBlocked || xa.isOnRope) {
			xa.isFalling = false;
		}

		if(xa.isBeforeLadder &&	xa.isUp && !xa.isFalling) {
			IsOnLadder(true);
		}
		// is not before ladder
		else if(!xa.isBeforeLadder){
			IsOnLadder(false);
		}

		if(xa.isOnRope) {
			animator.SetBool("rope", true);
		}
		else {
			animator.SetBool("rope", false);
		}

		if(xa.shoot && !xa.isShooting && !xa.isFalling && !xa.isOnLadder && !xa.isOnRope) {
			xa.isShooting = true;
			float xOffset = -0.3f;
			Shoot();
			if(xa.facingRight) {
				xOffset = 0.3f;
			}
			Transform shoot = (Transform) Instantiate(ShootPrefab, new Vector3(transform.position.x + xOffset, transform.position.y, 0), Quaternion.identity);
			if(xa.facingRight) {
				Vector3 theScale = shoot.localScale;
				theScale.x *= -1;
				shoot.localScale = theScale;
			}
		}

		if(xa.isFalling) {
			animator.SetBool("falling", true);
		}
		else {
			animator.SetBool("falling", false);
		}


		UpdateMovement();
	}

	void UpdateMovement () {
		movementX = 0;
		movementY = 0;

		if(xa.isOnLadder) {
			if(xa.isUp) {
				movementY = 1f;
				animator.SetBool("climb_move", true);
				transform.position = new Vector2(currentLadder.transform.position.x, transform.position.y);
			}
			else if(xa.isDown && !xa.bottomBlocked) {
				movementY = -1f;
				animator.SetBool("climb_move", true);
				transform.position = new Vector2(currentLadder.transform.position.x, transform.position.y);
			}
			else {
				animator.SetBool("climb_move", false);
			}

			// no x movement on ladder
			movementX = 0;
		}
		else {
			if(xa.isLeft && !xa.leftBlocked) movementX = -1f;
			else if(xa.isRight && !xa.rightBlocked) movementX = 1f;
			else animator.SetBool("walk", false);

			if((xa.isLeft && !xa.leftBlocked || xa.isRight && !xa.rightBlocked) && xa.isOnRope)
				animator.SetBool("rope_move", true);
			else
				animator.SetBool("rope_move", false);

			animator.SetBool("climb_move", false);
		}

		if(xa.isFalling) {
			movementY = -1f;
			xa.lastMoveX = 0;
		}
		else {
			xa.lastMoveX = movementX * Time.deltaTime;
		}
		xa.lastMoveY = movementY * Time.deltaTime;

		transform.position = new Vector2(transform.position.x + xa.lastMoveX, transform.position.y + xa.lastMoveY);
	}

	void Shoot() {
		StartCoroutine("ShootingAnimation");
	}

	IEnumerator ShootingAnimation() {
		animator.SetBool("shoot", true);
		yield return new WaitForSeconds(.5f);
		animator.SetBool("shoot", false);
		xa.isShooting = false;
	}

	void IsOnLadder(bool state) {
		xa.isOnLadder = state;
		animator.SetBool("climb", state);
	}


	void Flip() {
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void updateRaycast() {

		RaycastHit2D hitBottommLeft = Physics2D.Raycast(new Vector2(transform.position.x - boxCollider2D.size.x / 2, transform.position.y), -Vector2.up, 0.3f, groundLadderMask);
		RaycastHit2D hitBottommRight = Physics2D.Raycast(new Vector2(transform.position.x + boxCollider2D.size.x / 2, transform.position.y), -Vector2.up, 0.3f, groundLadderMask);
		
		// ground hit
		if (!hitBottommLeft && !hitBottommRight) {
			xa.isFalling = true;
			xa.bottomBlocked = false;
		}

		float rayLengthY = Mathf.Abs(xa.lastMoveY * 2) + boxCollider2D.size.y / 2;
		float rayLengthX = Mathf.Abs(xa.lastMoveX * 2) + boxCollider2D.size.x / 2;

		// is falling or pushing down
		if(xa.isFalling || xa.isDown) {
			RaycastHit2D hitBottomLeft = Physics2D.Raycast(new Vector2(transform.position.x - boxCollider2D.size.x / 2, transform.position.y), -Vector2.up, rayLengthY, groundMask);
			RaycastHit2D hitBottomRight = Physics2D.Raycast(new Vector2(transform.position.x + boxCollider2D.size.x / 2, transform.position.y), -Vector2.up, rayLengthY, groundMask);


			// ground hit
			if (hitBottomLeft || hitBottomRight) {
				xa.bottomBlocked = true;
				IsOnLadder(false);
				if(hitBottomLeft)
					transform.position = new Vector2(transform.position.x, hitBottomLeft.collider.transform.position.y + boxCollider2D.size.y);
				if(hitBottomRight)
					transform.position = new Vector2(transform.position.x, hitBottomRight.collider.transform.position.y + boxCollider2D.size.y);
			}
		}
		// is trying to climb a ladder
		if(xa.isUp && xa.isBeforeLadder) {
			RaycastHit2D hitLadder = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - boxCollider2D.size.y / 2), Vector2.up, rayLengthY, ladderMask);

			// ground hit
			if (lastLadderHit && !hitLadder) {
				xa.bottomBlocked = true;
				xa.isBeforeLadder = false;
				//transform.position = new Vector2(transform.position.x, lastLadderHit.collider.transform.position.y + boxCollider2D.size.y);
			}
			else {
				lastLadderHit = hitLadder;
			}
		}

		// is not on a ladder
		if(!xa.isOnLadder) {
			// is pressing down
			if(xa.isDown) {
				// raycast to ladder
				RaycastHit2D hitLadderLeft = Physics2D.Raycast(new Vector2(transform.position.x - boxCollider2D.size.x / 2, transform.position.y - 0.3f), -Vector2.up, boxCollider2D.size.y, ladderMask);
				RaycastHit2D hitLadderRight = Physics2D.Raycast(new Vector2(transform.position.x + boxCollider2D.size.x / 2, transform.position.y - 0.3f), -Vector2.up, boxCollider2D.size.y, ladderMask);

				// top ladder hit
				if (hitLadderLeft || hitLadderRight) {
					xa.bottomBlocked = false;
					IsOnLadder(true);
				}

				// snap player to ladder
				if(hitLadderLeft)
					transform.position = new Vector2(hitLadderLeft.transform.position.x, transform.position.y);
				if(hitLadderRight)
					transform.position = new Vector2(hitLadderRight.transform.position.x, transform.position.y);
			}

			// is pressing right, no right blocked or falling
			if(xa.isRight && !xa.rightBlocked || xa.isFalling) {
				RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, rayLengthY, groundMask);
				if (hitRight) {
					xa.rightBlocked = true;
					transform.position = new Vector2(hitRight.collider.transform.position.x - ((BoxCollider2D)hitRight.collider).size.x, transform.position.y);
				}
				else {
					xa.rightBlocked = false;
				}
			}

			// is pressing left, no left blocked or falling
			if(xa.isLeft && !xa.leftBlocked || xa.isFalling) {
				RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -Vector2.right, rayLengthY, groundMask);
				if (hitLeft) {
					xa.leftBlocked = true;

					transform.position = new Vector2(hitLeft.collider.transform.position.x + ((BoxCollider2D)hitLeft.collider).size.x, transform.position.y);
				}
				else {
					xa.leftBlocked = false;
				}
			}
		}


	}
	
	void OnTriggerStay2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			if(ladderEntered > 0) {
				xa.isBeforeLadder = true;
			}
			else {
				xa.isBeforeLadder = false;
			}
		}

		if(other.CompareTag("Rope")) {
			if(ropeEntered > 0) {
				xa.isOnRope = true;
			}
			else {
				xa.isOnRope = false;
			}
		}
	}
	
	void OnTriggerEnter2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			ladderEntered += 1;
			currentLadder = other;
		}

		if(other.CompareTag("Rope")) {
			ropeEntered += 1;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if(other.CompareTag("Ladder")) {
			ladderEntered -= 1;
			if(ladderEntered == 0) {
				xa.isBeforeLadder = false;
			}
		}

		if(other.CompareTag("Rope")) {
			ropeEntered -= 1;
			xa.isOnRope = false;
			if(ropeEntered == 0) {
				xa.isOnRope = false;
			}
		}
	}
}
