using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSensor : MonoBehaviour
{
    // Variabile che definisce la lunghezza massima del laser
    public float maxDistance = 10.0f;

    // LayerMask per definire quali layer il laser può colpire (ad esempio ostacoli)
    //public LayerMask obstacleLayer;

    // Variabile che indica la direzione del laser (di default è la direzione in avanti)
    public Vector3 laserDirection = Vector3.forward;

    // Metodo per ottenere la distanza dall'ostacolo
    public float GetDistanceToObstacle()
    {
        // Raycast che parte dalla posizione dell'oggetto e va nella direzione del laser
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(laserDirection), out hit, maxDistance))
        {
            // Se il laser colpisce un ostacolo, restituiamo la distanza
            return hit.distance;
        }

        // Se non colpisce nessun ostacolo, restituiamo la distanza massima
        return maxDistance;
    }

    // Metodo per visualizzare il raggio del laser nell'editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(laserDirection) * maxDistance;
        
        // Se l'editor è attivo, mostriamo il laser anche lì per debugging
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(laserDirection), out hit, maxDistance))
        {
            // Se colpisce un ostacolo, visualizziamo una linea fino all'ostacolo
            Gizmos.DrawLine(transform.position, hit.point);
        }
        else
        {
            // Altrimenti visualizziamo una linea lunga quanto la distanza massima
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }
    }
}
