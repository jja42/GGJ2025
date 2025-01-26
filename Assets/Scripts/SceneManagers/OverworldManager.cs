using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldManager : MonoBehaviour, InputSystem.IOverworldActions
{
    public static OverworldManager instance;

    public Unit selected_unit;

    //Unit objects to spawn
    public List<GameObject> unit_objects;

    public enum UnitType
    {
        player, enemy
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.input.Overworld.SetCallbacks(this);
        GameManager.instance.input.Overworld.Enable();
        LoadUnits();
    }

    //Using GameManager units list
    //Spawn units at designated positions
    public void LoadUnits()
    {
        SpawnUnit(UnitType.player, Vector3.zero);
        SpawnUnit(UnitType.enemy, new Vector3(1, 1));
        TileSystem.instance.UpdateAllNodes();
    }

    //Create Gameobject, Add Unit to list
    void SpawnUnit(UnitType type, Vector3 position)
    {
        GameObject obj = Instantiate(unit_objects[(int)type], position, Quaternion.identity);
        Unit unit = obj.GetComponent<Unit>();
        GameManager.instance.units.Add(unit);
    }

    //Calls movement for the unit
    //Removes Movement Squares UI
    //Prevents interruption
    public void MoveSelectedUnit(Vector3 destination)
    {
        selected_unit.Movement(destination);
        TileSystem.instance.ClearHighlights();
        GameManager.instance.canInterrupt = false;
    }

    //Set selected unit to null
    //Clear UI
    public void UnselectUnit()
    {
        selected_unit = null;
        TileSystem.instance.ClearHighlights();
        OverworldUI.instance.CloseItemMenu();
        OverworldUI.instance.CloseUnitMenu();
    }

    //TODO: Track Unit Position on Grid
    //Update Grid Position with function
    public void UpdatePosition(Vector3 position)
    {
        GameManager.instance.canInterrupt = true;
    }

    //Unselect a unit when we right click
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (selected_unit != null)
        {
            UnselectUnit();
        }
    }

    //Resets Unit Flags after turn end
    void ClearUnitFlags()
    {
        foreach (Unit unit in GameManager.instance.units)
        {
            unit.Acted = false;
            unit.Moved = false;
        }
    }
}
