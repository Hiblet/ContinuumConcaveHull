using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlteryxGuiToolkit.Plugins;

namespace ContinuumConcaveHull
{
    public class ConcaveHull : IPlugin
    {
        private System.Drawing.Bitmap _icon;
        private string _iconResource = "ContinuumConcaveHull.Resources.ConcaveHull_171.png";


        public IPluginConfiguration GetConfigurationGui()
        {
            return new ConcaveHullUserControl();
        }

        public EntryPoint GetEngineEntryPoint()
        {
            return new AlteryxGuiToolkit.Plugins.EntryPoint("ContinuumConcaveHull.dll", "ContinuumConcaveHull.ConcaveHullNetPlugin", true);
        }

        public Image GetIcon()
        {
            // DIAG
            // To see the actual name of the embedded resource, as the assembly sees it.
            var arrResources = typeof(ConcaveHull).Assembly.GetManifestResourceNames();
            // END DIAG

            if (_icon == null)
            {
                System.IO.Stream s = typeof(ConcaveHull).Assembly.GetManifestResourceStream(_iconResource);
                if (s == null)
                {
                    throw new ArgumentNullException("Could not find local resource [" + _iconResource + "]");
                }

                _icon = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(s);
                _icon.MakeTransparent();
            }

            return _icon;
        }

        public Connection[] GetInputConnections()
        {
            return new Connection[] { new Connection("Input") };
        }

        public Connection[] GetOutputConnections()
        {
            return new Connection[] { new Connection("Output") };

        }
    }
}
