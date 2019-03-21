using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Basis;
using BuildItems;
namespace BuildItems
{
    public class RessourceBlock : Attackable, RessourceProduction
    {
        MinableRessource ressource;
        int amount;
        BuildTile origin;
        GameObject appearance;
        public (Ressource, int) Mine()
        {
            return (ressource, amount);
        }
        public void Initilize(MinableRessourceType type, int amount, BuildTile origin)
        {
            ressource = new MinableRessource();
            ressource.type = type;
            this.amount = amount;
            this.origin = origin;
            RessourceObjectHandler handler = GameObject.Find("RessourceObjectHandler").GetComponent<RessourceObjectHandler>();
            appearance = handler.getPrefabForRessource(ressource);
        }
        public override void onDeath()
        {
            RessourceObjectHandler handler = GameObject.Find("RessourceObjectHandler").GetComponent<RessourceObjectHandler>();
            var stackReference = handler.getPrefabForRawRessource(ressource);
            var stack = Instantiate(stackReference, origin.transform.position, Quaternion.identity);
            origin.SetItem(stack);
            foreach (var comp in gameObject.GetComponents<Component>())
            {
                if (!(comp is Transform))
                {
                    Destroy(comp);
                }
            }
        }
    }

}
