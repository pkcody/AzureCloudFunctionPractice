using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.ClientModels;

public class HelloWorld : MonoBehaviour
{
    public string myKey = "inputValue";
    public string myValue = "Blobbers";

    // Catalog info
    public string catalogName = "Messier";
    private List<CatalogItem> catalog;
    private List<Messier> messiers = new List<Messier>();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            CallCSharpExecuteFunction();
        }
        // Catalog button
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetCatalog();
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

    public void GetCatalog()
    {
        GetCatalogItemsRequest getCatalogRequest = new GetCatalogItemsRequest
        {
            CatalogVersion = catalogName
        };

        PlayFabClientAPI.GetCatalogItems(getCatalogRequest,
            result =>
            {
                catalog = result.Catalog;
            },
            error => Debug.Log(error.ErrorMessage)
        );

        Invoke("SplitCatalog", 3f);
    }

    public void SplitCatalog()
    {
        foreach (CatalogItem item in catalog)
        {
            Messier m = JsonUtility.FromJson<Messier>(item.CustomData);
            m.name = item.DisplayName;
            m.description = item.Description;
            messiers.Add(m);
        }

        ShowCatalog();
    }

    public void ShowCatalog()
    {
        foreach (Messier m in messiers)
        {
            string logMsg = "***" + m.name + "***";
            logMsg += "\n" + m.description;
            logMsg += "\ndistance: " + m.dist;
            logMsg += "\nseason: " + m.season;
            logMsg += "\ntype: " + m.type;
            Debug.Log(logMsg);

        }
    }


    public class Messier
    {
        public string name;
        public string description;
        public string dist;
        public string season;
        public string type;
    }
}
