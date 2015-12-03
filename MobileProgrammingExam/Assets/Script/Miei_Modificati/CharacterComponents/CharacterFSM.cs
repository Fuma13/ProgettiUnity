using UnityEngine;
using System.Collections;

public class CharacterFSM : MonoBehaviour 
{
    protected void OnEnable()
    {
        m_oGameManager.OnStartEvent += WalkEvent;
        m_oGameManager.OnUnpauseEvent += WalkEvent;
        m_oGameManager.OnRestartEvent += OnRestart;
        m_oGameManager.OnMainMenuEvent += Reset;
        m_oGameManager.OnPauseEvent += Reset;
        m_oGameManager.OnDeadEvent += Dead;

        for(int i = 0; i < m_aoStateEventDispatcher.Length; ++i)
        {
            m_aoStateEventDispatcher[i].OnStateExitEvent += OnStateExit;
        }
    }

    protected void OnDisable()
    {
        m_oGameManager.OnStartEvent -= WalkEvent;
        m_oGameManager.OnUnpauseEvent -= WalkEvent;
        m_oGameManager.OnRestartEvent -= OnRestart;
        m_oGameManager.OnMainMenuEvent -= Reset;
        m_oGameManager.OnPauseEvent -= Reset;
        m_oGameManager.OnDeadEvent -= Dead;

        for (int i = 0; i < m_aoStateEventDispatcher.Length; ++i)
        {
            m_aoStateEventDispatcher[i].OnStateExitEvent -= OnStateExit;
        }
    }

    public void OnStateExit(AnimationState eAnimationState)
    {
        switch(eAnimationState)
        {
            case AnimationState.JUMP:
            case AnimationState.ATTACK:
                m_eAnimationState = AnimationState.WALK;
                break;
            case AnimationState.ATTACK_SPRINT:
                m_eAnimationState = AnimationState.RUN_SPRINT;
                break;
        }
    }

    public bool Walk()
    {
        switch(m_eAnimationState)
        {
            case AnimationState.IDLE:
                SetTrigger(mk_sIdleToWalk);
                m_eAnimationState = AnimationState.WALK;
                return true;
            case AnimationState.RUN_SPRINT:
                SetTrigger(mk_sRunToWalk);
                m_eAnimationState = AnimationState.WALK;
                return true;
        }
        return false;
    }

    public void Reset()
    {
        SetTrigger(mk_sReset);
        m_eAnimationState = AnimationState.IDLE;
    }

    public void Dead()
    {
        SetTrigger(mk_sDead);
        m_eAnimationState = AnimationState.DEAD;
    }

    public void RunSprint()
    {
        SetTrigger(mk_sRunSprint);
        m_eAnimationState = AnimationState.RUN_SPRINT;
    }

    public bool AttackSprint()
    {
        switch(m_eAnimationState)
        {
            case AnimationState.RUN_SPRINT:
                SetTrigger(mk_sRunToAttackS);
                //m_eAnimationState = AnimationState.ATTACK_SPRINT;
                return true;
            case AnimationState.JUMP_SPRINT:
                SetTrigger(mk_sJumpSToAttackS);
                //m_eAnimationState = AnimationState.ATTACK_SPRINT;
                return true;
        }
        return false;
    }

    public bool JumpSprint()
    {
        if (m_eAnimationState == AnimationState.RUN_SPRINT)
        {
            SetTrigger(mk_sRunToJumpS);
            //m_eAnimationState = AnimationState.JUMP_SPRINT;
            return true;
        }
        return false;
    }

    public bool Jump()
    {
        if (m_eAnimationState == AnimationState.WALK)
        {
            SetTrigger(mk_sWalkToJump);
            //m_eAnimationState = AnimationState.JUMP;
            return true;
        }
        return false;
    }

    public bool Attack()
    {
        switch(m_eAnimationState)
        {
            case AnimationState.WALK:
                SetTrigger(mk_sWalkToAttack);
                //m_eAnimationState = AnimationState.ATTACK;
                return true;
            case AnimationState.JUMP:
                SetTrigger(mk_sJumpToAttack);
                //m_eAnimationState = AnimationState.ATTACK;
                return true;
        }

        return false;
    }


    private void WalkEvent()
    {
        Walk();
    }

    private void OnRestart()
    {
        Reset();
        Walk();
    }


    private void SetTrigger(string sCommand)
    {
        m_oAnimator.SetTrigger(sCommand);
    }

    private void SetBoolean(string sCommand, bool bValue)
    {
        m_oAnimator.SetBool(sCommand, bValue);
    }

    private void SetFloat(string sCommand, float fValue)
    {
        m_oAnimator.SetFloat(sCommand, fValue);
    }

    [SerializeField] GameManager m_oGameManager;
    [SerializeField] Animator m_oAnimator;
    [SerializeField] StateEventDispatcher[] m_aoStateEventDispatcher;

    private AnimationState m_eAnimationState;

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
