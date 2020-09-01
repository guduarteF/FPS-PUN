using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun", menuName ="Gun")]
public class Gun : ScriptableObject
{
    public string g_name;
    public int damage;
    public float firerate;
    public float bloom;
    public float recoil;
    public float kickback;
    public float aimSpeed;
    public GameObject prefab;
    public int ammo;
    public int clipsize;
    private int clip; //current clip
    private int stash; //current ammo
    public float reload;
    public int burst; //0 semi / 1 auto / 2+ burst

    public void Initialize()
    {
        stash = ammo;
        clip = clipsize;
    }
    public bool FireBullet()
    {
        if (clip > 0)
        {
            clip -= 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reload()
    {
        stash += clip;
        clip = Mathf.Min(clipsize, stash);
        stash -= clip; 
        
    }

    public int GetStash()
    {
        return stash;
    }

    public int GetClip()
    {
        return clip;
    }
}
