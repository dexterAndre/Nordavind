using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
	// Input
	private Vector2 inputL;
	private Vector2 inputR;
	private float inputTriggers;
	[SerializeField]
	private float triggersSensitivity;

	// Player State
	[System.Serializable]
	public enum PlayerState
	{
		Walk,
		Fall,
		Hang,
		Climb,
		Throw
	}

	// Movement
	[SerializeField]
	private CharacterController characterController;
	[SerializeField]
	private float movementSpeed;
	private Vector3 movementVector;

	// Camera
	[SerializeField]
	private CinemachineFreeLook freeLook;
	private Quaternion cameraDirection;
	private float[] rigRadii = new float[3];
	private float cameraDistance = 1f;



	private void Start ()
	{
		characterController = GetComponent<CharacterController>();
		for (int i = 0; i < 3; i++)
		{
			rigRadii[i] = freeLook.m_Orbits[i].m_Radius;
		}
	}

	private void Update ()
	{
		// Input
		inputL = new Vector2
		(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		);
		inputR = new Vector2
		(
			Input.GetAxisRaw("RHorizontal"),
			Input.GetAxisRaw("RVertical")
		);
		inputTriggers = Input.GetAxisRaw("Triggers");
		cameraDirection = Camera.main.transform.rotation;

		// Movement
		movementVector = new Vector3(inputL.x, movementVector.y, inputL.y);
		movementVector = Vector3.ProjectOnPlane((cameraDirection * movementVector), Vector3.up).normalized * movementSpeed * Time.deltaTime;

		// Camera controls
		// [The right stick is configured within Cinemachine]
		cameraDistance += inputTriggers * triggersSensitivity;
		if (cameraDistance < 0.0f)
			cameraDistance = 0.0f;

		for (int i = 0; i < 3; i++)
		{
			freeLook.m_Orbits[i].m_Radius = rigRadii[i] * cameraDistance;
		}

		characterController.Move(movementVector);
	}

	private void FixedUpdate ()
	{
	}
}