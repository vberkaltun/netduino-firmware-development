namespace netduinoMaster
{
    public class SDeviceArray
    {
        private SDevice[] Device = new SDevice[] { };

        public int Length
        {
            get
            {
                return Device.Length;
            }
        }

        public SDevice this[int index]
        {
            get
            {
                return Device[index];
            }
        }

        #region Public

        /// <summary>
        /// Removes all objects from the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        public void Clear()
        {
            // The best clearing way of an array
            Device = null;
            Device = new SDevice[] { };
        }

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The Object to locate in the Queue. The Function can be null.</param>
        /// <returns>true if obj is found in the Queue{ } otherwise, false.</returns>
        public bool Contain(SDevice target)
        {
            for (int index = 0; index < Device.Length; index++)
                if (Device[index] == target)
                    return true;

            return false;
        }

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <param name="target">The object to add to the Queue. The Function can be null.</param>
        public void Enqueue(SDevice target)
        {
            // Clone main data and after resize it
            SDevice[] newData = new SDevice[Device.Length + 1];
            Fill(ref newData[newData.Length - 1], ref target);

            if (Device.Length != 0)
                for (int index = 0; index < Device.Length; index++)
                    Fill(ref newData[index], ref Device[index]);

            Device = new SDevice[newData.Length];
            Device = (SDevice[])newData.Clone();
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public SDevice Dequeue()
        {
            if (Device.Length == 0)
                return new SDevice();

            // Clone will return data and after resize it
            SDevice returnData = new SDevice();
            Fill(ref returnData, ref Device[0]);

            if (Device.Length == 1)
                Clear();
            else
            {
                // Clone main data and after resize it
                SDevice[] newData = new SDevice[Device.Length - 1];

                for (int index = 1; index < Device.Length; index++)
                    Fill(ref newData[index - 1], ref Device[index]);

                Device = new SDevice[newData.Length];
                Device = (SDevice[])newData.Clone();
            }

            return returnData;
        }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="Function">The Function object.</param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public SDevice Peek()
        {
            // Clone will return data and after resize it
            SDevice returnData = new SDevice();
            Fill(ref returnData, ref Device[0]);

            return returnData;
        }

        /// <summary>
        /// Core of enqueue, fill and given source to given target.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The object will fill to target.</param>
        private void Fill(ref SDevice source, ref SDevice target)
        {
            source = new SDevice(target.Vendor, target.Function, target.Handshake, target.Address);
        }

        #endregion
    }
}