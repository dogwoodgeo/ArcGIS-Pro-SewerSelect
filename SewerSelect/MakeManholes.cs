﻿using System;
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
    internal class MakeManholes : Button
    {
        protected override void OnClick()
        {
            QueuedTask.Run(() =>
            {
                //Get the active map view.
                var mapView = MapView.Active.Map;

                var linesExists = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().Any(s => s.Name == "Manholes");

                if (linesExists)
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

                else
                {
                    MessageBox.Show("There is no layer named 'Manholes' in map. " +
                        "\n\nIf a manholes layer is present, make sure the layer is named 'Manholes'. " +
                        "This tool will not work unless the layer is spelled exactly like above.", "Warning");
                }
            });
        }
    }
}