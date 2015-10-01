using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FlareDUC
{
    public partial class Form1 : Form
    {
        public class EditRequest
        {
            public string act { get; set; }
        }

        public class EditProps
        {
            public int proxiable { get; set; }
            public int cloud_on { get; set; }
            public int cf_open { get; set; }
            public int ssl { get; set; }
            public int expired_ssl { get; set; }
            public int expiring_ssl { get; set; }
            public int pending_ssl { get; set; }
            public int vanity_lock { get; set; }
        }

        public class EditObj
        {
            public string rec_id { get; set; }
            public string rec_tag { get; set; }
            public string zone_name { get; set; }
            public string name { get; set; }
            public string display_name { get; set; }
            public string type { get; set; }
            public object prio { get; set; }
            public string content { get; set; }
            public string display_content { get; set; }
            public string ttl { get; set; }
            public int ttl_ceil { get; set; }
            public object ssl_id { get; set; }
            public object ssl_status { get; set; }
            public object ssl_expires_on { get; set; }
            public int auto_ttl { get; set; }
            public string service_mode { get; set; }
            public EditProps props { get; set; }
        }

        public class EditRec
        {
            public EditObj obj { get; set; }
        }

        public class EditResponse
        {
            public EditRec rec { get; set; }
        }

        public class EditRootObject
        {
            public EditRequest request { get; set; }
            public EditResponse response { get; set; }
            public string result { get; set; }
            public object msg { get; set; }
        }

        public class ZoneRequest
        {
            public string act { get; set; }
        }

        public class ZoneProps
        {
            public int dns_cname { get; set; }
            public int dns_partner { get; set; }
            public int dns_anon_partner { get; set; }
            public string plan { get; set; }
            public int pro { get; set; }
            public int expired_pro { get; set; }
            public int pro_sub { get; set; }
            public int plan_sub { get; set; }
            public int ssl { get; set; }
            public int expired_ssl { get; set; }
            public int expired_rs_pro { get; set; }
            public int reseller_pro { get; set; }
            public List<object> reseller_plans { get; set; }
            public int force_interal { get; set; }
            public int ssl_needed { get; set; }
            public int alexa_rank { get; set; }
            public int has_vanity { get; set; }
        }

        public class ZoneConfirmCode
        {
            public string zone_delete { get; set; }
            public string zone_deactivate { get; set; }
            public string zone_dev_mode1 { get; set; }
        }

        public class ZoneObj
        {
            public string zone_id { get; set; }
            public string user_id { get; set; }
            public string zone_name { get; set; }
            public string display_name { get; set; }
            public string zone_status { get; set; }
            public string zone_mode { get; set; }
            public object host_id { get; set; }
            public string zone_type { get; set; }
            public object host_pubname { get; set; }
            public object host_website { get; set; }
            public object vtxt { get; set; }
            public List<string> fqdns { get; set; }
            public string step { get; set; }
            public string zone_status_class { get; set; }
            public string zone_status_desc { get; set; }
            public List<object> ns_vanity_map { get; set; }
            public string orig_registrar { get; set; }
            public object orig_dnshost { get; set; }
            public string orig_ns_names { get; set; }
            public ZoneProps props { get; set; }
            public ZoneConfirmCode confirm_code { get; set; }
            public List<string> allow { get; set; }
        }

        public class Zones
        {
            public bool has_more { get; set; }
            public int count { get; set; }
            public List<ZoneObj> objs { get; set; }
        }

        public class ZoneResponse
        {
            public Zones zones { get; set; }
        }

        public class ZoneRootObject
        {
            public ZoneRequest request { get; set; }
            public ZoneResponse response { get; set; }
            public string result { get; set; }
            public object msg { get; set; }
        }

        public class Request
        {
            public string act { get; set; }
        }

        public class Props
        {
            public int proxiable { get; set; }
            public int cloud_on { get; set; }
            public int cf_open { get; set; }
            public int ssl { get; set; }
            public int expired_ssl { get; set; }
            public int expiring_ssl { get; set; }
            public int pending_ssl { get; set; }
            public int vanity_lock { get; set; }
        }

        public class Mx
        {
            public object auto { get; set; }
        }

        public class Obj
        {
            public string rec_id { get; set; }
            public string rec_tag { get; set; }
            public string zone_name { get; set; }
            public string name { get; set; }
            public string display_name { get; set; }
            public string type { get; set; }
            public string prio { get; set; }
            public string content { get; set; }
            public string display_content { get; set; }
            public string ttl { get; set; }
            public int ttl_ceil { get; set; }
            public object ssl_id { get; set; }
            public object ssl_status { get; set; }
            public object ssl_expires_on { get; set; }
            public int auto_ttl { get; set; }
            public string service_mode { get; set; }
            public Props props { get; set; }
            public Mx mx { get; set; }
        }

        public class Recs
        {
            public bool has_more { get; set; }
            public int count { get; set; }
            public List<Obj> objs { get; set; }
        }

        public class Response
        {
            public Recs recs { get; set; }
        }

        public class RootObject
        {
            public Request request { get; set; }
            public Response response { get; set; }
            public string result { get; set; }
            public object msg { get; set; }
        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        String tkn, id, email, z, name, selected_interface;
        IPAddress ip, lastip = new IPAddress(0x00000000);
        public Form1()
        {
            InitializeComponent();
        }

        public static void log(string logMessage)
        {
            StreamWriter w = File.AppendText("debug.log");
            w.WriteLine("{0} {1}\t{2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), logMessage);
            w.Close();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
            log("Program Started");
            load_config();
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(tkn))
            {
                zone_load_multi();
            }

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface network in networkInterfaces)
            {
                IPInterfaceProperties properties = network.GetIPProperties();
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    ComboboxItem item = new ComboboxItem();
                    item.Text = network.Name + " (" + address.Address.ToString() + ")";
                    item.Value = network.Id;
                    comboBox1.Items.Add(item);
                }
            }

            foreach (ComboboxItem item in comboBox1.Items)
            {
                if (item.Value.ToString() == selected_interface)
                    comboBox1.SelectedItem = item;
            }
            
        }

        private void load_config()
        {
            System.Reflection.Assembly exe = System.Reflection.Assembly.GetEntryAssembly();
            string exePath = System.IO.Path.GetDirectoryName(exe.Location);

            IniFile ini = new IniFile(exePath + "\\settings.ini");
            email = ini.IniReadValue("Account", "Email");
            tkn = ini.IniReadValue("Account", "Key");
            z = ini.IniReadValue("DNS", "Zone");
            id = ini.IniReadValue("DNS", "HostID");
            name = ini.IniReadValue("DNS", "HostName");
            selected_interface = ini.IniReadValue("Network", "Interface");
            numericUpDown1.Value = int.Parse(ini.IniReadValue("Network", "Interval"));
            textBox1.Text = email;
            textBox2.Text = tkn;
            timer1.Interval = (int)numericUpDown1.Value * 60 * 1000;
        }

        private IPAddress getNewIPAddress()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface network in networkInterfaces)
            {
                if (network.Id.ToString() == selected_interface)
                {
                    if (network.OperationalStatus != OperationalStatus.Up)
                        return null;
                    IPInterfaceProperties properties = network.GetIPProperties();
                    foreach (IPAddressInformation address in properties.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
                            continue;

                        if (IPAddress.IsLoopback(address.Address))
                            continue;

                        return address.Address;
                    }
                    break;
                }
            }
            return null;
        }

        private void zone_load_multi()
        {
            log("Getting domain list");
            try
            {
                WebRequest request = WebRequest.Create("https://www.cloudflare.com/api_json.html");

                var postData = "a=zone_load_multi";
                postData += "&tkn=" + tkn;
                postData += "&email=" + email;

                var data = Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();

                ZoneRootObject rootobj = JsonConvert.DeserializeObject<ZoneRootObject>(responseFromServer);

                if (comboBox3.Items.Count > 0)
                    comboBox3.Items.Clear();
                foreach (ZoneObj zonerec in rootobj.response.zones.objs)
                {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = zonerec.display_name;
                    item.Value = zonerec.zone_name;
                    comboBox3.Items.Add(item);
                }


                foreach (ComboboxItem item in comboBox3.Items)
                {
                    if (item.Value.ToString() == z)
                        comboBox3.SelectedItem = item;
                }
            }
            catch
            {
                log("Failed to get domain list");
                toolStripStatusLabel1.Text = "Failed to get domain list";
            }
        }

        private void rec_load_all()
        {
            log("Getting DNS record of " + z);
            try
            {
                WebRequest request = WebRequest.Create("https://www.cloudflare.com/api_json.html");

                var postData = "a=rec_load_all";
                postData += "&tkn=" + tkn;
                postData += "&email=" + email;
                postData += "&z=" + z;

                var data = Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();

                RootObject rootobj = JsonConvert.DeserializeObject<RootObject>(responseFromServer);

                if (comboBox2.Items.Count > 0)
                    comboBox2.Items.Clear();
                foreach (Obj rec in rootobj.response.recs.objs)
                {
                    if (rec.type == "A")
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = rec.name;
                        item.Value = rec.rec_id;
                        comboBox2.Items.Add(item);
                    }
                }

                foreach (ComboboxItem item in comboBox2.Items)
                {
                    if (item.Value.ToString() == id)
                        comboBox2.SelectedItem = item;
                }
            }
            catch
            {
                log("Failed to get DNS record of " + z);
                toolStripStatusLabel1.Text = "Failed to get DNS record of " + z;
            }
        }

        private void update_now()
        {
            log("Updating " + name + " with IP " + ip.ToString());
            try
            {
                WebRequest request = WebRequest.Create("https://www.cloudflare.com/api_json.html");

                var postData = "a=rec_edit";
                postData += "&tkn=" + tkn;
                postData += "&id=" + id;
                postData += "&email=" + email;
                postData += "&z=" + z;
                postData += "&type=A";
                postData += "&name=" + name;
                postData += "&content=" + ip.ToString();
                postData += "&service_mode=0";
                postData += "&ttl=1";

                var data = Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();

                EditRootObject rootobj = JsonConvert.DeserializeObject<EditRootObject>(responseFromServer);


                if (rootobj.result == "success")
                {
                    lastip = ip;
                    log(name + " successfully updated with IP " + ip.ToString() + " on " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    toolStripStatusLabel1.Text = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] " + name + " successfully updated with IP " + ip.ToString();
                }
            }
            catch
            {
                log(name + " failed to update with IP " + ip.ToString() + " on " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                toolStripStatusLabel1.Text = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] " + name + " update failed";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            email = textBox1.Text;
            tkn = textBox2.Text;

            System.Reflection.Assembly exe = System.Reflection.Assembly.GetEntryAssembly();
            string exePath = System.IO.Path.GetDirectoryName(exe.Location);

            IniFile ini = new IniFile(exePath + "\\settings.ini");
            ini.IniWriteValue("Account", "Email", email);
            ini.IniWriteValue("Account", "Key", tkn);

            zone_load_multi();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (getNewIPAddress() != null)
            {
                ip = getNewIPAddress();
                update_now();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selected_interface = (comboBox1.SelectedItem as ComboboxItem).Value.ToString();
            log("Using Network Interface " + (comboBox1.SelectedItem as ComboboxItem).Text.ToString());

            System.Reflection.Assembly exe = System.Reflection.Assembly.GetEntryAssembly();
            string exePath = System.IO.Path.GetDirectoryName(exe.Location);

            IniFile ini = new IniFile(exePath + "\\settings.ini");
            ini.IniWriteValue("Network", "Interface", selected_interface);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            id = (comboBox2.SelectedItem as ComboboxItem).Value.ToString();
            name = (comboBox2.SelectedItem as ComboboxItem).Text.ToString();

            System.Reflection.Assembly exe = System.Reflection.Assembly.GetEntryAssembly();
            string exePath = System.IO.Path.GetDirectoryName(exe.Location);

            IniFile ini = new IniFile(exePath + "\\settings.ini");
            ini.IniWriteValue("DNS", "HostID", id);
            ini.IniWriteValue("DNS", "HostName", name);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            z = (comboBox3.SelectedItem as ComboboxItem).Value.ToString();

            System.Reflection.Assembly exe = System.Reflection.Assembly.GetEntryAssembly();
            string exePath = System.IO.Path.GetDirectoryName(exe.Location);

            IniFile ini = new IniFile(exePath + "\\settings.ini");
            ini.IniWriteValue("DNS", "Zone", z);

            rec_load_all();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(comboBox3.Items.Count == 0)
                zone_load_multi();
            if(comboBox2.Items.Count == 0)
                rec_load_all();
            if (getNewIPAddress() != null)
            {
                if (getNewIPAddress().ToString() != lastip.ToString())
                {
                    ip = getNewIPAddress();
                    log("IP has changed, new IP is " + ip.ToString());
                    update_now();
                }
                else
                {
                    toolStripStatusLabel1.Text = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "] IP unchanged, no update needed";
                }
            }

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (WindowState != FormWindowState.Normal)
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                }
                else
                {
                    WindowState = FormWindowState.Minimized;
                    Hide();
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int) numericUpDown1.Value * 60 * 1000;

            System.Reflection.Assembly exe = System.Reflection.Assembly.GetEntryAssembly();
            string exePath = System.IO.Path.GetDirectoryName(exe.Location);

            IniFile ini = new IniFile(exePath + "\\settings.ini");
            ini.IniWriteValue("Network", "Interval", numericUpDown1.Value.ToString());
        }

    }
}
