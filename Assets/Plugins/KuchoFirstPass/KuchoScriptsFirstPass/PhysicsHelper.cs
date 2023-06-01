using UnityEngine;
using System.Collections.Generic;

public struct CheapDualDistance
{
    public Vector2 dist;
    public float cheapMag;
}

public static class PhysicsHelper
{

    public static bool queriesHitTriggers;
    public static bool queriesStartInColliders;

    public static RaycastHit2D[] lineCastResult = new RaycastHit2D[20];
    public static RaycastHit2D[] onlyOnelineCastResult = new RaycastHit2D[1];
    public enum LineCastType { CenterOfCollider = 0, TopOfCollider = 1 , BottomOfCollider = 2 }; // para ser totalmente perfecto deberia hacer tambien Bottom y Left Y Right, pero creo que con estos dos para como es el juego, me basta

    public static Vector2 GetCenterPfPolygon(Vector2[] poly)
    {
        Vector2 max;
        Vector2 min;
        max.x = float.MinValue;
        max.y = float.MinValue;
        min.x = float.MaxValue;
        min.y = float.MaxValue;
        
        foreach(Vector2 p in poly)
        {
            if (p.x > max.x)
                max.x = p.x;
            if (p.y > max.y)
                max.y = p.y;
            if (p.x < min.x)
                min.x = p.x;
            if (p.y < min.y)
                min.y = p.y;
        }

        Rect rect = new Rect();
        rect.min = min;
        rect.max = max;
        return rect.center;
    }

    #region LINE CAST TO POINT ################################################################################################################
    /// <summary>
     /// devuelve 0 si no encuentra ningun collider, si los encuentra los resultados estan en physycHelper.linecastResult
     /// </summary>
   public static bool LineCastToPointNoTriggers(Vector3 pos, Vector2 end, int mask)
    {
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = false;
        Physics2D.queriesStartInColliders = false;
        int hitCount = Physics2D.LinecastNonAlloc(pos, end, lineCastResult, mask);
        RestorePhysics2DQueries();
        if (hitCount == 0)
            return true;
        else
            return false;
    }

    public static CheapDualDistance LineCastToPointAndReturnObstacleThickness(Vector3 pos, Vector2 end, int mask)
    {
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = false;
        Physics2D.queriesStartInColliders = false;
        CheapDualDistance cheapDualDist  = new CheapDualDistance();
        int hitCount = Physics2D.LinecastNonAlloc(pos, end, onlyOnelineCastResult, mask);
        if (hitCount > 0)
        {
            point1 = onlyOnelineCastResult[0].point;
            hitCount = Physics2D.LinecastNonAlloc(end, pos, onlyOnelineCastResult, mask);
            if (hitCount == 0)
            {
                cheapDualDist.cheapMag = -1;
                cheapDualDist.dist = Constants.zero2;
            }
            else
            {
                point2 = onlyOnelineCastResult[0].point;
                cheapDualDist.dist = point1 - point2;
                cheapDualDist.cheapMag = KuchoHelper.CheapMagnitude(cheapDualDist.dist);
            }
        }
        RestorePhysics2DQueries();
        return cheapDualDist;
    }

    public static Vector2 point1;
    public static Vector2 point2;

