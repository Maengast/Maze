using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	//Component
	private CharacterController ChrController;
	private Rigidbody PlayerRigidbody;
	public  Animator PlayerAnimator;
	
	//Value
	public float Speed;
	public float Gravity;
	public float AngularSpeed;
	private float GravityModifier = 0;
	public float PropulseFactor;

	private bool IsFloating = false;
	
	
	Vector3 MoveDir = Vector3.zero;

	private GameManager GameMngr;
	
	// Use this for initialization
	void Start ()
	{
		ChrController= gameObject.GetComponent<CharacterController>();
		PlayerRigidbody = gameObject.GetComponent<Rigidbody>();
		GravityModifier = 0;
		GameMngr = GameManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Play Anim
		PlayerAnimator.SetFloat("CurrentSpeed", ChrController.velocity.magnitude);
		
		//Reset Movement on y
		if (ChrController.isGrounded && !IsFloating)
		{
			MoveDir.y = 0.0f;
			GravityModifier = 0;
			PlayerAnimator.SetBool("Grounded", true);
		}
		

		float MoveForward = (Input.GetAxis("Vertical")>0)? Input.GetAxis("Vertical") : 0;

		float LastGravityModif = GravityModifier;
		
		if (GravityModifier > 0)
		{
			GravityModifier -= 0.1f;
		}
		
		if (IsFloating)
		{
			GravityModifier = LastGravityModif;
		}
		
		MoveDir.y += (Gravity + GravityModifier) * Time.deltaTime ;

		

		//Rotate and move player
		transform.Rotate(0.0f, Input.GetAxis("Horizontal")*AngularSpeed,0.0f);
		ChrController.Move(((transform.forward * MoveForward)+ MoveDir) * Speed * Time.deltaTime);
		
	}

	public void Float(int _Factor)
	{
		float FloatSpeed = (_Factor < 0) ? PropulseFactor*-1 : PropulseFactor + Mathf.Abs(Gravity);
		SetGravityModifier(_Factor*FloatSpeed);
	}

	public void SetIsFloating(bool _IsFloating)
	{
		IsFloating = _IsFloating;
		if (_IsFloating)
		{
			PlayerAnimator.SetBool("Grounded", false);
		}
	}

	private void SetGravityModifier(float _value)
	{
		GravityModifier = _value;
	}

	private void OnCollisionEnter(Collision other)
	{
		
	}
}
