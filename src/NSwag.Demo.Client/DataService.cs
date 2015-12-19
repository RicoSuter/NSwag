using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Generated using the NSwag toolchain v1.15.5831.1699 (http://NSwag.org)

namespace NSwag.Demo.Client
{
    public partial class DataService 
    {
        public DataService() : this("") { }

        public DataService(string baseUrl)
        {
            BaseUrl = baseUrl; 
        }

        partial void PrepareRequest(HttpClient request);

        partial void ProcessResponse(HttpClient request, HttpResponseMessage response);

        public string BaseUrl { get; set; }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> XyzAsync(string data )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Person/xyz/{data}");

            url = url.Replace("{data}", data.ToString());

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.PutAsync(url, new StringContent("")).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "200") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<string>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<ObservableCollection<Person>> GetAllAsync()
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Get");

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "200") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<ObservableCollection<Person>>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <summary>Gets a person.</summary>
        /// <param name="id">The ID of the person.</param>
        /// <returns>The person.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<Person> GetAsync(long id )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Get/{id}");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "200") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<Person>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            if (status == "500") 
            {
                try
                {
                    var exception = JsonConvert.DeserializeObject<PersonNotFoundException>(responseData);
                    throw new SwaggerException<PersonNotFoundException>("A server side error occurred.", response.StatusCode, exception, null);
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the exception body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <summary>Creates a new person.</summary>
        /// <param name="value">The person.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> PostAsync(Person value )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Post");

            var client = new HttpClient();
            PrepareRequest(client);

            var content = new StringContent(JsonConvert.SerializeObject(value));
            content.Headers.ContentType.MediaType = "application/json";

            var response = await client.PostAsync(url, content).ConfigureAwait(false);
            ProcessResponse(client, response);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "204") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<string>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <summary>Updates the existing person.</summary>
        /// <param name="id">The ID.</param>
        /// <param name="value">The person.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> PutAsync(long id, Person value )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Put/{id}");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client);

            var content = new StringContent(JsonConvert.SerializeObject(value));
            content.Headers.ContentType.MediaType = "application/json";

            var response = await client.PutAsync(url, content).ConfigureAwait(false);
            ProcessResponse(client, response);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "204") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<string>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> DeleteAsync(long id )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Delete/{id}");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.DeleteAsync(url).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "204") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<string>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <summary>Calculates the sum of a, b and c.</summary>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<long> CalculateAsync(long a, long b, long c )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Person/Calculate/{a}/{b}");

            url = url.Replace("{a}", a.ToString());
            url = url.Replace("{b}", b.ToString());

            url += string.Format("c={0}&", Uri.EscapeUriString(c.ToString()));

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "200") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<long>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<DateTime> AddHourAsync(DateTime time )
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/AddHour");

            url += string.Format("time={0}&", Uri.EscapeUriString(time.ToString()));

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "200") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<DateTime>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<long> TestAsync()
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/TestAsync");

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "200") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<long>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<Car> LoadComplexObjectAsync()
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/LoadComplexObject");

            var client = new HttpClient();
            PrepareRequest(client);

            var response = await client.GetAsync(url).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var status = ((int)response.StatusCode).ToString();
            if (status == "200") 
            {
                try
                {
                    return JsonConvert.DeserializeObject<Car>(responseData);		
                } 
                catch (Exception exception) 
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, null);
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

    /// <summary>The DTO class for a person.</summary>
    public partial class Person : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private ObservableCollection<Car> _cars;
        private ObjectType _type;

        /// <summary>Gets or sets the first name.</summary>
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

        [JsonProperty("Cars", Required = Required.Default)]
        public ObservableCollection<Car> Cars
        {
            get { return _cars; }
            set 
            {
                if (_cars != value)
                {
                    _cars = value; 
                    RaisePropertyChanged();
                }
            }
        }

        [JsonProperty("Type", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType Type
        {
            get { return _type; }
            set 
            {
                if (_type != value)
                {
                    _type = value; 
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
        private ObjectType _type;

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

        [JsonProperty("Type", Required = Required.Always)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType Type
        {
            get { return _type; }
            set 
            {
                if (_type != value)
                {
                    _type = value; 
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

    public enum ObjectType
    {
        Foo = 0, 
        Bar = 1, 
    }

    public partial class PersonNotFoundException : Exception, INotifyPropertyChanged
    {
        private long _personId;

        [JsonProperty("PersonId", Required = Required.Always)]
        public long PersonId
        {
            get { return _personId; }
            set 
            {
                if (_personId != value)
                {
                    _personId = value; 
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ToJson() 
        {
            return JsonConvert.SerializeObject(this);
        }

        public static PersonNotFoundException FromJson(string data)
        {
            return JsonConvert.DeserializeObject<PersonNotFoundException>(data);
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}