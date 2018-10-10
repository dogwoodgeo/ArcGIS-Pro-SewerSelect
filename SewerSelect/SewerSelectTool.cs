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
    internal class SewerSelectTool : MapTool
    {
        public SewerSelectTool()
        {
            IsSketchTool = true;
            SketchType = SketchGeometryType.Lasso;
            SketchOutputMode = SketchOutputMode.Screen;
        }

        protected override Task OnToolActivateAsync(bool active)
        {
            return QueuedTask.Run(() =>
            {
                try
                {
                    var map = MapView.Active.Map;
                    var mhExists = map.GetLayersAsFlattenedList().OfType<FeatureLayer>().Any(m => m.Name == "Manholes");
                    var sewerExists = map.GetLayersAsFlattenedList().OfType<FeatureLayer>().Any(s => s.Name == "Sewer Lines");
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
                        var layers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>();
                        foreach (var layer in layers)
                        {
                            if (layer.Name == "Manholes" || layer.Name == "Sewer Lines")
                            //if (layer.Name == "Manholes")
                            {
                                layer.SetSelectable(true);
                            }

                            else
                            {
                                layer.SetSelectable(false);
                            }
                        }
                    }
                }

                catch (Exception)
                {
                    string caption = "Failed to add data";
                    string message = "Process failed.\n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                        "If problem persist, contact your local GIS nerd.";

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
                    ActiveMapView.SelectFeatures(geometry, SelectionCombinationMethod.Add);       
                }

                catch (Exception)
                {
                    string caption = "Failed to get street view";
                    string message = "Process failed. \n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                        "If problem persist, contact your local GIS nerd.";

                    //Using the ArcGIS Pro SDK MessageBox class
                    MessageBox.Show(message, caption);
                }
                return true;
            });
        }
    }
}
