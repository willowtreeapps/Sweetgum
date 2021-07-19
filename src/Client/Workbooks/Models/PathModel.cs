// <copyright file="PathModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace WillowTree.Sweetgum.Client.Workbooks.Models
{
    /// <summary>
    /// A path for a folder or a request.
    /// </summary>
    public sealed class PathModel : IEquatable<PathModel>
    {
        /// <summary>
        /// The root path model.
        /// </summary>
        public static readonly PathModel Root = new(new List<string>());

        /// <summary>
        /// Initializes a new instance of the <see cref="PathModel"/> class.
        /// </summary>
        /// <param name="segments">A read-only list of path segments.</param>
        public PathModel(IReadOnlyList<string> segments)
        {
            this.Segments = segments;
        }

        /// <summary>
        /// Gets the path segments.
        /// </summary>
        public IReadOnlyList<string> Segments { get; }

        /// <summary>
        /// Compares two path models and returns true if they are equal.
        /// </summary>
        /// <param name="left">The left path model.</param>
        /// <param name="right">The right path model.</param>
        /// <returns>A value indicating whether or not the two paths are equal.</returns>
        public static bool operator ==(PathModel? left, PathModel? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two paths and returns true if they are not equal.
        /// </summary>
        /// <param name="left">The left path model.</param>
        /// <param name="right">The right path model.</param>
        /// <returns>A value indicating whether or not the two paths are not equal.</returns>
        public static bool operator !=(PathModel? left, PathModel? right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Checks if this path model is equal to another path model.
        /// </summary>
        /// <param name="other">The other path model.</param>
        /// <returns>A value indicating whether or not the path models are equal.</returns>
        public bool Equals(PathModel? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Segments.SequenceEqual(other.Segments);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is PathModel other && this.Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            foreach (var segment in this.Segments)
            {
                hashCode.Add(segment);
            }

            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Checks if the path is referring to the implicit root path.
        /// </summary>
        /// <returns>A value indicating if the path is the root path.</returns>
        public bool IsRoot()
        {
            return this.Segments.Count == 0;
        }

        /// <summary>
        /// Gets the parent path to this path.
        /// </summary>
        /// <returns>The parent path, or null if this is the root path.</returns>
        public PathModel GetParent()
        {
            var segmentCount = this.Segments.Count;

            if (segmentCount <= 0)
            {
                throw new Exception("This is already the root path and has no parent.");
            }

            return new PathModel(this.Segments.Take(segmentCount - 1).ToList());
        }

        /// <summary>
        /// Adds a segment to the end of the path.
        /// </summary>
        /// <param name="segment">The segment to add.</param>
        /// <returns>An instance of <see cref="PathModel"/>.</returns>
        public PathModel AddSegment(string segment)
        {
            return new(this.Segments.Append(segment).ToList());
        }

        /// <summary>
        /// Checks if this path starts with another path.
        /// </summary>
        /// <param name="checkPath">The path to check for at the start of this path.</param>
        /// <returns>A value indicating whether or not the path starts with the specified path.</returns>
        public bool StartsWith(PathModel checkPath)
        {
            var checkSegmentCount = checkPath.Segments.Count;

            if (checkSegmentCount <= 0)
            {
                return true;
            }

            var segmentCount = this.Segments.Count;

            if (checkSegmentCount > segmentCount)
            {
                return false;
            }

            for (var segmentIndex = 0; segmentIndex < checkSegmentCount; segmentIndex++)
            {
                if (this.Segments[segmentIndex] != checkPath.Segments[segmentIndex])
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsRoot())
            {
                return "<root>";
            }

            return string.Join('/', this.Segments);
        }
    }
}
