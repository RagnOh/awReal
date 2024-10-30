using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DroneAgent : Agent
{
    public float throttlePower = 25f; // Potenza per la spinta verticale
    public float yawSpeed = 15f; // Velocità di rotazione (yaw)
    public float pitchSpeed = 15f; // Velocità di inclinazione (pitch)
    public float rollSpeed = 15f; // Velocità di rotazione laterale (roll)
    public float forwardForce = 100f; // Forza per muoversi avanti/indietro
  
    public float lateralForce = 100f;
    public Transform[] otherAgents; 
    private Rigidbody rb;
    private Vector3 startingPosition;
    private EnvironmentController environmentController;
    public float maxDistance = 20f;  // Distanza massima per la normalizzazione
    public float maxSpeed = 5f;  // Velocità massima per la normalizzazione
    public float maxEnemySpeed = 10f; 

   // private float angoloRoll = 0f;
    //private float angoloPitch = 0f;
    private float rollTrack =0f;
    private float pitchTrack = 0f;

    private Vector3 forwardMovement;
    private Vector3 lateralMovement;

    public Transform enemyDroneTransform;
    private Rigidbody enemyDroneRb;

    public List<LaserDistance> laserSensors = new List<LaserDistance>();
    private SimpleSensors sensors;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position; // Salva la posizione iniziale del drone
        environmentController = FindFirstObjectByType<EnvironmentController>();
        if (enemyDroneTransform != null)
        {
            enemyDroneRb = enemyDroneTransform.GetComponent<Rigidbody>();
        }

        sensors = new SimpleSensors();
    }

    public override void OnEpisodeBegin()
    {
        // Reset dell'agente e dell'ambiente all'inizio di ogni episodio
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;
        //angoloPitch = 0f;
        pitchTrack = 0f;
        rollTrack =0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Osservazioni: puoi aggiungere variabili come la velocità e la posizione del drone

        float distanceToEnemy = Vector3.Distance(transform.localPosition, enemyDroneTransform.localPosition);
        sensor.AddObservation(distanceToEnemy); // Posizione del drone
        
        //Sensoristica Statica
        //g
        sensor.AddObservation(rb.velocity); // Velocità attuale del drone
        sensor.AddObservation(sensors.GetAltitudeData(transform));
        sensor.AddObservation(sensors.GetGyroscopeData(transform));
        sensor.AddObservation(sensors.GetAccelerometerData(rb));

        //Sensoristica laser
        sensor.AddObservation(laserSensors[0].getDistance());
        sensor.AddObservation(laserSensors[1].getDistance());
        sensor.AddObservation(laserSensors[2].getDistance());
        sensor.AddObservation(laserSensors[3].getDistance());
        sensor.AddObservation(laserSensors[4].getDistance());
        sensor.AddObservation(laserSensors[5].getDistance());


        //sensor.AddObservation(rb.angularVelocity); // Velocità angolare del drone
        foreach (var otherAgent in otherAgents)
        {
            if (otherAgent != null && otherAgent != transform)  // Escludi se stesso
            {
                // Normalizza la posizione relativa dell'altro agente
                //Vector3 relativeOtherAgentPosition = (otherAgent.localPosition - transform.localPosition) / maxDistance;
                //sensor.AddObservation(relativeOtherAgentPosition);

                // Aggiungi la distanza tra questo agente e l'altro
                float distanceToOtherAgent = Vector3.Distance(transform.localPosition, otherAgent.localPosition) / maxDistance;
                sensor.AddObservation(distanceToOtherAgent);
            }
        }

       if (enemyDroneTransform != null)
        {
            sensor.AddObservation(enemyDroneTransform.localPosition); // Posizione (x, y, z)
        }

        // Aggiungi la velocità del drone nemico come osservazione
        if (enemyDroneRb != null)
        {
            sensor.AddObservation(enemyDroneRb.velocity); // Velocità (x, y, z)
            //sensor.AddObservation(enemyDroneRb.position);
        }
    }

    private void FixedUpdate()
    {
         
       if( rollTrack != 0){
          rb.AddForce(lateralMovement, ForceMode.Acceleration);
       }
        
    
        if( pitchTrack != 0){
        rb.AddForce(forwardMovement, ForceMode.Acceleration);
        }
    
    //Debug.Log(pitchTrack);
   
         //rb.AddForce(forwardMovement, ForceMode.Acceleration);
        // rb.AddForce(lateralMovement, ForceMode.Acceleration);

        //float altezza = 0f;
        

        //altezza = sensors.GetAltitudeData(transform);
        //Vector3 direz = new Vector3(sensors.GetGyroscopeData(transform));

        //Debug.Log(sensors.GetGyroscopeData(transform));

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Azioni ricevute dal modello di apprendimento
        // Azioni ricevute dal modello di apprendimento
    float moveUp = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f); // Azione per spinta verticale
    float yaw = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f); // Azione per rotazione yaw
    float pitch = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f); // Azione per inclinazione pitch (avanti/indietro)
    float roll = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f); // Azione per rotazione roll (laterale)
    
    float distanceToEnemy = Vector3.Distance(transform.position, enemyDroneTransform.position);

    // Controlla se il drone è sufficientemente vicino per considerare l'azione di sparo
    if (distanceToEnemy <= 0.5f)
    {
        int fireAction = actions.DiscreteActions[0]; // Legge l'azione discreta solo se è nel raggio

        // Se l'azione di sparo è attiva, esegue la funzione di sparo
        if (fireAction == 1)
        {
            Explode();
        }
    }
   // Debug.Log("roll:"+ roll);

    // Calcola l'inclinazione attuale del drone (pitch e roll) per la spinta verticale
    float currentPitch = Vector3.Angle(transform.forward, Vector3.up) - 90f;
    float currentRoll = Vector3.Angle(transform.right, Vector3.up) - 90f;

    // Calcola la spinta verticale in base a quanto il drone è parallelo al pavimento
    float verticalThrottle = Mathf.Clamp(1f - Mathf.Abs(currentPitch) / 90f - Mathf.Abs(currentRoll) / 90f, 0f, 1f);
    
    // Se il drone è parallelo al pavimento, la spinta dei motori lo fa salire
    rb.AddForce(Vector3.up * throttlePower * moveUp * verticalThrottle * 2f, ForceMode.Acceleration);

    // Applica le rotazioni (yaw, pitch, roll)
    rb.AddTorque(Vector3.up * yawSpeed * yaw, ForceMode.Acceleration); // Rotazione sullo yaw
    rb.AddTorque(transform.right * pitchSpeed * pitch, ForceMode.Acceleration); // Inclinazione pitch
    rb.AddTorque(transform.forward * rollSpeed * roll, ForceMode.Acceleration); // Inclinazione roll

    // Applica la forza in avanti/indietro in base al pitch (inclinazione avanti/indietro)
    if (pitch  > 0.1f) // Solo se il pitch è significativo
    {
        forwardMovement = transform.forward * pitch * forwardForce; // La forza viene applicata lungo l'asse locale del drone
        pitchTrack = pitchTrack + pitch;
       // angoloPitch = transform.eulerAngles.x;
        
    }

    if (pitch < -0.1f) // Solo se il pitch è significativo
    {
        forwardMovement = transform.forward * pitch * forwardForce; // La forza viene applicata lungo l'asse locale del drone
        pitchTrack = pitchTrack + pitch;
       // angoloPitch = transform.eulerAngles.x;
        
    }

    
    

    // Applica la forza laterale in base al roll (inclinazione destra/sinistra)
    if (roll > 0.2f) // Solo se il roll è significativo
    {
        lateralMovement = (-1f) * transform.right * roll * lateralForce; // La forza viene applicata lungo l'asse locale del drone
      //         angoloRoll = transform.eulerAngles.z;
        rollTrack = rollTrack + roll;
        
    }

    if (roll < -0.2f) // Solo se il roll è significativo
    {
        lateralMovement = (-1f) * transform.right * roll * lateralForce; // La forza viene applicata lungo l'asse locale del drone
      //         angoloRoll = transform.eulerAngles.z;
        rollTrack = rollTrack + roll;
       // Debug.Log("entrato");
        
    }
    

    
    

    // Aggiungi un piccolo drag (resistenza) per stabilizzare il drone se necessario
    rb.velocity *= 0.99f;
        // Ricompense e penalità
        float distanceToGround = transform.position.y;
        float rollAngle = transform.eulerAngles.z;

    // Correggi l'angolo di roll per avere valori compresi tra -180 e 180
    if (rollAngle > 180f) rollAngle -= 360f;

    // Penalizza se il roll supera un certo bound, ad esempio ±45 gradi
    float rollBound = 50f;
    if (Mathf.Abs(rollAngle) > rollBound)
    {
        // Penalizza l'agente se l'angolo di roll supera il bound
        environmentController.AddRewardToAgent(this,-0.2f); // Penalità per ogni frame in cui supera il limite
    }
       /* if (distanceToGround < 0.5f)
        {
            SetReward(-1f); // Penalizza se il drone è troppo vicino al suolo
            EndEpisode();
        }
        */
        if (distanceToGround <= 50f)
        {
            // Assegna una ricompensa proporzionale all'altezza, ad esempio
            environmentController.AddRewardToAgent(this,((distanceToGround / 50f)*0.4f));
        }
        else
        {
            // Penalizzazione se supera l'altezza di 70
            environmentController.AddRewardToAgent(this,-15f); // Penalità costante per altezza eccessiva
        }


        //float distanceToEnemy = Vector3.Distance(transform.localPosition, enemyDroneTransform.localPosition);

        // Reward basato sulla distanza
        // Se il drone si avvicina al nemico, premi l'agente (negativa se l'obiettivo è allontanarsi)

         // Penalità per distanza maggiore, ricompensa per distanza minore


        // Assegna il reward calcolato all'agente
       if (Mathf.Abs(distanceToEnemy)>1f){
            float reward = Mathf.Abs(distanceToEnemy) * 0.01f;
            environmentController.AddRewardToAgent(this,(-1f)*reward);
        }

        if (Mathf.Abs(distanceToEnemy)<1f){
            float reward = Mathf.Abs(distanceToEnemy) * 0.1f;
            environmentController.AddRewardToAgent(this,reward);
        }

        
        


    }

     private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            // Quando tocca il nemico, notifica l'EnvironmentController
            
            //droneModel.ResetDrone();
            
            //gameObject.SetActive(false);

            // Notifica al controller dell'ambiente per rimuovere gli agenti
            environmentController.AgentFloor(this);
            //environmentController.RemoveAgent(this);
           // environmentController.RemoveAgent(collision.gameObject.GetComponent<DroneAgent>());
        
        }

        else if (collision.gameObject.CompareTag("wall"))
        {
            // Quando tocca un muro, notifica l'EnvironmentController
            environmentController.AgentHitWall(this);

            // Disabilita l'agente
            //gameObject.SetActive(false);

            // Notifica al controller dell'ambiente per rimuovere questo agente
            //environmentController.RemoveAgent(this);
        }
        else if (collision.gameObject.CompareTag("house"))
        {
            // Quando tocca un muro, notifica l'EnvironmentController
            environmentController.AgentHitTarget(this);
        }
        else if (collision.gameObject.CompareTag("enemy"))
        {
            // Quando tocca un muro, notifica l'EnvironmentController
            //Debug.Log("Nemico eliminato");
            environmentController.EnemyStopped(this);
        }
        else if (collision.gameObject.CompareTag("agent"))
        {
            // Quando tocca un muro, notifica l'EnvironmentController
            environmentController.AddRewardToAgent(this,-0.8f);
            environmentController.AgentHitAgent(this);
            //environmentController.AddRewardToAgent(collision.gameObject.GetComponent<DroneAgent>(),-0.8f);
            //environmentController.AgentHitAgent(collision.gameObject.GetComponent<DroneAgent>());

            // Disabilita entrambi gli agenti coinvolti
            //collision.gameObject.SetActive(false);
           // gameObject.SetActive(false);

            // Notifica al controller dell'ambiente per rimuovere gli agenti
           // environmentController.RemoveAgent(this);
           // environmentController.RemoveAgent(collision.gameObject.GetComponent<DroneAgent>());
        }
        else
        {
            environmentController.AddRewardToAgent(this,- 0.15f); // Ricompensa positiva per rimanere in aria
        }

         
    }

    public Vector3 GetInitialPosition()
    {
        return startingPosition;
    }

    private void Explode()
    {
        Debug.Log("Boom");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Implementazione di controlli manuali per testare l'agente
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.Space) ? 1f : (Input.GetKey(KeyCode.LeftControl) ? -1f : 0f); // Spinta verticale
        continuousActions[1] = Input.GetKey(KeyCode.J) ? -1f : (Input.GetKey(KeyCode.L) ? 1f : 0f); // Rotazione yaw
        continuousActions[2] = Input.GetKey(KeyCode.I) ? 1f : (Input.GetKey(KeyCode.K) ? -1f : 0f); // Inclinazione pitch (avanti/indietro)
        continuousActions[3] = Input.GetKey(KeyCode.Q) ? -1f : (Input.GetKey(KeyCode.E) ? 1f : 0f); // Rotazione roll (laterale)
    }
}

