using System;
using System.IO.Ports;
using System.Threading;

namespace lab1
{
    class ConnectionTwoPorts
    {
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

        public void ComChat(string data)
        {
            /*serialPort.RtsEnable = true; *///установка бита RTS для подключения передатчика
            byte[] info = System.Text.Encoding.Unicode.GetBytes(data);
            serialPort.Write(info, 0, info.Length);
            //Thread.Sleep(100);//пауза для корректного завершения передатчика
            //serialPort.RtsEnable = false;
        }

        #region events
        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e) => SerialPortError?.Invoke(this, EventArgs.Empty);

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] info = new byte[serialPort.BytesToRead];
            serialPort.Read(info, 0, info.Length);
            _message = System.Text.Encoding.Unicode.GetString(info); //получаем string из byte[]
            ReceivedMessage?.Invoke(this, EventArgs.Empty);//вызов функции receivedMessage
        }
        #endregion
    }
}
