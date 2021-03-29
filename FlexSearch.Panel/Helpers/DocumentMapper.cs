using System;
using FlexSearch.Panel.Models.CoreModels;
using FlexSearch.Panel.Models.ViewModels;

namespace FlexSearch.Panel.Helpers
{
    public static class DocumentMapper
    {
        public static DocumentModel MapDocumentModel(DocumentViewModel viewModel)
        {
            return new DocumentModel
            {
                Id = viewModel.Id ?? Guid.NewGuid(),
                Value = viewModel.Json
            };
        }

        public static DocumentViewModel MapViewModel(DocumentModel documentModel, string db = null, string index = null)
        {
            return new()
            {
                Id = documentModel.Id,
                Value = documentModel.Value.ToString(),
                DbName = db,
                Index = index
            };
        }
    }
}