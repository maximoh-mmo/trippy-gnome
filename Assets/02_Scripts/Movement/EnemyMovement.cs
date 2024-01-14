using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public int movementPattern;
    public float moveToPlayerSpeed = 2;
    public float movementPatternSpeed = 1;
    private float startTime, circleSize = 0.03f;
    private float circleGrowSpeed = 0.002f;
    private void Awake()
    {
        if (Random.Range(0, 2) > 0)
        {
            movementPatternSpeed *= -1;
        }
        startTime = Time.time;
    }

    public void ProcessMove(Transform player)
    {
        if (movementPattern == 1) Pattern1(player);
        if (movementPattern == 2) Pattern2(player);
        startTime += 0.01f;
    }

    //Pattern1 side to side movement based on sinecurve moving ever toward player
    private void Pattern1(Transform player)
    {
        var pPos = player.position;
        var tPos = transform.position;
        var directionTowardPlayer = pPos - tPos;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(startTime) * Time.deltaTime * movementPatternSpeed);
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        tPos += leftMultipliedBySineCurve + movementInPlayerDirection;
        transform.position = tPos;
    }
    //pattern2 spiral
    private void Pattern2(Transform player)
    {
        var pPos = player.position;
        var tPos = transform.position;
        var directionTowardPlayer = pPos - tPos;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(startTime) * circleSize);
        var verticalMultipliedByCosineCurve = Vector3.up.normalized * (Mathf.Cos(startTime) * circleSize );
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        circleSize -= circleGrowSpeed * Time.deltaTime;
        tPos += (movementInPlayerDirection + leftMultipliedBySineCurve + verticalMultipliedByCosineCurve)* movementPatternSpeed;
        transform.position = tPos;
    }
}
