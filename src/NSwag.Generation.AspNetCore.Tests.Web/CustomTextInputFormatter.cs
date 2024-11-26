﻿using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using Microsoft.Net.Http.Headers;

namespace NSwag.Generation.AspNetCore.Tests.Web
{
    public class CustomTextInputFormatter : TextInputFormatter
    {
        // Required to use text/html or foo/bar in ConsumesAttribute, see ConsumesController and ConsumesTests

        public CustomTextInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/html"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("foo/bar"));

            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            return Task.FromResult(InputFormatterResult.Success(null));
        }
    }
}
