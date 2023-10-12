using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect _footStep;
    public ParticleSystem Blade_01;
    public ParticleSystem Blade_02;
    public ParticleSystem Blade_03;
    
    public VisualEffect Slash;
    public VisualEffect Heal;

   
    public void update_footstep(bool stete)
    {
        if(stete)
            _footStep.Play();
        else
            _footStep.Stop();
    }

    public void Play_Blade_01()
    {
        Blade_01.Play();
    }


    public void PlayBlade02()
    {
        Blade_02.Play();
    }
    
    public void PlayBlade03()
    {
        Blade_03.Play();
        
    }

    public void StopBlade()
    {
        Blade_01.Simulate(0);
        Blade_01.Stop();
        Blade_02.Simulate(0);
        Blade_02.Stop();
        Blade_03.Simulate(0);
        Blade_03.Stop();
        
    }
    
    public void PlaySlash(Vector3 pos)
    {
        Slash.transform.position = pos;
        Slash.Play();
    }

    public void PlayHeal()
    {
        Heal.Play();
    }
    
    
}
