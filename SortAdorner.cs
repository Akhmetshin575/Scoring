using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Scoring3
{
    public class SortAdorner : Adorner
    {
        static Geometry ascendingGeometry = Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");
        static Geometry descendingGeometry = Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection sortDirection { get; private set; }

        public SortAdorner(UIElement elementUI, ListSortDirection direction) : base(elementUI)
        {
            this.sortDirection = direction;
        }

        protected override void OnRender(DrawingContext context)
        {
            base.OnRender(context);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transformation = new TranslateTransform
                (
                    AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5) / 2
                );
            context.PushTransform(transformation);

            Geometry  geometry1 = ascendingGeometry;
            if (this.sortDirection == ListSortDirection.Descending) geometry1 = descendingGeometry;
            context.DrawGeometry(Brushes.Black, null, geometry1);

            context.Pop();
        }
    }
}
