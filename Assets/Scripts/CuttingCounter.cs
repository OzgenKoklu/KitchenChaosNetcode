using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
   public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //there is no kitchenObject here
            if (player.HasKitchenObject())
            {
                //player is carrying something.
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                //If player has a kitchen object with matching recipe
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                }
            }
            else
            {
                //player not caring anything 
            }
        }
        else
        {
            //there is a kitchenObject on the counter
            if (player.HasKitchenObject())
            {//player is cariyng something

            }
            else
            {//player is not cariyng anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //cut the thing if theres a kitchen object that can be cut
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
           
           GetKitchenObject().DestroySelf();
           KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
        else
        {
           
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach(CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if(cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }

}
