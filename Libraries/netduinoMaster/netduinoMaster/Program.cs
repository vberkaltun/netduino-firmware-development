using Microsoft.SPOT;

namespace netduinoMaster
{
    public class Program
    {
        public static void Main()
        {
            //SFunction n0 = null;

            //SFunction n1 = new SFunction();
            //n1.Name = "aaa";

            //SFunction n2 = new SFunction();
            //n2.Name = "bbb";

            //SFunction[] Func = new SFunction[] { };
            //EFunction.Enqueue(ref Func, ref n1);
            //EFunction.Enqueue(ref Func, ref n2);

            //n1.Name = "ccc";

            //EFunction.Enqueue(ref Func, ref n1);
            //EFunction.Dequeue(ref Func);
            //EFunction.Enqueue(ref Func, ref n1);

            //SFunction n3 = EFunction.Peek(ref Func);

            //Debug.Print(EFunction.Equals(ref n1, ref n2).ToString());
            //Debug.Print(EFunction.Equals(ref n2, ref n2).ToString());

            //Debug.Print(EFunction.Contain(ref Func, ref n1).ToString());
            //Debug.Print(EFunction.Contain(ref Func, ref n2).ToString());

            //Debug.Print(Func.Length.ToString());
            //EFunction.Clear(ref Func);

            // =================================================================
            // =================================================================
            // =================================================================

            //SDevice n1 = new SDevice();
            //n1.Vendor.Brand = "aaa";
            //n1.Handshake = EHandshake.Ready;

            //SDevice n2 = new SDevice();
            //n2.Vendor.Brand = "bbb";
            //n2.Handshake = EHandshake.Unknown;

            ////SFunction f2 = new SFunction();
            ////f2.Name = "bbb";
            ////EFunction.Enqueue(ref &(n2.Function), ref f2);

            //SDevice[] Func = new SDevice[] { };
            //EDevice.Enqueue(ref Func, ref n1);
            //EDevice.Enqueue(ref Func, ref n2);

            // =================================================================
            // =================================================================
            // =================================================================

            SFunction n1 = new SFunction();
            n1.Name = "ccc";

            SFunction n2 = new SFunction();
            n2.Name = "ddd";

            SFunctionArray Func = new SFunctionArray();
            Func.Enqueue(n1);
            Func.Enqueue(n2);

            SDevice s1 = new SDevice();
            s1.Vendor.Brand = "aaa";
            s1.Handshake = EHandshake.Ready;
            s1.Function.Enqueue(n1);

            SDevice s2 = new SDevice();
            s2.Vendor.Brand = "bbb";
            s2.Handshake = EHandshake.Unknown;
            s2.Function.Enqueue(n2);

            SDeviceArray e2 = new SDeviceArray();
            e2.Enqueue(s2);
            e2.Enqueue(s1);

            SMaster m1 = new SMaster();
            m1.Transmit = "test";
            m1.Function.Enqueue(n1);
            m1.Function.Enqueue(n2);
        }

    }
}
