namespace netduinoMaster
{
    public static class EDevice
    {
        #region Public

        /// <summary>
        /// Removes all objects from the Queue.
        /// </summary>
        /// <param name="source">The source object.</param>
        public static void Clear(ref SDevice[] source)
        {
            // The best clearing way of an array
            source = null;
            source = new SDevice[] { };
        }

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The Object to locate in the Queue. The source can be null.</param>
        /// <returns>true if obj is found in the Queue{ } otherwise, false.</returns>
        public static bool Contain(ref SDevice[] source, ref SDevice target)
        {
            for (int index = 0; index < source.Length; index++)
                if (source[index] == target)
                    return true;

            return false;
        }

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The object to add to the Queue. The source can be null.</param>
        public static void Enqueue(ref SDevice[] source, ref SDevice target)
        {
            // Clone main data and after resize it
            SDevice[] newData = new SDevice[source.Length + 1];
            Fill(ref newData[newData.Length - 1], ref target);

            if (source.Length != 0)
                for (int index = 0; index < source.Length; index++)
                    Fill(ref newData[index], ref source[index]);

            source = new SDevice[newData.Length];
            source = (SDevice[])newData.Clone();
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public static SDevice Dequeue(ref SDevice[] source)
        {
            if (source.Length == 0)
                return new SDevice();

            // Clone will return data and after resize it
            SDevice returnData = new SDevice();
            Fill(ref returnData, ref source[0]);

            if (source.Length == 1)
                Clear(ref source);
            else
            {
                // Clone main data and after resize it
                SDevice[] newData = new SDevice[source.Length - 1];

                for (int index = 1; index < source.Length; index++)
                    Fill(ref newData[index - 1], ref source[index]);

                source = new SDevice[newData.Length];
                source = (SDevice[])newData.Clone();
            }

            return returnData;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object{ } otherwise, false.</returns>
        public static bool Equals(ref SDevice source, ref SDevice target)
        {
            return source == target;
        }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public static SDevice Peek(ref SDevice[] source)
        {
            // Clone will return data and after resize it
            SDevice returnData = new SDevice();
            Fill(ref returnData, ref source[0]);

            return returnData;
        }

        /// <summary>
        /// Core of enqueue, fill and given source to given target.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The object will fill to target.</param>
        public static void Fill(ref SDevice source, ref SDevice target)
        {
            source = new SDevice(target.Vendor, target.Function, target.Handshake, target.Address);
        }

        #endregion
    }
}
