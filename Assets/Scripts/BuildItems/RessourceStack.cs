using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceStack : MonoBehaviour, RessourceProduction
{
    MinableRessource ressource;
    int amount;



    public (Ressource, int) Mine()
    {
        return (ressource,amount);
    }
}
