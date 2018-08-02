namespace netduinoMaster
{
    public static class EFunction
    {
        #region Public

        /// <summary>
        /// Removes all objects from the Queue.
        /// </summary>
        /// <param name="source">The source object.</param>
        public static void Clear(ref SFunction[] source)
        {
            // The best clearing way of an array
            source = null;
            source = new SFunction[] { };
        }

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The Object to locate in the Queue. The source can be null.</param>
        /// <returns>true if obj is found in the Queue{ } otherwise, false.</returns>
        public static bool Contain(ref SFunction[] source, ref SFunction target)
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
        public static void Enqueue(ref SFunction[] source, ref SFunction target)
        {
            // Clone main data and after resize it
            SFunction[] newData = new SFunction[source.Length + 1];
            Fill(ref newData[newData.Length - 1], ref target);

            if (source.Length != 0)
                for (int index = 0; index < source.Length; index++)
                    Fill(ref newData[index], ref source[index]);

            source = new SFunction[newData.Length];
            source = (SFunction[])newData.Clone();
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public static SFunction Dequeue(ref SFunction[] source)
        {
            if (source.Length == 0)
                return new SFunction();

            // Clone will return data and after resize it
            SFunction returnData = new SFunction();
            Fill(ref returnData, ref source[0]);

            if (source.Length == 1)
                Clear(ref source);
            else
            {
                // Clone main data and after resize it
                SFunction[] newData = new SFunction[source.Length - 1];

                for (int index = 1; index < source.Length; index++)
                    Fill(ref newData[index - 1], ref source[index]);

                source = new SFunction[newData.Length];
                source = (SFunction[])newData.Clone();
            }

            return returnData;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object{ } otherwise, false.</returns>
        public static bool Equals(ref SFunction source, ref SFunction target)
        {
            return source == target;
        }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public static SFunction Peek(ref SFunction[] source)
        {
            // Clone will return data and after resize it
            SFunction returnData = new SFunction();
            Fill(ref returnData, ref source[0]);

            return returnData;
        }

        /// <summary>
        /// Core of enqueue, fill and given source to given target.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The object will fill to target.</param>
        public static void Fill(ref SFunction source, ref SFunction target)
        {
            source = new SFunction(target.Name, target.Request, target.Listen);
        }

        #endregion
    }
}
