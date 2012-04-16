﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using Get.Model.Graph;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml.Linq;

namespace Get.UI
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Get.UI.GraphVisualization"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Get.UI.GraphVisualization;assembly=Get.UI.GraphVisualization"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    [ContentProperty("Graph")]
    public partial class GraphVisualization : Canvas
    {
        #region Members
        /// <summary>
        /// Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.
        /// http://msdn.microsoft.com/en-us/library/system.random.aspx?queryresult=true
        /// </summary>
        protected Random _Random = new Random(DateTime.Now.Millisecond);

        public static RoutedCommand AddVertex = new RoutedCommand();

        public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
        "MouseDoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GraphVisualization));

        #endregion

        static GraphVisualization()
        {
            //set the backgroundcolor 
            BackgroundProperty.OverrideMetadata(typeof(GraphVisualization), new FrameworkPropertyMetadata(Brushes.Transparent));
        }
        /// <summary>
        /// Called before the MouseLeftButtonDown event occurs.
        /// Preprares the drag and drop of the Vertex item. Raises the MouseDoubleClickEvent if the click counts equal two.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.ClickCount.Equals(2) && VisualTreeHelper.HitTest(this, e.GetPosition(this)).VisualHit.Equals(this))
            {
                RaiseMouseDoubleClickEvent();
            }

            if (e.Source != null && e.Source.GetType().Equals(typeof(VertexVisualization)) && e.LeftButton.Equals(MouseButtonState.Pressed) && SelectedItem == null)
            {
                this.SelectedItem = (FrameworkElement)e.Source;
                this.SelectedItem.Focus();
                Point p = new Point((e.GetPosition(this).X - SelectedItem.ActualWidth / 2), (e.GetPosition(this).Y - SelectedItem.ActualHeight / 2));
                setPosition(SelectedItem, p);
            }
        }
        /// <summary>
        /// Called before the MouseMove event occurs. Moves the Vertex item.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton.Equals(MouseButtonState.Pressed) && SelectedItem != null && SelectedItem.GetType().Equals(typeof(VertexVisualization)))
            {
                VertexVisualization v = SelectedItem as VertexVisualization;
                Point p = new Point((e.GetPosition(this).X - SelectedItem.ActualWidth / 2), (e.GetPosition(this).Y - SelectedItem.ActualHeight / 2));
                v.Position = p;
                setPosition(SelectedItem, p);
            }
        }
        /// <summary>
        /// Called before the MouseLeftButtonUp event occurs. Exits the drag and drop operation of the vertex item.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (e.Source != null && e.LeftButton.Equals(MouseButtonState.Released) && SelectedItem != null && SelectedItem.GetType().Equals(typeof(VertexVisualization)))
            {
                VertexVisualization v = SelectedItem as VertexVisualization;
                Point p = new Point((e.GetPosition(this).X - SelectedItem.ActualWidth / 2), (e.GetPosition(this).Y - SelectedItem.ActualHeight / 2));
                v.Position = p;
                setPosition(SelectedItem, p);
                SelectedItem = null;
            }
        }
        protected FrameworkElement SelectedItem { get; set; }


        /// <summary>
        /// Measures the size of the current Canvas for the layout.
        /// http://msdn.microsoft.com/en-us/library/hh401019.aspx?queryresult=true
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();
            foreach (UIElement element in Children)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }

            // add some extra margin
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        /// <summary>
        /// Creates for each Vertex and Edge the proper control to display the Graph.
        /// </summary>
        /// <param name="vertices"></param>
        protected virtual void InitialiseGraph(ObservableCollection<Vertex> vertices)
        {
            InitialiseGraph(vertices, null);
        }
        /// <summary>
        /// Creates for each Vertex a VertexVisualization and for each Edge a EdgeVisualization Control. 
        /// </summary>
        /// <param name="vertices">List of Vertices</param>
        /// <param name="e">The last added EdgeVisualization. EdgeVisualization.VertexVisualizationV will be set.</param>
        protected virtual void InitialiseGraph(ObservableCollection<Vertex> vertices, EdgeVisualization e)
        {
            foreach (Vertex a in vertices)
            {
                VertexVisualization u;
                bool vertexexists = getItem(a)==null ? false : true;

                if (!vertexexists)
                {
                    //a.Edges.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
                    u = addVertex(a);
                    Canvas.SetZIndex(u, 1);
                }
                else
                {
                    u = VertexVisualizations.Where(g => g.Vertex.Equals(a)).First();
                }

                if (e != null)
                {
                    e.PositionV = getPosition(u);
                    e.VertexVisualizationV = u;

                    Binding binding = new Binding("Position");
                    binding.Source = u;
                    binding.Mode = BindingMode.TwoWay;
                    binding.NotifyOnSourceUpdated = true;
                    binding.NotifyOnTargetUpdated = true;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Converter = Converters.PointAdderConverter;
                    binding.ConverterParameter = new Point(u.Width / 2, u.Height / 2);
                    e.SetBinding(EdgeVisualization.PositionVProperty, binding);

                    Canvas.SetZIndex(e, -1);
                }

                if (vertexexists) return;


                foreach (Edge ed in a.Edges)
                {
                    EdgeVisualization edv = new EdgeVisualization() { Edge = ed };
                    edv.PositionU = getPosition(u);
                    edv.VertexVisualizationU = u;

                    Binding binding = new Binding("Position");
                    binding.Source = u;
                    binding.Mode = BindingMode.TwoWay;
                    binding.NotifyOnSourceUpdated = true;
                    binding.NotifyOnTargetUpdated = true;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Converter = Converters.PointAdderConverter;
                    binding.ConverterParameter = new Point(u.Width / 2, u.Height / 2);
                    edv.SetBinding(EdgeVisualization.PositionUProperty, binding);

                    InitialiseGraph(new ObservableCollection<Vertex>() { ed.V }, edv);

                    this.Children.Add(edv);
                }
            }
        }

        private static void OnGraphChanged(DependencyObject pDependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null && pDependencyObject != null && pDependencyObject.GetType().Equals(typeof(GraphVisualization)))
            {
                GraphVisualization graphVisualization = pDependencyObject as GraphVisualization;
                graphVisualization.Children.Clear();
                return;
            }
            if(e.NewValue.GetType() != (typeof(Graph))) return;
            if (pDependencyObject != null && pDependencyObject.GetType().Equals(typeof(GraphVisualization)))
            {
                GraphVisualization graphVisualization = pDependencyObject as GraphVisualization;
                Graph graph = e.NewValue as Graph;

                //graphVisualization.Graph.Vertices.CollectionChanged += new NotifyCollectionChangedEventHandler(graphVisualization.CollectionChanged);

                graphVisualization.InitialiseGraph(graph.Vertices);
            }
        }
        /// <summary>
        /// Adds a Vertex to the _Canvas.
        /// Important to know is that the VertexVisualization will be encapsulated by a ContentControl which is using the DesignerItemTemplate ControlTemplate. 
        /// The DesignerItemTemplate contains the MoveAbelItem which enables dragging on the "VertexVisualization" control.
        /// The position of VertexVisualization will be set randomly on the canvas.
        /// </summary>
        /// <param name="v">Vertex which should be added to the VertexVisualization</param>
        /// <returns>Returns the created VertexVisualization</returns>
        protected virtual VertexVisualization addVertex(Vertex v)
        {
            return addVertex(v, new Point(GetRandomNumber(0, this.ActualWidth - 10), GetRandomNumber(0, this.ActualHeight - 10)));
        }
        /// <summary>
        /// Adds a Vertex to the _Canvas.
        /// Important to know is that the VertexVisualization will be encapsulated by a ContentControl which is using the DesignerItemTemplate ControlTemplate. 
        /// The DesignerItemTemplate contains the MoveAbelItem which enables dragging on the "VertexVisualization" control.
        /// </summary>
        /// <param name="v">Vertex which should be added to the VertexVisualization</param>
        /// <param name="point">Sets the position where the VertexVisualization should be placed on the _Canvas</param>
        /// <returns>Returns the created VertexVisualization</returns>
        protected virtual VertexVisualization addVertex(Vertex v, Point point)
        {
            VertexVisualization vertexcontrol = new VertexVisualization();
            vertexcontrol.Vertex = v;
            
            vertexcontrol.Position = point;
            SetLeft(vertexcontrol, point.X);
            SetTop(vertexcontrol, point.Y);

            this.Children.Add(vertexcontrol);

            return vertexcontrol;
        }
        /// <summary>
        /// Returns the position of the delivered VertexVisualization control
        /// </summary>
        /// <param name="u">From which VertexVisualization the position should be returned</param>
        /// <returns>Position of the VertexVisualization control</returns>
        protected virtual Point getPosition(FrameworkElement u)
        {
            double left = Canvas.GetLeft(u as UIElement);
            double top = Canvas.GetTop(u as UIElement);
            return new Point(left + (u.Width / 2), top + (u.Height / 2));
        }
        protected virtual void setPosition(FrameworkElement f, Point p)
        {
            Canvas.SetLeft(f, p.X);
            Canvas.SetTop(f, p.Y);
        }
        /// <summary>
        /// Searches for the overgiven vertex and returns the vertexVisualization control which is representing it
        /// </summary>
        /// <param name="v">Vertex which should be looked up</param>
        /// <returns>Control which is using the Vertex</returns>
        protected virtual VertexVisualization getItem(Vertex v)
        {
            return VertexVisualizations.Where(a => a.Vertex.Equals(v)).FirstOrDefault<VertexVisualization>();
        }
        /// <summary>
        /// Searches for the overgiven edge and returns the EdgeVisualization control which is representing it
        /// </summary>
        /// <param name="v">edge which should be looked up</param>
        /// <returns>Control which is using the edge</returns>
        protected virtual EdgeVisualization getItem(Edge e)
        {
            return EdgeVisualizations.Where(a => a.Edge.Equals(e)).FirstOrDefault<EdgeVisualization>();
        }
        /// <summary>
        /// Calls the focus method on the VertexVisualization control
        /// </summary>
        /// <param name="v"></param>
        public virtual void setFocus(Vertex v)
        {
            if (getItem(v) != null)
                getItem(v).Focus();
        }
        /// <summary>
        /// Calls the focus method on the VertexVisualization control
        /// </summary>
        /// <param name="v"></param>
        public virtual void setFocus(Edge e)
        {
            if (getItem(e) != null)
                getItem(e).Focus();
        }

        /// <summary>
        /// Generates a random number with the parameter minimum and maximum
        /// </summary>
        /// <param name="minimum">Minimum random value</param>
        /// <param name="maximum">Maximum random value</param>
        /// <returns>Randoum number between minimum and maximum</returns>
        private double GetRandomNumber(double minimum, double maximum)
        {
            return _Random.NextDouble() * (maximum - minimum) + minimum;
        }

        public Graph Graph
        {
            get { return (Graph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Graph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof(Graph), typeof(GraphVisualization), new UIPropertyMetadata(null, OnGraphChanged));

        /// <summary>
        /// This method raises the MouseDoubleClick event
        /// </summary>
        private void RaiseMouseDoubleClickEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(GraphVisualization.MouseDoubleClickEvent);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// MouseDoubleClick CLR accessors for the event
        /// </summary>
        public event RoutedEventHandler MouseDoubleClick
        {
            add { AddHandler(MouseDoubleClickEvent, value); }
            remove { RemoveHandler(MouseDoubleClickEvent, value); }
        }
        /// <summary>
        /// Represents a dynamic data collection of EdgeVisualizations that provides notifications when items get added, removed, or when the whole list is refreshed.
        /// http://msdn.microsoft.com/en-us/library/ms668604.aspx?queryresult=true
        /// </summary>
        public IEnumerable<EdgeVisualization> EdgeVisualizations
        {
            get
            {
                return Children.OfType<EdgeVisualization>();
            }
        }
        /// <summary>
        /// Represents a dynamic data collection of VertexVisualizations that provides notifications when items get added, removed, or when the whole list is refreshed.
        /// http://msdn.microsoft.com/en-us/library/ms668604.aspx?queryresult=true
        /// </summary>
        public IEnumerable<VertexVisualization> VertexVisualizations
        {
            get
            {
                return Children.OfType<VertexVisualization>();
            }
        }


    }
}
