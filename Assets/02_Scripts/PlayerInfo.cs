using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public Transform Transform { get { return transform; } }
    public float CurrentHealth {  get { return GetComponent<HealthManager>().CurrentHealth; } } 
    public float CurrentSpeed { get { return GetComponent<MoveWithPath>().Speed; } set { GetComponent<MoveWithPath>().Speed = value; } }

}
