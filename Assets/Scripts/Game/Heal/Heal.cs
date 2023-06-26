using UnityEngine;

namespace Game.Heal
{
    public class Heal : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float pushForce;
        
        public void Spawn(Transform point)
        {
            transform.position = point.position;
            gameObject.SetActive(true);
            
            Vector3 randomDirection = Random.onUnitSphere + Vector3.up;
            rb.AddForce(randomDirection * pushForce, ForceMode.Impulse);
        }
    }
}