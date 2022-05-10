using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TuringSym.UI
{
    public class Transition : MonoBehaviour
    {
        UILineRenderer lineRenderer;
        RectTransform rectTransform;
        Text label;

        Machine.Transition transition;

        // Start is called before the first frame update
        void Start()
        {
            lineRenderer = GetComponentInChildren<UILineRenderer>();
            rectTransform = GetComponent<RectTransform>();
            label = GetComponentInChildren<Text>();
        }

        // Update is called once per frame
        void Update()
        {

            if(transition != null)
            {
                Vector2 originPos = new Vector2(transition.GetOriginState().x, transition.GetOriginState().y);
                Vector2 targetPos = new Vector2(transition.GetTargetState().x, transition.GetTargetState().y);
                Vector2 pos = (targetPos + originPos) / 2;

                // If this transition a lasso
                if (originPos.Equals(targetPos))
                {
                    pos += new Vector2(-100, 0);
                    originPos += new Vector2(-50, -10);
                    targetPos += new Vector2(-50, 10);
                    lineRenderer.points[0] = originPos - pos;
                    lineRenderer.points[1] = new Vector2(0, -10);
                    lineRenderer.points[2] = new Vector2(0, 10);
                    lineRenderer.points[3] = targetPos - pos;
                }
                else
                {
                    Vector2 originDir = (originPos - pos).normalized;
                    Vector2 targetDir = (targetPos - pos).normalized;

                    lineRenderer.points[0] = (originPos - pos) + (originDir * -50);
                    lineRenderer.points[1] = Vector2.zero;
                    lineRenderer.points[2] = Vector2.zero;
                    lineRenderer.points[3] = (targetPos - pos) + (targetDir * -50);
                }

                rectTransform.anchoredPosition = pos;

                lineRenderer.SetAllDirty();

                label.text = $"R:{transition.readValue}\n" +
                             $"W:{transition.writeValue}\n" +
                             $"M:{transition.moveValue}\n" +
                             $"S:{transition.targetState.name}";
            }
        }

        public void SetTransition(Machine.Transition transition)
        {
            this.transition = transition;
        }
    }
}

