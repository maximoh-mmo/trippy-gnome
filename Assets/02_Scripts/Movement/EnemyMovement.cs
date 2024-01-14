using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public int movementPattern;
    public float moveToPlayerSpeed = 2;
    public float movementPatternSpeed = 1;
    private Vector3 startPosition;
    private float startTime;
    private void Awake()
    {
        if (Random.Range(0, 2) > 0)
        {
            movementPatternSpeed *= -1;
        }
        startTime = Time.time;
    }

    public Vector3 ProcessMove(Vector3 currentPosition, Transform player)
    {
        if (movementPattern == 1) return Pattern1(currentPosition, player);
        if (movementPattern == 2) return Pattern2(currentPosition, player);
        return currentPosition;
    }

    //Pattern1 side to side movement based on sinecurve 
    private Vector3 Pattern1(Vector3 currentPosition, Transform player)
    {
        var directionTowardPlayer = player.position - currentPosition;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(startTime) * Time.deltaTime * movementPatternSpeed);
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        startTime += 0.01f;
        return leftMultipliedBySineCurve + movementInPlayerDirection;
    }
    //pattern2 spiral
    private Vector3 Pattern2(Vector3 currentPosition, Transform player)
    {
        var directionTowardPlayer = player.position - currentPosition;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var rotationPoint = directionTowardPlayer;
        // var direction = Quaternion.Euler(0, 0, 60) * direction;
        // transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        // var distanceThisFrame = movementPatternSpeed * Time.deltaTime;
        // var out = direction * distanceThisFrame;
        return currentPosition;
    }
}
