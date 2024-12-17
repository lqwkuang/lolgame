using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FogOfWar.Demo
{
    public class AutoMoveAgent : MonoBehaviour
    {
        public float arriveDistance = 1;

        private FieldManagerTest field;

        private NavMeshAgent agent;

        // Start is called before the first frame update
        void Start()
        {
            field = FindObjectOfType<FieldManagerTest>();
            agent = GetComponent<NavMeshAgent>();
            agent.SetDestination(field.GetRandomPos(transform.position.y));
        }

        // Update is called once per frame
        void Update()
        {
            if (agent.remainingDistance < arriveDistance)
                agent.SetDestination(field.GetRandomPos(transform.position.y));
        }
    }
}
