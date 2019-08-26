using MyProject.STS.Identity.Configuration.Intefaces;

namespace MyProject.STS.Identity.Configuration
{
    public class RegisterConfiguration : IRegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
    }
}
