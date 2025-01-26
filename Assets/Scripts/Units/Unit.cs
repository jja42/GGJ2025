using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HealthBar))]
public class Unit : MonoBehaviour, IPointerClickHandler
{
    public string Name;
    public bool Acted;
    public bool Moved;
    public int health;
    public int maxHealth;
    public int speed;
    public int range;
    public int damage;
    public int defense;
    public float moveSpeed = 2.5f;
    protected List<Vector3> path;
    HealthBar healthBar;
    public OverworldManager.UnitType type;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        path = new List<Vector3>();
        healthBar = GetComponent<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.canInterrupt)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (OverworldManager.instance.selected_unit == null)
                {
                    OverworldManager.instance.selected_unit = this;
                    OverworldUI.instance.UnitMenuStats();
                }
            }
        }
    }

    public void Movement(Vector3 destination)
    {
        path = TileSystem.instance.GetPath(transform.position, destination);
        ParseMovement();
    }

    void ParseMovement()
    {
        Vector3 destination = path[0];
        path.Remove(destination);

        StartCoroutine(Move(destination));
    }

    protected IEnumerator Move(Vector3 dest)
    {
        float t = 0;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, dest, t);
            yield return null;
        }

        if (path.Count > 0)
        {
            ParseMovement();
        }
        else
        {
            Moved = true;
            if(type == OverworldManager.UnitType.ally)
            {
                OverworldUI.instance.OpenUnitMenu();
            }
            GameManager.instance.canInterrupt = true;
        }

        yield return null;
    }

    public void Attack(Unit target)
    {
        Acted = true;
        target.TakeDamage(damage, this);
        TileSystem.instance.ClearHighlights();
        StartCoroutine(OverworldUI.instance.CombatText(this, target, damage - target.defense));
    }

    public void TakeDamage(int Damage, Unit Attacker)
    {
        Damage = Damage - defense;
        health -= Damage;
        health = Mathf.Max(health, 0);
        healthBar.SetHealth(health);
        if (health == 0)
        {
            GameManager.instance.units.Remove(this);
            if(type == OverworldManager.UnitType.enemy)
            {
                GameManager.instance.enemyCount--;
            }
            else
            {
                GameManager.instance.allyCount--;
            }
            TileSystem.instance.UpdateNode(TileSystem.instance.GetNode(transform.position), false, false);
            Destroy(gameObject);
        }
    }
}
