using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJsonSerializable
{
    public string DumpJSON();
}
public interface IJsonDeserializable
{

}
public interface IJsonConvertible: IJsonSerializable, IJsonDeserializable
{
    
}