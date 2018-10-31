using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //// Methods to check the state/condition for make layers buttons.
        ///******I never could get this to work :( ********

        //private static readonly string mhStateID = "manholes_state";
        //internal static void ManholesState()
        //{
        //    var mapView = MapView.Active.Map;
        //    var mhLayer = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Manholes");

        //    var mhOIDList = mhLayer.GetSelection().GetObjectIDs();
        //    if (mhOIDList.Count > 0)

        //    {
        //        FrameworkApplication.State.Activate(mhStateID);
        //    }

        //    else
        //    {
        //        FrameworkApplication.State.Deactivate(mhStateID);
        //    }
        //}


        //public static void LinesState()
        //{
        //    var mapView = MapView.Active.Map;
        //    var linesLayer = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Sewer Lines");

        //    var linesOIDList = linesLayer.GetSelection().GetObjectIDs();
        //    if (linesOIDList.Count > 0)
        //    {
        //        ;
        //    }

        //    else
        //    {
        //        ;
        //    }
        //}

        ////public static void SewersState()
        ////{
        ////    var mapView = MapView.Active.Map;
        ////    var mhLayer = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Manholes");
        ////    var linesLayer = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Sewer Lines");

        ////    //Get the selected features from the map.
        ////    var mhOIDList = mhLayer.GetSelection().GetObjectIDs();
        ////    var linesOIDList = linesLayer.GetSelection().GetObjectIDs();

        ////    if (mhOIDList.Count() == 0 || linesOIDList.Count() == 0)
        ////    {
        ////        ;
        ////    }

        ////    else 
        ////    {
        ////        ;
        ////    }
        ////}


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
        


        // Methods to create the selection layer.  
        public static void MakeSewersLayers(Map mapView)
        {
            try
            { 
                var mh = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Manholes");
                var lines = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Sewer Lines");

                //Get the selected features from the map.
                var mhOIDList = mh.GetSelection().GetObjectIDs();// Gets a list of Object IDs
                var mhOID = mh.GetTable().GetDefinition().GetObjectIDField();// Gets the OBJECTID field name for the def query
                var linesOIDList = lines.GetSelection().GetObjectIDs();
                var linesOID = lines.GetTable().GetDefinition().GetObjectIDField();

                //Check to see if there are mmanhole or sewer line features selected in map.
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

                    // CREATE THE SEWER LINES SELECTION LAYER
                    // Create the defenition query
                    string linesDefQuery = $"{linesOID} in ({string.Join(",", linesOIDList)})";
                    string linesURL = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.SEWERS_VIEW";

                    // Create the Uri object create the feature layer.
                    Uri linesURI = new Uri(linesURL);
                    var linesSelLayer = LayerFactory.Instance.CreateFeatureLayer(linesURI, mapView, 0, "Sewer Lines SELECTION");
                    
                    // Apply the definition query
                    linesSelLayer.SetDefinitionQuery(linesDefQuery);

                    // Create the line symbol renderer.
                    CIMLineSymbol lineSymbol = SymbolFactory.Instance.ConstructLineSymbol(
                        ColorFactory.Instance.RedRGB,
                        3.0,
                        SimpleLineStyle.Solid
                        );
                    CIMSimpleRenderer lineRenderer = linesSelLayer.GetRenderer() as CIMSimpleRenderer;

                    // Renference the existing renderer
                    lineRenderer.Symbol = lineSymbol.MakeSymbolReference();
                    // Apply the new renderer
                    linesSelLayer.SetRenderer(lineRenderer);


                    // CREATE THE MANHOLES SELECTION LAYER
                    // Create the defenition query
                    string mhDefQuery = $"{mhOID} in ({string.Join(",", mhOIDList)})";
                    string mhURL = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.MANHOLES_VIEW";
                    
                    // Create the Uri object create the feature layer.
                    Uri mhURI = new Uri(mhURL);
                    var mhSelLayer = LayerFactory.Instance.CreateFeatureLayer(mhURI, mapView, 0, "Manholes SELECTION");

                    // Apply the definition query
                    mhSelLayer.SetDefinitionQuery(mhDefQuery);
                    
                    // Create the point symbol renderer.
                    CIMPointSymbol pointSymbol = SymbolFactory.Instance.ConstructPointSymbol(
                        ColorFactory.Instance.GreenRGB,
                        8.0,
                        SimpleMarkerStyle.Circle
                        );
                    CIMSimpleRenderer pointRenderer = mhSelLayer.GetRenderer() as CIMSimpleRenderer;

                    // Renference the existing renderer
                    pointRenderer.Symbol = pointSymbol.MakeSymbolReference();
                    // Apply the new renderer
                    mhSelLayer.SetRenderer(pointRenderer);
                }
            }

            catch (Exception ex)
            {
                
                string caption = "Module1.MakeSewersLayers method failed!";
                string message = "Process failed. \n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                    $"If problem persist, contact your local GIS nerd.\n\n{ex}";

                //Using the ArcGIS Pro SDK MessageBox class
                MessageBox.Show(message, caption);
                
            }
        }

        public static void MakeManholesLayer(Map mapView)
        {
            try
            {
                // Create the "Manholes"  layer object.
                var mh = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Manholes");

                //Get the selected features from the map.
                var mhOIDList = mh.GetSelection().GetObjectIDs();// Gets a list of Object IDs
                var mhOID = mh.GetTable().GetDefinition().GetObjectIDField(); // Gets the OBJECTID field name for the def query

                // Check to see if there are manhole features selected in the map.
                if (mhOIDList.Count() == 0)
                {
                    MessageBox.Show("There are no manholes selected.", "Warning");
                }

                else
                {
                    // Create the defenition query
                    string defQuery = $"{mhOID} in ({string.Join(",", mhOIDList)})";
                    string url = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.MANHOLES_VIEW";

                    // Create the Uri object create the feature layer.
                    Uri uri = new Uri(url);
                    var selectionLayer = LayerFactory.Instance.CreateFeatureLayer(uri, mapView, 0, "Manholes SELECTION");

                    // Apply the definition query
                    selectionLayer.SetDefinitionQuery(defQuery);

                    // Create the point symbol renderer.
                    CIMPointSymbol pointSymbol = SymbolFactory.Instance.ConstructPointSymbol(
                        ColorFactory.Instance.GreenRGB,
                        8.0,
                        SimpleMarkerStyle.Circle
                        );
                    CIMSimpleRenderer renderer = selectionLayer.GetRenderer() as CIMSimpleRenderer;
                    // Reference the existing renderer
                    renderer.Symbol = pointSymbol.MakeSymbolReference();
                    // Apply new renderer
                    selectionLayer.SetRenderer(renderer);
                }
            }

            catch (Exception ex)
            {
                string caption = "Module1.MakeManholesLayer method failed!";
                string message = "Process failed. \n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                    $"If problem persist, contact your local GIS nerd.\n\n{ex}";

                //Using the ArcGIS Pro SDK MessageBox class
                MessageBox.Show(message, caption);
            }

        }

        public static void MakeLinesLayer(Map mapView)
        {
            try
            {
                // Create the "Sewer Lines" object.
                var lines = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().FirstOrDefault(s => s.Name == "Sewer Lines");
                
                //Get the selected features from the map.
                var linesOIDList = lines.GetSelection().GetObjectIDs();// Gets a list of Object IDs
                var linesOID = lines.GetTable().GetDefinition().GetObjectIDField();// Gets the OBJECTID field name for the def query

                // Check to see if there are sewer lines features selected inthe map.
                if (linesOIDList.Count() == 0)
                {
                    MessageBox.Show("There are no sewer lines selected.", "Warning");
                }

                else
                {
                    //Create the defenition query
                    string defQuery = $"{linesOID} in ({string.Join(",", linesOIDList)})";
                    string url = @"O:\SHARE\405 - INFORMATION SERVICES\GIS_Layers\GISVIEWER.SDE@SQL0.sde\SDE.SEWERMAN.SEWERS_VIEW";

                    // Create the Uri object create the feature layer.
                    Uri uri = new Uri(url);
                    var selectionLayer = LayerFactory.Instance.CreateFeatureLayer(uri, mapView, 0, "Sewer Lines SELECTION");

                    // Apply the definition query
                    selectionLayer.SetDefinitionQuery(defQuery);

                    // Create the line symbol renderer.
                    CIMLineSymbol lineSymbol = SymbolFactory.Instance.ConstructLineSymbol(
                        ColorFactory.Instance.RedRGB,
                        3.0,
                        SimpleLineStyle.Solid
                        );
                    CIMSimpleRenderer renderer = selectionLayer.GetRenderer() as CIMSimpleRenderer;

                    // Reference the existing renderer
                    renderer.Symbol = lineSymbol.MakeSymbolReference();
                    // Apply the new renderer
                    selectionLayer.SetRenderer(renderer);
                }
            }

            catch (Exception ex)
            {
                string caption = "Module1.MakeLinesLayer method failed!";
                string message = "Process failed. \n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                    $"If problem persist, contact your local GIS nerd.\n\n{ex}";

                //Using the ArcGIS Pro SDK MessageBox class
                MessageBox.Show(message, caption);
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
