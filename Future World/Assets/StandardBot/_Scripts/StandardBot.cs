﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBot : MonoBehaviour, Target {

    [Header("Set in Inspector")]
    public GameObject me;

    protected float health = 15f;
    protected float movementSpeed = 2f;

    protected int level = 1;

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
        Debug.Log("Standard Bot took " + amount + " damage");
        this.health -= amount;
        if (this.health <= 0)
        {
            Die();
        }
    }

    public int Level
	{
		get { return this.level; }
		set { this.level = value; }
	}
}
