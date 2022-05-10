using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace TuringSym.Networking
{
    /// <summary>
    /// Unity Singleton class
    /// Manages requests to the TuringMaster server
    /// This script must be attached to a game object
    /// </summary>
    public class ServerHandler : MonoBehaviour
    {
        private static ServerHandler handler;
        private string accessToken;
        private string email;

        private static string serverURL = "https://turingmaster.herokuapp.com/api/";

        void Awake()
        {
            handler = this;
        }

        /// <summary>
        /// Executes a POST on the requested route
        /// </summary>
        /// <param name="route">The relative route to execute the POST</param>
        /// <param name="postData">JSON data to post</param>
        /// <param name="callback">Handler for successful response</param>
        /// <param name="errorCallback">Handler for error response</param>
        /// <returns></returns>
        static IEnumerator Post(string route, string postData, Action<string> callback, Action<string> errorCallback)
        {
            string url = serverURL + route;
            
            //using (UnityWebRequest www = UnityWebRequest.Post(url, postData))
            using(UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                byte[] data = Encoding.UTF8.GetBytes(postData);
                www.uploadHandler = new UploadHandlerRaw(data);
                www.downloadHandler = new DownloadHandlerBuffer();

                // Wait for response
                yield return www.SendWebRequest();

                // If response is Sucess, execute callback
                if (www.result == UnityWebRequest.Result.Success)
                {
                    callback(www.downloadHandler.text);
                }

                // If response is other than success, execute errorCallback
                else
                {
                    errorCallback(www.error);
                }
            }
        }

        /// <summary>
        /// Sends one machine to simulate
        /// </summary>
        /// <param name="machine"></param>
        public static void SimulateMachine(Machine machine)
        {
            string json = machine.ToJson();

            handler.StartCoroutine(Post(
                route: "tmachines/simulate",
                postData: json,
                // callback executes if response is SUCCESS
                // TODO receive Action success from Graph.cs
                callback: (string response) => {
                    
                    // TODO Serialize response into a Core object 
                    print(response);
                },
                // errorCallback executes if response is other than SUCCESS
                // TODO receive Action error from Graph.cs
                errorCallback: (string error) => {

                    // TODO Serialize error message and process
                    print(error);
                }));
        }
    }
}

