using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sensori di posizione e direzione.

public class SimpleSensors: MonoBehaviour
{
    private float gyroSensitivity = 1.0f;
    private float accelSensitivity = 1.0f;
    private float altitudeSensitivity = 1.0f;
    
    // Giroscopio Velocity
    public  Vector3 GetGyroscopeVelocityData(Rigidbody droneRigidbody)
    {
        
        return droneRigidbody.angularVelocity * gyroSensitivity;
    }

    // Accellerometro
    public  Vector3 GetAccelerometerData(Rigidbody droneRigidbody)
    {
        
        return droneRigidbody.velocity / Time.deltaTime * accelSensitivity;
    }

    // Altimetro
    public float GetAltitudeData(Transform droneTransform)
    {
        return droneTransform.position.y * altitudeSensitivity;
    }

    // Giroscopio
    public Vector3 GetGyroscopeData(Transform droneTransform)
    {
       
        return droneTransform.rotation.eulerAngles;
    }



    
}
