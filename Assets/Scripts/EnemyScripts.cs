using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Riferimento al cubo (di default al centro della scena)
    public Transform target; 
    public float speed = 5f; // Velocità del drone
    public float stoppingDistance = 50.5f; // Distanza di fermata

    private Rigidbody rb; 
    private Vector3 startPosition;
    public EnvironmentController environmentController; // Riferimento all'EnvironmentController specifico

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        startPosition = transform.position;

        // Trova il target se non è stato assegnato
        if (target == null)
        {
            target = GameObject.FindWithTag("house").transform; // Usa il tag "house" per trovare il target
        }

         if (environmentController == null)
        {
            environmentController = FindFirstObjectByType<EnvironmentController>();
            if (environmentController == null)
            {
                Debug.LogError("EnvironmentController non trovato nella scena!");
            }
        }
    }

    // Metodo per impostare il riferimento al controller e il target dall'esterno (in modo dinamico)
    

    private void Update()
    {
        if (target != null)
        {
            // Calcola la direzione verso il target
            Vector3 direction = (target.position - transform.position).normalized;

            // Calcola la distanza attuale dal target
            float distance = Vector3.Distance(transform.position, target.position);

            // Se il drone è abbastanza lontano, continua a muoversi
            if (distance > stoppingDistance)
            {
                // Muovi il drone verso il target
                transform.position += direction * speed * Time.deltaTime;

                // Orienta il drone verso il target
                transform.LookAt(target.position);
            }
        }
    }

    // Usa OnTriggerEnter invece di OnCollisionEnter per maggiore precisione
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("house"))
        {
            // Notifica all'EnvironmentController che il nemico ha raggiunto il centro
            if (environmentController != null)
            {
                environmentController.EnemyReachedCenter();
            }
            else
            {
                Debug.LogError("EnvironmentController non assegnato a " + gameObject.name);
            }
        }
    }
   

     public void ResetPosition()
    {
        transform.position = startPosition;
        //environmentController.resetBlocked();
    }
    
}
