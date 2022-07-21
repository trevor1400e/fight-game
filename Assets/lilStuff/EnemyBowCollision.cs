using StarterAssets;
using UnityEngine;

namespace lilStuff
{
    public class EnemyBowCollision : MonoBehaviour
    {
        public bool attacking;
        public bool justAttacked;
        private Collider m_Collider;

        [SerializeField] private GameObject _blood;
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Invulnerable = Animator.StringToHash("Invulnerable");
    
        // [SerializeField]
        // private float _swordForce = 100f;
        void Start()
        {
            attacking = false;
            _blood = GameObject.Find("FX_BloodSplat_01");
            m_Collider = GetComponent<Collider>();
            Destroy(this.gameObject, 2);
        }

        void Update()
        {
        }

        void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                ThirdPersonController playerScript = collision.GetComponent<ThirdPersonController>();
                //maybe an arraylist of enemies we've hit per swing? or make enemies invulnerable after hit animation triggered?
                //or set animation event at end and detect that

                //hit animation
                // collision.GetComponent<Animator>().SetTrigger("Hit");

                // collision.GetComponent<Rigidbody>().AddForce(-transform.forward * _swordForce, ForceMode.Impulse);
                Set_Collider_Inactive();
                //maybe set timeout here too per different weapon stun time
                //playerScript.invulnerable = true;
                Debug.Log("BOW PLAYER while ATTACKING");
                
                GameObject go = Instantiate(_blood, collision.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                Destroy(go, 1);
                playerScript.TakeDamage(25);
                justAttacked = true;
                Destroy(this.gameObject);
            }
        }

        public void Set_Collider_Inactive()
        {
            m_Collider.enabled = false;
        }
    
    }
}
