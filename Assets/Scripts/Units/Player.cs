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
        if (GameManager.instance.canInterrupt)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (OverworldManager.instance.selected_unit == null)
                {
                    OverworldManager.instance.selected_unit = this;
                    OverworldUI.instance.OpenUnitMenu();
                }
                else
                {
                    OverworldUI.instance.OpenUnitMenu();
                }
            }
        }
    }
}
