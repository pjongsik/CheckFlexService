using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml;


namespace CheckFlexService
{
    static class Program
    {

           /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        /// [ServiceKnownType(typeof(Job))]
        
        [STAThread]
        static void Main()
        {
           
            


            //flexService

            
            
          

         //flexService.get

            //foreach (var item in flexService.GetScheduleByGroup(_중랑숲, "0001", "201507"))
            //{
            //    FlexService.ScheduleGroup grp = (FlexService.ScheduleGroup)item;

            //    Console.WriteLine(" grp.GroupName : {0} ", grp.GroupName);
            //}


            
            

            //int count = result.Count;
            

            //foreach (var item in result)
            //{
            //    Console.WriteLine(item.ToString());
            //}

            //FlexService.ArrayOfAnyType result = new FlexService.ArrayOfAnyType();
            //FlexService.GetScheduleByGroupRequestBody reqBody = new FlexService.GetScheduleByGroupRequestBody("0012", "0001", "201506");
            //FlexService.GetScheduleByGroupRequest req = new FlexService.GetScheduleByGroupRequest(reqBody);
            
            
            //FlexService.GetScheduleByGroupResponseBody resBody = new FlexService.GetScheduleByGroupResponseBody(result);
                

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }


        public static BasicHttpBinding GetEndpointAddress(string bindingName, int maxBufferSize = 65536 * 5)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            try
            {
                binding.Name = bindingName;                                //  "UploaderSoap";
                binding.CloseTimeout = TimeSpan.FromMinutes(1);
                binding.OpenTimeout = TimeSpan.FromMinutes(1);
                binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
                binding.SendTimeout = TimeSpan.FromMinutes(1);
                binding.AllowCookies = false;
                binding.BypassProxyOnLocal = false;
                binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                binding.MaxBufferSize = maxBufferSize;
                binding.MaxBufferPoolSize = maxBufferSize;
                binding.MessageEncoding = WSMessageEncoding.Text;
                binding.TextEncoding = System.Text.Encoding.UTF8;
                binding.TransferMode = TransferMode.Buffered;
                binding.UseDefaultWebProxy = true;
                binding.MaxReceivedMessageSize = maxBufferSize;

                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 8192;
                binding.ReaderQuotas.MaxArrayLength = 16384;
                binding.ReaderQuotas.MaxBytesPerRead = 4096;
                binding.ReaderQuotas.MaxNameTableCharCount = 16384;

                binding.Security.Mode = BasicHttpSecurityMode.None;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                binding.Security.Transport.Realm = "";
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return binding;
        }


        public static void CallWebService()
        {
            var _url = "http://xxxxxxxxx/Service1.asmx";
            var _action = "http://xxxxxxxx/Service1.asmx?op=HelloWorld";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                Console.Write(soapResult);
            }
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/1999/XMLSchema""><SOAP-ENV:Body><HelloWorld xmlns=""http://tempuri.org/"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><int1 xsi:type=""xsd:integer"">12</int1><int2 xsi:type=""xsd:integer"">32</int2></HelloWorld></SOAP-ENV:Body></SOAP-ENV:Envelope>");
            return soapEnvelop;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

        }
    }
}
