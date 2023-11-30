using System.Collections;
using UnityEngine;

public class Loot:MonoBehaviour
{
    public LootItem[] loots;
    public void GetLoot(Vector3 position)
    {
        float drawn = Random.Range(0f, 100f);
        int i=1;
        foreach (LootItem loot in loots)
        {
            if (drawn <= loot.chance)
            {
                GameObject powerup = Instantiate(loot.PowerUp);
                powerup.transform.position = position;
            }
        }
    }

}
[System.Serializable]
public class LootItem
{
    public GameObject PowerUp;
    [Range(0,100)]public float chance;
}