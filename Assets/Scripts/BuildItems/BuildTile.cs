using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldGen;

namespace BuildItems
{
    public class BuildTile : MonoBehaviour
    {

        /// Sollte Standardmäßig der Block sein auf dem Gebaut wird außer es wird in einem neuen Stockwerk gebaut, dann muss das Dach vom darunter liegenden Block genutzt werden.
        private BuildItem floor;

        private GameObject ceiling;

        private GameObject tableLike;

        private GameObject item;

        private int x, y;

        private GameObject wallLeft, wallRight, wallBack, wallFront;


        public void SetFloor(BuildItem floor)
        {
            this.floor = floor;
        }
        public void SetTableLike(GameObject tableLike)
        {
            this.tableLike = tableLike;
        }
        public void SetItem(GameObject item)
        {
            this.item = item;
        }
        public void SetWall(TilePosition position, GameObject wall)
        {
            switch (position)
            {
                case TilePosition.Left: wallLeft = wall; break;
                case TilePosition.Back: wallBack = wall; break;
                case TilePosition.Right: wallRight = wall; break;
                case TilePosition.Front: wallFront = wall; break;

            }
        }
        public BuildItem getFloor()
        {
            return floor;
        }
        public void setPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        // Start is called before the first frame update

    }
}
