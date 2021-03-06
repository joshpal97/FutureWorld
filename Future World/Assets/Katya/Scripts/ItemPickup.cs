﻿using UnityEngine;

public class ItemPickup : Interactable {

	public Item item;	// Item to put in the inventory if picked up

	// When the player interacts with the item
	public override void Interact()
	{
		PickUp();
	}

	// Pick up the item
	void PickUp ()
	{
		Debug.Log("Picking up " + item.name);
        // GameObject itemGO = gameObject;
        // item.setGameObject(itemGO);
		Inventory.instance.Add(item);	// Add to inventory
        
		Destroy(gameObject);	// Destroy item from scene
	}

}
