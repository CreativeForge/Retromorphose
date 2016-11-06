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

	// Variables
	private bool isGrounded = false;
	private uint ignoreGrounded = 0;		// How many frames should ground collision be ignored? (isGrounded fix)
	private bool actionButton = false;


	// Initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
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
		float speed = Input.GetAxis("P" + playerID.ToString() + " Vertical") * walkSpeed;			// Walk speed value

		// Rotation
		transform.Rotate(Vector3.up * rotation);

		// Position, Gravity
		if(!isGrounded)
		{
			_rigidbody.velocity += -transform.up;
		}
		else
		{
			_rigidbody.velocity = transform.forward * speed;
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
