using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuildItems
{
    public class RessourceObjectHandler : MonoBehaviour
    {

        public GameObject StoneBlock;
        public GameObject Stone;
        void start()
        {
            name = "RessourceObjectHandler";
        }

        public GameObject getPrefabForRessource(MinableRessource ressource)
        {
            switch(ressource.type)
            {
                case MinableRessourceType.Stone: return StoneBlock; 
                default: return StoneBlock;
            }
        }
        public GameObject getPrefabForRawRessource(MinableRessource ressource)
        {
            switch (ressource.type)
            {
                case MinableRessourceType.Stone: return Stone;
                default: return Stone;
            }
        }
    }

}
