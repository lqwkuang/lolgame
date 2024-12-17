using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendClass 
{
   //Vector3->V3info
   public static V3Info ToV3Info(this Vector3 vector)
   {
        V3Info v3Info = new V3Info();
        v3Info.X = (int)(vector.x * 1000);
        v3Info.Y = (int)(vector.y * 1000);
        v3Info.Z = (int)(vector.z * 1000);
        return v3Info;
    }
    //V3info->Vector3
    public static Vector3 ToVector3(this V3Info v3Info)
    {
        Vector3 vector3 = new Vector3();
        vector3.x = (int)(v3Info.X / 1000);
        vector3.y = (int)(v3Info.Y / 1000);
        vector3.z = (int)(v3Info.Z / 1000);
        return vector3;
    }
}
