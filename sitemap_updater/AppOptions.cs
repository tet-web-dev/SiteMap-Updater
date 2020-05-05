using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sitemap_updater
{
    class AppOptions
    {
        public IConfiguration Configuration { get; set; }
    }
}
