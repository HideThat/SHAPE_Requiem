using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPC_State
{
    State0,
    State1,
    State2,
    State3,
    State4,
    State5,
    State6,
    State7,
    State8,
    State9,
}

public class NPC : MonoBehaviour
{
    public string NPC_name;
    public Animator animator;
    public List<string> animationName = new List<string>();
    public NPC_State state;

    void Start()
    {
        
    }

    
    void Update()
    {
        switch (state)
        {
            case NPC_State.State0:
                break;
            case NPC_State.State1:
                break;
            case NPC_State.State2:
                break;
            case NPC_State.State3:
                break;
            case NPC_State.State4:
                break;
            case NPC_State.State5:
                break;
            case NPC_State.State6:
                break;
            case NPC_State.State7:
                break;
            case NPC_State.State8:
                break;
            case NPC_State.State9:
                break;
            default:
                break;
        }
    }

    void AnimationPlay(NPC_State _state)
    {

    }
}
