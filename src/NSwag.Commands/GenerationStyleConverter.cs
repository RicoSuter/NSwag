using NSwag.Commands.Commands.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSwag.Commands
{
    internal class GenerationStyleNameConverter
    {
        internal static CSharpGenerationStyle GetGenerationStyle(IOperationNameGenerator operationNameGenerator)
        {
            if (operationNameGenerator is MultipleClientsFromOperationIdOperationNameGenerator)
            {
                return OperationGenerationMode.MultipleClientsFromOperationId;
            }

            if (operationNameGenerator is MultipleClientsFromPathSegmentsOperationNameGenerator)
            {
                return OperationGenerationMode.MultipleClientsFromPathSegments;
            }

            if (operationNameGenerator is MultipleClientsFromFirstTagAndPathSegmentsOperationNameGenerator)
            {
                return OperationGenerationMode.MultipleClientsFromFirstTagAndPathSegments;
            }

            if (operationNameGenerator is MultipleClientsFromFirstTagAndOperationIdGenerator)
            {
                return OperationGenerationMode.MultipleClientsFromFirstTagAndOperationId;
            }

            if (operationNameGenerator is MultipleClientsFromFirstTagAndOperationNameGenerator)
            {
                return OperationGenerationMode.MultipleClientsFromFirstTagAndOperationName;
            }

            if (operationNameGenerator is SingleClientFromOperationIdOperationNameGenerator)
            {
                return OperationGenerationMode.SingleClientFromOperationId;
            }

            if (operationNameGenerator is SingleClientFromPathSegmentsOperationNameGenerator)
            {
                return OperationGenerationMode.SingleClientFromPathSegments;
            }

            return OperationGenerationMode.MultipleClientsFromOperationId;
        }
    }
}
