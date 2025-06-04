using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> list = new List<GameObject>();
    [SerializeField] private GameObject Validation;
    void Start()
    {
        foreach(var obj in GetComponentsInChildren<CapsuleCollider>())
        {
            list.Add(obj.gameObject);
            obj.GetComponent<MeshRenderer>().enabled = false;
        }
        StartCoroutine(PlayWithDelay());
    }
    IEnumerator PlayWithDelay()
    {
        yield return new WaitForSeconds(3.5f);
        foreach (GameObject obj in list)
        {
            Debug.Log(obj.name);
            obj.GetComponent<MeshRenderer>().enabled = true;
            yield return new WaitForSeconds(3.5f);
            obj.GetComponent<MeshRenderer>().enabled = false;
        }

        StartCoroutine(StartValidation());
    }

    IEnumerator StartValidation()
    {
        yield return new WaitForSeconds(3.5f);
        Validation.SetActive(true);
        Validation.GetComponentInChildren<RectangleMover>().Init();
        gameObject.SetActive(false);
    }
}
