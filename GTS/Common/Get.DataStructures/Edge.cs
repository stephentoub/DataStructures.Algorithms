﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Get.DataStructure
{
    [DebuggerDisplay("Edge = {Weighted},U={U}, V = {V}")]
    public class Edge<W> : IEdge<W> where W : IComparable<W>
    {
        #region Members
        protected IVertex<W> u;
        protected IVertex<W> v;
        protected W weight;
        #endregion

        /// <summary>
        /// Initializes a new instance of the Edge class.
        /// </summary>
        /// <param name="pu">Vertex of the Edge</param>
        /// <param name="pv">Vertex of the Edge</param>
        public Edge(IVertex<W> pu, IVertex<W> pv)
        {
            u = pu;
            v = pv;
        }
        /// <summary>
        /// Initializes a new instance of the Edge class.
        /// </summary>
        /// <param name="pu">Vertex of the Edge</param>
        /// <param name="pv">Vertex of the Edge</param>
        /// <param name="pweighted">Sets the Weighted of the Edge</param>
        public Edge(IVertex<W> pu, IVertex<W> pv, W pweight)
        {
            this.u = pu;
            this.v = pv;
            weight = pweight;
        }

        /// <summary>
        /// Get or sets the Vertex of the Edge
        /// </summary>
        public virtual IVertex<W> U { get { return u; } set { u = value; } }
        /// <summary>
        /// Get or sets the vertex of the edge
        /// </summary>
        public virtual IVertex<W> V { get { return v; } set { v = value; } }
        /// <summary>
        /// Gets or sets the weight of the edge
        /// </summary>
        public virtual W Weight { get { return weight; } set { weight = value; } }
        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are considered equal; otherwise, false.</returns>
        public sealed override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(typeof(IEdge<W>))) return false;

            //true if objA is the same instance as objB or if both are null; otherwise, false.
            if (Object.ReferenceEquals(this, obj)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(this, null) || Object.ReferenceEquals(obj, null)) return false;

            IEdge<W> edge = obj as IEdge<W>;

            return Equals(edge, false);
        }

        /// <summary>
        /// Serves as a hash function for the type edge.
        /// The implementation of the GetHashCode method does not guarantee unique return values for different objects.
        /// The HasCode will be calculated with the GetHasCode functions from the vertex u and v. Transported edges have the same values.
        /// http://msdn.microsoft.com/en-us/library/system.object.gethashcode.aspx
        /// </summary>
        /// <returns>A hash code for the current Object.</returns>
        public sealed override int GetHashCode()
        {
            return Math.Abs(U.GetHashCode()) + Math.Abs(V.GetHashCode());
        }

    }
}