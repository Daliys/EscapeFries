using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class AIContorller : MonoBehaviour
{
    public SplineComputer splineComputer;
    public SplineFollower follower;
    public float offset;
    
    private int currentIndex = 1;
    SplineSample[] path;
    private List<Vector3> ourProjection;

    private Vector3 moveTo;
    void Start()
    {
        SampleCollection pathPointPosition = new SampleCollection();
        splineComputer.GetSamples(pathPointPosition);
        path = pathPointPosition.samples;
        ourProjection = new List<Vector3>();

        Vector3 first = path[0].position;
        first.x += offset;
        
        ourProjection.Add(first);
        moveTo = path[currentIndex].position;
        moveTo.x += offset;
        moveTo.y = 1.00000f;
    }
    
    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, moveTo, Time.deltaTime * 3);
       if(Input.GetKey(KeyCode.W)) transform.Translate(Vector3.right*Time.deltaTime);
 

        if (Vector3.Distance(transform.position, moveTo) < 1)
        {
            currentIndex++;

            float angle = Vector3.Angle(path[currentIndex].position - path[currentIndex-1].position, path[currentIndex].position - path[currentIndex+1].position);
            print("__________Index " + currentIndex + "   Angle: " + angle);
            angle = Mathf.Deg2Rad * angle;
            moveTo = path[currentIndex].position;
            Vector3 normal = (path[currentIndex].position - path[currentIndex-1].position);
            Vector3 nextNormal = (path[currentIndex +1 ].position - path[currentIndex].position);
            print("Norm1 " + normal.normalized + "  next normal " + nextNormal.normalized);
            print("goal : " +  moveTo);
            
            // это смещение бота на offset от центра пути по Ох и Оz
            moveTo.x += offset * Mathf.Sign(normal.z) * Mathf.Abs(normal.normalized.z);
            moveTo.z -= offset * Mathf.Sign(normal.x) * Mathf.Abs(normal.normalized.x);
           
            print("Sin: " + Mathf.Sin(angle) );
            print("between " + moveTo);
            
            // это смещение на поворотах 
            moveTo.x += Mathf.Sin(angle) * offset * Mathf.Sign(nextNormal.normalized.z) * Mathf.Abs(nextNormal.normalized.z);//* (normal.normalized.z == 0 ? 1 : normal.normalized.x);
            moveTo.z -= Mathf.Sin(angle) * offset * Mathf.Sign(nextNormal.normalized.x) * Mathf.Abs(nextNormal.normalized.x);;
       
          
            
            var a = Mathf.Sin(angle);
            var b = offset;
            var c = Mathf.Sign(nextNormal.x);
            print("after " + moveTo);
            moveTo.y = 1.05f;
          //  if (Mathf.Sin(angle) == 1) currentIndex++;
        }

    }
}
