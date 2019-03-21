using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Basis
{
    public abstract class Attackable : MonoBehaviour
    {
        int healtPoints;
        public abstract void onDeath();
    }

}
