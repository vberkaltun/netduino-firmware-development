namespace netduinoMaster
{
    public class SColorbook : IColorbook
    {
        private double red=0;
        public double Red
        {
            get
            {
                return red;
            }
        }

        private double green = 0;
        public double Green
        {
            get
            {
                return green;
            }
        }

        private double blue = 0;
        public double Blue
        {
            get
            {
                return blue;
            }
        }

        private double hue = 0;
        public double Hue
        {
            get
            {
                return hue;
            }
        }

        private double saturation = 0;
        public double Saturation
        {
            get
            {
                return saturation;
            }
        }

        private double value = 0;
        public double Value
        {
            get
            {
                return value;
            }
        }

        #region Constructor

        public SColorbook() { }

        public SColorbook(double red, double green, double blue, double hue, double saturation, double value)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;

            this.hue = hue;
            this.saturation = saturation;
            this.value = value;
        }

        #endregion
    }
}
