using UnityEngine;
using UnityEngine.EventSystems;

public class MovementSquare : MonoBehaviour, IPointerClickHandler
{
    //When a movement square is clicked, the selected unit moves there
    public void OnPointerClick(PointerEventData eventData)
    {
        if(OverworldManager.instance.selected_unit != null)
        {
            OverworldManager.instance.MoveSelectedUnit(transform.position);
        }
    }
}
