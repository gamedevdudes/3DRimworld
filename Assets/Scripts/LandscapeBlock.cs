using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldGen
{
    public class LandscapeBlock : MonoBehaviour
    {

        private int height;
        private LandscapeType Material;
        ///<summary>
        /// Hier werden diverse Parameter zur verfügung gestellt wie MovementSpeed, Aussehen, etc
        ///</summary>
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public enum LandscapeType
    {
        Stone,
        Earth,
        Water
    }

}
