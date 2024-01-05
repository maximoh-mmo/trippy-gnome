using System.Linq;
using UnityEngine;

public class GetNearestTarget : MonoBehaviour
{
   private Transform aimDirection;
   private void Awake()
   {
      aimDirection = FindFirstObjectByType<CraftFollowCrosshair>().transform;
   }

   public Transform[] GetTargets(int numberToGet)
   {
      var potentials = FindObjectsOfType<GameObject>();
      var targets = potentials
         .Select(t => t.GetComponent<HealthManager>())
         .Where(t => t != null)
         .Distinct()
         .Select(u => u.GetComponent<Transform>())
         .OrderBy(u => Vector3.Dot(aimDirection.forward, u.transform.position))
         .Take(numberToGet)
         .ToArray();
      return targets.Length > 0 ? targets : null;
   }
}