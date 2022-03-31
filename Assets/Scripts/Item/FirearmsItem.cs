using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmsItem : BaseItem
{
    public enum FirearmsType 
    { 
        AssaultRifle,
        HandGun,
    }

    public FirearmsType CurrentFirearmsType;
    public string WeaponName;
}
