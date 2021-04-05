using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateHerd : MonoBehaviour
{
    public Transform prefabBuffalo;
    public int herdSize;
    Vector3 randomPos;
    Vector3 randomRot;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < herdSize; i++)
        {
            randomPos = new Vector3(
                Random.Range(-90.0f, 90.0f), 0.0f, Random.Range(-90.0f, 90.0f)
            );
            randomRot = new Vector3(
                0, Random.Range(0f, 360f), 0
            );
            Instantiate(prefabBuffalo, randomPos, Quaternion.Euler(randomRot));
        }
    }
}
