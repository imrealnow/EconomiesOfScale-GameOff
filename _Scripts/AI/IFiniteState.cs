public interface IFiniteState
{
    /// <summary>
    /// Initialise reference to the finite state machine, and initialise any other key information.
    /// </summary>
    /// <param name="fsm">The parent finite state machine this finite state is being run from.</param>
    /// <returns>Returns an initialised copy of the state.</returns>
    public IFiniteState Initialise(FiniteStateMachine fsm);

    /// <summary>
    /// Get the state's preferred update frequency
    /// </summary>
    /// <returns>The preferred frequency to call the update function</returns>
    public float GetUpdateFrequency();

    /// <summary>
    /// Called when this state becomes the current state.
    /// </summary>
    public void OnEnter();

    /// <summary>
    /// Called before the state is changed to the next state.
    /// </summary>
    public void OnLeave();

    /// <summary>
    /// Gives the option to pass arguments into the state after it has been initialised.
    /// </summary>
    /// <param name="args">The arguments for the state to receive and do something with.</param>
    public void IngestArguments(params object[] args);

    /// <summary>
    /// Called on every FixedUpdate loop from the FiniteStateMachine.
    /// </summary>
    /// <returns>The next state to change to (if it is not the same as the current state)</returns>
    public IFiniteState Update();

}