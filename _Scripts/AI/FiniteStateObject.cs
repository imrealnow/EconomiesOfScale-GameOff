using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiniteStateObject : ScriptableObject, IFiniteState
{
    protected FiniteStateMachine parentMachine = null;

    public virtual IFiniteState Initialise(FiniteStateMachine fsm)
    {
        FiniteStateObject copy = Instantiate(this);
        copy.parentMachine = fsm;
        return copy;
    }

    public virtual void IngestArguments(params object[] arguments)
    {
        throw new System.NotImplementedException();
    }

    public abstract void OnEnter();
    public abstract void OnLeave();
    public abstract IFiniteState Update();
    public abstract float GetUpdateFrequency();
}
