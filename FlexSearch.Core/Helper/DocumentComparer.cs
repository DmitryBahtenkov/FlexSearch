using System;
using System.Collections;
using System.Collections.Generic;
using Core.Models;

namespace Core.Helper
{
    public class DocumentComparer : IEqualityComparer<DocumentModel>
    {
        public bool Equals(DocumentModel x, DocumentModel y)
        {
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.GetType() == y.GetType() && x.Id.Equals(y.Id);
        }

        public int GetHashCode(DocumentModel obj)
        {
            //для сравнения
            return 0;
        }
    }
}