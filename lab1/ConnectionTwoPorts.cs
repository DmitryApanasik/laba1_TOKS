using System;
using System.IO.Ports;
using System.Threading;


namespace lab1
{
    class ConnectionTwoPorts
    {
        byte[] jam = { 0xB3 };
        private int tryNumber = 15;
        private Random randomSleep = new Random(1);
        private SerialPort serialPort;
        public event EventHandler ReceivedMessage;
        public event EventHandler SerialPortError;
        private string _message;


        //проверка состояния
        public bool isReady() => serialPort.BytesToRead == 0;

        public string Message
        {
            get { return _message; }
        }

        public int Boud
        {
            get { return serialPort.BaudRate; }
        }

        public int Bits
        {
            get { return serialPort.DataBits; }
        }

        public string Name
        {
            get { return serialPort.PortName; }
        }

        public bool CheckOpenPort()
        {
            if (serialPort == null) return false;
            else return serialPort.IsOpen;
        }

        public void Com_Init(string name) //Инициализация порта
        {
            serialPort = new SerialPort(name, //Имя
                19200, // Скорость передачи
                Parity.None,  //Паритета
                8, //размер байта
                StopBits.One); //Количество сто-битов
            serialPort.Open();
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }

        public void ComChat(string data, Form1 obj)
        { 
            byte[] info = System.Text.Encoding.Unicode.GetBytes(data);
            for (int i = 0; i < info.Length; i++)
            {
                int countTry = 0;
                while(countTry < this.tryNumber)
                {
                    if(DateTime.Now.Second % 2 != 0)
                    {
                        serialPort.Write(info, i, 1);
                        Thread.Sleep(10);
                        if (DateTime.Now.Second % 2 == 0)
                        {
                            serialPort.Write(jam, 0, 1);
                            Thread.Sleep(10);
                            obj.setTextToDebug("X");
                            countTry++;
                            Thread.Sleep(randomSleep.Next((int)Math.Pow(2, tryNumber)));
                        }
                        break;
                    }
                }
            }
        }

        #region events
        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e) => SerialPortError?.Invoke(this, EventArgs.Empty);

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Byte nextByte = (Byte)serialPort.ReadByte();
            if (nextByte != jam[0])
            {
                _message = ((char)nextByte).ToString(); //получаем string из byte[]
                ReceivedMessage?.Invoke(this, EventArgs.Empty);//вызов функции receivedMessage
            }
        }
        #endregion
    }
}
