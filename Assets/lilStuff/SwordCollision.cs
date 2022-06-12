using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public bool attacking;
    public bool justAttacked;

    [SerializeField] private GameObject _blood;

    void Start()
    {
        attacking = false;
        justAttacked = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter(Collider collision) {
        
        if(collision.gameObject.tag == "Enemy")
        {
            if (attacking && justAttacked == false)
                {
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
