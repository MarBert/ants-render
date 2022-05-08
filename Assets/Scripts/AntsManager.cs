using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntsManager : MonoBehaviour
{
    public static AntsManager instance;
    public AntController AntPrefab;

    private List<Transform> bases;

    private void Awake(){
        if(instance!=null){
            Destroy(gameObject);
            return;
        }

        instance = this;
        bases = new List<Transform>();
        for(var i = 0; i<transform.childCount; i++){
            bases.Add(transform.GetChild(i));
        }
    }

    private void Start(){
        StartCoroutine(SpawnWithDelay(15,2,5));
    }

    private IEnumerator SpawnWithDelay(int quantity,float delay,int iterations){
        for(var i = 0;i<iterations;i++){
            SpawnAnts(quantity);
            yield return new WaitForSeconds(delay);
        }
    }

    public void SpawnAnts(int quantity){
        for(var i = 0; i<quantity;i++){
            var b = GetNewBase();
            var a = Instantiate(AntPrefab,b.position,Quaternion.Euler(0, 0, Random.Range(0,360)));
            a.SetUp(b);
        }
    }

    public Transform GetNewBase(){
        var i = Random.Range(0,bases.Count);
        return bases[i];
    }
}
