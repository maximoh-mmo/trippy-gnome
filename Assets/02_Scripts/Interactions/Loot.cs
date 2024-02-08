using UnityEngine;

public class Loot:MonoBehaviour
{
    public LootItem[] loots;
    public float scale = 1f;
    public void GetLoot(Vector3 position)
    {
        foreach (LootItem loot in loots)
        {
            var temp = loot;
            if (Random.Range(0f, 100f) <= temp.chance)
            {
                GameObject powerup = Instantiate(temp.powerUp);
                powerup.GetComponent<LootBehaviour>().Type = temp.type;
                powerup.transform.position = position;
                powerup.transform.localScale = Vector3.one * scale;
                return;
            }
        }
    }
}
[System.Serializable]
public class LootItem
{
    public GameObject powerUp;
    [Range(0,2)]public int type; 
    [Range(0,100)]public float chance;
}