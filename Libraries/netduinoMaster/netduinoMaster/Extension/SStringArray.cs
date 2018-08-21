namespace netduinoMaster
{
    class SStringArray
    {
        string[] String = new string[] { };

        public int Length
        {
            get
            {
                return String.Length;
            }
        }

        public string this[int index]
        {
            get
            {
                return String[index];
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
            String = null;
            String = new string[] { };
        }

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The Object to locate in the Queue. The Function can be null.</param>
        /// <returns>true if obj is found in the Queue{ } otherwise, false.</returns>
        public bool Contain(string target)
        {
            for (int index = 0; index < String.Length; index++)
                if (String[index] == target)
                    return true;

            return false;
        }

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The object to add to the Queue. The Function can be null.</param>
        public void Enqueue(string target)
        {
            // Clone main data and after resize it
            string[] newData = new string[String.Length + 1];
            newData[newData.Length - 1]= target;

            if (String.Length != 0)
                for (int index = 0; index < String.Length; index++)
                    newData[index] = String[index];

            String = new string[newData.Length];
            String = (string[])newData.Clone();
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public string Dequeue()
        {
            if (String.Length == 0)
                return null;

            // Clone will return data and after resize it
            string returnData = String[0];

            if (String.Length == 1)
                Clear();
            else
            {
                // Clone main data and after resize it
                string[] newData = new string[String.Length - 1];

                for (int index = 1; index < String.Length; index++)
                    newData[index - 1] = String[index];

                String = new string[newData.Length];
                String = (string[])newData.Clone();
            }

            return returnData;
        }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public string Peek()
        {
            if (String.Length == 0)
                return null;

            // Returns beginning of the Queue without removing it
            return String[0];
        }

        #endregion
    }
}
