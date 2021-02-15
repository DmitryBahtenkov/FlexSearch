using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;

namespace SearchApi.Mappings
{
    public static class DocumentMapper
    {
        public static DocumentDto MapToDto(DocumentModel documentModel)
        {
            return new DocumentDto
            {
                Id = documentModel.Id,
                Value = JsonDocument.Parse(documentModel.Value.ToString())
            };
        }

        public static IEnumerable<DocumentDto> MapToDtos(IEnumerable<DocumentModel> documentModels)
        {
            return documentModels.Select(MapToDto);
        }
    }
}