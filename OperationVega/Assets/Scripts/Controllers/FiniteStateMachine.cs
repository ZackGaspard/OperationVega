
namespace Assets.Scripts.Controllers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The finite state machine class.
    /// </summary>
    /// <typeparam name="T">
    /// The T is for a generic value
    /// </typeparam>
    public class FiniteStateMachine<T>
    {
        /// <summary>
        /// The list of states in the machine.
        /// </summary>
        private readonly List<State> theStates;

        /// <summary>
        /// The transition dictionary.
        /// Holds all states and the transitions.
        /// </summary>
        private Dictionary<State, List<Transition>> transitionTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteStateMachine{T}"/> class. 
        /// </summary>
        public FiniteStateMachine()
        {
            this.transitionTable = new Dictionary<State, List<Transition>>();
            this.theStates = new List<State>();
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// The create state function.
        /// The function used to create states.
        /// <para></para>
        /// <remarks><paramref name="theState"></paramref> -The state to create.</remarks>
        /// <para></para>
        /// <remarks><paramref name="del"></paramref> -The delegate to give to the state.</remarks>
        /// </summary>
        public void CreateState(T theState, Delegate del)
        {
            // Create a new state with the passed in variables
            State newState = new State(theState.ToString(), del);

            // If the list and dictionary does not contain the state
            if (!this.theStates.Contains(newState) & !this.transitionTable.ContainsKey(newState))
            { // Add the state to the list
                this.theStates.Add(newState);
              // Add the state to the dictionary
                this.transitionTable.Add(newState, new List<Transition>());
            }

            // Set the current state to the first state in the list
            this.CurrentState = this.theStates[0];
        }

        /// <summary>
        /// The Add Transition function.
        /// The function used to add transitions to a state.
        /// <para></para>
        /// <remarks><paramref name="from"></paramref> -The state to transition from.</remarks>
        /// <para></para>
        /// <remarks><paramref name="to"></paramref> -The state to transition to.</remarks>
        /// <para></para>
        /// <remarks><paramref name="input"></paramref> -The string used to change the state.</remarks> 
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddTransition(T from, T to, string input)
        {
            // Create 2 variable to store information
            State fromstate = new State();
            State destination = new State();

            // Loop through the states list
            foreach (State s in this.theStates)
            {
                // If the current state name is equal to the passed in from variable as a string
                if (s.Statename == from.ToString())
                {// Set the from state
                    fromstate = s;
                }

                // If the current state name is equal to the passed in to variable as a string
                if (s.Statename == to.ToString())
                {// Set the destination state
                    destination = s;
                }
            }

            // If the dictionary contains the key
            if (this.transitionTable.ContainsKey(fromstate))
            {// Create a new transition 
                Transition transition = new Transition(input, destination);

                // Add the transition to the key
                this.transitionTable[fromstate].Add(transition);
            }
            else
            {// If it doesnt have the key then return false
                return false;
            }

            // Return true the transition was successfully added
            return true;
        }

        /// <summary>
        /// The feed function.
        /// Activates the transition of states and calls the functions within the destination state.
        /// <para></para>
        /// <remarks><paramref name="token"></paramref> -The token that is used to call the transition.</remarks>
        /// <para></para>
        /// <remarks><paramref name="a_params"></paramref> -The a_parameters are the parameters passed in. This can also be empty.</remarks>
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Feed(string token, params object[] a_params)
        {
            // Foreach transition in the current state
            foreach (Transition t in this.transitionTable[this.CurrentState])
            {// If the input is equal to the passed in token variable
                if (t.Input == token)
                {// Set the currentstate to the destination state
                    this.CurrentState = t.DestinationState;

                    // Invoke all the functions in the state
                    this.CurrentState.InvokeFunctions(a_params);

                    // Return true
                    return true;
                }
            }

            // Return false if the token is not valid or the input is not valid
            return false;
        }
    }

    /// <summary>
    /// The state class.
    /// </summary>
    public class State
    {
        /// <summary>
        /// The name of the state.
        /// </summary>
        public string Statename;

        /// <summary>
        /// The delegate for the state.
        /// </summary>
        private readonly Delegate del;

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        public State()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// <para></para>
        /// <remarks><paramref name="stateName"></paramref> -The name given to this state.</remarks>
        /// <para></para>
        /// <remarks><paramref name="del"></paramref> -The delegate that will hold the functions for this state.</remarks>
        /// </summary>
        public State(string stateName, Delegate del)
        {
            this.Statename = stateName;
            this.del = del;
        }

        /// <summary>
        /// The invoke functions function.
        /// This function will invoke all functions in the state
        /// <para></para>
        /// <remarks><paramref name="a_params"></paramref> -The a_parameters variable is optional parameters passed in. Can also be empty.</remarks>
        /// </summary>
        public void InvokeFunctions(params object[] a_params)
        {
            // If the del is not equal to null
            if (this.del != null)
            {// Invoke all the methods
                this.del.DynamicInvoke(a_params);
            }
        }
    }

    /// <summary>
    /// The transition class.
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transition"/> class.
        /// <para></para>
        /// <remarks><paramref name="theToken"></paramref> -The token used to transition from a state to another.</remarks>
        /// <para></para>
        /// <remarks><paramref name="theStateToGoTo"></paramref> -The state to transition to.</remarks>
        /// </summary>
        public Transition(string theToken, State theStateToGoTo)
        {
            this.Input = theToken;
            this.DestinationState = theStateToGoTo;
        }

        /// <summary>
        /// Gets the input used to transition to next state.
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// Gets the destination state to transition to.
        /// </summary>
        public State DestinationState { get; private set; }
    }
}
