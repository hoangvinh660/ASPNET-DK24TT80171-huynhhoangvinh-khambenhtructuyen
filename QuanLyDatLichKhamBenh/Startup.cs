using Microsoft.Owin;
using Owin;
using QuanLyDatLichKhamBenh;

[assembly: OwinStartup(typeof(Startup))]

namespace QuanLyDatLichKhamBenh
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IdentityConfig.ConfigureAuth(app);
        }
    }
}