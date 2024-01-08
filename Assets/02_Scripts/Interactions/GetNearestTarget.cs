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
      var potentials = FindObjectsOfType<EnemyBehaviour>();
      var targets = potentials
         .Where(t => t.IsTargetted == false)
         .Select(t => t.GetComponent<Transform>())
         .Distinct()
         .OrderBy(t => Vector3.Dot(aimDirection.forward, t.transform.position))
         .Take(numberToGet)
         .ToArray();
      return targets.Length > 0 ? targets : null;
   }

   public Transform GetTarget()
   {
      var potentials = FindObjectsOfType<EnemyBehaviour>();
      var targets = potentials
         .Where(t => t.IsTargetted == false)
         .Select(t => t.GetComponent<Transform>())
         .Distinct()
         .OrderBy(t => Vector3.Dot(aimDirection.forward, t.transform.position))
         .Take(1)
         .ToArray();
      if (targets.Length == 0) return null;
      return targets[0];
   }
}