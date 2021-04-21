using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticleShooter : GameBehaviour
{
    [SerializeField]
    private DanmakuParticleData particle;
    private DanmakuParticleEmitter emitter;

    public override void GameStart()
    {
        emitter = new DanmakuParticleEmitter(particle, transform);
    }

    private void OnDrawGizmos()
    {
        if (particle == null)
            return;

        Gizmos.color = Color.green;

        if(particle.shapeModule.shapeType == ShapeType.Edge)
        {
            float angle = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
            Vector3 half = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * particle.shapeModule.edgeWidth / 2;
            Gizmos.DrawLine(transform.position - half, transform.position + half);

            angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector3 toward = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Gizmos.DrawWireSphere(transform.position + toward, 0.5f);
        }

        if (particle.shapeModule.shapeType == ShapeType.Sector)
        {
            float delta = (particle.shapeModule.spread / 50f) * Mathf.Deg2Rad;
            float startAngle = (transform.rotation.eulerAngles.z - particle.shapeModule.spread / 2) * Mathf.Deg2Rad;
            float endAngle = startAngle + delta;

            for (int i = 0; i < 50; i++)
            {
                Vector3 start = transform.position + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * particle.shapeModule.radius;
                Vector3 end = transform.position + new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * particle.shapeModule.radius;

                Gizmos.DrawLine(start, end);
                startAngle = endAngle;
                endAngle = startAngle + delta;
            }

            startAngle = (transform.rotation.eulerAngles.z - particle.shapeModule.spread / 2) * Mathf.Deg2Rad;
            endAngle = startAngle + delta;

            for (int i = 0; i < 50; i++)
            {
                Vector3 start = transform.position + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * particle.shapeModule.radius * (1 - particle.shapeModule.radiusThickness);
                Vector3 end = transform.position + new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * particle.shapeModule.radius * (1 - particle.shapeModule.radiusThickness);

                Gizmos.DrawLine(start, end);
                startAngle = endAngle;
                endAngle = startAngle + delta;
            }

            startAngle = (transform.rotation.eulerAngles.z - particle.shapeModule.spread / 2) * Mathf.Deg2Rad;
            endAngle = (transform.rotation.eulerAngles.z + particle.shapeModule.spread / 2) * Mathf.Deg2Rad;

            if(particle.shapeModule.spread < 360)
            {
                Vector3 a = transform.position + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * particle.shapeModule.radius;
                Vector3 b = transform.position + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle)) * particle.shapeModule.radius * (1 - particle.shapeModule.radiusThickness);
                Gizmos.DrawLine(a, b);
                a = transform.position + new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * particle.shapeModule.radius;
                b = transform.position + new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * particle.shapeModule.radius * (1 - particle.shapeModule.radiusThickness);
                Gizmos.DrawLine(a, b);
            }
        }
    }
}
