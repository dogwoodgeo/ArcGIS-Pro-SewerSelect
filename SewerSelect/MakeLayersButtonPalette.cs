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
    internal class MakeLayersButtonPalette_sewersbutton : Button
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
                    MessageBox.Show("Manholes & Sewers are missing from map.", "Warning");
                }
                else if (mhExists == false && sewerExists)
                {
                    MessageBox.Show("Sewer Lines layer is present. \n\nManholes layer is missing from map.", "Warning");
                }
                else if (mhExists && sewerExists == false)
                {
                    MessageBox.Show("Manholes layer is present. \n\nSewers layer is missing from map.", "Warning");
                }

                else
                {
                    Module1.MakeSewersLayers(mapView);                
                }
            });

        }
    }

    internal class MakeLayersButtonPalette_manholesbutton : Button
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
                    Module1.MakeManholesLayer(mapView);
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

    internal class MakeLayersButtonPalette_linesbutton : Button
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
                    Module1.MakeLinesLayer(mapView);
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
