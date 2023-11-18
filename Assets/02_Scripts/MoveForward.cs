using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] float speed=10f;
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime *speed;
    }
}
