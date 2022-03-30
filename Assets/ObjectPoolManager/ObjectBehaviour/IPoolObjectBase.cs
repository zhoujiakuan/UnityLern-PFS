using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IPoolObjectBase
{
       void OnInstant();

       void OnSpawnBefore();
      
      void OnSpawnEnd();
      
      void OnDeSpawn();
}