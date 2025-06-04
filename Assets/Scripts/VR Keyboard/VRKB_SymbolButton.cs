using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRKB_SymbolButton : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI label;
    [SerializeField] private RectTransform rectTransform;

    private VRKB keyboard;
    private string symbol = "";   

    public void Init(string newSymbol, string visualSymbolOverride, float widthOverride, float heightOverride, 
        VRKB newKeyboard)
    {
        symbol = newSymbol;
        keyboard = newKeyboard;

        if (visualSymbolOverride != null && visualSymbolOverride != string.Empty)
        {
            label.text = visualSymbolOverride;
        }
        else
        {
            label.text = symbol;
        }

        if (widthOverride == null || widthOverride <= 0)
        {
            widthOverride = rectTransform.sizeDelta.x;
        }
        if (heightOverride == null || heightOverride <= 0)
        {
            heightOverride = rectTransform.sizeDelta.y;
        }
        rectTransform.sizeDelta = new Vector2(widthOverride, heightOverride);
    }

    public void CapsLockOn()
    {
        symbol = symbol.ToUpper();
        label.text = label.text.ToUpper();
    }

    public void CapsLockOff()
    {
        symbol = symbol.ToLower();
        label.text = label.text.ToLower();
    }

    public void ButtonPressed()
    {
        keyboard.SymbolButtonPressed(symbol);
    }
}
