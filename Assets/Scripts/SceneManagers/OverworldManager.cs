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

    public UnitType turn;

    public enum UnitType
    {
        ally, enemy
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
        StartCoroutine(OverworldUI.instance.PlayerBanner());
        turn = UnitType.ally;
    }

    //Spawn units at designated positions
    public void LoadUnits()
    {
        SpawnUnit(UnitType.ally, 1, new Vector3(-2, -9));
        SpawnUnit(UnitType.ally, 2, new Vector3(-4, -10));
        SpawnUnit(UnitType.ally, 3, new Vector3(-7, -9));
        SpawnUnit(UnitType.ally, 4, new Vector3(-1, -6));
        SpawnUnit(UnitType.ally, 5, new Vector3(-7, -7));
        SpawnUnit(UnitType.ally, 6, new Vector3(-3, -7));
        SpawnUnit(UnitType.enemy, 0, new Vector3(-16, 2));
        SpawnUnit(UnitType.enemy, 0, new Vector3(15, -2));
        SpawnUnit(UnitType.enemy, 0, new Vector3(-4, 8));
        SpawnUnit(UnitType.enemy, 0, new Vector3(16, 6));
        SpawnUnit(UnitType.enemy, 0, new Vector3(2, 4));
        TileSystem.instance.UpdateAllNodes();
    }

    //Create Gameobject, Add Unit to list
    void SpawnUnit(UnitType type, int unitIndex, Vector3 position)
    {
        GameObject obj = Instantiate(unit_objects[unitIndex], position, Quaternion.identity);
        Unit unit = obj.GetComponent<Unit>();
        unit.type = type;
        GameManager.instance.units.Add(unit);
        if(type == UnitType.ally)
        {
            GameManager.instance.allyCount++;
        }
        if(type == UnitType.enemy)
        {
            GameManager.instance.enemyCount++;
        }
    }

    //Calls movement for the unit
    //Removes Movement Squares UI
    //Prevents interruption
    public void MoveSelectedUnit(Vector3 destination)
    {
        TileSystem.instance.UpdateNode(TileSystem.instance.GetNode(selected_unit.gameObject.transform.position), false, false);
        TileSystem.instance.UpdateNode(TileSystem.instance.GetNode(destination), false, true);
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

    //Unselect a unit when we right click
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (selected_unit != null)
        {
            UnselectUnit();
        }
    }

    void CheckPlayerTurnEnd()
    {
        if(GameManager.instance.enemyCount == 0)
        {
            Victory();
            return;
        }

        int allyActedCount = 0;
        foreach (Unit unit in GameManager.instance.units)
        {
            if (unit.type == UnitType.ally)
            {
                if (unit.Acted)
                {
                    allyActedCount++;
                }
            }
        }
        if (allyActedCount == GameManager.instance.allyCount)
        {
            EndPlayerTurn();
        }
    }

    private void Victory()
    {
        GameManager.instance.canInterrupt = false;
        OverworldUI.instance.Victory();
    }

    private void Loss()
    {
        GameManager.instance.canInterrupt = false;
        OverworldUI.instance.Loss();
    }

    void EndPlayerTurn()
    {
        ClearUnitFlags();
        turn = UnitType.enemy;
        StartCoroutine(OverworldUI.instance.EnemyBanner());
        GameManager.instance.canInterrupt = false;
    }

    void EndEnemyTurn()
    {
        ClearUnitFlags();
        turn = UnitType.ally;
        StartCoroutine(OverworldUI.instance.PlayerBanner());
        GameManager.instance.canInterrupt = true;
    }


    public void Attack(Unit unit)
    {
        GameManager.instance.canInterrupt = false;
        selected_unit.Attack(unit);
        CheckPlayerTurnEnd();
        GameManager.instance.canInterrupt = true;
    }

    public void Pass()
    {
        selected_unit.Acted = true;
        CheckPlayerTurnEnd();
    }

    void CheckEnemyTurnEnd()
    {
        if (GameManager.instance.allyCount == 0)
        {
            Loss();
            return;
        }
        int enemyActedCount = 0;
        foreach (Unit unit in GameManager.instance.units)
        {
            if (unit.type == UnitType.enemy)
            {
                if (unit.Acted)
                {
                    enemyActedCount++;
                }
            }
        }
        if (enemyActedCount == GameManager.instance.enemyCount)
        {
            EndEnemyTurn();
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

    public void OnPan(InputAction.CallbackContext context)
    {
        if (turn == UnitType.ally)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Vector2 value = context.ReadValue<Vector2>();
                CameraManager.instance.SetPan(value);
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                CameraManager.instance.SetPan(Vector2.zero);
            }
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (turn == UnitType.ally)
        {
            if (context.phase == InputActionPhase.Started)
            {
                float value = context.ReadValue<float>();
                CameraManager.instance.Zoom(value);
            }
        }
    }

    public IEnumerator ProcessEnemyTurn()
    {
        CameraManager.instance.Zoom(-1);
        foreach(Unit unit in GameManager.instance.units)
        {
            if(unit.type == UnitType.enemy)
            {
                Enemy enemy = unit.gameObject.GetComponent<Enemy>();
                //enemy.Pass();
                yield return StartCoroutine(enemy.TakeTurn());
            }
        }
        yield return new WaitForSecondsRealtime(1f);
        CheckEnemyTurnEnd();
        yield return null;
    }
}
