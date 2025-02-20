using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuperBallSpawnAnimator : MonoBehaviour
{
    [SerializeField] private GameObject[] parts;
    [SerializeField] float alphaMultiplier = 1f, partDistanceMultiplier = 1.1f;
    [SerializeField] Color color;
    private List<Vector3> positions;
    private List<float> times;
    private List<Material> materials;
    private bool initialized = false;
    private float alpha = 0;
    public float animTime = 4;

    public void Init()
    {
        positions = new List<Vector3>();
        times = new List<float>();
        materials = new List<Material>();
        alpha = 0;
        color.a = alpha;
        foreach (GameObject part in parts)
        {
            //Debug.Log($"iterating: {part.name}");
            Vector3 pivot = part.GetComponent<MeshRenderer>().bounds.center;
            pivot = pivot - gameObject.transform.position;
            //Debug.Log($"pivot pre-multiplied: {pivot}");
            pivot = new Vector3(pivot.x * partDistanceMultiplier, pivot.y * partDistanceMultiplier, pivot.z * partDistanceMultiplier);
            //Debug.Log($"pivot multiplied: {pivot}");
            pivot = pivot - gameObject.transform.position;
            //Debug.Log($"pivot pre-multiplied: {pivot}");
            pivot = new Vector3(pivot.x * partDistanceMultiplier, pivot.y * partDistanceMultiplier, pivot.z * partDistanceMultiplier);
            //Debug.Log($"pivot multiplied: {pivot}");
            part.transform.localPosition = pivot;
            positions.Add(pivot);
            times.Add(Random.Range(animTime / 10, animTime));
            materials.Add(part.GetComponent<MeshRenderer>().material);
            materials.Last().color = color;
        }
        initialized = true;

        //Debug.Log($"parts = {parts.ToString()}, count = {parts.Length}");
        //Debug.Log($"positions = {positions.ToString()}, count = {positions.Count}");
        //Debug.Log($"times = {times.ToString()}, count = {times.Count}");
    }


    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        alpha = alpha + (Time.deltaTime / animTime);

        if (alpha >= 1)
        {
            gameObject.SetActive(false);
            return;
        }

        alpha = Mathf.Clamp01(alpha);
        color.a = alpha * alphaMultiplier;

        //Debug.Log($"color.a = {color.a}, alpha = {alpha}");

        foreach (GameObject part in parts)
        {
            int index = Array.IndexOf(parts, part);
            if (times[index] > 0)
            {
                times[index] = times[index] - Time.deltaTime;
                part.transform.localPosition = Vector3.Lerp(Vector3.zero, positions[index],
                    times[index] / animTime);
            }
            materials[index].color = color;
        }
    }
}
