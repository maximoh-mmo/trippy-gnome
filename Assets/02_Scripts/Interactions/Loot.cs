using UnityEngine;

public class Loot:MonoBehaviour
{
    public LootItem[] loots;
    public float scale = 1f;
    public void GetLoot(Vector3 position)
    {
        float drawn = Random.Range(0f, 100f);
        foreach (LootItem loot in loots)
        {
            if (drawn <= loot.chance)
            {
                GameObject powerup = Instantiate(loot.powerUp);
                powerup.GetComponent<LootBehaviour>().Type = loot.type;
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