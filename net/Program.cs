using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace meArmPiNet
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().main( args );
        }


        void main( string[] args)
        {
            Console.WriteLine("test ServoDriver");

            var sv = new ServoDriver();

                // 256MB model
            Console.WriteLine("Setup..");
            if (sv.Setup(0) == false)
            {
                Console.WriteLine("error Setup");
                return;
            }
            Thread.Sleep(5000);

            Console.WriteLine("SetPWMFreq..");
            sv.SetPWMFreq();    // サーボ制御パルスの設定 60Hz
            // サーボをセンター位置へ
            Console.WriteLine("Init..");
            sv.setServoPulse(0, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);
            sv.setServoPulse(1, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);
            sv.setServoPulse(2, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);
            sv.setServoPulse(3, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);

		return ;

            Console.WriteLine("Move..");
            Thread.Sleep(1);
            sv.setServoPulse(0, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US - ServoDriver.SERVO_CENTER_PULSE_WIDTH_US/4 );
            Thread.Sleep(3000);
            sv.setServoPulse(0, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);

            // キー待ち
            // var key = Console.ReadKey();

        }
    }
}
