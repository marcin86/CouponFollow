using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Net;

namespace CouponFollow
{
    [TestClass]
    public class GET_Tests
    {
        const string url = "https://couponfollow.com/api/extension/trendingOffers";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        [TestMethod]
        public void CorrectScenario_ListOfElementsIsReturned()
        {
            // Arrange           
            request.Headers.Add("catc-version: 5.0.0");

            // Act
            var result = Get_Response_Content();

            // Assert
            Assert.IsInstanceOfType(result.Children(), typeof(JEnumerable<JToken>));
            Assert.IsTrue(result.Count >= 0 && result.Count <= 20);
        }

        [TestMethod]
        public void InCorrectScenario_403ForbiddenDueToMissingHeader()
        {
            try
            {
                var result = Get_Response_Content();
                Assert.Fail("403 Forbidden should have been thrown");
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Forbidden, ((HttpWebResponse)ex.Response).StatusCode);
            }
        }

        [TestMethod]
        public void CorrectScenario_NbOfReturnedElementsIsNoHigherThan20()
        {
            // Arrange           
            request.Headers.Add("catc-version: 5.0.0");

            // Act
            var result = Get_Response_Content();

            // Assert
            Assert.IsTrue(result.Count <= 20);
        }

        [TestMethod]
        public void CorrectScenario_DomainNameIsUnique()
        {
            // Arrange           
            request.Headers.Add("catc-version: 5.0.0");

            // Act
            var result = Get_Response_Content();
            int count = result.GroupBy(x => x).Where(y => y.Count() > 1).Select(z => z.Children()["DomainName"]).ToList().Count;

            // Assert
            Assert.AreEqual(0, count);
        }


        private JArray Get_Response_Content()
        {
            JArray result_json = null;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                result_json = (JArray)JsonConvert.DeserializeObject(result);
            }
    
            return result_json;
        }
    }
}
