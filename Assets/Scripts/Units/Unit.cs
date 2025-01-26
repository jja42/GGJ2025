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
    List<Vector3> path;
    HealthBar healthBar;

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
                    //Show Status
                }
            }
        }
    }

    public void Movement(Vector3 destination)
    {
        TileSystem.instance.UpdateNode(TileSystem.instance.GetNode(transform.position), false, false);
        TileSystem.instance.UpdateNode(TileSystem.instance.GetNode(destination), false, true);
        path = TileSystem.instance.GetPath(transform.position, destination);
        Moved = true;
        ParseMovement();
    }

    void ParseMovement()
    {
        Vector3 destination = path[0];
        path.Remove(destination);

        StartCoroutine(Move(destination));
    }

    IEnumerator Move(Vector3 dest)
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

        yield return null;
    }

    public void Attack(Unit target)
    {
        print(Name + " Attacks " + target.Name + "!");
        target.TakeDamage(damage, this);
        TileSystem.instance.ClearHighlights();
        GameManager.instance.canInterrupt = false;
    }

    public void TakeDamage(int Damage, Unit Attacker)
    {
        Damage = Damage - defense;
        print(Attacker.Name + " Deals " + Damage.ToString() + " Damage to " + Name + "!");
        health -= Damage;
        health = Mathf.Max(health, 0);
        healthBar.SetHealth(health);
        GameManager.instance.canInterrupt = true;
    }
}
