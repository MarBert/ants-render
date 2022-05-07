using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

enum AntStatus {
    idle,
    walk,
    defend,
    travel
};

public class AntController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxDegreeTurn;
    [SerializeField] private float aiUpdateTimer;

    private Animator _antAnimator;
    private AntStatus _currentStatus;
    private bool _isAlive;

    private void Start(){
        _antAnimator = GetComponent<Animator>();
        _isAlive = true;
        _currentStatus = AntStatus.walk;
        StartCoroutine(AiUpdate());
    }

    private void Update(){
        switch (_currentStatus){
            case AntStatus.walk:
                transform.position += transform.up * Time.deltaTime * movementSpeed;
                break;
            default:
                break;
        }
    }

    private IEnumerator AiUpdate(){
        while(_isAlive){
            //change status
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
                default:
                    break;
            }
            yield return new WaitForSeconds(aiUpdateTimer);
        }
    }


}
