using UnityEngine;

public class Trigger_Relation : MonoBehaviour
{
    [SerializeField] private Shooter_generator generator;
    [SerializeField] private AudioSource success;
    [SerializeField] private AudioSource wrong;
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Ground"))
        {
            Destroy(other.transform.parent.gameObject);
            if(!other.transform.parent.GetComponent<Shooter_controller>().is_catched)
            {
                if (other.transform.parent.GetComponent<Shooter_controller>().is_false_stimul)
                {
                    success.Play();
                    Change(0, 1);
                }
                else
                {
                    wrong.Play();
                    Change(1, 0);
                }
            }
        }
    }

    private void Change(int miss, int success)
    {
        generator.points_counter.Invoke(miss, success);
    }
}
