using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TuringSym.UI
{
    public class State : MonoBehaviour
    {
        public Machine.State state;

        private RectTransform rectTransform;
        private Text label;
        private Image background;
        private Image border;

        private static List<Color> colors;

        private Camera mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            label.text = state.name;

            // TODO store (x,y) in Machine.State
            state.SetPosition((int)rectTransform.anchoredPosition.x, (int)rectTransform.anchoredPosition.y);
        }

        /// <summary>
        /// Sets the state information for this state
        /// </summary>
        /// <param name="state"></param>
        public void SetState(Machine.State state)
        {
            rectTransform = GetComponent<RectTransform>();
            label = GetComponentInChildren<Text>();
            foreach (Transform t in transform)
            {
                if (t.name == "Background")
                {
                    background = t.GetComponent<Image>();
                }
                else if (t.name == "Border")
                {
                    border = t.GetComponent<Image>();
                }
            }

            if (colors == null)
            {
                colors = new List<Color>();
            }

            this.state = state;
            label.text = state.name;
            if (state.initialState)
            {
                background.color = Color.black;
                border.color = Color.white;
                label.color = Color.white;
            }
            else if (state.IsFinalState())
            {
                border.color = Color.black;
            }
            else
            {
                Color color = GetRandomPastelColor();
                background.color = color;
                border.color = color;
                label.color = Color.black;
            }
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.position = position;
        }

        /// <summary>
        /// Converts an integer value to a Color
        /// Assumes the value comes in RGB (0-255) format
        /// </summary>
        /// <param name="hexValue">The value of the color from 0 to 16777216</param>
        /// <returns></returns>
        private Color HexToColor(uint hexValue)
        {
            float b = (hexValue % 255) / 255;
            hexValue = hexValue / 255;

            float g = (hexValue % 255) / 255;
            hexValue = hexValue / 255;

            float r = (hexValue % 255) / 255;

            return new Color(r, g, b);
        }

        /// <summary>
        /// Generates a random color with any hue, saturation between 25% and 95% and lightness between 85% and 95%
        /// </summary>
        /// <returns></returns>
        private Color GetRandomPastelColor()
        {
            float h = Random.Range(0, 1);
            float s = Random.Range(0.25f, 0.95f);
            float v = Random.Range(0.85f, 0.95f);

            return Color.HSVToRGB(h, s, v);
        }

        public void OnDrag()
        {
            rectTransform.position = Input.mousePosition;
        }

        public void OnClick()
        {
            print(state.name);
        }
        
    }
}

