using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat
{
    public partial class Form1 : Form
    {
        private static IPAddress remoteIPAddress;
        private static int remotePort;
        private static int localPort;
        private static string name;
        UdpClient sender;
        Thread tListener = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
           
            try
            {
                name = textBoxNickname.Text;
                remoteIPAddress = IPAddress.Parse(textBoxIP.Text);
                localPort = Convert.ToInt16(textBoxLocalPort.Text);
                remotePort = Convert.ToInt16(textBoxPort.Text);
                tListener = new Thread(new ThreadStart(Receiver));
                tListener.IsBackground = true;
                tListener.Start();
                buttonSend.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Receiver()
        {
            UdpClient receivingUdpClient = null;
            IPEndPoint RemoteIpEndPoint = null;
           
            try
            {
                receivingUdpClient = new UdpClient(localPort);
               this.Invoke(new Action(() =>
                {
                    richTextBoxHistory.Text +=
                  $"Connected {DateTime.Now}\n-----------*******Общий чат*******-----------";
                }));
               

                while (true)
                {
                    // Ожидание дейтаграммы
                    byte[] receiveBytes = receivingUdpClient.Receive(
                       ref RemoteIpEndPoint);

                    // Преобразуем и отображаем данные
                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    
                   this.Invoke(new Action(() =>
                    {
                        richTextBoxHistory.Text +=returnData.ToString();
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private static string Send(string datagram)
        {
            // Создаем UdpClient
            UdpClient sender = null;
            if (sender == null)
                sender = new UdpClient();

            // Создаем endPoint по информации об удаленном хосте
            IPEndPoint endPoint = null;
            if(endPoint == null)
                endPoint = new IPEndPoint(remoteIPAddress, remotePort);

            try
            {
                // Преобразуем данные в массив байтов
                byte[] bytes = Encoding.UTF8.GetBytes(datagram);

                // Отправляем данные
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Закрыть соединение
                sender.Client.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            return datagram;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            
            if (richTextBoxMessage.Text == String.Empty)
            {
                return;
            }
            this.richTextBoxHistory.Text += Send("\n" + DateTime.Now.ToLongTimeString() + " " +
                   name + " " + richTextBoxMessage.Text);
            this.richTextBoxMessage.Text = String.Empty;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() => {
                tListener.Suspend();
            }));
                
        }
    }
}
