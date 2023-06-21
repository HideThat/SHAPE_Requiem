using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryCharge : MonoBehaviour
{
    public ParticleSystem ps; // ��ƼŬ �ý���
    public float strength = 10f; // �߽����� ������� ���� ����

    void LateUpdate()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];

        ps.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.Particle p = particles[i];

            Vector3 particleWorldPosition;

            if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
            {
                particleWorldPosition = transform.TransformPoint(p.position);
            }
            else
            {
                particleWorldPosition = p.position;
            }

            Vector3 directionToTarget = (transform.position - particleWorldPosition);
            Vector3 seekForce = directionToTarget.normalized * strength;

            p.velocity += seekForce * Time.deltaTime;

            particles[i] = p;
        }

        ps.SetParticles(particles, particles.Length);
    }
}
