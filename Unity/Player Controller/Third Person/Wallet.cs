using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : ControllerModule
{
    private int currency;

    public int CheckFunds()
    {
        return currency;
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public bool RemoveCurrency(int amount)
    {
        if (currency - amount >= 0)
        {
            currency -= amount;
            return true;
        }
        return false;
    }
}
