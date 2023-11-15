using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPathing : MonoBehaviour
{
    // GOAL : create a path that follows the terrain below path "0,0,0" should be above ground enough that starcraft can fly over and around objects depending on the path.

    // Get Spine path as created in unity by designer, determine ship height based on path and then move in the appropriate direction

    // Camera must also not be in the ground....
    [SerializeField] private float minHeight = 1f;
    float currentHeight;
   
    private void Update()
    { 
        currentHeight = GetHeightFromGround(transform.position);
        if (currentHeight < minHeight)
        {
            Debug.Log("X:"+transform.position.x + " Y:" +transform.position.y + " modifier ("+ (minHeight - currentHeight) +" Z:"+ transform.position.y);
            transform.position = new Vector3(transform.position.x,transform.position.y + (minHeight-currentHeight), transform.position.y);
        }
    }

    private float GetHeightFromGround(Vector3 p)
    {
        Physics.Raycast(p, Vector3.down, out RaycastHit hitInfo);
        Debug.Log(hitInfo.collider);
        return hitInfo.distance;
    }
}
