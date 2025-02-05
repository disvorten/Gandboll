using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PicoInputs : MonoBehaviour
{
    [SerializeField] private GameObject right_arm;
    [SerializeField] private GameObject left_arm;
    private bool is_line_shown_right = false;
    private bool is_line_shown_left = false;
    [SerializeField] private InputActionReference Show_Hide_Line_Button_Right;
    [SerializeField] private InputActionReference Show_Hide_Line_Button_Left;
    void Start()
    {
        Show_Hide_Line_Button_Right.action.started += ChangeLineRight;
        Show_Hide_Line_Button_Left.action.started += ChangeLineLeft;
    }

    private void ChangeLineRight(InputAction.CallbackContext obj)
    {
        if (is_line_shown_right)
        {
            right_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 0f;
            is_line_shown_right = false;
        }
        else
        {
            right_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 50f;
            is_line_shown_right = true;
        }
    }
    private void ChangeLineLeft(InputAction.CallbackContext obj)
    {
        if (is_line_shown_left)
        {
            left_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 0f;
            is_line_shown_left = false;
        }
        else
        {
            left_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 50f;
            is_line_shown_left = true;
        }
    }


    private void OnDestroy()
    {
        Show_Hide_Line_Button_Right.action.started -= ChangeLineRight;
        Show_Hide_Line_Button_Left.action.started -= ChangeLineLeft;
    }
}
