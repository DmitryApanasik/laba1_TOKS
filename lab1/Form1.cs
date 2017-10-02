using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        private string message;

        private string head;
     
        private readonly ConnectionTwoPorts serialPort;

        public string Message { get { return message; }  private set { message = value; } }

        public string Header { get { return head; } private set { head = value; } }

        public Form1()
        {
            InitializeComponent();
            serialPort = new ConnectionTwoPorts();
          //установка обработчиков
            serialPort.ReceivedMessage += new EventHandler(receivedMessage);
            serialPort.SerialPortError += new EventHandler(errorInPort);

            buttonConnect.Click += new EventHandler(buttonConnectClick);
            buttonSendMessage.Click += new EventHandler(buttonSendMessageClick);
        }

        public string PortName
        {
           get
            {
                if (nameCOMPort.Text == String.Empty) return "Input Port Name, pls!";
                else return nameCOMPort.Text;
           }
       }

        //установка текста в форму выводы сообщения
        public void setTextToOutput(string data) {
            receivedData.Text = data;
        }
        //уустановка текста в окно дебаг
        public void setTextToDebug(string data) {
            textInformation.Text += data + "\n\n";
        }

        private void buttonConnectClick(object sender, EventArgs e) {
            try
            {
                if (serialPort.CheckOpenPort())
                {
                    this.setTextToDebug(" yor connected!\n\n ");
                    return;
                }
                else
                {
                    string name = this.PortName;
                    if (name == " Port! ") throw new Exception(name + "\n\n");
                    serialPort.Com_Init(name);
                    if (serialPort.CheckOpenPort())
                    {
                        this.setTextToDebug(" Уou are connected to the port: " + serialPort.Name + ", " + serialPort.Boud + " Boud, " + serialPort.Bits + " bits.");
                    }
                }
            }
            catch (Exception ex)
            {
                this.setTextToDebug(ex.Message);
            }
        }

        private void buttonSendMessageClick(object sender, EventArgs e)
        {
            Message = textBoxSendMessage.Text;
            textBoxSendMessage.Clear();
            try
            {
                if (serialPort.isReady()) serialPort.ComChat(this.Message);
                else this.setTextToDebug(" The channel is busy! Wait... ");
            }
            catch (Exception ex)
            {
                this.setTextToDebug(ex.Message);
            }
            return;
        }
        private void receivedMessage(object sender, EventArgs e)
        {
            this.setTextToOutput(serialPort.Message);
            this.setTextToDebug(serialPort.Header);
        }
        private void errorInPort(object sender, EventArgs e)
        {
            this.setTextToDebug(" Error.\n\n");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            serialPort.setAS(1);
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            serialPort.setAS(2);
        }
    }
}
