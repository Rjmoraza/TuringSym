using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Machine
{
    #region Serializable Classes
    [Serializable]
    public class TMachine
    {
        public ulong id;
        public MachineOwner owner;
        public MachineOwner[] collaborators;
        public string description;
        public State[] states;
    }

    [Serializable]
    public class MachineOwner
    {
        public ulong id;

        public MachineOwner(ulong id)
        {
            this.id = id;
        }
    }

    [Serializable]
    public class State
    {
        public string name;
        public bool initialState;
        public Transition[] incomingTransitions;
        public Transition[] exitTransitions;

        // TODO store (x,y) in State
        public int x; // Relative position X in the canvas
        public int y; // Relative position Y in the canvas
        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsFinalState()
        {
            return exitTransitions == null || exitTransitions.Length == 0;
        }
    }

    [Serializable]
    public class StateReference
    {
        public string name;

        public StateReference(string stateName)
        {
            name = stateName;
        }
    }

    [Serializable]
    public class Transition
    {
        private State _originState;
        private State _targetState;

        public StateReference originState;
        public StateReference targetState;
        public string readValue;
        public string writeValue;
        public string moveValue;

        public void SetOriginState(State state)
        {
            _originState = state;
            originState = new StateReference(state.name);
        }

        public void SetTargetState(State state)
        {
            _targetState = state;
            targetState = new StateReference(state.name);
        }

        public State GetOriginState()
        {
            return _originState;
        }

        public State GetTargetState()
        {
            return _targetState;
        }

        public void UpdateStateReferences()
        {
            originState = new StateReference(_originState.name);
            targetState = new StateReference(_targetState.name);
        }
    }
    #endregion

    public TMachine tMachine;
    public string input;
    public string blank;

    private Dictionary<string, State> states;
    private List<Transition> transitions;
    private List<MachineOwner> collaborators;

    #region Machine Initialization and initial values
    public Machine()
    {
        states = new Dictionary<string, State>();
        transitions = new List<Transition>();
        collaborators = new List<MachineOwner>();
        tMachine = new TMachine();
    }

    public void SetInput(string input)
    {
        this.input = input;
    }

    public void SetBlank(string blank)
    {
        this.blank = blank;
    }

    // TODO Get the UID from the session cookie
    // Read https://forum.unity.com/threads/accessing-session-cookie-with-webgl-build.333437/ and https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
    public void SetOwnerID(ulong id)
    {
        tMachine.owner = new MachineOwner(id);
    }

    // Machine ID should not be set if this is a new machine
    public void SetID(ulong id)
    {
        tMachine.id = id;
    }

    public void SetDescription(string description)
    {
        tMachine.description = description;
    }

    // Search for collaborator ID in API
    public void AddCollaborator(ulong uid)
    {
        collaborators.Add(new MachineOwner(uid));
    }
    #endregion

    #region Machine edition
    public State AddState(string stateName)
    {
        State state = new State();
        state.name = stateName;
        states.Add(stateName, state);
        return state;
    }

    public void RenameState(string oldName, string newName)
    {
        State state = states[oldName];
        state.name = newName;
        foreach(Transition t in transitions)
        {
            if (t.originState.name == oldName || t.targetState.name == oldName)
            {
                t.UpdateStateReferences();
            }
        }
        states.Remove(oldName);
        states.Add(newName, state);
    }

    public State[] GetStates()
    {
        State[] statesArray = new State[states.Count];
        states.Values.CopyTo(statesArray, 0);
        return statesArray;
    }

    public Transition[] GetTransitions()
    {
        return transitions.ToArray();
    }

    #endregion

    #region Machine Serialization and Deserialization
    public static Machine FromJSON(String json)
    {
        Machine machine = JsonUtility.FromJson<Machine>(json);
        if(machine != null)
        {
            machine.Deserialize();
        }
        return machine;
    }

    public string ToJson()
    {
        Serialize();
        return JsonUtility.ToJson(this, true);
    }

    private void Deserialize()
    {
        states = new Dictionary<string, State>();
        transitions = new List<Transition>();
        collaborators = new List<MachineOwner>();

        // First get all states in a dictionary
        foreach(State s in tMachine.states)
        {
            states.Add(s.name, s);
        }

        // Now map all incoming transitions to their respective origin state
        foreach(State s in tMachine.states)
        {
            foreach(Transition t in s.incomingTransitions)
            {
                t.SetOriginState(states[t.originState.name]);
                transitions.Add(t);
            }
        }

        // Finally, map all exit transitions to their respective target state
        foreach (State s in tMachine.states)
        {
            foreach (Transition e in s.exitTransitions)
            {
                // At this point every transition should exist in the transition list
                // Find each transition and map its target state accordingly
                Transition t = transitions.Find(x => 
                    x.originState.name == s.name &&
                    x.readValue == e.readValue &&
                    x.writeValue == e.writeValue &&
                    x.moveValue == e.moveValue
                );

                if(t != null)
                {
                    t.SetTargetState(states[e.targetState.name]);
                }
                else
                {
                    Debug.Log("Could not find matching transition for\n" + JsonUtility.ToJson(e, true));
                }
            }
        }

        foreach(MachineOwner mo in tMachine.collaborators)
        {
            collaborators.Add(mo);
        }
    }

    private void Serialize()
    {
        foreach(State s in states.Values)
        {
            List<Transition> incomingTransitions = transitions.FindAll(x => x.targetState.name == s.name);
            s.incomingTransitions = incomingTransitions.ToArray();

            List<Transition> exitTransitions = transitions.FindAll(x => x.originState.name == s.name);
            s.exitTransitions = exitTransitions.ToArray();
        }

        tMachine.states = new State[states.Values.Count];
        states.Values.CopyTo(tMachine.states, 0);

        tMachine.collaborators = collaborators.ToArray();
    }
    #endregion
}




