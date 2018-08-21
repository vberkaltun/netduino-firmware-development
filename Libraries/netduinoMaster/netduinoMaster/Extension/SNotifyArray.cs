namespace netduinoMaster
{
    public class SNotifyArray
    {
        ENotify[] Notify = new ENotify[] { };

        public int Length
        {
            get
            {
                return Notify.Length;
            }
        }

        public ENotify this[int index]
        {
            get
            {
                return Notify[index];
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
            Notify = null;
            Notify = new ENotify[] { };
        }

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The Object to locate in the Queue. The Function can be null.</param>
        /// <returns>true if obj is found in the Queue{ } otherwise, false.</returns>
        public bool Contain(ENotify target)
        {
            for (int index = 0; index < Notify.Length; index++)
                if (Notify[index] == target)
                    return true;

            return false;
        }

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The object to add to the Queue. The Function can be null.</param>
        public void Enqueue(ENotify target)
        {
            // Clone main data and after resize it
            ENotify[] newData = new ENotify[Notify.Length + 1];
            Fill(ref newData[newData.Length - 1], ref target);

            if (Notify.Length != 0)
                for (int index = 0; index < Notify.Length; index++)
                    Fill(ref newData[index], ref Notify[index]);

            Notify = new ENotify[newData.Length];
            Notify = (ENotify[])newData.Clone();
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public ENotify Dequeue()
        {
            if (Notify.Length == 0)
                return new ENotify();

            // Clone will return data and after resize it
            ENotify returnData = new ENotify();
            Fill(ref returnData, ref Notify[0]);

            if (Notify.Length == 1)
                Clear();
            else
            {
                // Clone main data and after resize it
                ENotify[] newData = new ENotify[Notify.Length - 1];

                for (int index = 1; index < Notify.Length; index++)
                    Fill(ref newData[index - 1], ref Notify[index]);

                Notify = new ENotify[newData.Length];
                Notify = (ENotify[])newData.Clone();
            }

            return returnData;
        }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public ENotify Peek()
        {
            if (Notify.Length == 0)
                return new ENotify();

            // Clone will return data and after resize it
            ENotify returnData = new ENotify();
            Fill(ref returnData, ref Notify[0]);

            return returnData;
        }

        /// <summary>
        /// Core of enqueue, fill a given source to given target.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The object will fill to target.</param>
        private void Fill(ref ENotify source, ref ENotify target)
        {
            source = target;
        }

        #endregion
    }
}
