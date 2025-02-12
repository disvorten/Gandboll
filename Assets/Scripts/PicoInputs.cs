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
    //[SerializeField] private InputActionReference Show_Hide_Line_Button_Right;
    //[SerializeField] private InputActionReference Show_Hide_Line_Button_Left;
    SuperInputActions superInputActions;

    private void Awake()
    {
        superInputActions = new SuperInputActions();
    }

    private void OnEnable()
    {
        //Show_Hide_Line_Button_Right.action.started += ChangeLineRight;
        //Show_Hide_Line_Button_Left.action.started += ChangeLineLeft;
        //Show_Hide_Line_Button_Right.action.Enable();
        //Show_Hide_Line_Button_Left.action.Enable();
        superInputActions.Newactionmap.LeftTrigger.performed += TurnOnLineLeft;
        superInputActions.Newactionmap.RightTrigger.performed += TurnOnLineRight;
        superInputActions.Newactionmap.LeftTrigger.Enable();
        superInputActions.Newactionmap.RightTrigger.Enable();
        superInputActions.Newactionmap.LeftGrip.performed += TurnOffLineLeft;
        superInputActions.Newactionmap.RightGrip.performed += TurnOffLineRight;
        superInputActions.Newactionmap.LeftGrip.Enable();
        superInputActions.Newactionmap.RightGrip.Enable();
    }

    private void TurnOnLineRight(InputAction.CallbackContext obj)
    {
        if (is_line_shown_right)
        {
            return;
        }
        else
        {
            right_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 50f;
            is_line_shown_right = true;
        }
    }
    private void TurnOnLineLeft(InputAction.CallbackContext obj)
    {
        if (is_line_shown_left)
        {
            return;
        }
        else
        {
            left_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 50f;
            is_line_shown_left = true;
        }
    }
    private void TurnOffLineRight(InputAction.CallbackContext obj)
    {
        if (is_line_shown_right)
        {
            right_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 0f;
            is_line_shown_right = false;
        }
        else
        {
            return;
        }
    }
    private void TurnOffLineLeft(InputAction.CallbackContext obj)
    {
        if (is_line_shown_left)
        {
            left_arm.GetComponent<XRRayInteractor>().maxRaycastDistance = 0f;
            is_line_shown_left = false;
        }
        else
        {
            return;
        }
    }


    private void OnDestroy()
    {
        //Show_Hide_Line_Button_Right.action.started -= ChangeLineRight;
        //Show_Hide_Line_Button_Left.action.started -= ChangeLineLeft;
        //Show_Hide_Line_Button_Right.action.Disable();
        //Show_Hide_Line_Button_Left.action.Disable();
        superInputActions.Newactionmap.LeftTrigger.performed -= TurnOnLineLeft;
        superInputActions.Newactionmap.RightTrigger.performed -= TurnOnLineRight;
        superInputActions.Newactionmap.LeftTrigger.Disable();
        superInputActions.Newactionmap.RightTrigger.Disable();
        superInputActions.Newactionmap.LeftGrip.performed -= TurnOffLineLeft;
        superInputActions.Newactionmap.RightGrip.performed -= TurnOffLineRight;
        superInputActions.Newactionmap.LeftGrip.Disable();
        superInputActions.Newactionmap.RightGrip.Disable();
    }
}
