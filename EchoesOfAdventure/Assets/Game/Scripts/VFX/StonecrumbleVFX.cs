using System;
using UnityEngine;

public class StonecrumbleVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float _loopInterval = 2f;
    
    private float time = 0;


    private void Update()
    {
        time += Time.deltaTime;
        if (time < _loopInterval)
            return;

        time = 0;
        _particleSystem.Play();
        
    }
}
