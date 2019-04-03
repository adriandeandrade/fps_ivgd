using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class ConnectedWaypoint: PatrolWaypoint
    {
        [SerializeField] protected float connectivityRadius = 25f;
        
        List<ConnectedWaypoint> connections;

        private void Start()
        {
            //Get all waypoint objs in scene
            GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

            //Create list of waypoints for later reference
            connections = new List<ConnectedWaypoint>();

            //Check if they're a connected waypoint
            for(int i = 0; i < allWaypoints.Length; i++)
            {
                ConnectedWaypoint nextWaypoint = allWaypoints[i].GetComponent<ConnectedWaypoint>();

                //i.e. we found a waypoint
                if(nextWaypoint != null)
                {
                    if(Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= connectivityRadius && nextWaypoint != this)
                    {
                        connections.Add(nextWaypoint);
                    }
                }
            }
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, connectivityRadius);
        }

        public ConnectedWaypoint NextWaypoint(ConnectedWaypoint previousWaypoint)
        {
            if(connections.Count == 0)
            {
                //No waypoints and it's the previous one. Use that.
                Debug.LogError("Insufficient waypoint count.");
                return null;
            }
            else if(connections.Count == 1 && connections.Contains(previousWaypoint))
            {
                //Only one waypoint
                return previousWaypoint;
            }
            else
            {
                ConnectedWaypoint nextWaypoint;
                int nextIndex = 0;

                do
                {
                    nextIndex = UnityEngine.Random.Range(0, connections.Count);
                    nextWaypoint = connections[nextIndex];
                }
                while (nextWaypoint == previousWaypoint);

                return nextWaypoint;
            }
        }
    }
}