using MSSPAPI.Models;
using MSSPAPI.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Configuration;
using MSSPAPI.PolicyServiceWS;
using Newtonsoft.Json;
using System.Collections.Generic;
using Azure;
using System.Net.Http.Formatting;

namespace MSSPAPI.Controllers
{
    public class AzureBlobStorageController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [Authorize]
        [Route("api/UploadCertFiles")]
        public async Task<HttpResponseMessage> UploadCertFiles([FromBody] DocumentCertification value)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            formatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            //implement Azure blob storage
            try
            {
                //Folder_Name = idCertificate encripted  fileName = id document +/+ consecutive encripted + file extension
                //Example: 1/12_1.png => HBxwTNGEM960/wbR/gCshg==/WoAW8iQSV9yR3ztirwUAAA==.png

                var bytes = Convert.FromBase64String(value.Base64File);
                var mStream = new MemoryStream(bytes);
                string country = value.Country;
                string dealer = value.DealerCode;
                string idCert = value.IdCertificate;
                string cert = value.Certificate;


                string filelName = $"{country}/{dealer}/{cert}{idCert}.{value.Extension}";

                AzureBlob uploadRes = await UploadAsyncBlock(filelName, mStream, value.Extension);

                if (uploadRes.Error) // return the error from azure storage
                {
                    httpResponse.StatusCode = HttpStatusCode.BadRequest;
                    httpResponse.Content = new ObjectContent<object>(new { Error = uploadRes.Error, Message = uploadRes.Message }, formatter);
                }

                httpResponse.StatusCode = HttpStatusCode.OK;
                httpResponse.Content = new StringContent(uploadRes.Uri);
                

            }
            catch (Exception ex)
            {
                log.Error(string.Format("UploadChunk - {0}", ex.Message), ex);
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                httpResponse.Content = new ObjectContent<object>(new { Error = true, Message = ex.Message }, formatter);
            }

            return httpResponse;
        }

        private static async Task<AzureBlob> UploadAsyncBlock(string FileName, Stream fileStream, string contentType)
        {
            AzureBlob response = new AzureBlob();
            string content = (contentType == "pdf") ? "application/pdf" : "application/xml";

            try
            {
                List<ServiceProviderConfiguration> serviceProviderConfigurationList = DBOperations.GetServiceProviderConfiguration(null,"AZURE");
                ServiceProviderConfiguration serviceProviderConfiguration = serviceProviderConfigurationList[0];

                serviceProviderConfiguration.SvcPwd = EncDec.Decript(serviceProviderConfiguration.SvcPwd);

                BlobContainerClient containerClient = new BlobContainerClient(serviceProviderConfiguration.SvcPwd, serviceProviderConfiguration.SvcUsr);

                AppendBlobClient appendBlobClient = containerClient.GetAppendBlobClient(FileName); //use the original filename without chunk convention
                BlobHttpHeaders blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };// set the correct content type from js function
                Response<BlobContentInfo> containerInfo = await appendBlobClient.CreateIfNotExistsAsync(); //create the blob file if not exists
                
                Response<BlobInfo> blobInfo = await appendBlobClient.SetHttpHeadersAsync(blobHttpHeader);
                var maxBlockSize = appendBlobClient.AppendBlobMaxAppendBlockBytes;

                var buffer = new byte[maxBlockSize];
                if (fileStream.Length <= maxBlockSize)
                {
                    Response<BlobAppendInfo> responseAzure = await appendBlobClient.AppendBlockAsync(fileStream);
                    BlobAppendInfo blobAppendInfo = responseAzure.Value;
                }
                else  // when chunk is grater than the max allowed by blob API
                {
                    var bytesLeft = (fileStream.Length - fileStream.Position);
                    while (bytesLeft > 0)
                    {
                        if (bytesLeft >= maxBlockSize)
                        {
                            buffer = new byte[maxBlockSize];
                            await fileStream.ReadAsync
                                (buffer, 0, maxBlockSize);
                        }
                        else
                        {
                            buffer = new byte[bytesLeft];
                            await fileStream.ReadAsync
                                (buffer, 0, Convert.ToInt32(bytesLeft));
                        }

                        appendBlobClient.AppendBlock(new MemoryStream(buffer));
                        bytesLeft = (fileStream.Length - fileStream.Position);
                    }
                }

                BlockBlobClient blobClient = containerClient.GetBlockBlobClient(FileName);

                Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    ExpiresOn = DateTime.UtcNow.AddYears(1)
                };

                blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobAccountSasPermissions.Read);

                var sasUrl = blobClient.GenerateSasUri(blobSasBuilder);

                response.Error = false;
                response.Uri = sasUrl.AbsoluteUri;

            }
            catch (RequestFailedException ex)
            {
                response.Message = ex.Message;
                response.Error = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Error = true;
            }

            return response;
        }
    }
}