using System;

namespace WebApplication1.Repositories
{
    public class Sort
    {
        private readonly SortDirection _direction;
        private readonly string _property;

        public static Sort Asc(string property)
        {
            return new Sort(property,SortDirection.ASC);
        }

        public static Sort Desc(string property)
        {
            return new Sort(property,SortDirection.DESC);
        }

        private Sort(string property, SortDirection direction)
        {
            _property = property;
            _direction = direction;
        }

        public SortDirection Direction => _direction;
        
        public string Property => _property;

        public bool IsAscending => Direction == SortDirection.ASC;

        public bool IsDescending => Direction == SortDirection.DESC;
    }
}