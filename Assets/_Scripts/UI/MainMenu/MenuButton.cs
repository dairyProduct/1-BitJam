using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;  
using UnityEngine.UI;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TMP_Text theText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData) {
        theText.color = Color.white;
    }
    public void OnPointerUp(PointerEventData eventData) {
        theText.color = Color.black;
    }
}
