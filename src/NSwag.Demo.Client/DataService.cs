



using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

// Generated using the NSwag toolchain v0.1.5725.20357 (https://github.com/NSwag/NSwag)

namespace NSwag.Demo.Client
{
    public partial class DataService
    {
        public DataService() : this("/") { }

        public DataService(string baseUrl)
        {
            BaseUrl = baseUrl; 
        }

        partial void PrepareRequest(HttpClient request);

        partial void ProcessResponse(HttpClient request, HttpResponseMessage response);

        public string BaseUrl { get; set; }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<ObservableCollection<object>> GetAllAsync()
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Get");

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<ObservableCollection<object>>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<object> GetAsync(long id )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Get/{id}");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<object>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> PostAsync(object request )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Post");

            var client = new HttpClient();
            PrepareRequest(client);

            var content = new StringContent(JsonConvert.SerializeObject(request));

            var response = await client.PostAsync(url, content);
            ProcessResponse(client, response);
            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<string>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> PutAsync(long id, object request )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Put/{id}");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client);

            var content = new StringContent(JsonConvert.SerializeObject(request));

            var response = await client.PutAsync(url, content);
            ProcessResponse(client, response);
            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<string>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> DeleteAsync(long id )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Delete/{id}");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.DeleteAsync(url);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<string>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<long> CalculateAsync(long a, long b, long c )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Person/Calculate/{a}/{b}");

            url = url.Replace("{a}", a.ToString());
            url = url.Replace("{b}", b.ToString());

            url += string.Format("c={0}&", Uri.EscapeUriString(c.ToString()));

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<long>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<DateTime> AddHourAsync(DateTime time )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/AddHour");

            url += string.Format("time={0}&", Uri.EscapeUriString(time.ToString()));

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<DateTime>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<object> LoadComplexObjectAsync()
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/LoadComplexObject");

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync(); 
            var status = response.StatusCode.ToString();
            if (status == "200") {
                return JsonConvert.DeserializeObject<object>(responseData);		
            }

            throw new SwaggerException("The response was not expected.", response.StatusCode, null);
        }

        public class SwaggerException : Exception
        {
            public HttpStatusCode StatusCode { get; private set; }

            public SwaggerException(string message, HttpStatusCode statusCode, Exception innerException) : base(message, innerException)
            {
                StatusCode = statusCode;
            }
        }

        public class SwaggerException<TResponse> : SwaggerException
        {
            public TResponse Response { get; private set; }

            public SwaggerException(string message, HttpStatusCode statusCode, TResponse response, Exception innerException) : base(message, statusCode, innerException)
            {
                Response = response;
            }
        }
    }

    public partial class SwaggerException : INotifyPropertyChanged
    {
        private string _exceptionType;
        private string _message;
        private string _stackTrace;

        [JsonProperty("ExceptionType", Required = Required.Default)]
        public string ExceptionType
        {
            get { return _exceptionType; }
            set 
            {
                if (_exceptionType != value)
                {
                    _exceptionType = value; 
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("Message", Required = Required.Default)]
        public string Message
        {
            get { return _message; }
            set 
            {
                if (_message != value)
                {
                    _message = value; 
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("StackTrace", Required = Required.Default)]
        public string StackTrace
        {
            get { return _stackTrace; }
            set 
            {
                if (_stackTrace != value)
                {
                    _stackTrace = value; 
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson() 
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SwaggerException FromJson(string data)
        {
            return JsonConvert.DeserializeObject<SwaggerException>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class Person : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;

        [JsonProperty("firstName", Required = Required.Default)]
        public string FirstName
        {
            get { return _firstName; }
            set 
            {
                if (_firstName != value)
                {
                    _firstName = value; 
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("LastName", Required = Required.Default)]
        public string LastName
        {
            get { return _lastName; }
            set 
            {
                if (_lastName != value)
                {
                    _lastName = value; 
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson() 
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Person FromJson(string data)
        {
            return JsonConvert.DeserializeObject<Person>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class Car : INotifyPropertyChanged
    {
        private string _name;
        private Person _driver;

        [JsonProperty("Name", Required = Required.Default)]
        public string Name
        {
            get { return _name; }
            set 
            {
                if (_name != value)
                {
                    _name = value; 
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("Driver", Required = Required.Default)]
        public Person Driver
        {
            get { return _driver; }
            set 
            {
                if (_driver != value)
                {
                    _driver = value; 
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson() 
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Car FromJson(string data)
        {
            return JsonConvert.DeserializeObject<Car>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}