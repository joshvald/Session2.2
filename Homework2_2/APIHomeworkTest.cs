using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System.Net;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace Homework2_2
{
    [TestClass]
    public class APIHomeworkTest
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string UserEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {
            #region CREATE PET

            // CREATE PET DATA
            var petData = new PetModel()
            {
                Id = 0,
                Category = new Category()
                {
                    Id = 2521,
                    Name = "RestSharp Category",
                },
                Name = "RestSharp.Post",
                PhotoUrls = new string[]
                {
                    "string"
                },
                Tags = new Category[]
                {
                    new Category() { Id = 2521, Name = "RestSharp Category" }
                },
                Status = "available"
            };

            // SEND POST REQUEST
            var temp = GetURI(UserEndpoint);
            var postRestRequest = new RestRequest(GetURI(UserEndpoint)).AddJsonBody(petData);
            var postRestResponse = await restClient.ExecutePostAsync<PetModel>(postRestRequest);

            // GET AND STORE THE GENERATED ID ON petData.Id
            petData.Id = postRestResponse.Data.Id;

            // VERIFY POST REQUEST STATUS CODE
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200");

            #endregion

            #region CLEANUP DATA

            cleanUpList.Add(petData);

            #endregion

            #region GET PET DATA

            var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{petData.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetModel>(restRequest);
            
            #endregion

            #region ASSERTIONS

            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(petData.Name, restResponse.Data.Name, "Name did not match.");
            Assert.AreEqual(petData.Category.Id, restResponse.Data.Category.Id, "Category Id did not match.");
            Assert.AreEqual(petData.Category.Name, restResponse.Data.Category.Name, "Category name did not match.");
            Assert.AreEqual(petData.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "Photo URLs did not match.");
            Assert.AreEqual(petData.Tags[0].Id, restResponse.Data.Tags[0].Id, "Tags did not match.");
            Assert.AreEqual(petData.Tags[0].Name, restResponse.Data.Tags[0].Name, "Tags did not match.");
            Assert.AreEqual(petData.Status, restResponse.Data.Status, "Status did not match.");
            
            #endregion
        }
    }
}