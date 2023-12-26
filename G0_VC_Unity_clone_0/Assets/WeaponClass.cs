using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController Player_animatorOverrideController;
    [SerializeField] AnimatorOverrideController Gun_animatorOverrideController;
    private Vector3 position;
    private int array_spot;







    public int ArraySpot()
    {
        return array_spot;
    }





    public WeaponClass clone(int arraySpot)
    {
        WeaponClass clone = new WeaponClass();
        clone.setAttributes(Player_animatorOverrideController, Gun_animatorOverrideController, position, arraySpot);
        return clone;
    }    
    
    
    void setAttributes(AnimatorOverrideController Player_animatorOverrideController_input, AnimatorOverrideController Gun_animatorOverrideController_input, Vector3 position_input, int array_spot_input)
    {
        Player_animatorOverrideController = Player_animatorOverrideController_input;
        Gun_animatorOverrideController = Gun_animatorOverrideController_input;
        position = position_input;
        array_spot = array_spot_input;
    }
}
