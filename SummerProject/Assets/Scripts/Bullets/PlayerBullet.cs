using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet {

    public float speed = 1.0f;
    [SerializeField]
    private float energyCost = 30;
    [SerializeField]
    private float damage = 1.0f;
    private Player player;
    private bool canStartAbility = true;

    public void Start()
    {
        player = GetComponent<Player>();
    }

    public override void Shoot()
    {
        //Energy system for player
        player.Energy -= energyCost;
        //bullets spawn and travel
        GameObject obj = ObjectPool.current.GetPooledObject();
        if (obj == null) return;
        obj.transform.position = bulletSpawn.position;
        obj.transform.rotation = bulletSpawn.rotation;
        Rigidbody bulletRB = obj.GetComponent<Rigidbody>();
        bulletRB.velocity = bulletSpawn.forward * speed;
        obj.SetActive(true);
    }

    public bool CanStartAbility()
    {
        float cost = player.Energy - energyCost;

        if (cost < 0)
            canStartAbility = false;
        else
            canStartAbility = true;

        return canStartAbility;
    }
}
