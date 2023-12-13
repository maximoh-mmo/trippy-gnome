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
                GameObject powerup = Instantiate(loot.PowerUp);
                powerup.GetComponent<LootBehaviour>().Type = loot.Type;
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
    public GameObject PowerUp;
    [Range(0,2)]public int Type; 
    [Range(0,100)]public float chance;
}