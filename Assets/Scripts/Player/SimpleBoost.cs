using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBoost : MonoBehaviour
{
    public Color BoostColor;
    public Color FlyColor;
    public float BackThrusterBoostSpeed;
    public float BackThrusterFlySpeed;
    public float BackThrusterBoostSize;
    public float BackThrusterFlySize;
    public float LimbThrusterBoostSpeed;
    public float LimbThrusterFlySpeed;
    public float LimbThrusterBoostSize;
    public float LimbThrusterFlySize;
    [SerializeField] private List<ParticleSystem> BackThrusters = new List<ParticleSystem>();
    [SerializeField] private List<ParticleSystem> LimbThrusters = new List<ParticleSystem>();

    public void ThrusterStart()
    {

        foreach (ParticleSystem thruster in BackThrusters)
        {
            var main = thruster.main;
            SetThrusterValue(main, FlyColor, BackThrusterFlySpeed, BackThrusterFlySize);
            thruster.Play();
        }
        foreach (ParticleSystem thruster in LimbThrusters)
        {
            var main = thruster.main;
            SetThrusterValue(main, FlyColor, LimbThrusterFlySpeed, LimbThrusterFlySize);
            thruster.Play();
        }
    }
    public void ThrusterBoost()
    {
        foreach (ParticleSystem thruster in BackThrusters)
        {
            var main = thruster.main;
            SetThrusterValue(main, BoostColor, BackThrusterBoostSpeed, BackThrusterBoostSize);
            thruster.Play();
        }
        foreach (ParticleSystem thruster in LimbThrusters)
        {
            var main = thruster.main;
            SetThrusterValue(main, BoostColor, LimbThrusterBoostSpeed, LimbThrusterBoostSize);
            thruster.Play();
        }
    }
    private void SetThrusterValue(ParticleSystem.MainModule thruster, Color color, float speed, float size)
    {
        thruster.startColor = color;
        thruster.startSpeedMultiplier = speed;
        thruster.startSizeMultiplier = size;
    }

}