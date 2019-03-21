using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldGen
{
    public class WorldTileMaterilaProvider : MonoBehaviour
    {
        public Material earth;
        public Material snow;
        public Material water;

        public Material GetWorldTileMaterialByType(LandscapeType type)
        {
            switch (type)
            {
                case LandscapeType.Earth: return earth;
                case LandscapeType.Water: return water;
                case LandscapeType.Stone: return snow;
                default: return earth;
            }
        }
    }

}
