using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFunctions : MonoBehaviour
{
    //Returns true if both values are positive or both are negative
        //If xor is zero, returns false
    public static bool SameSign(float a, float b) {
        if(a > 0 && b > 0) {
            return true;
        }
        if(a < 0 && b < 0) {
            return true;
        }
        if(a == b) {
            return true;
        }
        return false;
    }
}
