using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMessage : BaseSerialized
{
    public override byte[] GetBytes()
    {
        throw new System.NotImplementedException();
    }

    public override int GetLength()
    {
        throw new System.NotImplementedException();
    }

    public override int GetID()
    {
        throw new System.NotImplementedException();
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        throw new System.NotImplementedException();
    }
}
