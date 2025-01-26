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

    public GameObject playerBanner;
    public GameObject enemyBanner;
    public GameObject combatText;
    public List<TextMeshProUGUI> attackText;

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
        List<TileSystem.Node> nodes = TileSystem.instance.CalculateAttack(unit.transform.position, unit.range, unit.type);
        TileSystem.instance.GenerateAttackHighlights(nodes);
        CloseUnitMenu();
    }

    public void UnitMenuItems()
    {

    }

    public void UnitMenuPass()
    {
        OverworldManager.instance.Pass();
        CloseUnitMenu();
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


    public IEnumerator PlayerBanner()
    {
        playerBanner.SetActive(true);
        yield return new WaitForSecondsRealtime(.75f);
        playerBanner.SetActive(false);
        yield return null;
    }

    public IEnumerator EnemyBanner()
    {
        enemyBanner.SetActive(true);
        yield return new WaitForSecondsRealtime(.75f);
        enemyBanner.SetActive(false);

        yield return new WaitForSecondsRealtime(.5f);
        StartCoroutine(OverworldManager.instance.ProcessEnemyTurn());
        yield return null;
    }

    public void Victory()
    {
        GameManager.instance.loadedStory = "Victory";
        GameManager.instance.loadedMusic = GameManager.instance.MusicFiles[0];
        GameManager.instance.LoadScene("StoryScene");
    }

    public void Loss()
    {
        GameManager.instance.loadedStory = "Loss";
        GameManager.instance.loadedMusic = GameManager.instance.MusicFiles[2];
        GameManager.instance.LoadScene("StoryScene");
    }

    public IEnumerator CombatText(Unit attacker, Unit target, int damage)
    {
        combatText.SetActive(true);
        attackText[0].text = attacker.Name + " Dealt " + damage + " Damage to " + target.Name + " !";
        attackText[1].text = "(" + attacker.damage + " ATK - " + target.defense + " DEF)";
        attackText[2].text = target.Name + " HP: " + target.health + " / " + target.maxHealth;
        yield return new WaitForSecondsRealtime(2.5f);
        combatText.SetActive(false);
    }
}
