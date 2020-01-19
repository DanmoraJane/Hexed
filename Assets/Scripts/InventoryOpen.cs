using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpen : MonoBehaviour
{

    public GameObject inventory;

    public void OpenInventory()
    {
        if (inventory != null){
            bool isActive = inventory.activeSelf;
            inventory.SetActive(!isActive);
        }
    }
}
