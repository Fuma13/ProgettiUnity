using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

//Character FSM: manage the target state and it's sync with the animator FSM
//When other component ask for new state it check if it is a possible transition and made it
public class CharacterFSM : MonoBehaviour 
{
    public Action<AnimationState> OnStateExitEvent;

    //Search all states of animator that dispatc events in exit state and register itself to the script
    protected void Awake()
    {
        StateEventDispatcher[] aoStateEventDispatcher = m_oAnimator.GetBehaviours<StateEventDispatcher>();

        Assert.IsNotNull(aoStateEventDispatcher, "No state event dispatcher found on the Animator!");

        for (int i = 0; i < aoStateEventDispatcher.Length; ++i)
        {
            aoStateEventDispatcher[i].CharacterFSMController = this;
        }
    }
    protected void OnEnable()
    {
        m_oGameManager.OnStartEvent += WalkEvent;
        m_oGameManager.OnUnpauseEvent += OnUnPauseEvent;
        m_oGameManager.OnRestartEvent += OnRestart;
        m_oGameManager.OnMainMenuEvent += Reset;
        m_oGameManager.OnPauseEvent += OnPauseEvent;
        m_oGameManager.OnDeadEvent += Dead;
    }

    protected void OnDisable()
    {
        m_oGameManager.OnStartEvent -= WalkEvent;
        m_oGameManager.OnUnpauseEvent -= WalkEvent;
        m_oGameManager.OnRestartEvent -= OnRestart;
        m_oGameManager.OnMainMenuEvent -= Reset;
        m_oGameManager.OnPauseEvent -= Reset;
        m_oGameManager.OnDeadEvent -= Dead;
    }

    //Event lauched from animator, it's useful to notify the end of and anumation that is palyed once and has the exit time
    public void OnStateExit(AnimationState eAnimationState)
    {
        Debug.Log("OnStateExit: " + eAnimationState.ToString());
        switch(eAnimationState)
        {
            case AnimationState.JUMP:
            case AnimationState.ATTACK:
                m_eAnimationState = AnimationState.WALK;
                break;
            case AnimationState.ATTACK_SPRINT:
            case AnimationState.JUMP_SPRINT:
                m_eAnimationState = AnimationState.RUN_SPRINT;
                break;
        }
        if(OnStateExitEvent != null)
        {
            OnStateExitEvent(eAnimationState);
        }
    }
    //Check if it's possible to start walk state and set the right trigger for animator
    public bool Walk()
    {
        switch(m_eAnimationState)
        {
            case AnimationState.IDLE:
                SetTrigger(mk_sIdleToWalk);
                m_eAnimationState = AnimationState.WALK;
                return true;
            case AnimationState.RUN_SPRINT:
            case AnimationState.JUMP_SPRINT:
            case AnimationState.ATTACK_SPRINT:
                SetTrigger(mk_sRunToWalk);
                m_eAnimationState = AnimationState.WALK;
                return true;
        }
        return false;
    }

    //Reset the the animator state to idle and set the right trigger for animator 
    public void Reset()
    {
        SetTrigger(mk_sReset);
        m_eAnimationState = AnimationState.IDLE;
    }

    //Set directly dead state, this state is reacheble from any state so it's useless the coherence check and set the right trigger for animator
    public void Dead()
    {
        SetTrigger(mk_sDead);
        m_eAnimationState = AnimationState.DEAD;
    }

    //Check if it can start the sprint and set the right trigger for animator
    public bool RunSprint()
    {
        if (m_eAnimationState != AnimationState.DEAD || m_eAnimationState != AnimationState.IDLE)
        {
            SetTrigger(mk_sRunSprint);
            m_eAnimationState = AnimationState.RUN_SPRINT;
            return true;
        }
        return false;
    }

    //Check if it can attack during the sprint and set the right trigger for animator
    public bool AttackSprint()
    {
        switch(m_eAnimationState)
        {
            case AnimationState.RUN_SPRINT:
                SetTrigger(mk_sRunToAttackS);
                m_eAnimationState = AnimationState.ATTACK_SPRINT;
                return true;
            case AnimationState.JUMP_SPRINT:
                SetTrigger(mk_sJumpSToAttackS);
                m_eAnimationState = AnimationState.ATTACK_SPRINT;
                return true;
        }
        return false;
    }

    //Check if it can jump during the sprint and set the right trigger for animator
    public bool JumpSprint()
    {
        if (m_eAnimationState == AnimationState.RUN_SPRINT)
        {
            SetTrigger(mk_sRunToJumpS);
            m_eAnimationState = AnimationState.JUMP_SPRINT;
            return true;
        }
        return false;
    }

    //Check if it can jump during walk and set the right trigger for animator
    public bool Jump()
    {
        if (m_eAnimationState == AnimationState.WALK)
        {
            SetTrigger(mk_sWalkToJump);
            m_eAnimationState = AnimationState.JUMP;
            return true;
        }
        return false;
    }

    //Check if it can attack during walk and set the right trigger for animator
    public bool Attack()
    {
        switch(m_eAnimationState)
        {
            case AnimationState.WALK:
                SetTrigger(mk_sWalkToAttack);
                m_eAnimationState = AnimationState.ATTACK;
                return true;
            case AnimationState.JUMP:
                SetTrigger(mk_sJumpToAttack);
                m_eAnimationState = AnimationState.ATTACK;
                return true;
        }

        return false;
    }

    //Event from the game manager that start to walk
    private void WalkEvent()
    {
        Walk();
    }

    //Event from the game manager that reset the component and restart to walk
    private void OnRestart()
    {
        Reset();
        Walk();
    }

    private void OnPauseEvent()
    {
        m_ePreviousAnimationState = m_eAnimationState;
        Reset();
    }

    private void OnUnPauseEvent()
    {
        switch (m_ePreviousAnimationState)
        {
            case AnimationState.RUN_SPRINT:
            case AnimationState.JUMP_SPRINT:
            case AnimationState.ATTACK_SPRINT:
                RunSprint();
                break;
            case AnimationState.WALK:
            case AnimationState.JUMP:
            case AnimationState.ATTACK:
                Walk();
                break;
        }
    }


    private void SetTrigger(string sCommand)
    {
        m_oAnimator.SetTrigger(sCommand);
    }

    public AnimationState CurrentState
    {
        get { return m_eAnimationState; }
    }

    [SerializeField] GameManager m_oGameManager;
    [SerializeField] Animator m_oAnimator;

    private AnimationState m_eAnimationState;
    private AnimationState m_ePreviousAnimationState;

    //String label corrisponding to animator triggers
    private const string mk_sReset = "Reset";
    private const string mk_sDead = "Dead";
    private const string mk_sRunSprint = "RunSprint";
    private const string mk_sIdleToWalk = "IdleToWalk";
    private const string mk_sWalkToIdle = "WalkToIdle";
    private const string mk_sWalkToJump = "WalkToJump";
    private const string mk_sWalkToAttack = "WalkToAttack";
    private const string mk_sJumpToAttack = "JumpToAttack";
    private const string mk_sRunToAttackS = "RunToAttackS";
    private const string mk_sRunToJumpS = "RunToJumpS";
    private const string mk_sJumpSToAttackS = "JumpSToAttackS";
    private const string mk_sRunToWalk = "RunToWalk";

    public enum AnimationState
    {
        IDLE = 0,
        WALK,
        JUMP,
        ATTACK,
        RUN_SPRINT,
        ATTACK_SPRINT,
        DEAD,
        JUMP_SPRINT
    }
}
