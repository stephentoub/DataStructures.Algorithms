﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System;

namespace Get.Model.Graph
{
    [DataContract(Namespace = "http://schemas.get.com/Graph/Vertex")]
    public class Vertex : INotifyPropertyChanged
    {
        #region Members
        protected ObservableCollection<Edge> _Edges = new ObservableCollection<Edge>();
        [DataMember(Name = "Guid", Order = 3, IsRequired = true)]
        protected Guid _Guid;

        protected int weighted;
        #endregion

        /// <summary>
        /// Initializes a new instance of the Vertex class.
        /// </summary>
        public Vertex() { _Guid = Guid.NewGuid(); }

        /// <summary>
        /// Initializes a new instance of the Vertex class that contains the specified weighted.
        /// </summary>
        /// <param name="pweighted"></param>
        public Vertex(int pweighted) :this()
        {
            weighted = pweighted;
        }
        /// <summary>
        /// Gets or sets the Weighted of the vertex
        /// </summary>
        [DataMember(Name = "Weighted", Order = 1, IsRequired = true)]
        public int Weighted { get { return weighted; } set { weighted = value; NotifyPropertyChanged("Weighted"); } }

        /// <summary>
        /// Gets or sets the list of edges which connects the vertex neighbours
        /// </summary>
        [DataMember(Name = "Edges", Order = 2, IsRequired = true)]
        public ObservableCollection<Edge> Edges { get { return _Edges; } set { _Edges = value; NotifyPropertyChanged("Edges"); } }

        /// <summary>
        /// Amount of neighbours
        /// </summary>
        public int VertexSize
        {
            get
            {
                return Edges.Count; //Knotengrad
            }
        }

        /// <summary>
        /// Adds a vertex by adding a connection from the instance to the overgiven vertex
        /// </summary>
        /// <param name="pu">The vertex which should be added to the instance</param>
        public void addEdge(Vertex pu)
        {
            _Edges.Add(new Edge(this, pu));
        }
        /// <summary>
        /// Adds a vertex by adding a connection from the instance to the overgiven vertex
        /// </summary>
        /// <param name="pu">The vertex which should be added to the instance</param>
        /// <param name="pweighted">Weighted of the added vertex</param>
        public void addEdge(Vertex pu, int pweighted)
        {
            _Edges.Add(new Edge(this, pu, pweighted));
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return base.ToString() + string.Empty + Weighted;
        }
        /// <summary>
        /// Determines with the guid whether the specified Object is equal to the current Object.
        /// http://msdn.microsoft.com/en-us/library/bsc2ak47.aspx
        /// </summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if(!obj.GetType().Equals(typeof(Vertex))) return false;

            return this._Guid.Equals((obj as Vertex)._Guid);
        }
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>A hash code for the current Object.</returns>
        public override int GetHashCode()
        {
            return _Guid.GetHashCode();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify using String property name
        /// </summary>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


    }
}
