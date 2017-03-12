using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BlackboardValue
{
    private Type type;
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

    private Object value;
    public Object Value
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