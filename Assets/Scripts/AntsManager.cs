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
    private List<Transform> groups;
    private bool _shouldSpawn;
    private bool _spawnRed;

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
        groups = new List<Transform>();
        foreach(var g in GameObject.FindGameObjectsWithTag("Avoid")){
            groups.Add(g.transform);
        }
    }

    private void Start(){
        _shouldSpawn = true;
        StartCoroutine(UpdateColors());
        //StartCoroutine(RandomSpawn());
    }

    private IEnumerator UpdateColors(){
        yield return new WaitForSeconds(720);
        var delay = (15*60 - 720) / 10;
        var prob = 8;
        for(var i = 0;i<10;i++){
            EventManager.instance.OnUpdateColor.Invoke(prob);
            prob += 8;
            yield return new WaitForSeconds(delay);
        }
        _spawnRed = true;
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.A)){
            EventManager.instance.OnStartTravel.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.B)){
            EventManager.instance.OnUpdateColor.Invoke(1);
        }
        if(Input.GetKeyDown(KeyCode.K)){
            StartCoroutine(SpawnGroupDelayed(200,200,.5f));
            //StartCoroutine(TimedSpawn(2000,300,150));
        }
    }

    public void UpdateColor(int probability){
        EventManager.instance.OnUpdateColor.Invoke(probability);
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
            a.SetUp(b,_spawnRed);
        }
    }
    
    public void SpawnAntsWithGroup(int quantity, int groups, float delay){
        StartCoroutine(SpawnGroupDelayed(quantity,groups,delay));
    }

    public Transform GetNewBase(bool isBase=true){
        var sel = 0;
        if(!isBase)
            sel = Random.Range(0,2);
        if(sel==0)
        {
            var i = Random.Range(0,bases.Count);
            return bases[i];
        }
        var j = Random.Range(0,groups.Count);
        return groups[j];
    }
}
