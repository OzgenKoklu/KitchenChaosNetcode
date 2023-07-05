using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter 
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;


 public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //there is no kitchenObject here
            if (player.HasKitchenObject())
            {
                //player is carrying something.
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //player not caring anything 
            }
        }
        else
        {
            //there is a kitchenObject on the counter
            if(player.HasKitchenObject())
            {//player is cariyng something

            }
            else
            {//player is not cariyng anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
