using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;

public class HelloWorld : MonoBehaviour
{
    public string myKey = "inputValue";
    public string myValue = "Blobbers";


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            CallCSharpExecuteFunction();
        }
    }
    private void CallCSharpExecuteFunction()
    {
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                Type = PlayFabSettings.staticPlayer.EntityType, //Get this from when you logged in
            },
            FunctionName = "HelloWorld2", //This should be the name of your Azure Function that you created.
            //FunctionParameter = new Dictionary<string, object>() { { "inputValue", "Test" } }, //This is the data that you would want to pass into your function.
            FunctionParameter = new Dictionary<string, object>() { { myKey, myValue } },
            GeneratePlayStreamEvent = true //Set this to true if you would like this call to show up in PlayStream
        }, (ExecuteFunctionResult result) =>
        {
            if (result.FunctionResultTooLarge ?? false)
            {
                Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                return;
            }
            Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
            Debug.Log($"Result: {result.FunctionResult.ToString()}");
        }, (PlayFabError error) =>
        {
            Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
        });
    }
}
