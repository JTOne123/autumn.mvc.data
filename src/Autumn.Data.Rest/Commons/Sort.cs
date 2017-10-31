namespace Autumn.Data.Rest.Commons
{
    public class Sort
    {
        private readonly SortDirection _direction;
        private readonly string _property;

        public static Sort Asc(string property)
        {
            return new Sort(property,SortDirection.Asc);
        }

        public static Sort Desc(string property)
        {
            return new Sort(property,SortDirection.Desc);
        }

        private Sort(string property, SortDirection direction)
        {
            _property = property;
            _direction = direction;
        }

        public SortDirection Direction => _direction;
        
        public string Property => _property;

        public bool IsAscending => Direction == SortDirection.Asc;

        public bool IsDescending => Direction == SortDirection.Desc;
    }
}