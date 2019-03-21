using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinableRessource : Ressource
{
    public MinableRessourceType type;
}

public enum MinableRessourceType
{
    Wood, Iron, Gold, Silver, Stone, Crystal
}
