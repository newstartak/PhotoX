using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;
using UnityEngine;

public partial class AdcTest : MonoBehaviour
{
    private void Start()
    {
        QueryTestablePermissions("a");
    }

    public static IList<Permission> QueryTestablePermissions(string fullResourceName)
    {
        var credential = GoogleCredential.GetApplicationDefault().CreateScoped(IamService.Scope.CloudPlatform);

        var service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = new QueryTestablePermissionsRequest
        {
            FullResourceName = fullResourceName
        };

        var response = service.Permissions.QueryTestablePermissions(request).Execute();

        foreach (var p in response.Permissions)
        {
            Debug.Log(p.Name);
        }

        return response.Permissions;
    }
}