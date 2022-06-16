using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public bool attacking;
    public bool justAttacked;

    [SerializeField] private GameObject _blood;
    private static readonly int Attack = Animator.StringToHash("Attack");

    void Start()
    {
        attacking = false;
        justAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //maybe an arraylist of enemies we've hit per swing? or make enemies invulnerable after hit animation triggered?
            //or set animation event at end and detect that
            if (attacking && justAttacked == false)
            {
                //hit animation
                collision.GetComponent<Animator>().SetTrigger("Hit");
                collision.GetComponent<Animator>().SetBool(Attack, false);

                Debug.Log("hit enemy wHILE ATTACKING");
                GameObject go = Instantiate(_blood, collision.gameObject.GetComponent<Transform>().position,
                    Quaternion.identity);
                Destroy(go, 1);
                collision.gameObject.GetComponent<EnemyAiTutorial>().TakeDamage(25);
                justAttacked = true;
            }
        }
    }
}