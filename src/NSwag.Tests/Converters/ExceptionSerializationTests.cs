using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag.Annotations;
using NSwag.Annotations.Converters;

namespace NSwag.Tests.Converters
{
    public class CompanyNotFoundException : Exception
    {
        internal CompanyNotFoundException()
        {
        }

        public CompanyNotFoundException(string message) : base(message)
        {
        }

        public CompanyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        [JsonProperty("CompanyKey")]
        public Guid CompanyKey { get; set; }
    }

    [TestClass]
    public class ExceptionSerializationTests
    {
        [TestMethod]
        public void When_custom_exception_is_serialized_then_everything_works()
        {
            //// Arrange
            var settings = CreateSettings();
            try
            {
                throw new CompanyNotFoundException("Foo", new Exception("Bar", new Exception("Hello World")))
                {
                    Source = "Bli",
                    CompanyKey = new Guid("E343DE26-1F13-4FE4-9368-5518E79DDBB9")
                };
            }
            catch (CompanyNotFoundException exception)
            {
                //// Act
                var json = JsonConvert.SerializeObject(exception, settings);
                var newException = JsonConvert.DeserializeObject<Exception>(json, settings) as CompanyNotFoundException;
                var newJson = JsonConvert.SerializeObject(newException, settings);

                //// Assert
                Assert.AreEqual(exception.CompanyKey, newException.CompanyKey);

                Assert.AreEqual(exception.Message, newException.Message);
                Assert.AreEqual(exception.Source, newException.Source);
                Assert.AreEqual(exception.InnerException.Message, newException.InnerException.Message);
                Assert.AreEqual(exception.InnerException.InnerException.Message, newException.InnerException.InnerException.Message);

                Assert.AreEqual(exception.StackTrace, newException.StackTrace);
            }
        }

        [TestMethod]
        public void When_stack_trace_hiding_is_enabled_then_stack_trace_is_HIDDEN()
        {
            //// Arrange
            var settings = CreateSettings(true);
            try
            {
                throw new CompanyNotFoundException();
            }
            catch (CompanyNotFoundException exception)
            {
                //// Act
                var json = JsonConvert.SerializeObject(exception, settings);
                var newException = JsonConvert.DeserializeObject<Exception>(json, settings) as CompanyNotFoundException;

                //// Assert
                Assert.AreEqual("HIDDEN", newException.StackTrace);
            }
        }

        [TestMethod]
        public void JsonExceptionConverter_is_thread_safe()
        {
            //// Arrange
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    When_custom_exception_is_serialized_then_everything_works();
                }));
            }

            //// Act
            Task.WaitAll(tasks.ToArray());

            //// Assert
            // No exceptions
        }

        private static JsonSerializerSettings CreateSettings(bool hideStackTrace = false)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters =
                {
                    new JsonExceptionConverter(hideStackTrace, new Dictionary<string, Assembly>
                    {
                        { typeof(ExceptionSerializationTests).Namespace , typeof(ExceptionSerializationTests).Assembly}
                    })
                }
            };
            return settings;
        }

        [TestMethod]
        public void When_ArgumentException_is_thrown_then_it_is_serialized_with_all_properties()
        {
            //// Arrange
            var settings = CreateSettings();

            try
            {
                throw new ArgumentException("foo", "bar");
            }
            catch (ArgumentException exception)
            {
                //// Act
                var json = JsonConvert.SerializeObject(exception, settings);
                var newException = JsonConvert.DeserializeObject<Exception>(json, settings) as ArgumentException;
                var newJson = JsonConvert.SerializeObject(newException, settings);

                //// Assert
                Assert.AreEqual(exception.ParamName, newException.ParamName);
            }
        }

        [TestMethod]
        public void When_InvalidOperationException_is_thrown_then_it_is_serialized_with_all_properties()
        {
            //// Arrange
            var settings = CreateSettings();

            try
            {
                throw new InvalidOperationException("hello");
            }
            catch (InvalidOperationException exception)
            {
                //// Act
                var json = JsonConvert.SerializeObject(exception, settings);
                var newException = JsonConvert.DeserializeObject<Exception>(json, settings) as InvalidOperationException;
                var newJson = JsonConvert.SerializeObject(newException, settings);

                //// Assert
                Assert.AreEqual(exception.Message, newException.Message);
            }
        }

        public class Teacher : Person
        {
            public string Class { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public void When_ArgumentOutOfRangeException_is_thrown_then_it_is_serialized_with_all_properties()
        {
            //// Arrange
            var settings = CreateSettings();

            try
            {
                throw new ArgumentOutOfRangeException("foo", new Person(), "bar");
            }
            catch (ArgumentOutOfRangeException exception)
            {
                //// Act
                var json = JsonConvert.SerializeObject(exception, settings);
                var newException = JsonConvert.DeserializeObject<Exception>(json, settings) as ArgumentOutOfRangeException;
                var newJson = JsonConvert.SerializeObject(newException, settings);

                //// Assert
                Assert.IsNotNull(newException.ActualValue);
                Assert.AreEqual(exception.ParamName, newException.ParamName);
            }
        }
    }
}