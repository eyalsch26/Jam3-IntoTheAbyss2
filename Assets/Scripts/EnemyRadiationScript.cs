using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadiationScript : MonoBehaviour
{
    public ParticleSystem radiationLauncher;

    private List<ParticleCollisionEvent> collisionEvents;
    
    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(radiationLauncher, other, collisionEvents);
    }
}
