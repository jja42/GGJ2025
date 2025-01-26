using UnityEngine;
using UnityEngine.EventSystems;

public class AttackSquare : MonoBehaviour, IPointerClickHandler
{
    //When a movement square is clicked, the selected unit moves there
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OverworldManager.instance.selected_unit != null)
        {
            if(TileSystem.instance.GetUnit(transform.position) != null)
            {
                OverworldManager.instance.Attack(TileSystem.instance.GetUnit(transform.position));
            }
        }
    }
}
