using Microsoft.SPOT;

namespace netduinoMaster
{
    public class Program
    {

        public static void Main()
        {
            SFunction n0 = null;

            SFunction n1 = new SFunction();
            n1.Name = "aaa";

            SFunction n2 = new SFunction();
            n2.Name = "bbb";

            //SMaster Master;
            //SDevice[] Device = new SDevice[] { };
            //ECommunication Communication;
            //ENotify Notify;

            SFunction[] Func = new SFunction[] { };
            EFunction.Enqueue(ref Func, ref n1);
            EFunction.Enqueue(ref Func, ref n2);

            n1.Name = "ccc";

            EFunction.Enqueue(ref Func, ref n1);
            EFunction.Dequeue(ref Func);
            EFunction.Enqueue(ref Func, ref n1);

            SFunction n3 = EFunction.Peek(ref Func);

            Debug.Print(EFunction.Equals(ref n1, ref n2).ToString());
            Debug.Print(EFunction.Equals(ref n2, ref n2).ToString());

            Debug.Print(EFunction.Contain(ref Func, ref n1).ToString());
            Debug.Print(EFunction.Contain(ref Func, ref n2).ToString());

            Debug.Print(Func.Length.ToString());
            EFunction.Clear(ref Func);
        }

    }
}
