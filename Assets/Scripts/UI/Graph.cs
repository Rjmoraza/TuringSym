using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TuringSym.UI
{
    public class Graph : MonoBehaviour
    {
        public NodeForm nodeForm;
        public GameObject statePrefab;
        public GameObject transitionPrefab;

        RectTransform rectTransform;
        float clickTimer;
        Camera mainCamera;
        Machine machine;
        List<State> states;
        List<Transition> transitions;

        // Start is called before the first frame update
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
            states = new List<State>();
            transitions = new List<Transition>();

            // TODO TESTING
            // Load the test machine add1.json
            TextAsset json = Resources.Load<TextAsset>("add1");
            machine = Machine.FromJSON(json.text);
            Machine.State[] stateList = machine.GetStates();
            foreach(Machine.State s in stateList)
            {
                RenderState(s);
            }
            Machine.Transition[] transitionList = machine.GetTransitions();
            foreach (Machine.Transition t in transitionList)
            {
                RenderTransition(t);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(clickTimer > 0)
            {
                clickTimer -= Time.deltaTime;
            }
        }

        public void OnPointerClick()
        {
            // Is this a double click?
            print("Click");
            if(clickTimer > 0)
            {
                Vector2 nodePosition = Input.mousePosition;
                nodeForm.ShowForm("New State", (string name) => { 
                    if(machine != null)
                    {
                        AddState(name, nodePosition);
                    }
                });
            }
            else
            {
                clickTimer = 0.3f;
            }
        }

        public void CreateNewState()
        {
            nodeForm.ShowForm("New State", (string name) => {
                if (machine != null)
                {
                    AddState(name, new Vector2(Random.Range(-400, 400), Random.Range(-200, 200)));
                }
            });
        }

        /// <summary>
        /// Adds a new state to the machine
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        public void AddState(string name, Vector2 position)
        {
            Machine.State state = machine.AddState(name);
            GameObject stateGO = Instantiate(statePrefab, transform);
            stateGO.GetComponent<State>().SetState(state);
            stateGO.GetComponent<State>().SetPosition(position);
            stateGO.GetComponent<RectTransform>().anchoredPosition = position;
            states.Add(stateGO.GetComponent<State>());
        }


        /// <summary>
        /// Renders an existing state into the graph
        /// </summary>
        /// <param name="state"></param>
        public void RenderState(Machine.State state)
        {
            Vector2 position = new Vector2(state.x, state.y);

            // TODO REMOVE THIS LINE
            // Provisionally generate a random position in the middle of the canvas
            position = new Vector2(Random.Range(-400, 400), Random.Range(-200, 200));

            GameObject stateGO = Instantiate(statePrefab, transform);
            stateGO.GetComponent<State>().SetState(state);
            stateGO.GetComponent<RectTransform>().anchoredPosition = position;
            states.Add(stateGO.GetComponent<State>());
        }

        /// <summary>
        /// Renders one transition of the Machine
        /// </summary>
        /// <param name="transition">Transition to render</param>
        public void RenderTransition(Machine.Transition transition)
        {
            GameObject transitionGO = Instantiate(transitionPrefab, transform);
            transitionGO.GetComponent<Transition>().SetTransition(transition);
        }

        /// <summary>
        /// Sends the current machine to the server to simulate
        /// This is an asyncronous operation
        /// TODO add Action success and Action error to TuringSym.Networking.ServerJandler.SimulateMachine
        /// </summary>
        public void Simulate()
        {
            if(machine != null)
            {
                // TODO process response
                Networking.ServerHandler.SimulateMachine(machine);
            }
        }
    }
}
