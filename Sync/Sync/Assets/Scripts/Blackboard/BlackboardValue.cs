using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BlackboardValue
{
    protected Type type;
    public Type Type
    {
        get
        {
            return type;
        }
        private set
        {
            type = value;
        }
    }

    protected object value;
    public virtual object Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            Type = this.value.GetType();
        }
    }
}