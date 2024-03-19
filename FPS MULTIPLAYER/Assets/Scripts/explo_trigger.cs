using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class explo_trigger : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(timing_to_explode());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            if (other.gameObject.layer == 9)
            {               
                other.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, 1000, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    IEnumerator timing_to_explode()
    {
        yield return new WaitForSeconds(2.5f);
        GetComponent<SphereCollider>().enabled = true;


    }
}
