using System.Collections;
using Unity.XR.PXR;
using UnityEngine;

public class ArmTrigger : MonoBehaviour
{
    [SerializeField] private Shooter_generator generator;
    [SerializeField] private AudioSource success;
    [SerializeField] private AudioSource wrong;
    [SerializeField] private bool right;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ball"))
        {
            if (other.transform.parent.GetComponent<Shooter_controller>().is_catched)
            {
                return;
            }
            other.transform.parent.GetComponent<Shooter_controller>().is_catched = true;
            if(!right)
                PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.LeftController, 0.3f, 400, 80);
            else
                PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.RightController, 0.3f, 400, 80);
            if (other.transform.parent.GetComponent<Shooter_controller>().is_false_stimul)
            {
                wrong.Play();
                generator.points_counter.Invoke(1, 0);
            }
            else
            {
                success.Play();
                generator.points_counter.Invoke(0, 1);
            }
            other.GetComponent<Rigidbody>().useGravity = true;
            StartCoroutine(DelayedDestroy(other.transform.parent.gameObject));
        }
    }

    private IEnumerator DelayedDestroy(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
