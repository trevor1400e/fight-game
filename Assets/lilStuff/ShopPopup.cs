using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPopup : MonoBehaviour
{
    
    [SerializeField] private GameObject _menu1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider collision) {
        
        if(collision.gameObject.tag == "Player")
        {
                Debug.Log("popup shoppp");
                if (_menu1 != null)
                {
                    _menu1.SetActive(true);
                }
                
        }
        
    }  
}
