namespace Autumn.Mvc.Data.EF.Configuration
{
    public abstract class AutumnEntityFrameworkCoreSettings
    {
        public string ConnectionString { get; set; }
        public bool Evolve { get; set; }
    }
}