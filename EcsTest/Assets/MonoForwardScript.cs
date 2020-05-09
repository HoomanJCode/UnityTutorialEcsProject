using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoForwardScript : MonoBehaviour
{
    [SerializeField] private float Speed;
    
    private float StartTime;
    // Start is called before the first frame update
    void Start()
    {
        Speed = Random.Range(0, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        var transformPosition = transform.position;
        transformPosition.z = Mathf.PerlinNoise((StartTime+=Time.deltaTime), Speed);
        transform.position = transformPosition;
    }
}
