
using System.Collections.Generic;

namespace Autumn.Mvc.Data.Models.Paginations
{
    public class Page<T> where T :class
    {
        
        public List<T> Content { get;}
        public long TotalElements { get; }
        public int Number { get; }
        public int NumberOfElements { get; }
        public int TotalPages { get; }
        public bool HasContent { get; }
        public bool HasNext { get; }
        public bool HasPrevious { get; }
        public bool IsFirst { get; }
        public bool IsLast { get; }

        public Page() : this(null, null)
        {
        }

        public Page(List<T> content, Pageable<T> pageable, long? total = null)
        {
            Content = content ?? new List<T>();
            if (total == null)
            {
                TotalElements = Content.Count;
            }
            else
            {
                TotalElements = total.Value < (long) Content.Count ? Content.Count : total.Value;
            }
            HasContent = Content.Count > 0;
            if (HasContent)
            {
                NumberOfElements = Content.Count;
            }
            if (pageable == null) return;
            Number = pageable.PageNumber;
            IsFirst = pageable.PageNumber == 0;
            HasPrevious = !IsFirst;
            HasNext = TotalElements > NumberOfElements + Number * pageable.PageSize;
            IsLast = !HasNext;
            if (TotalElements <= 0) return;
            var mod = (int) TotalElements % pageable.PageSize;
            var quo = ((int) TotalElements) - mod;
            TotalPages = (quo / pageable.PageSize) + (mod > 0 ? 1 : 0);
        }
    }
}