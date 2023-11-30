using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    //[SerializeField]Dictionary<GameObject,float> Drops;

    Transform player = null;
    void Start()
    {
        if (GameObject.Find("spaceship_ufo_2").transform != null)
        {
            player = GameObject.Find("spaceship_ufo_2").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(this.gameObject); }            
    }
    private void OnDestroy()
    {
        
    }
}
