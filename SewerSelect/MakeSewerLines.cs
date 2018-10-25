using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    internal class MakeSewerLines : Button
    {
        protected override void OnClick()
        {
            QueuedTask.Run(() =>
            {
                //Get the active map view.
                var mapView = MapView.Active.Map;

                var linesExists = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().Any(s => s.Name == "Sewer Lines");
                if (linesExists)
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

                else
                {
                    MessageBox.Show("There is no layer named 'Sewer Lines' in map. " +
                        "\n\nIf a sewer lines layer is present, make sure the layer is named 'Sewer Lines'. " +
                        "This tool will not work unless the layer is spelled exactly like above.", "Warning");
                }

            });

        }
    }
}
