using UnityEngine;

public class LaserDistance : MonoBehaviour
{
    public float maxDistance = 1f; // Distanza massima del laser
    public LineRenderer lineRenderer; // LineRenderer per visualizzare il laser
    public float width=5f;
    public string sensorName;
    private float distance = 0f;
    
    void Start()
    {
        // Assicurati che il LineRenderer sia assegnato
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = width;
        }
    }

    void Update()
    {
        // Inizializza il punto di partenza e la direzione del raycast
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // Effettua il raycast
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, maxDistance))
        {
            // Visualizza il laser
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
            Debug.Log("Distanza: " + hit.distance);
            distance = hit.distance;
        }
        else
        {
            // Se non colpisce nulla, visualizza fino alla distanza massima
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * maxDistance);
        }
    }

    public float getDistance()
    {
        return distance;
    }
}
