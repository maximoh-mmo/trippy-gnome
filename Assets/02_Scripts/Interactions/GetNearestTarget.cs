using System.Linq;
using UnityEngine;
 
public class GetNearestTarget : MonoBehaviour
{
   private Transform aimDirection;
   private void Awake()
   {
      aimDirection = GameObject.Find("spaceship_ufo_clean").transform;
   }

   public Transform[] GetTargets(int numberToGet)
   {
      var potentials = FindObjectsOfType<EnemyBehaviour>();
      if (potentials.Length<1) return null;
      var targets = potentials
         //.Where(t => t.IsTargetted == false)
         .Select(t => t.GetComponent<Transform>())
         .Distinct()
         .Where(t => Vector3.Dot(Vector3.Normalize(aimDirection.TransformDirection(Vector3.forward)),
            Vector3.Normalize(t.transform.position - aimDirection.position)) > 0)
         .OrderBy(t => Vector3.Dot(Vector3.Normalize(aimDirection.TransformDirection(Vector3.forward)),
            Vector3.Normalize(t.transform.position - aimDirection.position)))
         .Reverse()
         .Take(numberToGet)
         .ToArray();
      return targets.Length > 0 ? targets : null;
   }

   public Transform GetTarget()
   {
      var potentials = FindObjectsOfType<EnemyBehaviour>();
      if (potentials.Length<1) return null;
      var target = potentials
         .Where(t => Vector3.Dot(Vector3.Normalize(aimDirection.TransformDirection(Vector3.forward)),
            Vector3.Normalize(t.transform.position - aimDirection.position)) > 0)
         .OrderBy(t => Vector3.Dot(Vector3.Normalize(aimDirection.TransformDirection(Vector3.forward)),
            Vector3.Normalize(t.transform.position - aimDirection.position)))
         //.Where(t => t.IsTargetted == false)
         .Distinct();
      return target.Any() ? target.Last().transform : null;
   }
}