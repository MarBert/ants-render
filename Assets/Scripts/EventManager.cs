using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    private void Awake(){
        if(instance!=null){
            Destroy(gameObject);
            return;
        }

        instance = this;
        OnStartTravel = new UnityEvent();
        OnUpdateColor = new UnityEvent<int>();
    }

    public UnityEvent OnStartTravel;
    public UnityEvent<int> OnUpdateColor;
}
