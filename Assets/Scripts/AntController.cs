using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

enum AntStatus {
    idle,
    walk,
    backtobase,
    travel
};

public class AntController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxDegreeTurn;
    [SerializeField] private float aiUpdateTimer;
    [SerializeField] private float maxDistanceFromBase;
    [SerializeField][Range(0, 100)] private float idleProbability;

    private Animator _antAnimator;
    private AntStatus _currentStatus;
    private bool _isAlive;

    private bool _shouldTravel;

    private Transform _antBase;

    public void SetUp(Transform antBase){
        _antBase = antBase;
        _antAnimator = GetComponent<Animator>();
        _isAlive = true;
        _currentStatus = AntStatus.walk;
        StartCoroutine(AiUpdate());
    }

    private void Update(){
        switch (_currentStatus){
            case AntStatus.backtobase:
            case AntStatus.travel:
            case AntStatus.walk:
                transform.position += transform.up * Time.deltaTime * movementSpeed;
                break;
            default:
                break;
        }
    }

    private IEnumerator AiUpdate(){
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
                default:
                    break;
            }
            Debug.Log(_currentStatus);
            yield return new WaitForSeconds(aiUpdateTimer);
        }
    }

    private void TurnToObject(Transform target){
        var turnDir = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(turnDir, transform.up);
        var baseRot = transform.rotation.eulerAngles;
        var targetRot = new Vector3(baseRot.x, baseRot.y, baseRot.z + angle);
        transform.DORotate(targetRot,rotationSpeed);
    }
}
