using System.Linq;
using Unity.VisualScripting;
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
         .Select(u => u.GetComponent<Transform>())
         .Distinct()
         .OrderBy(t => Mathf.Abs(Vector3.SignedAngle(aimDirection.forward, t.transform.position,Vector3.up)))
         .Take(numberToGet)
         .ToArray();
      return targets.Length > 0 ? targets : null;
   }

   private void OnDrawGizmos()
   {
      if (aimDirection != null)
      {
         Gizmos.color = Color.red;
         Gizmos.DrawRay(aimDirection.position, aimDirection.forward * 1000);
      }

      var targets = GetTargets(2);
      Gizmos.color = Color.green;
      if (targets != null)
      {
         foreach (var t in targets)
         {
            Gizmos.DrawWireSphere(t.position, 2f);
         }
      }
   }
}