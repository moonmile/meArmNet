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

		ServoDriver sv;
		/// 初期化
		void setup() {
            sv = new ServoDriver();
            // 256MB model
            Console.WriteLine("Setup..");
            if (sv.Setup(0) == false)
            {
                Console.WriteLine("error Setup");
                return;
            }
            Console.WriteLine("SetPWMFreq..");
            Thread.Sleep(3000);
            sv.SetPWMFreq();    // サーボ制御パルスの設定 60Hz
		}
		/// 初期状態へ移動
		void init() {
            // サーボをセンター位置へ
            Console.WriteLine("Init..");
            sv.setServoPulse(0, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);
            sv.setServoPulse(1, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);
            sv.setServoPulse(2, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);
            sv.setServoPulse(3, ServoDriver.SERVO_CENTER_PULSE_WIDTH_US);
		}
		int sv0 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
		int sv1 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
		int sv2 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
		int sv3 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
		
		public int SV0 {
			get { return sv0; }
			set { sv0 = value; sv.setServoPulse(0, sv0 ); }
		}
		public int SV1 {
			get { return sv1; }
			set { sv1 = value; sv.setServoPulse(1, sv1 ); }
		}
		public int SV2 {
			get { return sv2; }
			set { sv2 = value; sv.setServoPulse(2, sv2 ); }
		}
		public int SV3 {
			get { return sv3; }
			set { sv3 = value; sv.setServoPulse(3, sv3 ); }
		}


        void main( string[] args)
        {
            Console.WriteLine("test ServoDriver");
            
            setup();
			init();            
			
			while( true ) {
				var key = Console.ReadKey();
            	Console.WriteLine("key :{0}", key.Key.ToString());
				if ( key.Key == ConsoleKey.Escape ) 
					break;
					
				switch ( key.Key.ToString() ) {
					case "Q": return;
					case "I":
						SV0 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
						SV1 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
						SV2 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
						SV3 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
	            		break;
					case "L": SV0 -= 20 ; break;
					case "R": SV0 += 20 ; break;
					case "U": SV1 -= 20 ; break;
					case "D": SV1 += 20 ; break;
					case "B": SV2 -= 20 ; break;
					case "F": SV2 += 20 ; break;
					case "O": SV3 -= 40 ; break;
					case "C": SV3 += 40 ; break;
				}
				Thread.Sleep(200);
			}
			return ;
        }
    }
}


