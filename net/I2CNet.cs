using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace meArmPiNet
{
    public class I2CNet
    {
        protected int _i2c = 0;

        [DllImport("libi2cnet", EntryPoint = "I2C_open", CharSet = CharSet.Ansi)]
        protected static extern int I2C_open(string devname, int mode);
        [DllImport("libi2cnet", EntryPoint = "I2C_ioctl")]
        protected static extern int I2C_ioctl(int fd, int mode, int addr);
        [DllImport("libi2cnet", EntryPoint = "I2C_close")]
        protected static extern void I2C_close(int fd);
        [DllImport("libi2cnet", EntryPoint = "I2C_write8")]
        protected static extern int I2C_write8(int fd, int addr, int data);
        [DllImport("libi2cnet", EntryPoint = "I2C_write5")]
        protected static extern int I2C_write5(int fd, int svNo, int onTime, int offTime);
        [DllImport("libi2cnet", EntryPoint = "I2C_read8")]
        protected static extern int I2C_read8(int fd, int addr);

        const int O_RDWR = 0x02;

        public bool Open(string devname)
        {
            _i2c = I2C_open(devname, O_RDWR);
            if (_i2c < 0) Console.WriteLine("error: Open");
            return _i2c < 0 ? false : true;
        }
        public bool Ioctl(int mode, int addr)
        {
            int ret = I2C_ioctl(_i2c, mode, addr);
            if (ret < 0) Console.WriteLine("error: Ioctl");
            return ret < 0 ? false : true;
        }
        public bool Write8(int addr, int data)
        {
            int ret = I2C_write8(_i2c, addr, data);
            if (ret < 0) Console.WriteLine("error: Write8");
            return ret < 0 ? false : true;
        }
        public bool Write5(int srvNo, int onTime, int offTime)
        {
            int ret = I2C_write5(_i2c, srvNo, onTime, offTime);
            if (ret < 0) Console.WriteLine("error: Write5");
            return ret < 0 ? false : true;
        }
        public int Read8(int addr)
        {
            int ret = I2C_read8(_i2c, addr);
            if (ret < 0) Console.WriteLine("error: Write5");
            return ret;
        }

    }

    public class ServoDriver : I2CNet
    {
        protected string I2CFileName { get; set; }
        const int driverAddress = 0x40;
        const int I2C_SLAVE = 0x703;
        const int PCA9685_MODE1 = 0x00;
        const int PCA9685_PRESCALE = 0xFE;

        public const int SERVO_CENTER_PULSE_WIDTH_US = 1600;   // 中央のパルス幅
        public double SERVO_CONTROL_FREQUENCY = 60.0; // ヘルツ数


        /// <summary>
        /// Setup PCA9685
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool Setup(int mode = 1)
        {
            if (mode == 0)
            {
                this.I2CFileName = "/dev/i2c-0"; // old model 256MB
            }
            else
            {
                this.I2CFileName = "/dev/i2c-1"; // model 512MB
            }
            if (Open(this.I2CFileName) == true)
            {
                if (Ioctl(I2C_SLAVE, driverAddress) == true)
                {
                    this.Reset();
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            base.Write8(PCA9685_MODE1, 0x0);
        }

        public void SetPWMFreq(double freq = 60.0)
        {
            SERVO_CONTROL_FREQUENCY = freq;

            double prescaleval = 25000000.0;    // 25MHz
            prescaleval /= 4096;                // 12bit
            prescaleval /= freq;
            prescaleval -= 1.0;
            int prescale = (int)(prescaleval + 0.5);   // floor
            int oldmode = this.Read8(PCA9685_MODE1);
            int newmode = (oldmode & 0x7F) | 0x10;

            this.Write8(PCA9685_MODE1, newmode);
            this.Write8(PCA9685_PRESCALE, prescale);
            this.Write8(PCA9685_MODE1, oldmode);
            Thread.Sleep(1000);
            // this.Write8(PCA9685_MODE1, oldmode | 0xA1);
            this.Write8(PCA9685_MODE1, oldmode | 0x80);
        }

        public void setServoPulse(int ch, double pulseWidth_us)
        {

            double pulselength;
            double pulseWidth;

            // 1秒=1000000usを60Hzで割ってパルス長を算出。
            pulselength = 1000000.0 ;
            pulselength /= SERVO_CONTROL_FREQUENCY;
            // 12bit(2^12=4096)分解能相当へ。1分解能当たりの時間算出。
            pulselength /= 4096.0;
            // PWMのパルス設定値を算出。
            pulseWidth = pulseWidth_us / pulselength;

            // PWM値設定。
            //  setPWM(channel, on_timing, off_timing)
            //  channelで指定したチャネルのPWM出力のon(0→1）になるタイミングと
            //  off(1→0)になるタイミングを0～4095で設定する。
            this.SetPWM(ch, 0, (int)pulseWidth);
        }

        public void SetPWM(int srvNo, int onTime, int offTime)
        {
            base.Write5(srvNo, onTime, offTime);
        }
    }
}
