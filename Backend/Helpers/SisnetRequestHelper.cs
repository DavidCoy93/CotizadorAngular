using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace MSSPAPI.Helpers
{
    public static class SisnetRequestHelper<T>
    {
        public static R SendRequestToSisNet<R>(T request) where R : class, new()
        {
            Type typeObjParam = typeof(T);
            
            var resultObj = new R();

            try
            {
                PropertyInfo[] propertiesParam = typeObjParam.GetProperties();

                foreach (PropertyInfo propR in resultObj.GetType().GetProperties())
                {
                    PropertyInfo propertyParam = typeObjParam.GetProperties().Where(p => p.Name == propR.Name).FirstOrDefault();

                    if (propertyParam != null)
                    {

                        var value = propertyParam.GetValue(request, null);
                        propR.SetValue(resultObj, value , null);
                    } 
                    else
                    {
                        propR.SetValue(resultObj, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultObj;
        }


        public static async Task<string> sendRequestToSisnet(T request)
        {
            string result = "";

            try
            {
                List<ServiceProviderConfiguration> serviceProviderList = DBOperations.GetServiceProviderConfiguration(null, "SISNET");
                ServiceProviderConfiguration serviceProvider = serviceProviderList[0];
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Usuario", serviceProvider.SvcUsr);
                httpClient.DefaultRequestHeaders.Add("Password", serviceProvider.SvcPwd);

                HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync<T>(serviceProvider.BaseUrl + "sisos/Execute", request);

                if (httpResponse.IsSuccessStatusCode)
                {
                    result = await httpResponse.Content.ReadAsStringAsync();
                }
            }
            catch (TimeoutException e)
            {
                result = e.Message;
            }
            catch (FormatException e)
            {
                result = e.Message;
            }

            return result;
        }
    }

    public class GenericRequestObj
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class GenericResponseObj
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsSingle {  get; set; }
        public string Address { get; set; }
    }
}