namespace netduinoMaster
{
    public class SFunctionArray
    {
        SFunction[] Function = new SFunction[] { };

        public int Length
        {
            get
            {
                return Function.Length;
            }
        }

        public SFunction this[int index]
        {
            get
            {
                return Function[index];
            }
        }

        #region Function

        /// <summary>
        /// Removes all objects from the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        public void Clear()
        {
            // The best clearing way of an array
            Function = null;
            Function = new SFunction[] { };
        }

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The Object to locate in the Queue. The Function can be null.</param>
        /// <returns>true if obj is found in the Queue{ } otherwise, false.</returns>
        public bool Contain(SFunction target)
        {
            for (int index = 0; index < Function.Length; index++)
                if (Function[index] == target)
                    return true;

            return false;
        }

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The object to add to the Queue. The Function can be null.</param>
        public void Enqueue(SFunction target)
        {
            // Clone main data and after resize it
            SFunction[] newData = new SFunction[Function.Length + 1];
            Fill(ref newData[newData.Length - 1], ref target);

            if (Function.Length != 0)
                for (int index = 0; index < Function.Length; index++)
                    Fill(ref newData[index], ref Function[index]);

            Function = new SFunction[newData.Length];
            Function = (SFunction[])newData.Clone();
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public SFunction Dequeue()
        {
            if (Function.Length == 0)
                return new SFunction();

            // Clone will return data and after resize it
            SFunction returnData = new SFunction();
            Fill(ref returnData, ref Function[0]);

            if (Function.Length == 1)
                Clear();
            else
            {
                // Clone main data and after resize it
                SFunction[] newData = new SFunction[Function.Length - 1];

                for (int index = 1; index < Function.Length; index++)
                    Fill(ref newData[index - 1], ref Function[index]);

                Function = new SFunction[newData.Length];
                Function = (SFunction[])newData.Clone();
            }

            return returnData;
        }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public SFunction Peek()
        {
            // Clone will return data and after resize it
            SFunction returnData = new SFunction();
            Fill(ref returnData, ref Function[0]);

            return returnData;
        }

        /// <summary>
        /// Core of enqueue, fill and given Function to given target.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The object will fill to target.</param>
        private void Fill(ref SFunction source, ref SFunction target)
        {
            source = new SFunction(target.Name, target.Request, target.Listen);
        }

        #endregion
    }
}
