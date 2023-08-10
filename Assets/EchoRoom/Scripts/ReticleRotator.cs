using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleRotator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rotator = gameObject.AddComponent<RotateObject>();
        rotator.SetIsRotating(true); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
