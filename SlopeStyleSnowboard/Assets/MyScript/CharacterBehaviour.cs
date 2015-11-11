using UnityEngine;
using System.Collections;

public class CharacterBehaviour : MonoBehaviour 
{
	public float speedRotation, speedTranslation, speedJump, angleFall;
	private float rotH, rotV, jumpTime, fallTime, rotTime;
	private Vector3 moveDirection;
	private CharacterController controller;
	private bool isJumping , isFalling, canRotate;
	private Vector2 jumpDir;
	private Quaternion rotQuaternion, endRotQuaternion;
	public MPJoystick leftJoystick;
	public Button rightJoystick;
	
	public float angleZ, CosangleZ;
	
	// Use this for initialization
	void Start () 
	{
		moveDirection = Vector3.zero;
		controller = GetComponent<CharacterController>();
		isJumping = false;
		isFalling = false;
		rotQuaternion = transform.parent.rotation;
	}
	
	// Update is called once per frame
	void Update () 
	{	
		// Rotate the controller
		//*
	#if UNITY_IPHONE || UNITY_ANDROID
		rotH = leftJoystick.position.x * speedRotation * Time.deltaTime;
		rotV = leftJoystick.position.y * speedRotation * Time.deltaTime;
		
		if(rotH < 0.1 && rotH > -0.1)
			rotH = 0;
		if(rotV < 0.1 && rotV > -0.1)
			rotV = 0;
		
		if((rotH - rotV) > 10)
			rotV = 0;
		else if((rotV - rotH) > 10)
			rotH = 0;
	#else
		
		rotH = Input.GetAxis("Horizontal") * speedRotation * Time.deltaTime;
		rotV = Input.GetAxis("Vertical") * speedRotation * Time.deltaTime;
	#endif

		transform.Rotate(new Vector3(rotV,rotH,0.0f), Space.World);
		
		CosangleZ = Vector3.Dot(transform.parent.forward.normalized, transform.forward.normalized);
		angleZ = Mathf.Acos(CosangleZ) * Mathf.Rad2Deg;
		
		//allineamento con la direzione ed eventuale caduta
		if(controller.isGrounded)
		{
			if((angleZ > angleFall) && (angleZ < 180 - angleFall))
			{
				//transform.Rotate(new Vector3(0.0f, 0.0f, (90.0f)*Mathf.Deg2Rad), Space.World);
			}
			else
			{
				if((rotH != 0) || (rotV != 0))
				{
					canRotate = false;
				}
				else if(!canRotate)
				{
					if(angleZ <= angleFall)
					{
						canRotate = true;
						rotTime = Time.time;
						rotQuaternion = controller.transform.rotation;
						endRotQuaternion = transform.parent.rotation;
					}
					else if(angleZ >= 180 - angleFall)
					{
						canRotate = true;
						rotTime = Time.time;
						rotQuaternion = controller.transform.rotation;
						endRotQuaternion = Quaternion.LookRotation(-transform.parent.forward, transform.parent.up);
					}
				}
				
				if(canRotate)
				{
					transform.rotation = Quaternion.Lerp(rotQuaternion, endRotQuaternion, Time.time - rotTime);
				}
			}
		}
		
		
		// Direction of Movement
		moveDirection = transform.parent.forward;
        
		// Jump
		//*
	#if UNITY_IPHONE || UNITY_ANDROID
		if((rightJoystick.singleShoot()) && controller.isGrounded)
		{
			isJumping = true;
			jumpTime = Time.time;
        }
	#else
		
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) 
		{
			isJumping = true;
			jumpTime = Time.time;
        }
	#endif
		
		if(!isJumping && !controller.isGrounded && !isFalling)
		{
			isFalling = true;
			fallTime = Time.time;
		}
		
		if(isJumping)
		{
			jumpDir = Impulse((transform.parent.up + moveDirection).normalized * speedJump, moveDirection.normalized, Time.time - jumpTime);
			// Apply direction of jump
            Vector3 resultJump = (transform.parent.forward * jumpDir.x) + (transform.parent.up * jumpDir.y);
			// Apply speed of transation
			moveDirection = resultJump.normalized;
			moveDirection.y *= speedJump;
			moveDirection.x *= speedTranslation;
			moveDirection.z *= speedTranslation;
		}
		else if(isFalling)
		{
			jumpDir = ImpulseFall(moveDirection.normalized * speedJump, moveDirection.normalized, Time.time - fallTime);
			// Apply direction of jump
            Vector3 resultJump = (transform.parent.forward * jumpDir.x) + (transform.parent.up * jumpDir.y);
			// Apply speed of transation
			moveDirection = resultJump.normalized;
			moveDirection.y *= speedJump;
			moveDirection.x *= speedTranslation;
			moveDirection.z *= speedTranslation;
		}
		else
		{
			// Apply Speed
			moveDirection = moveDirection.normalized;
			moveDirection *= speedTranslation;
    		// Apply gravity
    		moveDirection.y += Physics.gravity.y;
		}
    
    	// Move the controller
    	controller.Move(moveDirection * Time.deltaTime);
		
		if(controller.isGrounded)
		{
			if(jumpTime != Time.time)
			{
				isJumping = false;
			}
			if(fallTime != Time.time)
			{
				isFalling = false;
			}
		}
	}
	
	// Calcolate each frame witch direction the controller move to jump
	Vector2 Impulse(Vector3 direction, Vector3 localAxeX, float deltaTime)
    {
        float xt = 0;
        float yt = 0;
        float teta = 0;

        teta = Vector3.Dot(direction.normalized, localAxeX.normalized);

        xt = direction.magnitude * Mathf.Cos(teta) * deltaTime;
        yt = (direction.magnitude * Mathf.Sin(teta) * deltaTime) - (0.5f * 9.8f * deltaTime * deltaTime);

        return new Vector2(xt, yt);
    }
	
	Vector2 ImpulseFall(Vector3 direction, Vector3 localAxeX, float deltaTime)
    {
        float xt = 0;
        float yt = 0;
        float teta = 0;

        teta = Vector3.Dot(direction.normalized, localAxeX.normalized);

        xt = direction.magnitude * Mathf.Cos(teta) * deltaTime;
        yt = - (0.5f * 9.8f * deltaTime * deltaTime);

        return new Vector2(xt, yt);
    }
}
