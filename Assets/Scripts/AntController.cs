using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

enum AntStatus {
    idle,
    walk,
    backtobase,
    travel,
    groupUp
};

public class AntController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxDegreeTurn;
    [SerializeField] private float aiUpdateTimerMin;
    [SerializeField] private float aiUpdateTimerMax;
    [SerializeField] private float maxDistanceFromBase;
    [SerializeField][Range(0, 100)] private float idleProbability;
    [SerializeField][Range(0, 100)] private float travelProbability;
    [SerializeField] private Color endColor;

    private Animator _antAnimator;
    private AntStatus _currentStatus;
    private bool _isAlive;
    private bool _shouldTravel;
    private Transform _antBase;
    private Vector3 _target;

    private SpriteRenderer _antRenderer;

    private void Start(){
        transform.localScale = Vector3.zero;
        _antRenderer = GetComponentInChildren<SpriteRenderer>();
        EventManager.instance.OnStartTravel.AddListener(()=>{
            Debug.Log("TRAVEL!");
            var check = Random.Range(0,100);
            if(check <= travelProbability)
                _shouldTravel = true;
        });
        EventManager.instance.OnUpdateColor.AddListener(()=>{
            Debug.Log("COLOR!");
            var check = Random.Range(0,100);
            if(check <= travelProbability)
                ChangeColor(endColor);
        });
    }

    public void SetUp(Transform antBase){
        _antBase = antBase;
        _antAnimator = GetComponent<Animator>();
        _isAlive = true;
        _currentStatus = AntStatus.walk;
        StartCoroutine(AiUpdate());
        transform.DOScale(Vector3.one,.5f);
    }

    private void Update(){
        switch (_currentStatus){
            case AntStatus.backtobase:
            case AntStatus.travel:
            case AntStatus.groupUp:
            case AntStatus.walk:
                transform.position += transform.up * Time.deltaTime * movementSpeed;
                break;
            default:
                break;
        }
    }

    private void ChangeColor(Color targetColor){
        _antRenderer.DOColor(targetColor,.5f);
    }

    private IEnumerator AiUpdate(){
        var aiUpdateTimer = Random.Range(aiUpdateTimerMin,aiUpdateTimerMax);
        while(_isAlive){
            if(_shouldTravel){
                _currentStatus = AntStatus.travel;
                _shouldTravel = false;
            }
            else{
                var dist = Vector3.Distance(transform.position,_antBase.position);
                if(dist > maxDistanceFromBase)
                    _currentStatus = AntStatus.backtobase;
                else{
                    var check = Random.Range(0,100);
                    _currentStatus = check <= idleProbability ? AntStatus.idle : AntStatus.walk;
                }
            }
            switch (_currentStatus){
                case AntStatus.idle:
                    _antAnimator.SetBool("Walking",false);
                    break;
                case AntStatus.walk:
                    _antAnimator.SetBool("Walking",true);
                    var zRot = Random.Range(-maxDegreeTurn,maxDegreeTurn);
                    var baseRot = transform.rotation.eulerAngles;
                    var targetRot = new Vector3(baseRot.x, baseRot.y, baseRot.z + zRot);
                    transform.DORotate(targetRot,rotationSpeed);
                    break;
                case AntStatus.backtobase:
                    if(_antBase!=null){
                        _antAnimator.SetBool("Walking",true);
                        TurnToObject(_antBase);
                    }
                    break;
                case AntStatus.travel:
                    _antBase = AntsManager.instance.GetNewBase();
                    _antAnimator.SetBool("Walking",true);
                    TurnToObject(_antBase);
                    break;
                case AntStatus.groupUp:
                    _antAnimator.SetBool("Walking",true);
                    TurnToObject(_target);
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(aiUpdateTimer);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        var tag = other.tag;
        switch(tag){
            case "Avoid" :
                var baseRot = transform.rotation.eulerAngles;
                var targetRot = new Vector3(baseRot.x, baseRot.y, baseRot.z + 180);
                transform.DORotate(targetRot,rotationSpeed);
            break;
            case "Regroup" :
                _currentStatus = AntStatus.groupUp;
                _target = RandomPointInBounds(other.bounds);
                _antBase = other.transform;
            break;
            case "Recolor" :
            break;
        }
    }

    private void TurnToObject(Transform target){
        var turnDir = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(turnDir, transform.up);
        var baseRot = transform.rotation.eulerAngles;
        var targetRot = new Vector3(baseRot.x, baseRot.y, baseRot.z + angle);
        transform.DORotate(targetRot,rotationSpeed);
    }

    private void TurnToObject(Vector3 target){
        var turnDir = (target - transform.position).normalized;
        float angle = Vector3.Angle(turnDir, transform.up);
        var baseRot = transform.rotation.eulerAngles;
        var targetRot = new Vector3(baseRot.x, baseRot.y, baseRot.z + angle);
        transform.DORotate(targetRot,rotationSpeed);
    }

    public Vector3 RandomPointInBounds(Bounds bounds) {
    return new Vector3(
        Random.Range(bounds.min.x, bounds.max.x),
        Random.Range(bounds.min.y, bounds.max.y),
        Random.Range(bounds.min.z, bounds.max.z)
    );
}
}
