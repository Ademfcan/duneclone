using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class GravityManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float GravityConstant = 0.005f;
    private float GravitySpecial = 0.005f;
    private float Acceleration = 0.01f;
    private Rigidbody rigidbody;

    private bool ishit;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
