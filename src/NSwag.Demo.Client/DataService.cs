using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Generated using the NSwag toolchain v2.6.5949.30535 (http://NSwag.org)

namespace NSwag.Demo.Client
{
    public partial class PersonsClient
    {
        public PersonsClient() : this("") { }

        public PersonsClient(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        partial void PrepareRequest(HttpClient request, ref string url);

        partial void ProcessResponse(HttpClient request, HttpResponseMessage response);

        public string BaseUrl { get; set; }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<string> XyzAsync(object data)
        {
            return XyzAsync(data, CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<string> XyzAsync(object data, CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Person/xyz/{data}");

            if (data == null)
                throw new ArgumentNullException("data");

            url = url.Replace("{data}", data.ToString());

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.PutAsync(url, new StringContent(""), cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "200")
            {
                var result = default(string);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<string>(Encoding.UTF8.GetString(responseData));
                    return result;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<ObservableCollection<Person>> GetAllAsync()
        {
            return GetAllAsync(CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<ObservableCollection<Person>> GetAllAsync(CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Get");

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "200")
            {
                var result = default(ObservableCollection<Person>);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<ObservableCollection<Person>>(Encoding.UTF8.GetString(responseData));
                    return result;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <summary>Gets a person.</summary>
        /// <param name="id">The ID of the person.</param>
        /// <returns>The person.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<Person> GetAsync(int id)
        {
            return GetAsync(id, CancellationToken.None);
        }

        /// <summary>Gets a person.</summary>
        /// <param name="id">The ID of the person.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The person.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<Person> GetAsync(int id, CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Get/{id}");

            if (id == null)
                throw new ArgumentNullException("id");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "200")
            {
                var result = default(Person);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<Person>(Encoding.UTF8.GetString(responseData));
                    return result;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }
            }
            else
            if (status == "500")
            {
                var result = default(PersonNotFoundException);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<PersonNotFoundException>(Encoding.UTF8.GetString(responseData));
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }

                throw new SwaggerException<PersonNotFoundException>("A server side error occurred.", response.StatusCode, responseData, result, null);
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <summary>Creates a new person.</summary>
        /// <param name="value">The person.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task PostAsync(Person value)
        {
            return PostAsync(value, CancellationToken.None);
        }

        /// <summary>Creates a new person.</summary>
        /// <param name="value">The person.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task PostAsync(Person value, CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Post");

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var content = new StringContent(JsonConvert.SerializeObject(value));
            content.Headers.ContentType.MediaType = "application/json";

            var response = await client.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);
            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "204")
            {
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <summary>Updates the existing person.</summary>
        /// <param name="id">The ID.</param>
        /// <param name="value">The person.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task PutAsync(int id, Person value)
        {
            return PutAsync(id, value, CancellationToken.None);
        }

        /// <summary>Updates the existing person.</summary>
        /// <param name="id">The ID.</param>
        /// <param name="value">The person.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task PutAsync(int id, Person value, CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Put/{id}");

            if (id == null)
                throw new ArgumentNullException("id");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var content = new StringContent(JsonConvert.SerializeObject(value));
            content.Headers.ContentType.MediaType = "application/json";

            var response = await client.PutAsync(url, content, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);
            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "204")
            {
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task DeleteAsync(int id)
        {
            return DeleteAsync(id, CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/Delete/{id}");

            if (id == null)
                throw new ArgumentNullException("id");

            url = url.Replace("{id}", id.ToString());

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "204")
            {
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <summary>Calculates the sum of a, b and c.</summary>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<int> CalculateAsync(int a, int b, int c)
        {
            return CalculateAsync(a, b, c, CancellationToken.None);
        }

        /// <summary>Calculates the sum of a, b and c.</summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<int> CalculateAsync(int a, int b, int c, CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Person/Calculate/{a}/{b}");

            if (a == null)
                throw new ArgumentNullException("a");

            url = url.Replace("{a}", a.ToString());
            if (b == null)
                throw new ArgumentNullException("b");

            url = url.Replace("{b}", b.ToString());

            if (c == null)
                throw new ArgumentNullException("c");
            else
                url += string.Format("c={0}&", Uri.EscapeUriString(c.ToString()));

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "200")
            {
                var result = default(int);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<int>(Encoding.UTF8.GetString(responseData));
                    return result;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<DateTime> AddHourAsync(DateTime time)
        {
            return AddHourAsync(time, CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<DateTime> AddHourAsync(DateTime time, CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/AddHour");

            if (time == null)
                throw new ArgumentNullException("time");
            else
                url += string.Format("time={0}&", Uri.EscapeUriString(time.ToString("s", CultureInfo.InvariantCulture)));

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "200")
            {
                var result = default(DateTime);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<DateTime>(Encoding.UTF8.GetString(responseData));
                    return result;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<int> TestAsync()
        {
            return TestAsync(CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<int> TestAsync(CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/TestAsync");

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "200")
            {
                var result = default(int);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<int>(Encoding.UTF8.GetString(responseData));
                    return result;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public Task<Car> LoadComplexObjectAsync()
        {
            return LoadComplexObjectAsync(CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async Task<Car> LoadComplexObjectAsync(CancellationToken cancellationToken)
        {
            var url = string.Format("{0}/{1}?", BaseUrl, "api/Persons/LoadComplexObject");

            var client = new HttpClient();
            PrepareRequest(client, ref url);

            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            ProcessResponse(client, response);

            var responseData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var status = ((int)response.StatusCode).ToString();

            if (status == "200")
            {
                var result = default(Car);
                try
                {
                    if (responseData.Length > 0)
                        result = JsonConvert.DeserializeObject<Car>(Encoding.UTF8.GetString(responseData));
                    return result;
                }
                catch (Exception exception)
                {
                    throw new SwaggerException("Could not deserialize the response body.", response.StatusCode, responseData, exception);
                }
            }
            else
            {
            }

            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").", response.StatusCode, responseData, null);
        }

        public class SwaggerException : Exception
        {
            public HttpStatusCode StatusCode { get; private set; }

            public byte[] ResponseData { get; private set; }

            public SwaggerException(string message, HttpStatusCode statusCode, byte[] responseData, Exception innerException)
                : base(message, innerException)
            {
                StatusCode = statusCode;
                ResponseData = responseData;
            }

            public override string ToString()
            {
                return string.Format("HTTP Response: \n{0}\n{1}", Encoding.UTF8.GetString(ResponseData), base.ToString());
            }
        }

        public class SwaggerException<TResponse> : SwaggerException
        {
            public TResponse Response { get; private set; }

            public SwaggerException(string message, HttpStatusCode statusCode, byte[] responseData, TResponse response, Exception innerException)
                : base(message, statusCode, responseData, innerException)
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
        private DateTime _birthday;
        private decimal _height;
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

        [JsonProperty("Birthday", Required = Required.Default)]
        public DateTime Birthday
        {
            get { return _birthday; }
            set
            {
                if (_birthday != value)
                {
                    _birthday = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>Gets or sets the height in cm.</summary>
        [JsonProperty("Height", Required = Required.Default)]
        public decimal Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
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

        [JsonProperty("Type", Required = Required.Default)]
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

        [JsonProperty("Type", Required = Required.Default)]
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
        private int _personId;

        [JsonProperty("PersonId", Required = Required.Default)]
        public int PersonId
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