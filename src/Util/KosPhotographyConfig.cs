using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kosphotography
{
    class KosPhotographyConfig
    {
        public string PhotographLodText = "The power of two used for the resolution of the picture (128x is 7, 256x is 8)";
        public int PhotographLod;

        public KosPhotographyConfig()
        { }

        public static KosPhotographyConfig Current { get; set; }

        public static KosPhotographyConfig GetDefault()
        {
            KosPhotographyConfig defaultConfig = new KosPhotographyConfig();

            defaultConfig.PhotographLodText.ToString();
            defaultConfig.PhotographLod = 7;
            return defaultConfig;
        }
    }
}

