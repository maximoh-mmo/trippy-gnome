using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public int movementPattern;
    public float moveToPlayerSpeed = 2;
    public float movementPatternSpeed = 1;

    private void Awake()
    {
        if (Random.Range(0, 2) > 0)
        {
            movementPatternSpeed *= -1;
        }
    }

    public Vector3 ProcessMove(Vector3 startPosition, Transform player)
    {
        if (movementPattern == 1) return Pattern1(startPosition, player);
        return startPosition;
    }

    private Vector3 Pattern1(Vector3 startPosition, Transform player)
    {
        var directionTowardPlayer = player.position - startPosition;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(Time.time) * Time.deltaTime * movementPatternSpeed);
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        return leftMultipliedBySineCurve + movementInPlayerDirection;
    }
}
