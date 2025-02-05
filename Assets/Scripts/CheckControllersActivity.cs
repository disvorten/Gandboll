using Unity.XR.PXR;
using UnityEngine;

public class CheckControllersActivity : MonoBehaviour
{
    [SerializeField] private GameObject right_arm;
    [SerializeField] private GameObject left_arm;
    void FixedUpdate()
    {
        var right_status = PXR_Input.GetControllerStatus(PXR_Input.Controller.RightController);
        if(right_status == PXR_Input.ControllerStatus.Static || right_status == PXR_Input.ControllerStatus.Sleep)
            right_arm.SetActive(false);
        else
            right_arm.SetActive(true);
        var left_status = PXR_Input.GetControllerStatus(PXR_Input.Controller.LeftController);
        if (left_status == PXR_Input.ControllerStatus.Static || left_status == PXR_Input.ControllerStatus.Sleep)
            left_arm.SetActive(false);
        else
            left_arm.SetActive(true);
    }
}
