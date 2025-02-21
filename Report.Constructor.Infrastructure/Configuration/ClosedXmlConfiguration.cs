using ClosedXML.Excel;
using ClosedXML.Graphics;
using System.Reflection;

namespace Report.Constructor.Infrastructure.Configuration
{
    internal static class ClosedXmlConfiguration
    {
        public static void ConfigureClosedXml()
        {
            var assembly = Assembly.GetAssembly(typeof(DependencyInjection));
            var fontResourceName = assembly!
                .GetManifestResourceNames()
                .First(rn => rn.Contains("Font"));

            using var fontStream = assembly.GetManifestResourceStream(fontResourceName);

            LoadOptions.DefaultGraphicEngine = DefaultGraphicEngine.CreateOnlyWithFonts(fontStream);
        }
    }
}