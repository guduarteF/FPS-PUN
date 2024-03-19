using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponAnim : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = transform.Find("Anchor/Design/Gun/Arms/arms_gun").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void SprintAnim(bool isRunning)
    {
       
            anim.SetBool("sprint", isRunning);

            if (isRunning == true)
                anim.Play("run");

        



    }
}
