using System;
using System.Linq;
using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace SewerSelect
{
    internal class MakeSewers : Button
    {
        protected override void OnClick()
        {
            QueuedTask.Run(() =>
            {
                var mapView = MapView.Active.Map;
                var mhExists = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().Any(m => m.Name == "Manholes");
                var sewerExists = mapView.GetLayersAsFlattenedList().OfType<FeatureLayer>().Any(s => s.Name == "Sewer Lines");
                if (mhExists == false && sewerExists == false)
                {
                    MessageBox.Show("Manholes & Sewers are missing from map.", "Message");
                }
                else if (mhExists == false && sewerExists)
                {
                    MessageBox.Show("Sewer Lines layer is present. \n\nManholes layer is missing from map.", "Message");
                }
                else if (mhExists && sewerExists == false)
                {
                    MessageBox.Show("Manholes layer is present. \n\nSewers layer is missing from map.", "Message");
                }

                else
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
            });
        }
    }
}
