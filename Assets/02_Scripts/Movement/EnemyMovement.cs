using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public int movementPattern;
    public float moveToPlayerSpeed = 2;
    public float movementPatternSpeed = 1;
    public float direction = 1f;
    private float startTime, circleSize = 0.05f;
    private float circleGrowSpeed = 0.005f;
    
    private void Awake()
    {
        if (movementPattern == 1) if (Random.Range(0,2)>0) movementPatternSpeed *= -1;
        if (movementPattern == 2) if (Random.Range(0,2)>0) movementPatternSpeed *= -1;
        if (movementPattern == 3) if (gameObject.name.Contains("2")) direction *= -1;
    }
    public Vector3 ProcessMove(Transform player)
    {
        var output = Vector3.zero;
        if (movementPattern == 1) output = Sway(player);
        if (movementPattern == 2) output = Spiral(player);
        if (movementPattern == 3) output = Circle(player);
        startTime += 0.01f;
        return output;
    }
    private Vector3 Sway(Transform player)
    {
        var pPos = player.position;
        var tPos = transform.position;
        var directionTowardPlayer = pPos - tPos;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(startTime) * Time.deltaTime * movementPatternSpeed);
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        return leftMultipliedBySineCurve + movementInPlayerDirection;
    }
    private Vector3 Spiral(Transform player)
    {
        var pPos = player.position;
        var tPos = transform.position;
        var directionTowardPlayer = pPos - tPos;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (Mathf.Sin(startTime) * circleSize);
        var verticalMultipliedByCosineCurve = Vector3.up.normalized * (Mathf.Cos(startTime) * circleSize );
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        circleSize -= circleGrowSpeed * Time.deltaTime;
        return movementInPlayerDirection + ((leftMultipliedBySineCurve + verticalMultipliedByCosineCurve) * movementPatternSpeed);
    }
    
    private Vector3 Circle(Transform player)
    {
        var pPos = player.position;
        var tPos = transform.position;
        var directionTowardPlayer = pPos - tPos;
        var directionLeft = Vector3.Cross(directionTowardPlayer.normalized, Vector3.up).normalized;
        var leftMultipliedBySineCurve = directionLeft.normalized * (direction * (Mathf.Sin(startTime) * circleSize));
        var verticalMultipliedByCosineCurve = Vector3.up.normalized * (Mathf.Cos(startTime) * circleSize );
        var movementInPlayerDirection = directionTowardPlayer.normalized * (Time.deltaTime * moveToPlayerSpeed);
        return movementInPlayerDirection + (leftMultipliedBySineCurve + verticalMultipliedByCosineCurve) * movementPatternSpeed;
    }
}
