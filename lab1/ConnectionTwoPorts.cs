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
        private string _header;
        private int AS = 1;


        //проверка состояния
        public bool isReady() => serialPort.BytesToRead == 0;

        public string Message
        {
            get { return _message; }
        }

        public string Header { get { return _header; } }

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
        public byte[] make(byte[] info)
        {
            byte[] package = new byte[19];
            package[0] = Convert.ToByte(this.AS);
            package[1] = (this.AS == 1) ? Convert.ToByte(2) : Convert.ToByte(1);
            for (int i = 0, k = 2; i < info.Length; i++, k++)
            {
                package[k] = info[i];
                if (i == info.Length - 1)
                { 
                    for (int j = 0; j < package.Length - 2; j++)
                        package[k+1] = (byte)(package[k+1] ^ package[j]);
                    package[k+1] = Convert.ToByte(Convert.ToBoolean(package[k+1]));
                }
                
            }
            return package;
        }
        public byte[] byteStuffing(byte[] notStuffing)
        {
            int sizePackage = 0;
            for (int i = 0; i < notStuffing.Length; i++)
            {
                if (notStuffing[i] == Convert.ToByte(85)) sizePackage++;
            }
            byte[] newByte = new byte[19 + sizePackage];
            for (int i = 0, j = 0; i < notStuffing.Length; i++, j++)
            {
                if(notStuffing[i] == Convert.ToByte(85))
                {
                    newByte[j] = Convert.ToByte((int)notStuffing[i] - 1);
                    newByte[++j] = Convert.ToByte(94);
                    continue;
                }
                newByte[j] = notStuffing[i];
            }
            string bitStringFlag = "01010101";
            byte[] flag = new byte[bitStringFlag.Length / 8];
            for (int b = 0; b <= 7; b++)
            {
                flag[0] |= (byte)((bitStringFlag[b] == '1' ? 1 : 0) << (7 - b));
            }
            byte[] resultPackage = new byte[newByte.Length + 1];
            resultPackage[0] = flag[0];
            for (int i = 0; i < newByte.Length; i++)
            {
                resultPackage[i + 1] = newByte[i];
            }
            return resultPackage;
        }

<<<<<<< HEAD
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
=======
        public byte[] byteStuffingencode(byte[] Stuffing)
        {
            byte[] stuffingData = new byte[Stuffing.Length - 1];
            for (int i = 0; i < Stuffing.Length - 1; i++)
            {
                stuffingData[i] = Stuffing[i + 1];
            }
            byte[] newnotStuffing = new byte[19];
            for (int i = 0, j = 0; j < newnotStuffing.Length; i++, j++)
            {
                if (stuffingData[i] == 94)
                {
                    newnotStuffing[j - 1] = Convert.ToByte((int)newnotStuffing[j - 1] + 1);
                    j--;
                    continue;
                }
                newnotStuffing[j] = stuffingData[i];
            }
            return newnotStuffing;
        }
        public void setAS (int adresss)
        {
            this.AS = adresss;
        }
        public void ComChat(string data)
        {
            byte[] info = System.Text.Encoding.Unicode.GetBytes(data);
            byte[] onePackage = new byte[16];
            for (int i = 0, k = 0; i < info.Length + 1; i++, k++)
            {
                if(((i % 16) == 0 && i !=0 && k != 0) || i == info.Length)
                {
                    byte[] notStuff = make(onePackage);
                    byte[] message = byteStuffing(notStuff);
                    serialPort.Write(message, 0, message.Length);
                    k = -1;
                    if (i == info.Length) break;
                    i--;
                    for (int j = 0; j < onePackage.Length; j++)
                    {
                        onePackage[j] = 0;
                    }
                    continue;
                }
                onePackage[k] = info[i];
>>>>>>> ca70a0860861fa5e510b889533802b47235ef03a
            }
        }

        #region events
        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e) => SerialPortError?.Invoke(this, EventArgs.Empty);

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
<<<<<<< HEAD
            Byte nextByte = (Byte)serialPort.ReadByte();
            if (nextByte != jam[0])
            {
                _message = ((char)nextByte).ToString(); //получаем string из byte[]
                ReceivedMessage?.Invoke(this, EventArgs.Empty);//вызов функции receivedMessage
            }
=======
            byte[] info = new byte[serialPort.BytesToRead];
            serialPort.Read(info, 0, info.Length);
            byte[] data = byteStuffingencode(info);
            byte[] message = new byte[16];
            for (int i=0; i < message.Length; i++)
            {
                message[i] = data[i + 2];
            }
            _message = System.Text.Encoding.Unicode.GetString(message);
            _header = info[0].ToString() + ", " + data[0].ToString()+ ", " + data[1].ToString() + ", " +
                _message + ", " + data[data.Length-1].ToString() + "\n"; 
            ReceivedMessage?.Invoke(this, EventArgs.Empty);//вызов функции receivedMessage
>>>>>>> ca70a0860861fa5e510b889533802b47235ef03a
        }
        #endregion
    }
}
