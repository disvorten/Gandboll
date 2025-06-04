using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRKB_AutofillButton : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI label;

    private VRKB.AutofillUserData fillData;
    private VRKB keyboard;

    public void Init(VRKB.AutofillUserData fillData, string visibleString, VRKB keyboard)
    {
        this.fillData = fillData;
        this.keyboard = keyboard;
        label.text = visibleString;
    }

    public void ButtonPressed()
    {
        keyboard.AutofillButtonPressed(fillData);
    }
}
