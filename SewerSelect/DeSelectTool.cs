using System;
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
    internal class DeSelectTool : MapTool
    {
        public DeSelectTool()
        {
            IsSketchTool = true;
            SketchType = SketchGeometryType.Point;
            SketchOutputMode = SketchOutputMode.Screen;
        }

        protected override Task OnToolActivateAsync(bool active)
        {
            return QueuedTask.Run(() =>
           {
               try
               {
                   var mapView = MapView.Active.Map;
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

                   else
                   {
                       base.OnToolActivateAsync(active);
                   }
               }

               catch (Exception ex)
               {
                   string caption = "Failed to de-select features!";
                   string message = $"Process failed. \n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                       $"If problem persist, contact your local GIS nerd.\n\n{ex}";

                   //Using the ArcGIS Pro SDK MessageBox class
                   MessageBox.Show(message, caption);

               }

            });

        }

        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            return QueuedTask.Run(() =>
            {
                try
                {
                    ActiveMapView.SelectFeatures(geometry, SelectionCombinationMethod.Subtract);
                }

                catch (Exception ex)
                {
                    string caption = "Failed to de-select features!";
                    string message = "Process failed. \n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                        $"If problem persist, contact your local GIS nerd.\n\n{ex}";

                    //Using the ArcGIS Pro SDK MessageBox class
                    MessageBox.Show(message, caption);
                }
                return true;
            });
            
        }
    }
}
