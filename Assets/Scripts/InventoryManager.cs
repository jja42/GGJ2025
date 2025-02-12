using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public enum Items
    {
        GuarGum, //Heals
        FlavorExtract, //Raises Damage
        PowderedSugar,//Raises Speed
        TaffyCoat //Raises Def
    }

    public Dictionary<Unit, List<Items>> inventories;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        inventories = new Dictionary<Unit, List<Items>>();

        GenerateInventories();
    }

    void GenerateInventories()
    {
        foreach(Unit unit in GameManager.instance.allies)
        {
            inventories[unit] = new List<Items> { Items.GuarGum, Items.FlavorExtract, Items.PowderedSugar, Items.TaffyCoat };
        }
    }
}
