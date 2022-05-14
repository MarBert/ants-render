using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntsManager : MonoBehaviour
{
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;
    [SerializeField] private float maxAntSpawnNumber;

    public static AntsManager instance;
    public AntController AntPrefab;

    private List<Transform> bases;
    private bool _shouldSpawn;

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
        _shouldSpawn = true;
        //StartCoroutine(RandomSpawn());
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.A)){
            EventManager.instance.OnStartTravel.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.B)){
            EventManager.instance.OnUpdateColor.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.K)){
            StartCoroutine(SpawnGroupDelayed(200,200,.5f));
            //StartCoroutine(TimedSpawn(2000,300,150));
        }
    }

    private IEnumerator SpawnGroupDelayed(int total,int groups,float delay){
        if(groups>total || groups == 0) yield break;
        var iterSpawn = (int)Mathf.Ceil(total / groups);
        for(var i = 0;i<groups;i++){
            SpawnAnts(iterSpawn);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator SpawnLoopWithDelay(int quantity,float delay,int iterations){
        for(var i = 0;i<iterations;i++){
            SpawnAnts(quantity);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator TimedSpawn(int antsToSpawn,float timeToSpawn, int burstsNumber){
        var delay = timeToSpawn / burstsNumber;
        var burstAnts = antsToSpawn / burstsNumber;
        for(var i = 0;i<burstsNumber;i++){
            SpawnAnts(burstAnts);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator RandomSpawn(){
        while(_shouldSpawn){
            var spawnNumber = (int)Random.Range(0,maxAntSpawnNumber);
            SpawnAnts(spawnNumber);
            var delay = Random.Range(minSpawnDelay,maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    public void SpawnAnts(int quantity){
        for(var i = 0; i<quantity;i++){
            var b = GetNewBase();
            var scrambler = new Vector3(Random.Range(-0.3f,0.3f),Random.Range(-0.3f,0.3f),0);
            var scrambledPosition = b.position + scrambler;
            var a = Instantiate(AntPrefab,scrambledPosition,Quaternion.Euler(0, 0, Random.Range(0,360)));
            a.SetUp(b);
        }
    }

    public Transform GetNewBase(){
        var i = Random.Range(0,bases.Count);
        return bases[i];
    }
}
