using UnityEngine;
using System.Collections;

public class PlayerPrototype : MonoBehaviour
{
	[SerializeField] private ushort playerID = 1;
	[SerializeField] private float walkSpeed = 5f;
	[SerializeField] private float turnSpeed = 3f;
	[SerializeField] private float jumpForce = 3f;
	[SerializeField] private float interactionRadius = 0.8f;

	// On initialization
	private Rigidbody _rigidbody;
	private Animator _anim;
	private SmokeEmitter _smokeEmitter;

	// Variables
	private bool isGrounded = false;		// Is player grounded?
	private uint ignoreGrounded = 0;		// How many frames should ground collision be ignored? (isGrounded fix)
	private bool actionButton = false;		// Action-button
	private bool smokeEmitter = false;		// Should emit smoke?


	// Initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_anim = GetComponent<Animator>();
		_smokeEmitter = GetComponent<SmokeEmitter>();
	}

	void Update()
	{
		// Action-Button
		if(Input.GetKeyDown(KeyCode.Space))
		{
			actionButton = true;
		}

		// Collision fix: isGrounded detection
		if(ignoreGrounded > 0)
			ignoreGrounded--;
	}
	
	// Physics
	void FixedUpdate()
	{
		float rotation = Input.GetAxis("P" + playerID.ToString() + " Horizontal") * turnSpeed;		// Turn value
		float speed = Input.GetAxis("P" + playerID.ToString() + " Vertical");						// Walk speed value
		smokeEmitter = speed > 0f ? true : false;													// Walking player should emit smoke

		// Animation
		_anim.SetFloat("Speed", Mathf.Abs(speed));

		// Generate walking-speed
		speed *= walkSpeed;

		// Rotation
		transform.Rotate(Vector3.up * rotation);

		// Position, Gravity
		if(!isGrounded)
		{
			_rigidbody.velocity -= transform.up;
		}
		else
		{
			_rigidbody.velocity = transform.forward * speed;

			// Emit smoke?
			if(smokeEmitter)
				_smokeEmitter.SetActive(true);
			else
				_smokeEmitter.SetActive(false);
		}

		// Action-Button
		if(actionButton)
		{
			bool jump = true;

			actionButton = false;

			// Vehicle in range?
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius);

			foreach(Collider nearObject in hitColliders)
			{
				if(nearObject.tag == "VehicleDoor")
				{
					// Drive that car
					jump = false;
					isGrounded = false;
					nearObject.transform.parent.GetComponent<CarPrototype>().StartCar(playerID);
				}
			}

			// Jump?
			if(isGrounded && jump)
			{
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpForce, _rigidbody.velocity.z);
				isGrounded = false;
				ignoreGrounded = 5;
			}
		}
	}

	// Collision handling
	void OnCollisionStay(Collision collisionInfo)
	{
		bool collisionFeet = false;

		foreach(ContactPoint contact in collisionInfo.contacts)
		{
			if(contact.normal.y > 0f)
				collisionFeet = true;
		}

		if(collisionFeet && !isGrounded)
		{
			// Ignore grounded? Used for jumping...
			if(ignoreGrounded == 0)
			{
				isGrounded = true;
			}
		}
	}

	void OnCollisionExit(Collision collisionInfo)
	{
		isGrounded = false;
		_smokeEmitter.SetActive(false);
	}


	// Public methods

	// Virtually invert action-button
	public void ActionButton()
	{
		actionButton ^= true;
	}


	// Properties

	public ushort PlayerID
	{
		get { return this.playerID;}
	}
	
	public bool IsGrounded
	{
		get
		{
			return this.isGrounded;
		}
	}
}
