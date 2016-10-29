using System.Threading.Tasks;

namespace NSwag.Commands
{
    public class NSwagDocumentCommand : NSwagDocumentCommandBase
    {
        protected override async Task<NSwagDocumentBase> LoadDocumentAsync(string filePath)
        {
            return await NSwagDocument.LoadAsync(filePath);
        }
    }
}