    public static bool LineCastToPointNoTriggersTroughAllExceptIndestructible(Vector3 pos, Vector2 end, int mask)
    {
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = false;
        Physics2D.queriesStartInColliders = false;

        int hitCount = Physics2D.LinecastNonAlloc(pos, end, lineCastResult, mask);
        RestorePhysics2DQueries();
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D c = lineCastResult[i].collider;
           if (!c.isTrigger && c.gameObject.CompareTag("Indestructible")) // no podemos ver mas alla
                return false;
        }
        return true;
    }
    #endregion

    #region LINE CAST TO COLLIDER ################################################################################################################
   
    public static bool LineCast(Vector3 pos, Collider2D lookingFor, int mask, LineCastType lineCastType)
    {
        if (KuchoHelper.IntersectIncludingLimits(pos, lookingFor.bounds)) // estamos dentro del los bounds del collider que queremos detectar, esto ahorra linecast y ademas no iba a funcionar porque tiramos ryo desde dentro
            return true;
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = true;
        Physics2D.queriesStartInColliders = false;
        Vector2 end = GetPositionEvenIfPolygonCollyder(lookingFor, lineCastType);
        int hitCount = Physics2D.LinecastNonAlloc(pos, end, lineCastResult, mask);
        RestorePhysics2DQueries();

        for (int i = 0; i < hitCount; i++)
        {
            if (lineCastResult[i].collider == lookingFor) // es el que buscamos
                return true;
            else if (!lineCastResult[i].collider.isTrigger) // es solido , no podemos ver mas alla
                return false;
        }
        return false;
    }
    public static bool LineCast(Vector3 pos, Vector2 end, Collider2D lookingFor, int mask, bool tryExtraLineCastOnObstacleFound)
    {
        if (KuchoHelper.IntersectIncludingLimits(pos, lookingFor.bounds)) // estamos dentro del los bounds del collider que queremos detectar, esto ahorra linecast y ademas no iba a funcionar porque tiramos ryo desde dentro
            return true;
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = true;
        Physics2D.queriesStartInColliders = false;// supongo que sera para evitar mis propios colliders pero esto evita que detecte al enemigo si estoy dentro de su collider
        int hitCount = Physics2D.LinecastNonAlloc(pos, end, lineCastResult, mask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D c = lineCastResult[i].collider;
            if (c == lookingFor) // es el que buscamos
                return true;
            else if (!c.isTrigger && c.gameObject.layer != Layers.defaultLayer) // es solido , no podemos ver mas alla, las capas default las ignoro porque es la que usa PreventCollisioner
            {
                if (tryExtraLineCastOnObstacleFound)
                {
                    var h = lineCastResult[i];
                    if (h.distance < lookingFor.bounds.size.y * 2) // estamos cerca? cuento con que probablemente sea un circuloy de icual x o y
                    {
                        Vector2 side;
                        side.y = end.y;
                        float sign = Mathf.Sign(pos.x - end.x);
                        side.x = end.x + lookingFor.bounds.extents.x * sign;
                        hitCount = Physics2D.LinecastNonAlloc(pos, side, lineCastResult, mask); // ahora disparo al tope
                        for (i = 0; i < hitCount; i++)
                        {
                            c = lineCastResult[i].collider;
                            if (c == lookingFor) // es el que buscamos
                                return true;
                            else if (!c.isTrigger && c.gameObject.layer != Layers.defaultLayer) // es solido , no podemos ver mas alla, las capas default las ignoro porque es la que usa PreventCollisioner
                                break;
                        }
                    }
                }
                break; // estamos lejos , no hago otro intento al tope del collider
            }
        }
        RestorePhysics2DQueries();
        return false;
    }
    public static bool LineCastTroughAllExceptIndestructible(Vector3 pos, Collider2D lookingFor, int mask, LineCastType lineCastType)
    {
        if (KuchoHelper.IntersectIncludingLimits(pos, lookingFor.bounds)) // estamos dentro del los bounds del collider que queremos detectar, esto ahorra linecast y ademas no iba a funcionar porque tiramos ryo desde dentro
            return true;
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = true;
        Physics2D.queriesStartInColliders = false;
        Vector2 end = GetPositionEvenIfPolygonCollyder(lookingFor, lineCastType);
        int hitCount = Physics2D.LinecastNonAlloc(pos, end, lineCastResult, mask);
        RestorePhysics2DQueries();

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D c = lineCastResult[i].collider;
            if (c == lookingFor) // es el que buscamos
                return true;
            else if (!c.isTrigger && c.gameObject.CompareTag("Indestructible") && c.gameObject.layer != Layers.defaultLayer) // es solido , no podemos ver mas alla, las capas default las ignoro porque es la que usa PreventCollisioner
                return false;
        }
        return false;
    }
    public static bool LineCastTroughAllExceptIndestructible(Vector3 pos, Vector2 end, Collider2D lookingFor, int mask)
    {
        if (KuchoHelper.IntersectIncludingLimits(pos, lookingFor.bounds)) // estamos dentro del los bounds del collider que queremos detectar, esto ahorra linecast y ademas no iba a funcionar porque tiramos ryo desde dentro
            return true;
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = true;
        Physics2D.queriesStartInColliders = false;// supongo que sera para evitar mis propios colliders pero esto evita que detecte al enemigo si estoy dentro de su collider
        int hitCount = Physics2D.LinecastNonAlloc(pos, end, lineCastResult, mask);
        RestorePhysics2DQueries();

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D c = lineCastResult[i].collider;
            if (c == lookingFor) // es el que buscamos
                return true;

            if (!CanSeeTrough(c))
                return false;
            //if (!c.isTrigger && c.gameObject.CompareTag("Indestructible") && c.gameObject.layer != Layers.defaultLayer) // es solido , no podemos ver mas alla, las capas default las ignoro porque es la que usa PreventCollisioner
            //return false;
        }
        return false;
    }
    public static bool LineCastTroughAllExceptIndestructible(Vector3 pos, Vector2 end, Collider2D lookingFor, int mask, float maxDestructibleThickness)
    {
        if (KuchoHelper.IntersectIncludingLimits(pos, lookingFor.bounds)) // estamos dentro del los bounds del collider que queremos detectar, esto ahorra linecast y ademas no iba a funcionar porque tiramos ryo desde dentro
            return true;
        BackupPhysics2DQueries();
        Physics2D.queriesHitTriggers = true;
        Physics2D.queriesStartInColliders = false;// supongo que sera para evitar mis propios colliders pero esto evita que detecte al enemigo si estoy dentro de su collider
        int hitCount = Physics2D.LinecastNonAlloc(pos, end, lineCastResult, mask);
        RestorePhysics2DQueries();
        Vector2 destructibleInPoint = pos;
        bool destructibleFound = false;
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D c = lineCastResult[i].collider;
            if (c == lookingFor) // es el que buscamos
            {
                if (destructibleFound)
                {
                    int reverseHitCount = Physics2D.LinecastNonAlloc(lineCastResult[i].point, destructibleInPoint, lineCastResult, mask);
                    for (int ii = 0; ii < reverseHitCount; ii++)
                    {
                        if (lineCastResult[ii].collider.gameObject.CompareTag("Destructible")) // encontrado el destructible que atravesamos previamente
                        {
                            Vector2 destructibleOutPoint = lineCastResult[ii].point;
                            float distance = (destructibleInPoint - destructibleOutPoint).magnitude;
                            if (distance > maxDestructibleThickness)
                                return false;
                            else
                                return true;
                        }
                    }

                    //si sale del bucle es que habiendo encontrado punto de entrada no encuentra punto de salida
                    return true;
                }
                else // no habians encontrado destructible 
                {
                    return true;
                }
            }
            else
            {

                if (!c.isTrigger)
                {
                    if (c.gameObject.CompareTag("Destructible")) // podemos penetrarlo pero solo hasta cierta distancia
                    {
                        destructibleInPoint = lineCastResult[i].point;
                        destructibleFound = true;
                    }
                    else
                    {
                        //if (c.gameObject.CompareTag("Indestructible")) // no podemos ver mas alla
                        //return false;

                        if (!CanSeeTrough(c))
                            return false;
                    }

                }
            }
        }
        return false;
    }

    static bool CanSeeTrough(Collider2D c)
    {
        if (c.isTrigger)
            return true;
        if (c.gameObject.layer == Layers.defaultLayer) // las capas default las ignoro porque es la que usa PreventCollisioner
            return true;
        if (c.gameObject.CompareTag("Indestructible"))
            return false;
        if (c.gameObject.CompareTag("TurnBack")) // las sidewall de los grid corridor necesitan ser turn back para que los aliens no intenten saltar al nivel superior , asi que no me queda otra que hacerlas tambien impenetrables visiaulemnte para que las naves no puedan ver
            return false;
        if (c.gameObject.CompareTag("TurnBackAndDeflect")) // las sidewall de los grid corridor necesitan ser turn back para que los aliens no intenten saltar al nivel superior , asi que no me queda otra que hacerlas tambien impenetrables visiaulemnte para que las naves no puedan ver
            return false;
        if (c.gameObject.CompareTag("Door"))
            return false;
        return true;
    }
    #endregion
    
    public static Vector2 GetPositionEvenIfPolygonCollyder(Collider2D col, LineCastType lineCastType)
    {
        Vector2 result = Constants.zero2;
        if (col.GetType() == typeof(PolygonCollider2D))
        {
            PolygonCollider2D poly = (PolygonCollider2D) col;
            switch (lineCastType)
            {
                case (LineCastType.CenterOfCollider):
                    result = GetCenter(poly.points);
                    break;
                case (LineCastType.TopOfCollider):
                    result = GetHighestPointFromPolygon(poly.points);
                    break;
                case (LineCastType.BottomOfCollider):
                    result = GetLowestPointFromPolygon(poly.points);
                    break;
            }
            return result;
        }
        else
        {
            switch (lineCastType)
            {
                case (LineCastType.CenterOfCollider):
                    result = col.bounds.center;
                    break;
                case (LineCastType.TopOfCollider):
                    result.x = col.bounds.center.x;
                    result.y = col.bounds.max.y - 1; // si apuntara al pico real podria pasar sin rozar ojo esto no funciona si el collider es polygon
                    break;
                case (LineCastType.BottomOfCollider):
                    result.x = col.bounds.center.x;
                    result.y = col.bounds.min.y + 1; // si apuntara al pico real podria pasar sin rozar ojo esto no funciona si el collider es polygon
                    break;
            }
            return result;
        }

    }

    public static Vector2 GetCenter(Vector2[] points)
    {
        Vector2 sum = Constants.zero2;
        for (int i = 0; i < points.Length; i++)
        {
            sum.x += points[i].x;
            sum.y += points[i].y;
        }

        sum.x /= points.Length;
        sum.y /= points.Length;

        return sum;
    }
    public static Vector2 GetHighestPointFromPolygon(Vector2[] points)
    {
        Vector2 point;
        point.x = 0;
        point.y = float.MinValue;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].y > point.y)
            {
                point.x = points[i].x;
                point.y = points[i].y;
            }
        }
        return point;
    }
    public static Vector2 GetLowestPointFromPolygon(Vector2[] points)
    {
        Vector2 point;
        point.x = 0;
        point.y = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].y < point.y)
            {
                point.x = points[i].x;
                point.y = points[i].y;
            }
        }
        return point;
    }

    public static void BackupPhysics2DQueries()
    {
        queriesHitTriggers = Physics2D.queriesHitTriggers;
        queriesStartInColliders = Physics2D.queriesStartInColliders;
    }
    public static void RestorePhysics2DQueries()
    {
        Physics2D.queriesHitTriggers = queriesHitTriggers;
        Physics2D.queriesStartInColliders = queriesStartInColliders;
    }
    public static bool CollidersAreFromTheSameBody(Collider2D col1, Collider2D col2)
    {
        if (col1 == col2)
            return true;
        if (col1 == null)
        {
            if (col2 != null)
            {
                return false;
            }
        }
        // colliders son diferentes
        // si llega hasta aqui col2 no puede ser null ya que habria dado true en el primer if, tampoco col1 puede ser null
        if (col1.attachedRigidbody == col2.attachedRigidbody) // tienen el mismo body
        {
            return true;
        }
        return false; // tienen bodies diferentes
    }
    public static Vector2 ComputeTotalImpulse(Collision2D collision)
    {
        Vector2 impulse = Vector2.zero;

        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            var contact = collision.GetContact(0);
            impulse += contact.normal * contact.normalImpulse;
            impulse.x += contact.tangentImpulse * contact.normal.y;
            impulse.y -= contact.tangentImpulse * contact.normal.x;
        }
        return impulse;
    }

    public static void CopyTransformPositionToRigidbody(Transform transform, Rigidbody2D rb, float speed, float snapThreshold)
    {
        Vector2 posDiff;
        Vector2 absPosDiff;
        Vector2 newVelocity;
        Vector2 previousVelocity;
        Vector2 lerpedVelocity;
        
        posDiff.x = transform.position.x - rb.position.x;
        posDiff.y = transform.position.y - rb.position.y;
        absPosDiff.x = Mathf.Abs(posDiff.x);
        absPosDiff.y = Mathf.Abs(posDiff.y);
        
        
        if (absPosDiff.x > snapThreshold || absPosDiff.y > snapThreshold)
        {
            rb.MovePosition(transform.position);
            rb.velocity = Constants.zero2;
        }
        else
        {
            newVelocity.x = posDiff.x * speed;
            newVelocity.y = posDiff.y * speed;
            rb.velocity = newVelocity;
        }
    }

    public static Transform parentOfIndestructibleColliders; // asignado por worldmap
    public static Transform parentOfDestructibleColliders; // asignado por worldmap
    
    /// <summary>
    /// creada para CC y tomar decisiones basandose en los suelos , pero podria ser util para mas casos
    /// </summary>
    public static bool CollidersAreEqualOrShouldBeConsideredEqual(Collider2D col1, Collider2D col2)
    {
        if (col1 == col2) // ojo si ambos son null esta comparacion devuelve falso
            return true;
        if (col1 == null && col2 == null) 
            return true;
        if (col1 == null && col2 != null)
            return false;
        if (col2 == null && col1 != null)
            return false;
        if (col1.gameObject.layer != col2.gameObject.layer)
            return false;
        // si llega hasta aqui los colliders son diferentes pero tienen la misma layer
        Vector3 rot1 = col1.transform.localEulerAngles;
        Vector3 rot2 = col2.transform.localEulerAngles;
        //colliders diferewntes ambos no son null pero...
        if (col1.attachedRigidbody != col2.attachedRigidbody)
        {
            return false;
        }
        else // los rigidbody son iguales
        {
            if (col1.attachedRigidbody == null) // ambos son null // ESTO NO LO ENTIENDO---> colliders estaticos consideralos diferentes poruque este metodo estaba pensado para identificar colliders complementarios en objetos 3D
            {
                int layer1 = col1.gameObject.layer;
                int layer2 = col2.gameObject.layer;

                if (layer1 != layer2)
                    return false;
                //las layers de ambos son iguales
                if (layer1 == Layers.ground)
                {
                    bool col1IsActualGround;
                    bool col2IsActualGround;

                    if (col1.transform.parent == parentOfDestructibleColliders || col1.transform.parent == parentOfIndestructibleColliders)
                        col1IsActualGround = true;
                    else
                        col1IsActualGround = false;

                    if (col2.transform.parent == parentOfDestructibleColliders || col2.transform.parent == parentOfIndestructibleColliders)
                        col2IsActualGround = true;
                    else
                        col2IsActualGround = false;
                    
                    // asi era antes pero me impode usar el tag indestructible en otros colliders como en los tiles para que los malos no puedan ver a traves
                    /*if (col1.gameObject.CompareTag("Indestructible") || col1.gameObject.CompareTag("Destructible"))
                        col1IsActualGround = true;
                    else
                        col1IsActualGround = false;
                    
                    if (col2.gameObject.CompareTag("Indestructible") || col2.gameObject.CompareTag("Destructible"))
                        col2IsActualGround = true;
                    else
                        col2IsActualGround = false;
                    */
                    
                    if (col1IsActualGround != col2IsActualGround) // uno es ground de verdad y el otro no
                        return false;
                    // ambos son ground o ambos no lo son
                    if (col1IsActualGround) // es suelo tierra ( el de col2 tambien )
                        return true; // antes lo tenia como false, pero esto provoca que en un rampa que cambia de collider cell salte cuando no deberia
                    else // algun objeto tiene collider ground , como los interior grids
                        return false;
                }

                if (layer1 == Layers.ladder)
                    return false; // para evitar que se aplique ground inc y pegue salto en la escalera creoyendo que el collider se ha movido pero en realidad es otro
                else
                    return false; // no se que otros objetos puede haber que lleguen hasta aqui , pruebo con false
            }
        }

        if (col1.transform.parent != col2.transform.parent) // son de padres diferentes aun estando en el mismo rigidbody?
            return false;

        // paso a comprobar las diferencias de angulos uno por uno, empiezo por Y por que en la nave es el que realmente cambia, pero podria haber casos en los que el cruce se produce en otra coordenada
        float angle = rot1.y - rot2.y;
        float absAngle = Mathf.Abs(angle);
        float diff90;

        if (absAngle > 0.001)
        {
            diff90 = angle - 90;
            if (diff90 > -0.001f || diff90 < 0.001f
            ) // son colliders complementarios cruzados para que den la impresion de ser la misma superficie al rotar un objeto 3D
                return true;
        }

        angle = rot1.x - rot2.x;
        absAngle = Mathf.Abs(angle);
        if (absAngle < 0.001f) // no hay diferencia de rotacion , deben ser considerados diferentes (plataforma inferior)
        {
            return false;
        }
        else // hay diferencia de rotacion
        {
            diff90 = angle - 90;
            if (diff90 > -0.001f || diff90 < 0.001f) // son colliders complementarios cruzados para que den la impresion de ser la misma superficie al rotar un objeto 3D
                return true;
        }

        angle = rot1.z - rot2.z;
        diff90 = angle - 90;
        if (diff90 > -0.001f || diff90 < 0.001f) // son colliders complementarios cruzados para que den la impresion de ser la misma superficie al rotar un objeto 3D
            return true;
        return false;
    }

    /// <summary>
    /// para identificar level/groundColliders como equivalentes a la hora de decidir si merece la pena saltar a el desde la escalera, en interior grid hay levels(edge) combinados con GroundColliders(box)que deben ser considerados iguales
    /// </summary>
    public static bool CollidersAreEqualFromLadderPerspective(Collider2D col1, Collider2D col2)
    {
        if (col1 == null || col2 == null)
        {
            if (Application.isEditor)
                Debug.LogError(" OJO ALGUN COLLIDER COMPARADO EN PERSPECTIVA ESCALERA ES NULL:" + col1 + " o " + col2);
            return true;
        }
        if (col1 == col2)
            return true;
        if (col1.attachedRigidbody != col2.attachedRigidbody)
            return false;
        if (col1.transform.rotation != col2.transform.rotation)
            return false;
        if (Mathf.Abs(col1.transform.localPosition.y - col2.transform.localPosition.y) > 1)// este me fuerza a hacer los level siempre con el mismo local position y si es preciso cambiar alturas habria que usar el offset del collider
            return false;
        if (Mathf.Abs(col1.bounds.max.y - col2.bounds.max.y) > 1)// esto no funcionaria en levels inclinados OJO
            return false;
        return true;
    }

    public static bool CollidersAreFromTheSameLadder(Collider2D col1, Collider2D col2)
    {
        if (col1 == null || col2 == null)
            return false;
        if (!col1.CompareTag(col2.tag))
            return false;
        if (col1.gameObject.layer != col2.gameObject.layer)
            return false;
        if (col1.transform.parent != col2.transform.parent)
            return false;
        return true;
    }

    public static Collider2D[] cols1 = new Collider2D[32]; // multi purpose collider2d array
    public static Collider2D[] cols2 = new Collider2D[32]; // multi purpose collider2d array
    public static void IgnoreCollisionsWith(Rigidbody2D r1, Rigidbody2D r2)
    {
        int count1 = r1.GetAttachedColliders(cols1);
        int count2 = r2.GetAttachedColliders(cols2);
        for (int i = 0; i < count1; i++)
        {
            for (int k = 0; k < count2; k++)
            {
                Physics2D.IgnoreCollision(cols1[i], cols2[k]);
            }
        }
    }
    
    public static bool RigidBodyIsMovingTooFast(Rigidbody2D rb, float linearSpeedThreshold, float angularSpeedThreshold)
    {
        if (rb == null)
            return false;
        if (rb.velocity.x > linearSpeedThreshold || rb.velocity.x < -linearSpeedThreshold)
            return true;
        if (rb.velocity.y > linearSpeedThreshold || rb.velocity.y < -linearSpeedThreshold)
            return true;
        if (rb.angularVelocity > angularSpeedThreshold || rb.angularVelocity < -angularSpeedThreshold)
            return true;
        return false;
    }

    public static void IgnoreCollisionsWith(Collider2D col, List<Collider2D> cols)
    {
        if (col && cols != null)
        {
            for (int n = 0; n < cols.Count; n++)
            {
                if (cols[n]) 
                    Physics2D.IgnoreCollision(col, cols[n], true);
            }
        }
    }
    public static void IgnoreCollisionsWith(Collider2D col, Collider2D[] cols)
    {
        IgnoreCollisionsWith(col, cols, cols.Length);
    }
    public static void IgnoreCollisionsWith(Collider2D col, Collider2D[] cols, int colCount)
    {
        if (col && cols != null)
        {
            for (int n = 0; n < colCount; n++)
            {
                if (cols[n]) // TODO mi codigo esta mal, en la tabla de colliders de la armada se crean huecos al cambiar de nivel o al destruirse objetos de nivel como transportadores alien, deberia hacer un sistema mejor para evitar esto, pero por ahora, lo dejo asi y voy a ver si limpio la tabla en cada nivel
                    Physics2D.IgnoreCollision(col, cols[n], true);
            }
        }
        else
        {
            if (Constants.isDebugBuild)
            {
                string colName = " Null ";
                string colsLengthString = " 0 ";
                string col0Name = " Null ";

                if (col)
                    colName = col.name;

                if (cols != null)
                {
                    colsLengthString = cols.Length.ToString();
                    if (cols.Length > 0 && cols[0] != null)
                    {
                        col0Name = cols[0].name;
                    }
                }

                Debug.Log("NADIE DEBERIA ENVIAR NULLS, COL=" + colName + " COLS.LENGTH=" + colsLengthString + " COLS[0]=" + col0Name);
            }
        }
    }
    public static void RestoreCollisionsWith(Collider2D col, List<Collider2D> cols)
    {
        if (col && cols != null)
        {
            for (int n = 0; n < cols.Count; n++)
            {
                if (cols[n]) 
                    Physics2D.IgnoreCollision(col, cols[n], false);
            }
        }
    }
    public static void RestoreCollisionsWith(Collider2D col, Collider2D[] cols)
    {
        RestoreCollisionsWith(col, cols, cols.Length);
    }
    public static void RestoreCollisionsWith(Collider2D col, Collider2D[] cols, int colCount)
    {
        if (col && cols != null)
        {
            for (int n = 0; n < colCount; n++)
            {
                if (cols[n]) // TODO mi codigo esta mal, en la tabla de colliders de la armada se crean huecos al cambiar de nivel o al destruirse objetos de nivel como transportadores alien, deberia hacer un sistema mejor para evitar esto, pero por ahora, lo dejo asi y voy a ver si limpio la tabla en cada nivel
                    Physics2D.IgnoreCollision(col, cols[n], false);
            }
        }
        else
        {
            if (Constants.isDebugBuild)
            {
                string colName = " Null ";
                string colsLengthString = " 0 ";
                string col0Name = " Null ";

                if (col)
                    colName = col.name;

                if (cols != null)
                {
                    colsLengthString = cols.Length.ToString();
                    if (cols.Length > 0 && cols[0] != null)
                    {
                        col0Name = cols[0].name;
                    }
                }

                Debug.Log("NADIE DEBERIA ENVIAR NULLS, COL=" + colName + " COLS.LENGTH=" + colsLengthString + " COLS[0]=" + col0Name);
            }
        }
    }
}