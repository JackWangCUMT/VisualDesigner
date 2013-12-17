using System.Collections.Generic;
using Glass.Design.Pcl.CanvasItem;

namespace Glass.Design.Pcl.DesignSurface.VisualAids.Snapping
{
    public class NoEffectsCanvasItemSnappingEngine : NoEffectsSnappingEngine, ICanvasItemSnappingEngine
    {
        public IEnumerable<ICanvasItem> Items { get; set; }
    }
}