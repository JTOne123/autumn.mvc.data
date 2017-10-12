
using System.Collections.Generic;

namespace WebApplication1.Commons
{
    public class Page<T> : IPage<T>
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

        public Page(List<T> content, IPageable pageable, long total)
        {
            Content = content;
            TotalElements = total;
            HasContent = (Content != null && Content.Count > 0);
            if (HasContent)
            {
                NumberOfElements = content.Count;
            }
            if (pageable != null)
            {
                Number = pageable.PageNumber;
                IsFirst = pageable.Offset == 0;
                HasPrevious = !IsFirst;
                HasNext = false;
                if (total > NumberOfElements + Number * pageable.PageSize)
                {
                    HasNext = true;
                }
                IsLast = !HasNext;
                if (total > 0)
                {
                    var mod = (int) total % pageable.PageSize;
                    var quo = ((int) total) - mod;
                    TotalPages = (quo / pageable.PageSize) + (mod > 0 ? 1 : 0);
                }
            }
        }
    }
}