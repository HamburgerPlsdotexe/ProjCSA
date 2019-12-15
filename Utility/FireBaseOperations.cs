using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectCSA.Models;
namespace ProjectCSA.Utility
{
    public class FireBaseOperations
    {
        public static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "S1VfQJeuwZzyPI6KVQgJATyvqnRL995HnR4xkqj2",
            BasePath = "https://studentpre-a7d96.firebaseio.com/"
        };

        public static IFirebaseClient client;
        public static FirebaseResponse response;

        public static List<StudentModel> Retrieve()
        {
            client = new FireSharp.FirebaseClient(config);
            response = client.Get("/Students");
  
            string responseBody = response.Body;
            var firebaseLookup = JsonConvert.DeserializeObject<Dictionary<string, StudentModel>>(responseBody);
            var data = firebaseLookup.Values.ToList();

            return data;
        }
    }
}