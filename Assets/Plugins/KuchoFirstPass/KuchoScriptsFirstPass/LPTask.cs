using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.PlayerLoop;

[System.Serializable]
public class LPTasks{
    public  bool debug;
    public  bool debugPolyPoints;
    public  bool debugPolyOnOff;

    public static LPTaskArray tasks; // solo es una referncia que va variando de una lista a la otra

    public static uint id = 1; // 0 = tarea nula, identificador unico que se incrementa a cada tarea nueva pero jamas se resetea, sirve para comprobar si el propcesado de tareas funciona correctamente

    public LPTasks(){
        tasks = new LPTaskArray(2000);

    }



}
[System.Serializable]
public class LPTaskArray{
    public LPTask[] tasks;
    int executeIndex;
    int addIndex;
    int count;
    public bool executing;
    public static LPTaskType lastType;

    LPTask w_Task_Default = new LPTask(); // realmente solo se usa cuando tengo apagado LPManager , para que no de error, pero sus cambios caen en saco roto.
    LPTask w_Task;

    public LPTaskArray(int length){
        tasks = new LPTask[length];
        for (int i = 0; i < tasks.Length; i++) // relleno toda la tabla con lptask "vacias" para luego ir rellenando solo los datos que necesitamos cada vez.
            tasks[i] = new LPTask();
        executeIndex = 0;
        addIndex = 0;
        count = 0;
    }


    int count_old;
    uint id_old;
}
public enum LPTaskType {None, LPD2DCellPartToClone, LPParticleSystemSwitch, LPFixtureToSwitch, LPBodyToSwitch, LPBodyToSetPos, LPBodyToSetAngle, LPBodyToSetFixedRotation,
    LPBodyToSetBodyType, LPBodyToSetLinearVelocity, LPBodyToSetAngularVelocity, LPBodyToApplyForce, LPBodyToApplyForceAtPoint, LPBodyToApplyImpulseAtPoint,
	LPBodyToSetIsAwake, LPBodyToApplyTorque, LPBodyToApplyAngularImpulse, LPSpawnParticles}

[System.Serializable]
public class LPTask{
    public bool executePending;
    public LPTaskType type;
    public uint id;
    public bool onOff;
    public TS_PolygonSpriteCollider.Cell cell; // crear una fixture a partir de una cell
    public int pathIndex = -1;// el indice que identifica al path/fixture dentro de la cell
    public LPParticleSystem partSys;
    public LPFixture fix;
    public LPBody body;
    public Vector2 pos;
    public float angle;
    public LPBodyTypes bodyType;
    public Vector2 velocity;
    public float angularVelocity;
    public Vector2 force;
    public Vector2 point;
    public Vector2 impulse;
    public float torque;
    public float angularImpulse;
    public KuchoLPParticleSpawner spawner;

    public override string ToString()
    {
        return type + "/" + body + " ";
    }

}
