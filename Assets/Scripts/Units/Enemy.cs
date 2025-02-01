using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    Unit target;

    IEnumerator GetTarget()
    {
        float distance = Mathf.Infinity;
        foreach(Unit unit in GameManager.instance.allies)
        {
            if (Vector3.Distance(transform.position, unit.gameObject.transform.position) < distance)
            {
                target = unit;
                distance = Vector3.Distance(transform.position, unit.gameObject.transform.position);
            }
        }
        print("Target Selected: " + target.Name);
        yield return null;
    }

    IEnumerator AttemptMove()
    {
        List<TileSystem.Node> nodes = TileSystem.instance.CalculateMovement(transform.position, speed);
        TileSystem.instance.GenerateMovementHighlights(nodes);
        TileSystem.Node targetNode = TileSystem.instance.GetNode(target.transform.position);
        nodes.Clear();
        nodes = TileSystem.instance.CalculatePath(TileSystem.instance.GetNode(transform.position), targetNode);
        nodes.Reverse();

        for(int i = 0; i < speed; i++)
        {
            targetNode = nodes[i];
            print(targetNode.pos);
        }

        if(targetNode == null)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSecondsRealtime(.25f);
            OverworldManager.instance.selected_unit = this;
            OverworldManager.instance.MoveSelectedUnit(targetNode.pos);
            while (!Moved)
            {
                yield return null;
            }
            print("Moved");
            yield return null;
        }
    }

    IEnumerator AttemptAttack()
    {
        bool canAttack = false;
        Unit attackTarget = null;

        List<TileSystem.Node> nodes = TileSystem.instance.CalculateAttack(transform.position, range, type);
        foreach(TileSystem.Node node in nodes)
        {
            if (node.hasUnit)
            {
                Unit unit = TileSystem.instance.GetUnit(node.pos);
                if(unit.type == OverworldManager.UnitType.ally)
                {
                    canAttack = true;
                    attackTarget = unit;
                }
            }
        }


        if (canAttack)
        {
            TileSystem.instance.GenerateAttackHighlights(nodes);
            yield return new WaitForSecondsRealtime(.25f);
            OverworldManager.instance.selected_unit = this;
            OverworldManager.instance.Attack(attackTarget);
        }
        else
        {
            yield return null;
        }
    }




    public void Pass()
    {
        OverworldManager.instance.selected_unit = this;
        OverworldManager.instance.Pass();
    }

    public IEnumerator TakeTurn()
    {
        yield return StartCoroutine(GetTarget());

        yield return StartCoroutine(AttemptMove());
        yield return new WaitForSecondsRealtime(.25f);

        yield return StartCoroutine(AttemptAttack());

        Pass();

        yield return null;
    }
}
