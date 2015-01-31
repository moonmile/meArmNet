using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;

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
            Console.WriteLine("test ServoServer");

            setup();
			init();            

            StartHttp();

            // キー待ち
            var key = Console.ReadKey();
        }

        public void MoveCommand( string key ) {
			switch ( key ) {
				case "Q": return;
				case "I":
				case "/i":
					SV0 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
					SV1 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
					SV2 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
					SV3 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US;
            		break;
				case "/left":  SV0 -= 20 ; break;
				case "/right": SV0 += 20 ; break;
				case "/up":    SV1 -= 20 ; break;
				case "/down":  SV1 += 20 ; break;
				case "/back":  SV2 -= 20 ; break;
				case "/front": SV2 += 20 ; break;
				case "/open":  SV3 -= 40 ; break;
				case "/close": SV3 += 40 ; break;
			}
			if ( key.StartsWith("/SV0/") ) {
				int v = int.Parse(key.Replace("/SV0/",""));
				SV0 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US + v ;
			} else if ( key.StartsWith("/SV1/") ) {
				int v = int.Parse(key.Replace("/SV1/",""));
				SV1 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US + v ;
			} else if ( key.StartsWith("/SV2/") ) {
				int v = int.Parse(key.Replace("/SV2/",""));
				SV2 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US + v ;
			} else if ( key.StartsWith("/SV3/") ) {
				int v = int.Parse(key.Replace("/SV3/",""));
				SV3 = ServoDriver.SERVO_CENTER_PULSE_WIDTH_US + v ;
			}
        }
        
		private HttpListener listener;
		public void StartHttp()
		{
			try
			{
				listener = new System.Net.HttpListener();
				listener.Prefixes.Add("http://*:8088/");
				listener.Start();
				listener.BeginGetContext(ListenerCallback, listener);
				Console.WriteLine("waiting ...");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		public void StopHttp()
		{
			listener.Stop();
			listener = null;
			Console.WriteLine("stop");
		}

		public void ListenerCallback(IAsyncResult result)
		{
			HttpListenerContext context;
			try
			{
				context = listener.EndGetContext(result);
			}
			catch
			{
				// stop メソッドで例外が発生するので、対処
				return;
			}
			var req = context.Request;
			var url = req.RawUrl;
			var res = context.Response;

			var output = new StreamWriter(res.OutputStream);
			string msg = string.Format("url: {0}", url);
			output.WriteLine(msg);
			output.Close();
			
			Console.WriteLine( url );
			MoveCommand( url );
			
			// 次の受信の準備
			listener.BeginGetContext(ListenerCallback, listener);
		}

    }
}


