using UnityEngine;
using System.Collections;

public class PlayerPrototype : MonoBehaviour
{
	[SerializeField] private ushort playerID = 1;
	[SerializeField] private float walkSpeed = 5f;
	[SerializeField] private float turnSpeed = 3f;

	// On initialization
	private Rigidbody _rigidbody;

	// Use this for initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			// Vehicle in range?
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);

			foreach(Collider nearObject in hitColliders)
			{
				if(nearObject.transform.parent != null)
				{
					if(nearObject.transform.parent.tag == "Vehicle")
					{
						// Drive that car
						nearObject.transform.parent.GetComponent<CarPrototype>().StartCar(playerID);
					}
				}
			}
		}
	}
	
	// Physics
	void FixedUpdate()
	{
		float rotation = Input.GetAxis("P" + playerID.ToString() + " Horizontal") * turnSpeed;		// Turn value
		float speed = Input.GetAxis("P" + playerID.ToString() + " Vertical") * walkSpeed;			// Walk speed value

		// Rotation
		transform.Rotate(Vector3.up * rotation);

		// Position, Gravity
		if(!IsGrounded)
		{
			_rigidbody.velocity += -transform.up;
		}
		else
		{
			_rigidbody.velocity = transform.forward * speed;
		}
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
			return Physics.Raycast(transform.position, Vector3.down, 1.2f);
		}
	}
}
