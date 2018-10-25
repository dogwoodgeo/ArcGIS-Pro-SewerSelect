using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace SewerSelect
{
    internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current
        {
            get
            {
                return _this ?? (_this = (Module1)FrameworkApplication.FindModule("SewerSelect_Module"));
            }
        }

        public static void MakeSewersLayers(Map mapView)
        {
            var mh = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Manholes");
            var lines = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Sewer Lines");

            //Get the selected features from the map.
            var mhOIDList = mh.GetSelection().GetObjectIDs();
            var mhOID = mh.GetTable().GetDefinition().GetObjectIDField();
            var linesOIDList = lines.GetSelection().GetObjectIDs();
            var linesOID = lines.GetTable().GetDefinition().GetObjectIDField();

            if (mhOIDList.Count() == 0 && linesOIDList.Count() == 0)
            {
                MessageBox.Show("Manholes & Sewers contain no selected features.", "Warning");
            }

            else if (mhOIDList.Count() == 0 && linesOIDList.Count() > 0)
            {
                MessageBox.Show("Manholes layer contains no selected features.", "Warning");
            }

            else if (mhOIDList.Count() > 0 && linesOIDList.Count() == 0)
            {
                MessageBox.Show("Sewer Lines layer contains no selected features.", "Warning");
            }

            else
            {
                string linesDefQuery = $"{linesOID} in ({string.Join(",", linesOIDList)})";
                string linesURL = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.SEWERS_VIEW";

                Uri linesURI = new Uri(linesURL);
                var linesSelLayer = LayerFactory.Instance.CreateFeatureLayer(linesURI, mapView, 0, "Sewer Lines SELECTION");

                linesSelLayer.SetDefinitionQuery(linesDefQuery);

                CIMLineSymbol lineSymbol = SymbolFactory.Instance.ConstructLineSymbol(
                    ColorFactory.Instance.RedRGB,
                    3.0,
                    SimpleLineStyle.Solid
                    );

                CIMSimpleRenderer lineRenderer = linesSelLayer.GetRenderer() as CIMSimpleRenderer;
                lineRenderer.Symbol = lineSymbol.MakeSymbolReference();
                linesSelLayer.SetRenderer(lineRenderer);

                string mhDefQuery = $"{mhOID} in ({string.Join(",", mhOIDList)})";
                string mhURL = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.MANHOLES_VIEW";

                Uri mhURI = new Uri(mhURL);
                var mhSelLayer = LayerFactory.Instance.CreateFeatureLayer(mhURI, mapView, 0, "Manholes SELECTION");

                mhSelLayer.SetDefinitionQuery(mhDefQuery);

                CIMPointSymbol pointSymbol = SymbolFactory.Instance.ConstructPointSymbol(
                    ColorFactory.Instance.GreenRGB,
                    8.0,
                    SimpleMarkerStyle.Circle
                    );

                CIMSimpleRenderer pointRenderer = mhSelLayer.GetRenderer() as CIMSimpleRenderer;
                pointRenderer.Symbol = pointSymbol.MakeSymbolReference();
                mhSelLayer.SetRenderer(pointRenderer);

            }
        }

        public static void MakeManholesLayer(Map mapView)
        {
            var mh = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Manholes");

            //Get the selected features from the map.
            var mhOIDList = mh.GetSelection().GetObjectIDs();
            var mhOID = mh.GetTable().GetDefinition().GetObjectIDField();

            if (mhOIDList.Count() == 0)
            {
                MessageBox.Show("There are no manholes selected.", "Warning");
            }

            else
            {
                string defQuery = $"{mhOID} in ({string.Join(",", mhOIDList)})";
                string url = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.MANHOLES_VIEW";

                Uri uri = new Uri(url);
                var selectionLayer = LayerFactory.Instance.CreateFeatureLayer(uri, mapView, 0, "Manholes SELECTION");

                selectionLayer.SetDefinitionQuery(defQuery);

                CIMPointSymbol pointSymbol = SymbolFactory.Instance.ConstructPointSymbol(
                    ColorFactory.Instance.GreenRGB,
                    8.0,
                    SimpleMarkerStyle.Circle
                    );
                CIMSimpleRenderer renderer = selectionLayer.GetRenderer() as CIMSimpleRenderer;
                renderer.Symbol = pointSymbol.MakeSymbolReference();
                selectionLayer.SetRenderer(renderer);
            }

        }

        public static void MakeLinesLayer(Map mapView)
        {
            var lines = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Sewer Lines");
            //Get the selected features from the map.
            var linesOIDList = lines.GetSelection().GetObjectIDs();
            var linesOID = lines.GetTable().GetDefinition().GetObjectIDField();
            if (linesOIDList.Count() == 0)
            {
                MessageBox.Show("There are no sewer lines selected.", "Warning");
            }

            else
            {
                string defQuery = $"{linesOID} in ({string.Join(",", linesOIDList)})";
                string url = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.SEWERS_VIEW";

                Uri uri = new Uri(url);
                var selectionLayer = LayerFactory.Instance.CreateFeatureLayer(uri, mapView, 0, "Sewer Lines SELECTION");

                selectionLayer.SetDefinitionQuery(defQuery);

                CIMLineSymbol lineSymbol = SymbolFactory.Instance.ConstructLineSymbol(
                    ColorFactory.Instance.RedRGB,
                    3.0,
                    SimpleLineStyle.Solid
                    );
                CIMSimpleRenderer renderer = selectionLayer.GetRenderer() as CIMSimpleRenderer;
                renderer.Symbol = lineSymbol.MakeSymbolReference();
                selectionLayer.SetRenderer(renderer);
            }
        }

        public static bool ManholesLinesSelected(FeatureLayer mhLayer, FeatureLayer linesLayer)
        {
            //Get the selected features from the map.
            var mhOIDList = mhLayer.GetSelection().GetObjectIDs();
            var mhOID = mhLayer.GetTable().GetDefinition().GetObjectIDField();
            var linesOIDList = linesLayer.GetSelection().GetObjectIDs();
            var linesOID = linesLayer.GetTable().GetDefinition().GetObjectIDField();

            if (mhOIDList.Count() == 0 || linesOIDList.Count() == 0)
            {
                return false;
            }

            else 
            {
                return true;
            }
        }

        public static bool ManholesSelected(FeatureLayer mhLayer)
        {
            var mhOIDList = mhLayer.GetSelection().GetObjectIDs();
            var linesOID = mhLayer.GetTable().GetDefinition().GetObjectIDField();
            if (mhOIDList.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public static bool LinesSelected(FeatureLayer linesLayer)
        {
            var linesOIDList = linesLayer.GetSelection().GetObjectIDs();
            var linesOID = linesLayer.GetTable().GetDefinition().GetObjectIDField();
            if (linesOIDList.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

    }
}
