using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperVillain : MonoBehaviour
{
    [SerializeField] private Animator superVillainAnimator;

    public void angerState()
    {
        superVillainAnimator.SetTrigger("trAnger");
    }

    public void cheerState()
    {
        superVillainAnimator.SetTrigger("trCheer");
    }
}
