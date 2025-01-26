using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    Unit target;
    IEnumerator GetTarget()
    {
        float distance = Mathf.Infinity;
        foreach(Unit unit in GameManager.instance.units)
        {
            if(unit.type == OverworldManager.UnitType.ally)
            {
                if(Vector3.Distance(transform.position,unit.gameObject.transform.position) < distance) 
                {
                    target = unit;
                    distance = Vector3.Distance(transform.position, unit.gameObject.transform.position);
                }
            }
        }
        print("Target Selected: " + target.Name);
        yield return null;
    }

    IEnumerator AttemptMove()
    {
        List<TileSystem.Node> nodes = TileSystem.instance.CalculateMovement(transform.position, speed);
        TileSystem.instance.GenerateMovementHighlights(nodes);
        TileSystem.Node targetNode = null;

        float distance = Vector3.Distance(transform.position, target.gameObject.transform.position);
        float targetDistance = distance;
        //Get Node Closest to Target
        if (target != null)
        {
            foreach(TileSystem.Node node in nodes)
            {
                Vector3 pos = node.pos;
                if(Vector3.Distance(target.gameObject.transform.position, pos) < distance)
                {
                    distance = Vector3.Distance(target.gameObject.transform.position, pos);
                    targetNode = node;
                }
            }
        }

        if(targetNode == null || distance == targetDistance)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSecondsRealtime(.75f);
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
            yield return new WaitForSecondsRealtime(.75f);
            OverworldManager.instance.selected_unit = this;
            OverworldManager.instance.Attack(attackTarget);
            yield return new WaitForSecondsRealtime(.75f);
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
        yield return new WaitForSecondsRealtime(.25f);

        yield return StartCoroutine(AttemptMove());
        yield return new WaitForSecondsRealtime(.5f);

        StartCoroutine(AttemptAttack());
        yield return new WaitForSecondsRealtime(.25f);

        Pass();

        yield return null;
    }
}
