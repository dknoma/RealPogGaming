using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Allies {

    public struct TDVector : IEquatable<TDVector> {
        private static readonly TDVector zeroVector = new TDVector(0.0f, 0.0f, 0.0f);
        
        public float x, y, z;

        public TDVector(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    default:
                        throw new IndexOutOfRangeException("Invalid TDVector index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid TDVector index!");
                }
            }
        }
        
        /// <summary>
        ///   <para>Returns true if the given vector is exactly equal to this vector.</para>
        /// </summary>
        /// <param name="other"></param>
        public override bool Equals(object other)
        {
//            return other is TDVector other1 && this.Equals(other1);
            return true;
        }
        
        public bool Equals(TDVector other) {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override int GetHashCode() {
            // unchecked {
            //     var hashCode = h.GetHashCode();
            //     hashCode = (hashCode * 397) ^ d.GetHashCode();
            //     hashCode = (hashCode * 397) ^ x.GetHashCode();
            //     return hashCode;
            // }
            
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        }
        
        public static TDVector operator +(TDVector a, TDVector b)
        {
            return new TDVector(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        
        public static TDVector operator *(TDVector a, float d)
        {
            return new TDVector(a.x * d, a.y * d, a.z * d);
        }

        public static TDVector operator *(float d, TDVector a)
        {
            return new TDVector(a.x * d, a.y * d, a.z * d);
        }
    }
}