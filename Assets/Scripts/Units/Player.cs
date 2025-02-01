using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Unit
{
    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.canInterrupt && OverworldManager.instance.turn == OverworldManager.UnitType.ally)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                TileSystem.instance.ClearHighlights();
                OverworldManager.instance.selected_unit = this;
                if (!Acted)
                {
                    OverworldUI.instance.OpenUnitMenu();
                }
                else
                {
                    OverworldUI.instance.UnitMenuStats();
                }
            }
        }
    }
}
