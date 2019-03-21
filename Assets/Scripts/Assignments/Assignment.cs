using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldGen;

namespace Assignments
{
    public abstract class Assignment : MonoBehaviour
    {
        public WorldTile location;

        // Start is called before the first frame update
        abstract public void execute();
    }

}
