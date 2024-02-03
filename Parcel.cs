using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parcel : MonoBehaviour
{
    [SerializeField]
    private int boxID; 

    public int GetId()
    {
        return boxID; 
    }
}
