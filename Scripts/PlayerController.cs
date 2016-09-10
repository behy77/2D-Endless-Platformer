using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	private float moveSpeedStore;
	public float speedMultiplier;

	public float speedIncreaseMilestone;
	private float speedIncreaseMilestonStore;

	private float speedMilestoneCount;
	private float speedMilestoneCountStore;


	public float jumpForce;

	public float jumpTime;
	private float jumpTimeCounter;

	private bool stoppedJumping;
	private bool canDoubleJump;

	private Rigidbody2D myRigidbody;

	public bool grounded;
	public LayerMask whatIsGround;
	public Transform groundCheck;
	public float groundCheckRadius;

	//private Collider2D myCollider;

	private Animator myAnimator;

	public GameManager theGameManager;

	public AudioSource jumpSound;
	public AudioSource deathSound;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();

		//myCollider = GetComponent<Collider2D> ();

		myAnimator = GetComponent<Animator> ();

		jumpTimeCounter = jumpTime;

		speedMilestoneCount = speedIncreaseMilestone;

		moveSpeedStore = moveSpeed;
		speedMilestoneCountStore = speedMilestoneCount;
		speedIncreaseMilestonStore = speedIncreaseMilestone;

		stoppedJumping = true;
	}
	
	// Update is called once per frame
	void Update () {

		//grounded = Physics2D.IsTouchingLayers (myCollider, whatIsGround);

		grounded = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, whatIsGround);

		if (transform.position.x > speedMilestoneCount) {
			speedMilestoneCount += speedIncreaseMilestone;

			speedIncreaseMilestone = speedIncreaseMilestone * speedMultiplier;

			moveSpeed = moveSpeed * speedMultiplier;
		}

		myRigidbody.velocity = new Vector2 (moveSpeed, myRigidbody.velocity.y);

		if (Input.GetKeyDown (KeyCode.Space) || Input.GetMouseButtonDown (0)) {

			if (grounded) {
				myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpForce);	
				stoppedJumping = false;
				jumpSound.Play ();
			}

			if (!grounded && canDoubleJump) {
				myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpForce);
				jumpTimeCounter = jumpTime;
				stoppedJumping = false;
				canDoubleJump = false;
				jumpSound.Play ();
			}
		}

		if ((Input.GetKey (KeyCode.Space) || Input.GetMouseButton (0)) && !stoppedJumping) {
			if (jumpTimeCounter > 0) {
				myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpForce); 
				jumpTimeCounter -= Time.deltaTime;
			}
		}

		if (Input.GetKeyUp (KeyCode.Space) || Input.GetMouseButtonUp (0)) {
			jumpTimeCounter = 0;
			stoppedJumping = true;
		}

		if (grounded) {
			jumpTimeCounter = jumpTime;
			canDoubleJump = true;
		}

		myAnimator.SetFloat ("Speed", myRigidbody.velocity.x);
		myAnimator.SetBool ("Grounded", grounded);
	}

	void OnCollisionEnter2D (Collision2D other){
		if (other.gameObject.tag == "killbox") {
			theGameManager.RestartGame ();
			moveSpeed = moveSpeedStore;
			speedMilestoneCount = speedMilestoneCountStore;
			speedIncreaseMilestone = speedIncreaseMilestonStore;
			deathSound.Play ();
		}
	}
}
