using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TuringSym.UI
{
    public class NodeForm : MonoBehaviour
    {
        Text formLabel;
        InputField input;
        Action<string> accept;
        Action cancel;

        // Start is called before the first frame update
        void Start()
        {
            input = GetComponentInChildren<InputField>();
            formLabel = GetComponentInChildren<Text>();
            gameObject.SetActive(false);
        }

        public void ShowForm(string label, Action<string> accept, Action cancel = null)
        {
            if(input == null)
                input = GetComponentInChildren<InputField>();
            if(formLabel == null)
                formLabel = GetComponentInChildren<Text>();

            formLabel.text = label;
            this.accept = accept;
            this.cancel = cancel;
            gameObject.SetActive(true);
        }

        public void Accept()
        {
            if (accept != null)
            {
                accept(input.text);
            }
            accept = null;
            cancel = null;
            gameObject.SetActive(false);

        }

        public void Cancel()
        {
            if (cancel != null)
            {
                cancel();
            }
            accept = null;
            cancel = null;
            gameObject.SetActive(false);
        }
    }
}

