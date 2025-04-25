using System;
using UnityEngine;
using UnityEngine.Splines;

public class Wave : MonoBehaviour
{
    private SplineAnimate _splineAnimate;
    
    void Awake()
    {
        _splineAnimate = GetComponent<SplineAnimate>();
        _splineAnimate.Container = GameObject.FindGameObjectWithTag("Spline3").GetComponent<SplineContainer>();
    }

    private void Update()
    {
        if (!_splineAnimate.IsPlaying)
        {
            gameObject.transform.position = new Vector3(-30, 0, 0);
            gameObject.SetActive(false);
        }
    }
}
