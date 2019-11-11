using Abp.Modules;
using Abp.Reflection.Extensions;

namespace PQ
{
    public class PQModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PQModule).GetAssembly());

        }

        public override void PostInitialize()
        {
        }
    }
}
