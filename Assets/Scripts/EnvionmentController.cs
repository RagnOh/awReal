using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnvironmentController : MonoBehaviour
{
    public List<DroneAgent> agents;  // Lista di agenti
    private int activeAgents;  // Numero di agenti attivi
    public EnemyScript enemy;  // Riferimento al nemico
    private int blocked = 0;

    void Start()
    {
        // Inizializza il numero di agenti attivi
        activeAgents = agents.Count;
    }

    // Metodo per resettare un singolo agente
    public void ResetAgent(DroneAgent agent)
    {
        if (agent != null)
        {
            // Ripristina la posizione dell'agente alla sua posizione iniziale
            agent.transform.position = agent.GetInitialPosition(); 
            agent.gameObject.SetActive(true);  // Riattiva l'agente se era disabilitato
            agent.EndEpisode();  // Termina l'episodio dell'agente e ne inizia uno nuovo
        }
    }

    // Metodo per penalizzare un singolo agente
    public void PenalizeAgent(DroneAgent agent, float penalty)
    {
        if (agent != null)
        {
            agent.AddReward(penalty);  // Penalizza l'agente
        }
    }

    // Metodo per rimuovere un singolo agente dall'episodio
    public void RemoveAgent(DroneAgent agent)
    {
        if (agent != null)
        {
            // Riduci il conteggio degli agenti attivi
           // activeAgents--;

            // Disattiva l'agente e termina l'episodio
            //agent.gameObject.SetActive(false);
            agent.EndEpisode();

            // Se tutti gli agenti sono stati eliminati, resetta la posizione del nemico
        }
        else
        {
            Debug.LogError("L'agente passato a RemoveAgent è null.");
        }
    }

    // Metodo chiamato quando il nemico raggiunge il centro
    public void EnemyReachedCenter()
    {
        enemy.ResetPosition();
       // Debug.Log("Nemico ha raggiunto il centro");

        // Penalizza tutti gli agenti attivi
       if(blocked==0){
        foreach (var agent2 in agents)
        {
            if (agent2.gameObject.activeSelf)
            {
                agent2.AddReward(-26.0f);  // Penalità individuale
                agent2.EndEpisode();  // Termina il loro episodio
                ResetAgent(agent2);  // Resetta ogni agente individualmente
            }
        }
       }
       if(blocked==1){
        Debug.Log("Nemico Fermato2");
        foreach (var agent3 in agents)
        {
            if (agent3.gameObject.activeSelf)
            {
               // agent3.AddReward(-26.0f);  // Penalità individuale
                agent3.EndEpisode();  // Termina il loro episodio
                ResetAgent(agent3);  // Resetta ogni agente individualmente
            }
        }
        blocked=0;
       }
       
    }

    // Metodo chiamato quando un agente ferma il nemico
    public void EnemyStopped(DroneAgent agent)
    {
        enemy.ResetPosition();
        Debug.Log("Nemico Fermato");

        // Ricompensa l'agente che ha fermato il nemico
        agent.AddReward(30.0f);
        blocked=1;
        foreach (var agent2 in agents)
        {
            if (agent2.gameObject.activeSelf)
            {
                agent2.AddReward(5.0f);  // Penalità individuale
                agent2.EndEpisode();  // Termina il loro episodio
                ResetAgent(agent);  // Resetta ogni agente individualmente
            }
        }
    }

    // Metodo chiamato quando un agente tocca un muro
    public void AgentHitWall(DroneAgent agent)
    {
        Debug.Log("Agente ha toccato un muro");

        // Penalizza l'agente che ha toccato il muro
        agent.AddReward(-4f);
        ResetAgent(agent);  // Reset solo per l'agente che ha toccato il muro
    }

    // Metodo chiamato quando un agente tocca il pavimento
    public void AgentFloor(DroneAgent agent)
    {
        //Debug.Log("Agente ha toccato il pavimento");

        // Penalizza l'agente e termina l'episodio
        agent.AddReward(-4.0f);
        agent.EndEpisode();
        ResetAgent(agent);  // Reset solo per l'agente che ha toccato il pavimento
    }

    // Metodo chiamato quando un agente tocca il target
    public void AgentHitTarget(DroneAgent agent)
    {
        Debug.Log("Agente ha toccato il target");

        // Penalizza l'agente e termina l'episodio
        agent.AddReward(-2.0f);
        agent.EndEpisode();
        ResetAgent(agent);  // Reset solo per l'agente che ha toccato il target
    }

    public void AgentHitAgent(DroneAgent agent)
    {
        Debug.Log("Agente ha toccato agente");

        // Penalizza l'agente e termina l'episodio
        //agent.AddReward(-2.0f);
        agent.EndEpisode();
        ResetAgent(agent);  // Reset solo per l'agente che ha toccato il target
    }

    // Metodo per assegnare una ricompensa a un singolo agente
    public void AddRewardToAgent(DroneAgent agent, float reward)
    {
        if (agent != null)
        {
            agent.AddReward(reward);  // Assegna la ricompensa all'agente
        }
    }

    public void resetBlocked()
    {
        blocked = 0;
    }
}
