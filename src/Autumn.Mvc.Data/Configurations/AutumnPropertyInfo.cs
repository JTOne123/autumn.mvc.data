namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnPropertyInfo
    {
        public bool Insertable { get; set; }
        public bool Updatable { get; set; }

        public AutumnPropertyInfo()
        {
            Insertable = true;
            Updatable = true;
        }
    }
}