using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public int movementPattern;
    public float moveToPlayerSpeed = 2;
    public float movementPatternSpeed = 1;
    private float startTime, circleSize = 0.03f;
    private float circleGrowSpeed = 0.005f;
    private void Awake()
    {
        if (Random.Range(0, 2) > 0)
        {
            movementPatternSpeed *= -1;
        }
        startTime = Time.time;
    }

    public Vector3 ProcessMove(Transform player)
    {
        var output = Vector3.zero;
        if (movementPattern == 1) output = Pattern1(player);
        if (movementPattern == 2) output = Pattern2(player);
        startTime += 0.01f;
        return output;
    }

    //Pattern1 side to side movement based on sinecurve moving ever toward player
    private Vector3 Pattern1(Transform player)
    {
        var pPos = player.position;
        var tPos = transform.position;
        var directionTowardPlayer = pPos - tPos;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(startTime) * Time.deltaTime * movementPatternSpeed);
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        return leftMultipliedBySineCurve + movementInPlayerDirection;
    }
    //pattern2 spiral
    private Vector3 Pattern2(Transform player)
    {
        var pPos = player.position;
        var tPos = transform.position;
        var directionTowardPlayer = pPos - tPos;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(startTime) * circleSize);
        var verticalMultipliedByCosineCurve = Vector3.up.normalized * (Mathf.Cos(startTime) * circleSize );
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        circleSize -= circleGrowSpeed * Time.deltaTime;
        return (movementInPlayerDirection + leftMultipliedBySineCurve + verticalMultipliedByCosineCurve)* movementPatternSpeed;
    }
}
