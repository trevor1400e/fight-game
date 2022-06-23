using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public bool attacking;
    public bool justAttacked;

    [SerializeField] private GameObject _blood;
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Invulnerable = Animator.StringToHash("Invulnerable");
    
    // [SerializeField]
    // private float _swordForce = 100f;
    void Start()
    {
        attacking = false;
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyAiTutorial enemyAI = collision.GetComponent<EnemyAiTutorial>();
            //maybe an arraylist of enemies we've hit per swing? or make enemies invulnerable after hit animation triggered?
            //or set animation event at end and detect that
            
            if (attacking && enemyAI.invulnerable == false)
            {
                //hit animation
                collision.GetComponent<Animator>().SetTrigger("Hit");

                // collision.GetComponent<Rigidbody>().AddForce(-transform.forward * _swordForce, ForceMode.Impulse);
                
                //maybe set timeout here too per different weapon stun time
                enemyAI.invulnerable = true;

                GameObject go = Instantiate(_blood, collision.gameObject.GetComponent<Transform>().position,
                    Quaternion.identity);
                Destroy(go, 1);
                enemyAI.TakeDamage(25);
            }
        }
    }
}