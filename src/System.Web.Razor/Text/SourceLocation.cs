﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;

namespace System.Web.Razor.Text
{
    [Serializable]
    public struct SourceLocation : IEquatable<SourceLocation>, IComparable<SourceLocation>
    {
        public static readonly SourceLocation Undefined = CreateUndefined();
        public static readonly SourceLocation Zero = new SourceLocation(0, 0, 0);

        private int _absoluteIndex;
        private int _lineIndex;
        private int _characterIndex;

        public SourceLocation(int absoluteIndex, int lineIndex, int characterIndex)
        {
            _absoluteIndex = absoluteIndex;
            _lineIndex = lineIndex;
            _characterIndex = characterIndex;
        }

        public int AbsoluteIndex
        {
            get { return _absoluteIndex; }
        }

        // THIS IS 1-based!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// <summary>
        /// Gets the 1-based index of the line referred to by this Source Location.
        /// </summary>
        public int LineIndex
        {
            get { return _lineIndex; }
        }

        public int CharacterIndex
        {
            get { return _characterIndex; }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "({0}:{1},{2})", AbsoluteIndex, LineIndex, CharacterIndex);
        }

        public override bool Equals(object obj)
        {
            return (obj is SourceLocation) && Equals((SourceLocation)obj);
        }

        public override int GetHashCode()
        {
            // LineIndex and CharacterIndex can be calculated from AbsoluteIndex and the document content.
            return AbsoluteIndex;
        }

        public bool Equals(SourceLocation other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return AbsoluteIndex == other.AbsoluteIndex &&
                   LineIndex == other.LineIndex &&
                   CharacterIndex == other.CharacterIndex;
        }

        public int CompareTo(SourceLocation other)
        {
            return AbsoluteIndex.CompareTo(other.AbsoluteIndex);
        }

        public static SourceLocation Advance(SourceLocation left, string text)
        {
            SourceLocationTracker tracker = new SourceLocationTracker(left);
            tracker.UpdateLocation(text);
            return tracker.CurrentLocation;
        }

        public static SourceLocation Add(SourceLocation left, SourceLocation right)
        {
            if (right.LineIndex > 0)
            {
                // Column index doesn't matter
                return new SourceLocation(left.AbsoluteIndex + right.AbsoluteIndex, left.LineIndex + right.LineIndex, right.CharacterIndex);
            }
            else
            {
                return new SourceLocation(left.AbsoluteIndex + right.AbsoluteIndex, left.LineIndex + right.LineIndex, left.CharacterIndex + right.CharacterIndex);
            }
        }

        public static SourceLocation Subtract(SourceLocation left, SourceLocation right)
        {
            return new SourceLocation(left.AbsoluteIndex - right.AbsoluteIndex,
                                      left.LineIndex - right.LineIndex,
                                      left.LineIndex != right.LineIndex ? left.CharacterIndex : left.CharacterIndex - right.CharacterIndex);
        }

        private static SourceLocation CreateUndefined()
        {
            SourceLocation sl = new SourceLocation();
            sl._absoluteIndex = -1;
            sl._lineIndex = -1;
            sl._characterIndex = -1;
            return sl;
        }

        public static bool operator <(SourceLocation left, SourceLocation right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(SourceLocation left, SourceLocation right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator ==(SourceLocation left, SourceLocation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SourceLocation left, SourceLocation right)
        {
            return !left.Equals(right);
        }

        public static SourceLocation operator +(SourceLocation left, SourceLocation right)
        {
            return Add(left, right);
        }

        public static SourceLocation operator -(SourceLocation left, SourceLocation right)
        {
            return Subtract(left, right);
        }
    }
}
