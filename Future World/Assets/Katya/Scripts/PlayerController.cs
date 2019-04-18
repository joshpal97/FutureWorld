﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, Target
{

    public Transform rightGunBone;
    public Transform leftGunBone;
    public Arsenal[] arsenal;
    
    public Speed[] speeds;

    public Vector3 jump;

    private IDictionary<string, float> speedsDict;
    private string movementSpeedKey = "walk";

    private Actions actions;
    private Animator animator;
    private Vector3 movement;
    private Rigidbody playerRigidbody;

	public ParticleSystem muzzleFlash;
    
	public float health = 100f;
	public float range = 100f;

	private float yaw = 0.0f;

	private int arsenalIndex = 0;

    void Awake()
    {
        actions = GetComponent<Actions>();
        animator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
		if (arsenal.Length > 0)
			SetArsenal(arsenal[0].name);
        speedsDict = new Dictionary<string, float>();
        foreach (Speed item in speeds)
        {
            speedsDict.Add(item.name, item.speed);
        }
        jump = new Vector3(0.0f, 02.0f, 0.0f);
	}

	private void OnEnable()
    {
        playerRigidbody.isKinematic = false;
    }

    private void OnDisable()
    {
        playerRigidbody.isKinematic = true;
    }

    public void SetArsenal(string name)
    {
        foreach (Arsenal hand in arsenal)
        {
            if (hand.name == name)
            {
                if (rightGunBone.childCount > 0)
                    Destroy(rightGunBone.GetChild(0).gameObject);
                if (leftGunBone.childCount > 0)
                    Destroy(leftGunBone.GetChild(0).gameObject);
                if (hand.rightGun != null)
                {
                    GameObject newRightGun = (GameObject)Instantiate(hand.rightGun);
                    newRightGun.transform.parent = rightGunBone;
                    newRightGun.transform.localPosition = Vector3.zero;
                    newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
                if (hand.leftGun != null)
                {
                    GameObject newLeftGun = (GameObject)Instantiate(hand.leftGun);
                    newLeftGun.transform.parent = leftGunBone;
                    newLeftGun.transform.localPosition = Vector3.zero;
                    newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
				animator.runtimeAnimatorController = hand.controller;
				return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            actions.GetUp();
        }

        if (IsMoving() && IsJumping())
		{
            actions.Stay();
            actions.Jump();
            Jump();
        }
		else if (IsJumping())
		{
            actions.Jump();
            Jump();
        }
        else if (IsMoving() && IsRunning() && IsCrouching())
        {
            movementSpeedKey = "crouch-run";
            actions.Sitting();
            actions.Run();
        }
        else if (IsMoving() && IsRunning())
        {
            movementSpeedKey = "run";
            actions.Run();
        }
        else if (IsMoving() && IsCrouching())
        {
            movementSpeedKey = "crouch-walk";
            actions.Sitting();
            actions.Walk();
        }
        else if (IsMoving())
		{
            movementSpeedKey = "walk";
            actions.Walk();
		}
        else if (IsCrouching())
        {
            actions.Stay();
            actions.Sitting();
        }
		else
		{
			actions.Stay();
		}

		if (Input.GetMouseButtonDown(0))
		{
			actions.Attack();
			if (arsenalIndex != 0)
			{
				Shoot();
			}
			
		}
		if (Input.GetMouseButton(1))
		{
			actions.Aiming();
			
		}
        else if (Input.GetMouseButtonUp(1))
        {
            actions.Stay();
        }

		if (Input.GetKeyDown(KeyCode.F))
		{
			SwitchWeapon();
		}

	}

    private bool IsMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    }

    private bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private bool IsJumping()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool IsCrouching()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

	private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        Vector3 movement = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        gameObject.transform.position += movement * speedsDict[movementSpeedKey] * Time.deltaTime;
    }

    private void Turn()
    {
		yaw += speedsDict["turn"] * Input.GetAxis("Mouse X");
		gameObject.transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
    }

    private void Jump()
    {
        GetComponent<Rigidbody>().AddForce(jump * speedsDict["jump-force"], ForceMode.Impulse);
    }

	private void SwitchWeapon()
	{
		arsenalIndex++;
		if (arsenalIndex == arsenal.Length) arsenalIndex = 0;
		SetArsenal(arsenal[arsenalIndex].name);
		
	}

	void Shoot()
	{
        foreach (Arsenal hand in arsenal)
        {
            if (hand.name == arsenal[arsenalIndex].name)
            {
                if (hand.rightGun != null)
                {
                    GameObject rightGun = rightGunBone.GetChild(0).gameObject;
                    Shoot(rightGun, hand.damage);
                }
                if (hand.leftGun != null)
                {
                    GameObject leftGun = leftGunBone.GetChild(0).gameObject;
                    Shoot(leftGun, hand.damage);
                }
                return;
            }
        }		
	}

    private void Shoot(GameObject gun, float damage)
    {
        RaycastHit hit;
        if (Physics.Raycast(gun.transform.position, gameObject.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.takeDamage(damage);
            }
        }
    }

	public float Health
	{
		get { return this.health; }
		set { this.health = value; }
	}

	public void Die()
	{
		Destroy(gameObject);
	}

	public void takeDamage(float amount)
	{
		this.health -= amount;
		if (this.health <= 0)
		{
			actions.Death();
		}
		else
		{
			actions.Damage();
		}

	}

	private void OnCollisionEnter(Collision collision)
    {
        // TODO: Add collision handling depending on tags of objects
        actions.Damage();
    }

    [System.Serializable]
    public struct Arsenal
    {
        public string name;
        public GameObject rightGun;
        public GameObject leftGun;
        public RuntimeAnimatorController controller;
        public float damage;
    }

    [System.Serializable]
    public struct Speed
    {
        public string name;
        public float speed;
    }
}
