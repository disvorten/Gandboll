using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRKB_Line : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    private List<VRKB_SymbolButton> symbolButtons = new List<VRKB_SymbolButton>();

    public void Init(string symbols, float lineWidthOverride, float lineHeightOverride,
        float childrenWidthOverride, float childrenHeightOverride, VRKB keyboard,
        GameObject buttonPrefab)
    {
        if (lineWidthOverride == null || lineWidthOverride <= 0)
        {
            lineWidthOverride = rectTransform.sizeDelta.x;
        }
        if (lineHeightOverride == null || lineHeightOverride <= 0)
        {
            lineHeightOverride = rectTransform.sizeDelta.y;
        }
        rectTransform.sizeDelta = new Vector2(lineWidthOverride, lineHeightOverride);

        if (symbols != null && symbols != string.Empty)
        {
            foreach (char s in symbols)
            {
                GameObject go = Instantiate(buttonPrefab, transform);
                VRKB_SymbolButton buttonScript = go.GetComponent<VRKB_SymbolButton>();
                if (buttonScript)
                {
                    symbolButtons.Add(buttonScript);
                    buttonScript.Init(s.ToString(), null, childrenWidthOverride, childrenHeightOverride, keyboard);
                }
            }
        }
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        symbolButtons.Clear();
    }

    public void CapsLockOn()
    {
        if (symbolButtons == null || symbolButtons.Count == 0)
        {
            return;
        }

        foreach (var button in symbolButtons)
        {
            button.CapsLockOn();
        }
    }

    public void CapsLockOff()
    {
        if (symbolButtons == null || symbolButtons.Count == 0)
        {
            return;
        }

        foreach (var button in symbolButtons)
        {
            button.CapsLockOff();
        }
    }
}
