using Appalachia.CI.Integration.Attributes;
using Newtonsoft.Json;

namespace Appalachia.CI.Integration.Packages.NpmModel
{
    [DoNotReorderFields]
    public static class Serialize
    {
        public static string ToJson(this NpmPackage self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }
}
