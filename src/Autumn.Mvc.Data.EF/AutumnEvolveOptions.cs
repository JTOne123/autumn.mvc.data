namespace Autumn.Mvc.Data.EF
{
    public class AutumnEvolveOptions 
    {
        public bool CreateIfNotExist { get; set; }
        public string AdminPassword { get; set; }

        public AutumnEvolveOptions()
        {
            CreateIfNotExist = true;
        }
    }
}