using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour, IAbility {

    public float meleeCoolDown = .2f;
    public float meleeDamage = 1.0f;
    private float startTimeAbility = float.MinValue;

    public bool CanStartAbility()
    {
        return Time.time - startTimeAbility > meleeCoolDown;
    }

    public void StartAbility()
    {
        startTimeAbility = Time.time;
    }

    public void StopAbility() {
        return;
    }

}
