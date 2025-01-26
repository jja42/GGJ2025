using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OverworldUI : MonoBehaviour
{
    public static OverworldUI instance;
    public GameObject UnitMenu;
    public GameObject UnitMenuMoveButton;
    public GameObject ItemMenu;
    public List<TextMeshProUGUI> UnitStats;
    public GameObject UnitStatsUI;

    private void Awake()
    {
        instance = this;
    }

    //set unit menu active
    //if we've moved, remove the option to move
    public void OpenUnitMenu()
    {
        UnitMenu.SetActive(true);
        if (!OverworldManager.instance.selected_unit.Moved)
        {
            UnitMenuMoveButton.SetActive(true);
        }
        else
        {
            UnitMenuMoveButton.SetActive(false);
        }
    }

    public void CloseUnitMenu()
    {
        UnitMenu.SetActive(false);
    }

    public void CloseItemMenu()
    {
        ItemMenu.SetActive(false);
    }

    public void CloseUnitStats()
    {
        UnitStatsUI.SetActive(false);
    }

    //generate our available movement squares
    //generate ui elements for movement squares
    public void UnitMenuMove()
    {
        Unit unit = OverworldManager.instance.selected_unit;
        List<TileSystem.Node> nodes = TileSystem.instance.CalculateMovement(unit.transform.position, unit.speed);
        TileSystem.instance.GenerateMovementHighlights(nodes);
        CloseUnitMenu();
    }

    public void UnitMenuAttack()
    {
        Unit unit = OverworldManager.instance.selected_unit;
        List<TileSystem.Node> nodes = TileSystem.instance.CalculateAttack(unit.transform.position, unit.range);
        TileSystem.instance.GenerateAttackHighlights(nodes);
        CloseUnitMenu();
    }

    public void UnitMenuItems()
    {

    }

    public void UnitMenuStats()
    {
        UnitStatsUI.SetActive(true);
        Unit unit = OverworldManager.instance.selected_unit;
        UnitStats[0].text = unit.Name;
        UnitStats[1].text = unit.health.ToString() + " / " + unit.maxHealth.ToString();
        UnitStats[2].text = unit.damage.ToString();
        UnitStats[3].text = unit.defense.ToString();
        UnitStats[4].text = unit.range.ToString();
        UnitStats[5].text = unit.speed.ToString();
    }
}
